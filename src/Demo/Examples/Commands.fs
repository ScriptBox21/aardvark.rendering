﻿namespace Examples


open System
open Aardvark.Base
open FSharp.Data.Adaptive

open Aardvark.SceneGraph
open Aardvark.SceneGraph.Semantics
open Aardvark.Application
open Aardvark.Application.WinForms
open Aardvark.Rendering.Vulkan
open FSharp.Data.Adaptive.Operators
open Aardvark.Base.Rendering
open Aardvark.Base.ShaderReflection
open FShade
open FShade.Imperative

module CommandTest =

    let run() =
        use app = new VulkanApplication(false)
        let win = app.CreateSimpleRenderWindow(8)
        let runtime = app.Runtime
        let device = runtime.Device

        let cameraView  = DefaultCameraController.control win.Mouse win.Keyboard win.Time (CameraView.LookAt(3.0 * V3d.III, V3d.OOO, V3d.OOI))    
        let frustum     = win.Sizes    |> AVal.map (fun s -> Frustum.perspective 60.0 0.1 1000.0 (float s.X / float s.Y))       
        let viewTrafo   = cameraView    |> AVal.map CameraView.viewTrafo
        let projTrafo   = frustum       |> AVal.map Frustum.projTrafo        



        let rotor =
            let startTime = System.DateTime.Now
            win.Time |> AVal.map (fun t ->
                let t = (t - startTime).TotalSeconds
                Trafo3d.RotationZ (0.5 * t)
            )

        let viewTrafo =
            AVal.map2 (*) rotor viewTrafo




        let box = Primitives.unitBox
        let pos = box.IndexedAttributes.[DefaultSemantic.Positions] |> unbox<V3f[]> |> Array.map (fun v -> v / 2.0f)
        let n = box.IndexedAttributes.[DefaultSemantic.Normals] |> unbox<V3f[]>
        let tc = box.IndexedAttributes.[DefaultSemantic.DiffuseColorCoordinates] |> unbox<V2f[]>
        let index, fvc =
            match box.IndexArray with
                | null ->
                    None, pos.Length
                | index -> 
                    let idx = Aardvark.Base.BufferView(AVal.constant (ArrayBuffer index :> IBuffer), typeof<int>)
                    Some idx, index.Length

        let rand = RandomSystem()

        //let randomGeo


        let quadGeometry (offset : V3d) =
            let color = rand.UniformC3f().ToC4d().ToV4d()

            IndexedGeometry(
                Mode = IndexedGeometryMode.TriangleList,
                SingleAttributes =
                    SymDict.ofList [
                        Symbol.Create "NodeColor", (AVal.constant color) :> obj
                    ],

                IndexedAttributes =
                    SymDict.ofList [
                        DefaultSemantic.Positions, pos |> Array.map (fun v -> V3d v + offset |> V3f) :> Array
                        DefaultSemantic.Normals, n :> Array
                    ]
            )

        let state =
            {
                depthTest           = AVal.constant DepthTestMode.LessOrEqual
                depthBias           = AVal.constant (DepthBiasState(0.0, 0.0, 0.0))
                cullMode            = AVal.constant CullMode.None
                frontFace           = AVal.constant WindingOrder.CounterClockwise
                blendMode           = AVal.constant BlendMode.None
                fillMode            = AVal.constant FillMode.Fill
                stencilMode         = AVal.constant StencilMode.Disabled
                multisample         = AVal.constant true
                writeBuffers        = None
                globalUniforms      = 
                    UniformProvider.ofList [
                        "ModelTrafo", AVal.constant Trafo3d.Identity :> IAdaptiveValue
                        "ViewTrafo", viewTrafo :> IAdaptiveValue
                        "ProjTrafo", projTrafo :> IAdaptiveValue
                        "LightLocation", viewTrafo |> AVal.map (fun v -> v.Backward.C3.XYZ) :> IAdaptiveValue
                        "CameraLocation", viewTrafo |> AVal.map (fun v -> v.Backward.C3.XYZ) :> IAdaptiveValue
                    ]

                geometryMode        = IndexedGeometryMode.TriangleList
                vertexInputTypes    = Map.ofList [ DefaultSemantic.Positions, typeof<V3f>; DefaultSemantic.Normals, typeof<V3f> ]
                perGeometryUniforms = Map.ofList [ "NodeColor", typeof<V4d> ]
            }


        let effects =
            [|
                FShade.Effect.compose [
                    toEffect <| DefaultSurfaces.trafo
                    toEffect <| DefaultSurfaces.constantColor C4f.White
                ]

                FShade.Effect.compose [
                    toEffect <| DefaultSurfaces.trafo
                    toEffect <| DefaultSurfaces.constantColor C4f.White
                    toEffect <| fun (v : Effects.Vertex) ->
                        fragment {
                            let color : V4d = uniform?NodeColor
                            return color
                        }
                    toEffect <| DefaultSurfaces.simpleLighting
                ]
            |]


        let mutable oida =
            let size = 20
            [
                for x in -size .. size do
                    for y in -size .. size do
                        for z in -size .. size do
                            yield quadGeometry (V3d(float x, float y, float z))
                    
            ] |> System.Collections.Generic.Stack

        let mutable urdar = System.Collections.Generic.Stack()

        let geometries = cset oida

        let mutable up = false

        win.Keyboard.DownWithRepeats.Values.Add (fun k ->
            match k with
                | Keys.M ->
                    transact (fun () -> 
                        for a in 0 .. 1000 do
                            if oida.Count > 0 then
                                let newJOne = oida.Pop()
                                urdar.Push newJOne
                                if up then geometries.Add newJOne |> ignore
                                else geometries.Remove newJOne |> ignore
                            else 
                                up <- not up
                                Fun.Swap(&oida,&urdar)
                    )
                | _ ->
                    ()
        )

        let current = AVal.init 0

        win.Keyboard.DownWithRepeats.Values.Add (fun k ->
            match k with
                | Keys.Space ->
                    transact (fun () -> current.Value <- (1 + current.Value) % effects.Length)
                | _ ->
                    ()
        )


        //let cmd = RuntimeCommand.Geometries(effects, current, state, geometries)
        let cmd = RuntimeCommand.Geometries(effects.[1], state, geometries)
//
//        let objects1 = sg1.RenderObjects()
//        let objects2 = sg2.RenderObjects()
//        let cmd = 
//            RuntimeCommand.IfThenElse(
//                condition,
//                RuntimeCommand.Render objects1,
//                RuntimeCommand.Render objects2
//            )


        win.RenderTask <- new Temp.CommandTask(device, unbox win.FramebufferSignature, cmd)
        win.Run()

        win.Dispose()
  


