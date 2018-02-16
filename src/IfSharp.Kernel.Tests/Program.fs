open System
open Expecto
open IfSharp.Kernel.App
open IfSharp.Kernel

open NetMQ
open NetMQ.Sockets

let tests =
    
    //TODO: read these from file instead of hardcode
    let shellAddress = "tcp://127.0.0.1:14906"
    let hearbeatAddress = "tcp://127.0.0.1:14910"

    testList "Network tests" [
        test "Heartbeat test" {

            //Run full kernel

            async {
                App.Start([|@"sample.json"|])
            }
            |> Async.StartAsTask
            |> ignore

            let hbSocket = new DealerSocket()
            do hbSocket.Connect hearbeatAddress

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
            do hbSocket.Connect hearbeatAddress

            hbSocket.SendFrame("Hello new world")

            let subject = hbSocket.ReceiveFrameString()
    
            Expect.equal subject "Hello new world" "The strings should equal"

            App.Stop()
        }

        (*test "Simple adding (captured Jupyter sample)" {

            async {
                App.Start([|@"sample.json"|])
            }
            |> Async.StartAsTask
            |> ignore

            let shellSocket = new DealerSocket()
            do shellSocket.Connect shellAddress

            //Captured from Jupyter
            let message =
                [|
                    "BDF38CE001D04BEA843E9E9DD166AB72";
                    "<IDS|MSG>";
                    "62b055968c15cc058bf9e358632db2614618b913bbae879ad8fbbc12d5e1f7be";
                    """{"msg_id":"B6A6A99CF36E41A7948B1C8CA14D55BF","username":"username","session":"F74BFEF6229D441B8B5F6DC6D5AE7B42","msg_type":"execute_request","date":"2018-01-19T16:59:54.021467Z"}"""
                    "{}"
                    "{}"
                    """{"code":"5+5","silent":false,"store_history":true,"user_expressions":{},"allow_stdin":true,"stop_on_error":true,"user_variables":[]}"""
                |]
                |> Seq.map NetMQFrame
                |> NetMQMessage

            shellSocket.SendMultipartMessage message

            let target = Unchecked.defaultof<NetMQMessage>
            let success = shellSocket.TryReceiveMultipartMessage(TimeSpan.FromSeconds (5.0), ref target)

            //let x = shellSocket.TryReceive(

            //let subject = hbSocket.ReceiveFrameString()
    
            //Expect.equal subject "Hello new world" "The strings should equal"

            App.Stop()
        }*)
    ]


[<EntryPoint>]
let main args = 

    let config = {defaultConfig with parallel = false } //Single use of ports, instead should have a pool
    runTestsWithArgs config args tests
