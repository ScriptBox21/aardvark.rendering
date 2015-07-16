﻿namespace Aardvark.Rendering.GL.Compiler

open System
open System.Collections.Generic

open Aardvark.Base.Incremental
open Aardvark.Base
open Aardvark.Base.Rendering
open Aardvark.Rendering.GL

type OperationHint =
    | AddRenderJob of int
    | RemoveRenderJob of int
    | RunProgram

type IFragmentHandler<'f when 'f :> IDynamicFragment<'f>> =
    inherit IDisposable
    abstract member Prolog : 'f
    abstract member Epilog : 'f
    abstract member Create : seq<Instruction> -> 'f
    abstract member Delete : 'f -> unit
    abstract member Compile : unit -> ('f -> unit)
    abstract member AdjustStatistics : FrameStatistics -> FrameStatistics
    abstract member Hint : OperationHint -> unit

module FragmentHandlers =

    type DelayedTask(f : unit -> unit) =
        let f = ref f
        let timerLock = obj()
        let run = fun (o : obj) -> lock timerLock !f
        let timerCallback = System.Threading.TimerCallback(run)
        let mutable timer : Option<System.Threading.Timer> = None

        member x.TrySetDelay(delay : int) =
            match timer with
                | Some t -> t.Change(delay, System.Threading.Timeout.Infinite)
                | None -> let t = new System.Threading.Timer(timerCallback, null, delay, System.Threading.Timeout.Infinite)
                          timer <- Some t
                          true

        member x.Callback
            with get() = !f
            and set v = f := v

        new() = DelayedTask(id)

    let native() =
        let manager = new MemoryManager()

        let prolog =
            let f = new Fragment<unit>(manager, 0)
            f.Append (Assembler.functionProlog 6) |> ignore
            NativeDynamicFragment(f)

        let epilog =
            let f = new Fragment<unit>(manager, 0)
            f.Append (Assembler.functionEpilog 6) |> ignore
            NativeDynamicFragment(f)

        let create (s : seq<Instruction>) =
            if not (Seq.isEmpty s) then
                failwith "cannot create non-empty fragment"

            let f = new Fragment<unit>(manager, 0)
            NativeDynamicFragment(f)

        let defragment() =
            Log.startTimed "defragmentation"

            //Log.warn "defragmentation currently disabled"

            let mutable current = prolog.Fragment
            current.Freeze()
                
            let mutable index = 0
            while current.Next <> null do
                //printfn "%d" index
                current.DefragmentNext()

                let next = current.Next
                current.Unfreeze()
                //System.Threading.Thread.Sleep(100)
                next.Freeze()

                current <- next
                index <- index + 1

            current.Unfreeze()

            Log.stop()

        let mutable defrag = DelayedTask(defragment)
        let mutable totalChanges = 0
        let mutable ranOnce = false
        let hintDefragmentation(additional : int) = 
            totalChanges <- totalChanges + abs additional
            if ranOnce && totalChanges > 2 then
                if defrag.TrySetDelay(2000) then
                    totalChanges <- 0



        { new IFragmentHandler<NativeDynamicFragment<unit>> with
            member x.Dispose() = manager.Dispose()
            member x.Prolog = prolog
            member x.Epilog = epilog
            member x.Create s = create s
            member x.Delete f = f.Fragment.Dispose()
            member x.Compile() =
                let entryPtr = ref 0n
                let run = ref (fun () -> ())
                fun (f : NativeDynamicFragment<unit>) ->
                    let prolog = f.Fragment
                    if prolog.RealPointer <> !entryPtr then
                        entryPtr := prolog.RealPointer
                        run := UnmanagedFunctions.wrap !entryPtr
                    !run ()
            member x.AdjustStatistics s = s
            member x.Hint op =
                match op with 
                    | AddRenderJob v -> hintDefragmentation v
                    | RemoveRenderJob v -> hintDefragmentation -v
                    | RunProgram -> 
                        ranOnce <- true
                        hintDefragmentation 0
        }

    let managed() =
        { new IFragmentHandler<ManagedDynamicFragment> with
            member x.Dispose() = ()
            member x.Prolog = ManagedDynamicFragment()
            member x.Epilog = ManagedDynamicFragment()
            member x.Create s = ManagedDynamicFragment()
            member x.Delete f = f.Clear()
            member x.Compile() =
                fun (f : ManagedDynamicFragment) -> f.RunAll ()
            member x.AdjustStatistics s = s
            member x.Hint op = ()
        }

    let glvm() =
        { new IFragmentHandler<SwitchFragment> with
            member x.Dispose() = ()
            member x.Prolog = new SwitchFragment()
            member x.Epilog = new SwitchFragment()
            member x.Create s = new SwitchFragment()
            member x.Delete f = f.Dispose()
            member x.Compile() =
                fun (f : SwitchFragment) -> f.RunAll ()
            member x.AdjustStatistics s = s
            member x.Hint op = ()
        }

    let glvmRuntimeRedundancyChecks() =
        let lastStats = ref (VMStats())
        { new IFragmentHandler<SwitchFragment> with
            member x.Dispose() = ()
            member x.Prolog = new SwitchFragment()
            member x.Epilog = new SwitchFragment()
            member x.Create s = new SwitchFragment()
            member x.Delete f = f.Dispose()
            member x.Compile() =
                fun (f : SwitchFragment) -> 
                    lastStats := f.RunAll (VMMode.RuntimeRedundancyChecks)
            member x.AdjustStatistics s = 
                { s with ActiveInstructionCount = s.ActiveInstructionCount - float lastStats.Value.RemovedInstructions }
            member x.Hint op = ()
        }