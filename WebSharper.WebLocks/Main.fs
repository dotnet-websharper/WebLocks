namespace WebSharper.WebLocks

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let Lock =
        Class "Lock"
        |+> Instance [
            "mode" =? T<string> 
            "name" =? T<string> 
        ]

    let LockInfo =
        Pattern.Config "LockInfo" {
            Required = []
            Optional = [
                "name", T<string> 
                "mode", T<string> 
                "clientId", T<string>
            ]
        }

    let LockManagerQueryResult =
        Pattern.Config "LockManagerQueryResult" {
            Required = [
                "held", !| LockInfo 
                "pending", !| LockInfo 
            ]
            Optional = []
        }

    let LockRequestOptions =
        Pattern.Config "LockRequestOptions" {
            Required = []
            Optional = [
                "mode", T<string> 
                "ifAvailable", T<bool> 
                "steal", T<bool> 
                "signal", T<Dom.AbortSignal> 
            ]
        }

    let LockManager =

        let callbackFunc = T<obj>?callback ^-> T<Promise<unit>>

        Class "LockManager"
        |+> Instance [
            "request" => T<string>?name * !?LockRequestOptions?options * callbackFunc?callback ^-> T<Promise<unit>> 
            
            "query" => T<unit> ^-> T<Promise<_>>[LockManagerQueryResult] 
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.WebLocks" [
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
