# WebSharper Web Locks API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Web Locks API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Locks_API), allowing WebSharper applications to coordinate access to resources across multiple scripts.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Web Locks API.

2. **Sample Project**:
   - Demonstrates how to use the Web Locks API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/WebLocks/).

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.WebLocks
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/WebLocks.git
   cd WebLocks
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.WebLocks/WebSharper.WebLocks.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.WebLocks.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action:
   [https://dotnet-websharper.github.io/WebLocks/](https://dotnet-websharper.github.io/WebLocks/)

## Example Usage

Below is an example of how to use the Web Locks API in a WebSharper project:

```fsharp
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

    // Variable to track the lock status
    let statusMessage = Var.Create "Lock status: Not acquired"

    // Function to acquire a lock using the Web Locks API
    let acquireLock () =
        promise {
            let locks = As<Navigator>(JS.Window.Navigator).Locks
            do! locks.Request("my-lock", fun _ ->
                promise {
                    // Update UI to indicate the lock has been acquired
                    statusMessage.Value <- "Lock acquired!"
                    Console.Log("Lock acquired, doing some work...")

                    // Simulate some work (3-second delay)
                    do! Async.Sleep 3000

                    // Release the lock and update the status
                    Console.Log("Work done, releasing lock...")
                    statusMessage.Value <- "Lock released!"
                } |> As<Promise<unit>>
            )
        }

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate.Main()
            // Display lock status in the UI
            .Status(statusMessage.V)
            // Handle button click to acquire the lock
            .AcquireLock(fun _ ->
                async {
                    do! acquireLock() |> Promise.AsAsync
                }
                |> Async.StartImmediate
            )
            .Doc()
        |> Doc.RunById "main"
```

This example demonstrates how to acquire and release a lock using the Web Locks API in WebSharper.

## Important Considerations

- **Ensuring Safe Resource Access**: The Web Locks API helps coordinate shared resources and prevents race conditions.
- **Lock Scope**: Locks are limited to the same origin and cannot be shared across different tabs or windows.
- **Graceful Lock Handling**: Always ensure locks are released after work is completed to prevent deadlocks.
