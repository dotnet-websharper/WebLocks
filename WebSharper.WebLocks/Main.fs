namespace WebSharper.WebLocks

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let Lock =
        Class "Lock"
        |+> Instance [
            "mode" =? T<string> // Read-only: "exclusive" (default) or "shared"
            "name" =? T<string> // Read-only: Name of the lock
        ]

    let LockInfo =
        Pattern.Config "LockInfo" {
            Required = []
            Optional = [
                "name", T<string> // Name of the lock
                "mode", T<string> // "exclusive" or "shared"
                "clientId", T<string>
            ]
        }

    let LockManagerQueryResult =
        Pattern.Config "LockManagerQueryResult" {
            Required = [
                "held", !| LockInfo // List of held locks
                "pending", !| LockInfo // List of pending locks
            ]
            Optional = []
        }

    let LockRequestOptions =
        Pattern.Config "LockRequestOptions" {
            Required = []
            Optional = [
                "mode", T<string> // "exclusive" (default) or "shared"
                "ifAvailable", T<bool> // If true, grants the lock only if available
                "steal", T<bool> // If true, releases existing locks and grants this one
                "signal", T<Dom.AbortSignal> // AbortSignal to cancel the request if needed (requires separate binding)
            ]
        }

    let LockManager =

        let callbackFunc = T<obj>?callback ^-> T<Promise<unit>>

        Class "LockManager"
        |+> Instance [
            "request" => T<string>?name * !?LockRequestOptions?options * callbackFunc?callback ^-> T<Promise<unit>> 
            // Requests a lock with name and optional mode, and executes the callback
            "query" => T<unit> ^-> T<Promise<_>>[LockManagerQueryResult] // Retrieves information about held and pending locks
        ]

    let Navigator = 
        Class "Navigator" 
        |+> Instance [
            "locks" =? LockManager
        ]

    let WorkerNavigator = 
        Class "WorkerNavigator" 
        |+> Instance [
            "locks" =? LockManager
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.WebLocks" [
                WorkerNavigator
                Navigator
                LockManager
                LockRequestOptions
                LockManagerQueryResult
                LockInfo
                Lock
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
