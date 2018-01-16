open System
open Expecto
open IfSharp.Kernel.App
open IfSharp.Kernel

open NetMQ
open NetMQ.Sockets

let tests =
    testList "Network tests" [
        test "Heartbeat test" {

            //Run full kernel

            async {
                App.Start([|@"sample.json"|])
            }
            |> Async.StartAsTask
            |> ignore

            let hbSocket = new DealerSocket()
            do hbSocket.Connect("tcp://127.0.0.1:14910") //Configure this to the heartbeat matching the json configuration

            hbSocket.SendFrame("Hello world")

            let subject = hbSocket.ReceiveFrameString()
    
            Expect.equal subject "Hello world" "The strings should equal"

            App.Stop()
        }

        test "Heartbeat test (repeat to demonstrate sockets clear)" {

            async {
                App.Start([|@"sample.json"|])
            }
            |> Async.StartAsTask
            |> ignore

            let hbSocket = new DealerSocket()
            do hbSocket.Connect("tcp://127.0.0.1:14910") //Configure this to the heartbeat matching the json configuration

            hbSocket.SendFrame("Hello new world")

            let subject = hbSocket.ReceiveFrameString()
    
            Expect.equal subject "Hello new world" "The strings should equal"

            App.Stop()
        }
    ]


[<EntryPoint>]
let main args = 

    let config = {defaultConfig with parallel = false } //Single use of ports, instead should have a pool
    runTestsWithArgs config args tests
