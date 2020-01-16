﻿namespace Aardvark.SceneGraph.Semantics

open Aardvark.Base
open FSharp.Data.Adaptive
open Aardvark.Base.Ag
open Aardvark.SceneGraph
open Aardvark.Base.Rendering

open Aardvark.SceneGraph.Internal

[<AutoOpen>]
module UniformSemantics =

    type ISg with   
        member x.Uniforms : list<IUniformProvider> = x?Uniforms
 
    module Semantic =
        let uniforms (s : ISg) : list<IUniformProvider> = s?Uniforms

    [<Semantic>]
    type UniformSem() =
        member x.Uniforms(e : Root<ISg>) =
            e.Child?Uniforms <- ([] : list<IUniformProvider>)

        member x.Uniforms(u : Sg.UniformApplicator) =
            u.Child?Uniforms <- u.Uniforms :: u?Uniforms
