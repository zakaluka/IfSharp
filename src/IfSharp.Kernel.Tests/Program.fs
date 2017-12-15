open System
open Expecto
open IfSharp.Kernel.App
open IfSharp.Kernel

open NetMQ
open NetMQ.Sockets

let tests =
  test "A simple test" {

    async {
        App.Start([|@"sample.json"|])
    } |> Async.StartAsTask

    let hbSocket = new DealerSocket()
    do hbSocket.Connect("tcp://127.0.0.1:14910")

    hbSocket.SendFrame("Hello world")

    let subject = hbSocket.ReceiveFrameString()
    
    Expect.equal subject "Hello world" "The strings should equal"
  }

[<EntryPoint>]
let main args = 
    runTestsWithArgs defaultConfig args tests
