﻿module Culling

//    [<Semantic>]
//    type CullNodeSem() =
//        member x.RenderObjects(c : ViewFrustumCullNode) :  aset<RenderObject>=
//            let intersectsFrustum (b : Box3d) (f : Trafo3d) =
//                b.IntersectsFrustum(f.Forward)
//            
//            aset {
//
//                let! child = c.Child
//                let jobs = child?RenderObjects() : aset<RenderObject>
//
//                let viewProjTrafo = c?ViewProjTrafo() : aval<Trafo3d>
//
//                yield! jobs |> ASet.filterM (fun rj -> AVal.map2 intersectsFrustum (rj.GetBoundingBox()) viewProjTrafo)
////
////                for rj : RenderObject in jobs do
////                    let! viewProjTrafo = c?ViewProjTrafo() : Mod<Trafo3d>
////                    let! bb = rj.GetBoundingBox().Mod
////                    if intersectsFrustum bb viewProjTrafo 
////                    then yield rj
//            }