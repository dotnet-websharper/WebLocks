namespace WebSharper.WebLocks

open WebSharper
open WebSharper.JavaScript

[<JavaScript; AutoOpen>]
module Extensions =

    type Navigator with
        [<Inline "$this.locks">]
        member this.Locks with get(): LockManager = X<LockManager>

    type WorkerNavigator with
        [<Inline "$this.locks">]
        member this.Locks with get(): LockManager = X<LockManager>
