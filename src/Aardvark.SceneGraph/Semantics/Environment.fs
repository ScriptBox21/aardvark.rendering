﻿namespace Aardvark.SceneGraph.Semantics

open Aardvark.Base
open FSharp.Data.Adaptive
open Aardvark.Base.Ag
open Aardvark.SceneGraph
open Aardvark.Base.Rendering


[<AutoOpen>]
module EnvironmentSemantics =

    type ISg with
        member x.CameraLocation : aval<V3d> = x?CameraLocation

    [<Semantic>]
    type DefaultValues() =

        let getViewPosition (viewTrafo : Trafo3d) = viewTrafo.GetViewPosition()

        member x.CameraLocation(e : Sg.ViewTrafoApplicator) =
            e.Child?CameraLocation <- AVal.map getViewPosition e.ViewTrafo

        member x.LightLocation(e : obj) : aval<V3d> =
            e?CameraLocation


        member x.RcpViewportSize(e : ISg) = e?ViewportSize |> AVal.map (fun (s : V2i) -> 1.0 / (V2d s))
