﻿namespace Aardvark.SceneGraph.Semantics

open System

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Ag
open Aardvark.Base.AgHelpers
open Aardvark.SceneGraph

[<AutoOpen>]
module AttributeExtensions =
    type ISg with
        member x.FaceVertexCount : IMod<int> = x?FaceVertexCount
        member x.VertexAttributes : Map<Symbol, BufferView> = x?VertexAttributes
        member x.InstanceAttributes : Map<Symbol, BufferView> = x?InstanceAttributes
        member x.VertexIndexArray : IMod<Array> = x?VertexIndexArray

    module Semantic =
        let faceVertexCount (s : ISg) : IMod<int> = s?FaceVertexCount
        let vertexAttributes (s : ISg) : Map<Symbol, BufferView> = s?VertexAttributes
        let instanceAttributes (s : ISg) : Map<Symbol, BufferView> = s?InstanceAttributes
        let vertexIndexArray (s : ISg) : IMod<Array> = s?VertexIndexArray

module AttributeSemantics =

    let emptyIndex : IMod<Array> = Mod.constant ([||] :> Array)

    [<Semantic>]
    type AttributeSem() =
        static let zero = Mod.constant 0
        let (~%) (m : Map<Symbol, BufferView>) = m

        member x.FaceVertexCount (root : Root<ISg>) =
            root.Child?FaceVertexCount <- zero

        member x.FaceVertexCount (app : Sg.VertexIndexApplicator) =
            app.Child?FaceVertexCount <- app.Value |> Mod.map (fun a -> a.Length)

        member x.FaceVertexCount (app : Sg.VertexAttributeApplicator) =
            let res = app.VertexIndexArray

            if res <> emptyIndex then
                app.Child?FaceVertexCount <- app.FaceVertexCount
            else
                match Map.tryFind DefaultSemantic.Positions app.Values with
                    | Some pos ->
                        app.Child?FaceVertexCount <- 
                            pos.Buffer 
                                |> Mod.bind (fun buffer ->
                                    match buffer with
                                        | :? ArrayBuffer as a ->
                                            Mod.constant (a.Data.Length - pos.Offset)
            
                                        | _ -> app.FaceVertexCount
                                   )
                    | _ -> app.Child?FaceVertexCount <- app.FaceVertexCount

        member x.InstanceAttributes(root : Root<ISg>) = 
            root.Child?InstanceAttributes <- %Map.empty

        member x.VertexIndexArray(e : Root<ISg>) =
            e.Child?VertexIndexArray <- emptyIndex

        member x.VertexIndexArray(v : Sg.VertexIndexApplicator) =
            v.Child?VertexIndexArray <- v.Value

        member x.VertexAttributes(e : Root<ISg>) =
            e.Child?VertexAttributes <- %Map.empty

        member x.VertexAttributes(v : Sg.VertexAttributeApplicator) =
            v.Child?VertexAttributes <- Map.union v.VertexAttributes v.Values

        member x.InstanceAttributes(v : Sg.InstanceAttributeApplicator) =
            v.Child?InstanceAttributes <- Map.union v.InstanceAttributes v.Values
