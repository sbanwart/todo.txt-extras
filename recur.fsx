//match fsi.CommandLineArgs with
//    | [| scriptname; msg; |] ->
//        printfn "running script: %s" scriptname
//        printfn "messages: %s" msg
//    | _ ->
//        printfn "USAGE: [message]"
open System
open System.IO

// tests for calculating target date
//let z = "tHURSDAY"
//let d = z.Substring(0, 1).ToUpper() + z.Substring(1).ToLower()
//
//let y = Enum.Parse(typeof<DayOfWeek>, d) :?> DayOfWeek
//match y = DateTime.Now.DayOfWeek with
//    | true -> printfn "True"
//    | false -> printfn "False"
// end tests

let inputFile = @"C:\Work\todo.txt-extras\recur.txt"
let delimiter = " "

match fsi.CommandLineArgs with
| _ -> 
    let fileReader fileName = 
        seq { 
            use fileReader = new StreamReader(File.OpenRead(fileName))
            while not fileReader.EndOfStream do
                yield fileReader.ReadLine()
        }
        
    let getLine =
        let lines = fileReader inputFile
        lines
        |> Seq.map (fun line -> line.Substring(0, line.IndexOf(delimiter)), line.Substring(line.IndexOf(delimiter) + 1))
        |> Seq.choose (fun t ->
            match t with
            | (f, r) when f = "daily" -> Some(r)
            | (f, r : string) when f = "weekly" ->
                let day = r.Substring(0, r.IndexOf(delimiter))
                let task = r.Substring(r.IndexOf(delimiter) + 1)
                if day = "saturday" then Some(task) else None
            | _ -> None )
    
    let tasks = getLine

    tasks
    |> Seq.iter (fun t -> printfn "Description: %s" t)