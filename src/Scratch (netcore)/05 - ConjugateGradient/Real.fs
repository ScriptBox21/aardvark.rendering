﻿namespace ConjugateGradient

open Microsoft.FSharp.Quotations
open Aardvark.Base


type Real<'a> =
    {
        zero : 'a
        one : 'a
        add : 'a -> 'a -> 'a
        sub : 'a -> 'a -> 'a
        mul : 'a -> 'a -> 'a
        div : 'a -> 'a -> 'a
        neg : 'a -> 'a
        pow : 'a -> int -> 'a
        sqrt : 'a -> 'a
        fromInt : int -> 'a
        fromFloat : float -> 'a
        isTiny : 'a -> bool
        isTinyEps : float -> 'a -> bool
        isPositive : 'a -> bool
    }

module RealInstances =

    let Cfloat32 =
        {
            zero = 0.0f
            one = 1.0f
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> pown v n
            sqrt = sqrt
            fromInt = float32
            fromFloat = float32
            isTiny = Fun.IsTiny 
            isTinyEps = fun e v -> Fun.IsTiny(v, float32 e)
            isPositive = fun v -> v > 0.0f
        }

    let Cfloat64 =
        {
            zero = 0.0
            one = 1.0
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> pown v n
            sqrt = sqrt
            fromInt = float
            fromFloat = float
            isTiny = Fun.IsTiny 
            isTinyEps = fun e v -> Fun.IsTiny(v, e)
            isPositive = fun v -> v > 0.0
        }

    let CV2f =
        {
            zero = V2f.Zero
            one = V2f.II
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> V2f(pown v.X n, pown v.Y n)
            sqrt = fun v -> V2f(sqrt v.X, sqrt v.Y)
            fromInt = fun v -> V2f(float32 v, float32 v)
            fromFloat = fun v -> V2f(float32 v, float32 v)
            isTiny = fun v -> Fun.IsTiny(v.X) && Fun.IsTiny(v.Y)
            isTinyEps = fun e v -> Fun.IsTiny(v.X, float32 e) && Fun.IsTiny(v.Y, float32 e)
            isPositive = fun v -> v.AllGreater 0.0f
        }

    let CV2d =
        {
            zero = V2d.Zero
            one = V2d.II
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> V2d(pown v.X n, pown v.Y n)
            sqrt = fun v -> V2d(sqrt v.X, sqrt v.Y)
            fromInt = fun v -> V2d(float v, float v)
            fromFloat = fun v -> V2d(v, v)
            isTiny = fun v -> Fun.IsTiny(v.X) && Fun.IsTiny(v.Y)
            isTinyEps = fun e v -> Fun.IsTiny(v.X, e) && Fun.IsTiny(v.Y, e)
            isPositive = fun v -> v.AllGreater 0.0
        }

    let CV3f =
        {
            zero = V3f.Zero
            one = V3f.III
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> V3f(pown v.X n, pown v.Y n, pown v.Z n)
            sqrt = fun v -> V3f(sqrt v.X, sqrt v.Y, sqrt v.Z)
            fromInt = fun v -> V3f(float32 v, float32 v, float32 v)
            fromFloat = fun v -> V3f(float32 v, float32 v, float32 v)
            isTiny = fun v -> Fun.IsTiny(v.X) && Fun.IsTiny(v.Y) && Fun.IsTiny(v.Z)
            isTinyEps = fun e v -> Fun.IsTiny(v.X, float32 e) && Fun.IsTiny(v.Y, float32 e) && Fun.IsTiny(v.Z, float32 e)
            isPositive = fun v -> v.AllGreater 0.0f
        }
     
    let CV3d =
        {
            zero = V3d.Zero
            one = V3d.III
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> V3d(pown v.X n, pown v.Y n, pown v.Z n)
            sqrt = fun v -> V3d(sqrt v.X, sqrt v.Y, sqrt v.Z)
            fromInt = fun v -> V3d(float v, float v, float v)
            fromFloat = fun v -> V3d(v, v, v)
            isTiny = fun v -> Fun.IsTiny(v.X) && Fun.IsTiny(v.Y) && Fun.IsTiny(v.Z)
            isTinyEps = fun e v -> Fun.IsTiny(v.X, e) && Fun.IsTiny(v.Y, e) && Fun.IsTiny(v.Z, e)
            isPositive = fun v -> v.AllGreater 0.0
        }

    let CV4f =
        {
            zero = V4f.Zero
            one = V4f.IIII
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> V4f(pown v.X n, pown v.Y n, pown v.Z n, pown v.W n)
            sqrt = fun v -> V4f(sqrt v.X, sqrt v.Y, sqrt v.Z, sqrt v.W)
            fromInt = fun v -> V4f(float32 v, float32 v, float32 v, float32 v)
            fromFloat = fun v -> V4f(float32 v, float32 v, float32 v, float32 v)
            isTiny = fun v -> Fun.IsTiny v.X && Fun.IsTiny v.Y && Fun.IsTiny v.Z && Fun.IsTiny v.W
            isTinyEps = fun e v -> Fun.IsTiny(v.X, float32 e) && Fun.IsTiny(v.Y, float32 e) && Fun.IsTiny(v.Z, float32 e) && Fun.IsTiny(v.W, float32 e)
            isPositive = fun v -> v.AllGreater 0.0f
        }

    let CV4d =
        {
            zero = V4d.Zero
            one = V4d.IIII
            add = (+)
            sub = (-)
            mul = (*)
            div = (/)
            neg = (~-)
            pow = fun v n -> V4d(pown v.X n, pown v.Y n, pown v.Z n, pown v.W n)
            sqrt = fun v -> V4d(sqrt v.X, sqrt v.Y, sqrt v.Z, sqrt v.W)
            fromInt = fun v -> V4d(float v, float v, float v, float v)
            fromFloat = fun v -> V4d(v, v, v, v)
            isTiny = fun v -> Fun.IsTiny v.X && Fun.IsTiny v.Y && Fun.IsTiny v.Z && Fun.IsTiny v.W
            isTinyEps = fun e v -> Fun.IsTiny(v.X, e) && Fun.IsTiny(v.Y, e) && Fun.IsTiny(v.Z, e) && Fun.IsTiny(v.W, e)
            isPositive = fun v -> v.AllGreater 0.0
        }

    let internal table =
        LookupTable.lookupTable [
            typeof<float32>,    Cfloat32 :> obj
            typeof<float>,      Cfloat64 :> obj
            typeof<V2f>,        CV2f :> obj
            typeof<V2d>,        CV2d :> obj
            typeof<V3f>,        CV3f :> obj
            typeof<V3d>,        CV3d :> obj
            typeof<V4f>,        CV4f :> obj
            typeof<V4d>,        CV4d :> obj
        ]

    let instance<'a> = table typeof<'a> |> unbox<Real<'a>>




type ReflectedReal<'a> =
    {
        zero    : Expr<'a>
        one     : Expr<'a>
        add     : Expr<'a -> 'a -> 'a>
        sub     : Expr<'a -> 'a -> 'a>
        neg     : Expr<'a -> 'a>
        mul     : Expr<'a -> 'a -> 'a>
        div     : Expr<'a -> 'a -> 'a>
        pow     : Expr<'a -> 'a -> 'a>
        
        exp     : Expr<'a -> 'a>
        sin     : Expr<'a -> 'a>
        cos     : Expr<'a -> 'a>
        tan     : Expr<'a -> 'a>
        log     : Expr<'a -> 'a>

        min     : Expr<'a -> 'a -> 'a>
        max     : Expr<'a -> 'a -> 'a>
        pinf    : Expr<'a>
        ninf    : Expr<'a>

        fromV4  : Expr<V4d -> 'a>
        toV4    : Expr<'a -> V4d>
        format  : TextureFormat
    }

module ReflectedReal =
    open FShade 

    [<GLSLIntrinsic("(1.0 / 0.0)")>]
    let pinf() : float = onlyInShaderCode "pinf" //System.Double.PositiveInfinity
    
    [<GLSLIntrinsic("(-1.0 / 0.0)")>]
    let ninf() : float = onlyInShaderCode "ninf" //System.Double.NegativeInfinity
    
    [<GLSLIntrinsic("(1.0 / 0.0)")>]
    let fpinf() : float32 = onlyInShaderCode "pinf" //System.Single.PositiveInfinity
    
    [<GLSLIntrinsic("(-1.0 / 0.0)")>]
    let fninf() : float32 = onlyInShaderCode "ninf" //System.Single.NegativeInfinity

    let Cfloat64 =
        {
            zero    = <@ 0.0 @>
            one     = <@ 1.0 @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ ( ** ) @>

            min     = <@ min @>
            max     = <@ max @>
            pinf    = <@ pinf() @>
            ninf    = <@ ninf() @>
            
            exp     = <@ exp @>
            sin     = <@ sin @>
            cos     = <@ cos @>
            tan     = <@ tan @>
            log     = <@ log @>

            fromV4  = <@ fun v -> v.X @>
            toV4    = <@ fun v -> V4d(v, 0.0, 0.0, 0.0) @>
            format  = TextureFormat.R32f
        }

    let Cfloat32 =
        {
            zero    = <@ 0.0f @>
            one     = <@ 1.0f @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ ( ** ) @>
            
            exp     = <@ exp @>
            min     = <@ min @>
            max     = <@ max @>
            pinf    = <@ fpinf() @>
            ninf    = <@ fninf() @>

            
            sin     = <@ sin @>
            cos     = <@ cos @>
            tan     = <@ tan @>
            log     = <@ log @>

            fromV4  = <@ fun v -> float32 v.X @>
            toV4    = <@ fun v -> V4d(float v, 0.0, 0.0, 0.0) @>
            format  = TextureFormat.R32f
        }

    let CV2f =
        {
            zero    = <@ V2f.Zero @>
            one     = <@ V2f.II @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ fun v e -> V2f(v.X ** e.X, v.Y ** e.Y) @>
            min     = <@ fun l r -> V2f(min l.X r.X, min l.Y r.Y) @>
            max     = <@ fun l r -> V2f(max l.X r.X, max l.Y r.Y) @>
            pinf    = <@ V2f(fpinf(), fpinf()) @>
            ninf    = <@ V2f(fninf(), fninf()) @>

            
            exp     = <@ fun v -> V2f(exp v.X, exp v.Y) @>
            sin     = <@ fun v -> V2f(sin v.X, sin v.Y) @>
            cos     = <@ fun v -> V2f(cos v.X, cos v.Y) @>
            tan     = <@ fun v -> V2f(tan v.X, tan v.Y) @>
            log     = <@ fun v -> V2f(log v.X, log v.Y) @>

            fromV4  = <@ fun v -> V2f v.XY @>
            toV4    = <@ fun v -> V4d(float v.X, float v.Y, 0.0, 0.0) @>
            format  = TextureFormat.Rg32f
        }

    let CV2d =
        {
            zero    = <@ V2d.Zero @>
            one     = <@ V2d.II @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ fun v e -> V2d(v.X ** e.X, v.Y ** e.Y) @>
            min     = <@ fun l r -> V2d(min l.X r.X, min l.Y r.Y) @>
            max     = <@ fun l r -> V2d(max l.X r.X, max l.Y r.Y) @>
            pinf    = <@ V2d(pinf(), pinf()) @>
            ninf    = <@ V2d(ninf(), ninf()) @>

            
            exp     = <@ fun v -> V2d(exp v.X, exp v.Y) @>
            sin     = <@ fun v -> V2d(sin v.X, sin v.Y) @>
            cos     = <@ fun v -> V2d(cos v.X, cos v.Y) @>
            tan     = <@ fun v -> V2d(tan v.X, tan v.Y) @>
            log     = <@ fun v -> V2d(log v.X, log v.Y) @>

            fromV4  = <@ fun v -> v.XY @>
            toV4    = <@ fun v -> V4d(v.X, v.Y, 0.0, 0.0) @>
            format  = TextureFormat.Rg32f
        }

    let CV3f =
        {
            zero    = <@ V3f.Zero @>
            one     = <@ V3f.III @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ fun v e -> V3f(v.X ** e.X, v.Y ** e.Y, v.Z ** e.Z) @>
            min     = <@ fun l r -> V3f(min l.X r.X, min l.Y r.Y, min l.Z r.Z) @>
            max     = <@ fun l r -> V3f(max l.X r.X, max l.Y r.Y, max l.Z r.Z) @>
            pinf    = <@ V3f(fpinf(), fpinf(), fpinf()) @>
            ninf    = <@ V3f(fninf(), fninf(), fninf()) @>

            
            exp     = <@ fun v -> V3f(exp v.X, exp v.Y, exp v.Z) @>
            sin     = <@ fun v -> V3f(sin v.X, sin v.Y, sin v.Z) @>
            cos     = <@ fun v -> V3f(cos v.X, cos v.Y, cos v.Z) @>
            tan     = <@ fun v -> V3f(tan v.X, tan v.Y, tan v.Z) @>
            log     = <@ fun v -> V3f(log v.X, log v.Y, log v.Z) @>

            fromV4  = <@ fun v -> V3f v.XYZ @>
            toV4    = <@ fun v -> V4d(float v.X, float v.Y, float v.Z, 0.0) @>
            format  = TextureFormat.Rgb32f
        }

    let CV3d =
        {
            zero    = <@ V3d.Zero @>
            one     = <@ V3d.III @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ fun v e -> V3d(v.X ** e.X, v.Y ** e.Y, v.Z ** e.Z) @>
            min     = <@ fun l r -> V3d(min l.X r.X, min l.Y r.Y, min l.Z r.Z) @>
            max     = <@ fun l r -> V3d(max l.X r.X, max l.Y r.Y, max l.Z r.Z) @>
            pinf    = <@ V3d(pinf(), pinf(), pinf()) @>
            ninf    = <@ V3d(ninf(), ninf(), ninf()) @>
            
            exp     = <@ fun v -> V3d(exp v.X, exp v.Y, exp v.Z) @>
            sin     = <@ fun v -> V3d(sin v.X, sin v.Y, sin v.Z) @>
            cos     = <@ fun v -> V3d(cos v.X, cos v.Y, cos v.Z) @>
            tan     = <@ fun v -> V3d(tan v.X, tan v.Y, tan v.Z) @>
            log     = <@ fun v -> V3d(log v.X, log v.Y, log v.Z) @>

            fromV4  = <@ fun v -> v.XYZ @>
            toV4    = <@ fun v -> V4d(v.X, v.Y, v.Z, 0.0) @>
            format  = TextureFormat.Rgb32f
        }

    let CV4f =
        {
            zero    = <@ V4f.Zero @>
            one     = <@ V4f.IIII @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ fun v e -> V4f(v.X ** e.X, v.Y ** e.Y, v.Z ** e.Z, v.W ** e.W) @>
            min     = <@ fun l r -> V4f(min l.X r.X, min l.Y r.Y, min l.Z r.Z, min l.W r.W) @>
            max     = <@ fun l r -> V4f(max l.X r.X, max l.Y r.Y, max l.Z r.Z, max l.W r.W) @>
            pinf    = <@ V4f(fpinf(), fpinf(), fpinf(), fpinf()) @>
            ninf    = <@ V4f(fninf(), fninf(), fninf(), fninf()) @>
            
            exp     = <@ fun v -> V4f(exp v.X, exp v.Y, exp v.Z, exp v.W) @>
            sin     = <@ fun v -> V4f(sin v.X, sin v.Y, sin v.Z, sin v.W) @>
            cos     = <@ fun v -> V4f(cos v.X, cos v.Y, cos v.Z, cos v.W) @>
            tan     = <@ fun v -> V4f(tan v.X, tan v.Y, tan v.Z, tan v.W) @>
            log     = <@ fun v -> V4f(log v.X, log v.Y, log v.Z, log v.W) @>

            fromV4  = <@ fun v -> V4f v @>
            toV4  = <@ fun v -> V4d v @>
            format  = TextureFormat.Rgba32f
        }

    let CV4d =
        {
            zero    = <@ V4d.Zero @>
            one     = <@ V4d.IIII @>
            add     = <@ (+) @>
            sub     = <@ (-) @>
            mul     = <@ (*) @>
            div     = <@ (/) @>
            neg     = <@ (~-) @>
            pow     = <@ fun v e -> V4d(v.X ** e.X, v.Y ** e.Y, v.Z ** e.Y, v.W ** e.W) @>
            min     = <@ fun l r -> V4d(min l.X r.X, min l.Y r.Y, min l.Z r.Z, min l.W r.W) @>
            max     = <@ fun l r -> V4d(max l.X r.X, max l.Y r.Y, max l.Z r.Z, max l.W r.W) @>
            pinf    = <@ V4d(pinf(), pinf(), pinf(), pinf()) @>
            ninf    = <@ V4d(ninf(), ninf(), ninf(), ninf()) @>
            
            exp     = <@ fun v -> V4d(exp v.X, exp v.Y, exp v.Z, exp v.W) @>
            sin     = <@ fun v -> V4d(sin v.X, sin v.Y, sin v.Z, sin v.W) @>
            cos     = <@ fun v -> V4d(cos v.X, cos v.Y, cos v.Z, cos v.W) @>
            tan     = <@ fun v -> V4d(tan v.X, tan v.Y, tan v.Z, tan v.W) @>
            log     = <@ fun v -> V4d(log v.X, log v.Y, log v.Z, log v.W) @>
            fromV4  = <@ fun v -> v @>
            toV4  = <@ fun v -> v @>
            format  = TextureFormat.Rgba32f
        }

    let internal table =
        LookupTable.lookupTable [
            typeof<float32>,    Cfloat32 :> obj
            typeof<float>,      Cfloat64 :> obj
            typeof<V2f>,        CV2f :> obj
            typeof<V2d>,        CV2d :> obj
            typeof<V3f>,        CV3f :> obj
            typeof<V3d>,        CV3d :> obj
            typeof<V4f>,        CV4f :> obj
            typeof<V4d>,        CV4d :> obj
        ]

    let instance<'a> = table typeof<'a> |> unbox<ReflectedReal<'a>>

