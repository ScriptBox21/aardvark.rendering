﻿namespace Aardvark.Rendering.GL


open Aardvark.Base
open Aardvark.Base.Rendering
open Aardvark.Rendering.GL.OpenGl
        

module Translations =

    type private ABlendFactor = Aardvark.Base.Rendering.BlendFactor
    type private GLBlendFactor = Aardvark.Rendering.GL.OpenGl.Enums.BlendFactor
    type private ABlendOperation = Aardvark.Base.Rendering.BlendOperation
    type private GLBlendOperation = Aardvark.Rendering.GL.OpenGl.Enums.BlendOperation

    let toGLMode (m : IndexedGeometryMode) =
        match m with 
            | IndexedGeometryMode.TriangleList -> DrawMode.Triangles |> int
            | IndexedGeometryMode.TriangleStrip -> DrawMode.TriangleStrip |> int
            | IndexedGeometryMode.LineList -> DrawMode.Lines |> int
            | IndexedGeometryMode.LineStrip -> DrawMode.LineStrip |> int
            | IndexedGeometryMode.PointList -> DrawMode.Points |> int
            | IndexedGeometryMode.TriangleAdjacencyList -> DrawMode.TrianglesAdjacency |> int
            | IndexedGeometryMode.LineAdjacencyList -> DrawMode.LinesAdjacency |> int
            | IndexedGeometryMode.QuadList -> DrawMode.QuadList |> int
            | _ -> failwith "not handled IndexedGeometryMode"

    let toPatchCount (m : IndexedGeometryMode) =
            match m with 
            | IndexedGeometryMode.TriangleList -> 3
            | IndexedGeometryMode.TriangleStrip -> 3 
            | IndexedGeometryMode.LineList -> 2
            | IndexedGeometryMode.LineStrip -> 2
            | IndexedGeometryMode.PointList -> 1
            | IndexedGeometryMode.TriangleAdjacencyList -> 2
            | IndexedGeometryMode.LineAdjacencyList -> 3
            | IndexedGeometryMode.QuadList -> 4

            | _ -> failwith "not handled IndexedGeometryMode"

    let toGLFactor (f : ABlendFactor) =
        match f with
            | ABlendFactor.Zero -> GLBlendFactor.Zero |> int
            | ABlendFactor.One -> GLBlendFactor.One |> int
            | ABlendFactor.DestinationAlpha -> GLBlendFactor.DstAlpha |> int
            | ABlendFactor.DestinationColor -> GLBlendFactor.DstColor |> int
            | ABlendFactor.SourceAlpha -> GLBlendFactor.SrcAlpha |> int
            | ABlendFactor.SourceColor -> GLBlendFactor.SrcColor |> int
            | ABlendFactor.InvDestinationAlpha -> GLBlendFactor.InvDstAlpha |> int
            | ABlendFactor.InvDestinationColor -> GLBlendFactor.InvDstColor |> int
            | ABlendFactor.InvSourceAlpha -> GLBlendFactor.InvSrcAlpha |> int
            | ABlendFactor.InvSourceColor -> GLBlendFactor.InvSrcColor |> int
            | ABlendFactor.SecondarySourceColor -> GLBlendFactor.Src1Color |> int
            | ABlendFactor.SecondarySourceAlpha -> GLBlendFactor.Src1Alpha |> int
            | ABlendFactor.InvSecondarySourceColor -> GLBlendFactor.InvSrc1Color |> int
            | ABlendFactor.InvSecondarySourceAlpha -> GLBlendFactor.InvSrc1Alpha |> int
            | _ -> failwithf "unknown blend factor: %A" f

    let toGLOperation (f : ABlendOperation) =
        match f with
            | ABlendOperation.Add -> GLBlendOperation.Add |> int
            | ABlendOperation.Subtract -> GLBlendOperation.Subtract |> int
            | ABlendOperation.ReverseSubtract -> GLBlendOperation.ReverseSubtract |> int
            | ABlendOperation.Minimum -> GLBlendOperation.Minimum |> int
            | ABlendOperation.Maximum -> GLBlendOperation.Maximum |> int
            | _ -> failwithf "unknown blend operation %A" f

    let toGLComparison (f : DepthTestComparison) =
        match f with
            | DepthTestComparison.None -> 0
            | DepthTestComparison.Greater -> CompareFunction.Greater |> int
            | DepthTestComparison.GreaterOrEqual -> CompareFunction.GreaterEqual |> int
            | DepthTestComparison.Less -> CompareFunction.Less |> int
            | DepthTestComparison.LessOrEqual -> CompareFunction.LessEqual |> int
            | DepthTestComparison.Equal -> CompareFunction.Equal |> int
            | DepthTestComparison.NotEqual -> CompareFunction.NotEqual |> int
            | DepthTestComparison.Never -> CompareFunction.Never |> int
            | DepthTestComparison.Always -> CompareFunction.Always |> int
            | _ -> failwithf "unknown comparison %A" f

    let toGLCullMode (f : CullMode) =
        match f with
            | CullMode.None -> 0// glDisable(GL_CULL_FACE) / glCullFace will not be set and
            | CullMode.Front -> Face.Back |> int
            | CullMode.Back -> Face.Front |> int
            | CullMode.FrontAndBack-> Face.FrontAndBack |> int
            | _ -> failwithf "unknown comparison %A" f

    let toGLFrontFace (f : Aardvark.Base.Rendering.WindingOrder) =
        match f with
            | Aardvark.Base.Rendering.WindingOrder.Clockwise -> WindingOrder.CW |> int
            | Aardvark.Base.Rendering.WindingOrder.CounterClockwise -> WindingOrder.CCW |> int
            | _ -> failwithf "unknown winding order %A" f

    let toGLFunction (f : StencilCompareFunction) =
        match f with
            | StencilCompareFunction.Always -> CompareFunction.Always |> int
            | StencilCompareFunction.Equal -> CompareFunction.Equal |> int
            | StencilCompareFunction.Greater -> CompareFunction.Greater |> int
            | StencilCompareFunction.GreaterOrEqual -> CompareFunction.GreaterEqual |> int
            | StencilCompareFunction.Less -> CompareFunction.Less |> int
            | StencilCompareFunction.LessOrEqual -> CompareFunction.LessEqual |> int
            | StencilCompareFunction.Never -> CompareFunction.Never |> int
            | StencilCompareFunction.NotEqual -> CompareFunction.NotEqual |> int
            | _ -> failwithf "unknown comparison %A" f

    let toGLStencilOperation (o : StencilOperationFunction) =
        match o with
            | StencilOperationFunction.Decrement -> StencilOperation.Decrement |> int
            | StencilOperationFunction.DecrementWrap -> StencilOperation.DecrementWrap  |> int
            | StencilOperationFunction.Increment -> StencilOperation.Increment  |> int
            | StencilOperationFunction.IncrementWrap -> StencilOperation.IncrementWrap  |> int
            | StencilOperationFunction.Invert -> StencilOperation.Invert  |> int
            | StencilOperationFunction.Keep -> StencilOperation.Keep  |> int
            | StencilOperationFunction.Replace -> StencilOperation.Replace  |> int
            | StencilOperationFunction.Zero -> StencilOperation.Zero  |> int
            | _ -> failwithf "unknown stencil operation %A" o

    let toGLPolygonMode (f : FillMode) =
        match f with
            | FillMode.Fill -> Enums.PolygonMode.Fill |> int
            | FillMode.Line -> Enums.PolygonMode.Line |> int
            | FillMode.Point -> Enums.PolygonMode.Point |> int
            | _ -> failwithf "unknown FillMode: %A" f

    let toGLTarget (d : TextureDimension) (isArray : bool) (samples : int) =
        match (d, isArray, samples > 1) with
            | (TextureDimension.Texture1D, false, _) -> int TextureTarget.Texture1D
            | (TextureDimension.Texture1D, true, _) -> int TextureTarget.Texture1DArray
            | (TextureDimension.Texture2D, false, false) -> int TextureTarget.Texture2D
            | (TextureDimension.Texture2D, true, false) -> int TextureTarget.Texture2DArray
            | (TextureDimension.Texture2D, false, true) -> int TextureTarget.Texture2DMultisample
            | (TextureDimension.Texture2D, true, true) -> int TextureTarget.Texture2DMultisampleArray
            | (TextureDimension.Texture3D, false, _) -> int TextureTarget.Texture3D
            | (TextureDimension.TextureCube, false, _) -> int TextureTarget.TextureCubeMap
            | (TextureDimension.TextureCube, true, _) -> int TextureTarget.TextureCubeMapArray
            | _ -> failwithf "unknown TextureDimension: %A" d
