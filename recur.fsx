//match fsi.CommandLineArgs with
//    | [| scriptname; msg; |] ->
//        printfn "running script: %s" scriptname
//        printfn "messages: %s" msg
//    | _ ->
//        printfn "USAGE: [message]"
open System.IO

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
        let line = fileReader inputFile
        line
        |> Seq.map (fun line -> line.Substring(0, line.IndexOf(delimiter)), line.Substring(line.IndexOf(delimiter) + 1))
        |> Seq.iter (fun t -> 
               match t with
               | (f, r) when f = "daily" -> printfn "Frequency: %s, Remainder: %s" f r
               | (f, r) when f = "weekly" -> printfn "Frequency: %s, Remainder: %s" f r
               | (f, r) -> printfn "Invalid frequency")
    
    getLine
//let entries = "daily Enter time into CP"
//let t = entries.Substring(0, entries.IndexOf(delimiter))
//printfn "token: %s" t
