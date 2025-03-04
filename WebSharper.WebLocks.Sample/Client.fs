namespace WebSharper.WebLocks.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.WebLocks

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let statusMessage = Var.Create "Lock status: Not acquired"

    let acquireLock () =
        promise {
            let locks = As<Navigator>(JS.Window.Navigator).Locks
            do! locks.Request("my-lock", fun _ ->
                promise {
                    statusMessage.Value <- "Lock acquired!"
                    Console.Log("Lock acquired, doing some work...")

                    // Simulate some work (3-second delay)
                    do! Async.Sleep 3000

                    Console.Log("Work done, releasing lock...")
                    statusMessage.Value <- "Lock released!"
                } |> As<Promise<unit>>
            )
        }

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate.Main()
            .Status(statusMessage.V)
            .AcquireLock(fun _ ->
                async {
                    do! acquireLock() |> Promise.AsAsync
                }
                |> Async.StartImmediate
            )
            .Doc()
        |> Doc.RunById "main"
