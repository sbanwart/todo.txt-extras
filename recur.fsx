//match fsi.CommandLineArgs with
//    | [| scriptname; msg; |] ->
//        printfn "running script: %s" scriptname
//        printfn "messages: %s" msg
//    | _ ->
//        printfn "USAGE: [message]"
open System
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

    let dayOfWeek (input : string) =
        let day = input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower()
        Enum.Parse(typeof<DayOfWeek>, day) :?> DayOfWeek

    let processDateMacro (input : string) =
        let macrostart = input.IndexOf("%date")
        if macrostart > 0 then
            let macroend = input.IndexOf("%", macrostart + 1)
            let macro = input.Substring(macrostart, (macroend - macrostart) + 1)

            let operator = input.Substring(macrostart + 5, 1)
            if operator = "+" then
                let additionalDays = Double.Parse(input.Substring(input.IndexOf(operator, macrostart) + 1, macroend - macrostart - 6))
                input.Replace(macro, DateTime.Now.AddDays(additionalDays).ToString("yyyy-MM-dd"))
            else
                input.Replace(macro, DateTime.Now.ToString("yyyy-MM-dd"))
        else
            input
        
    let getLine =
        let lines = fileReader inputFile
        lines
        |> Seq.map (fun line -> line.Substring(0, line.IndexOf(delimiter)), processDateMacro(line.Substring(line.IndexOf(delimiter) + 1)))
        |> Seq.choose (fun t ->
            match t with
            | (f, r) when f = "daily" -> Some(r)
            | (f, r : string) when f = "weekly" ->
                let day = r.Substring(0, r.IndexOf(delimiter))
                let task = r.Substring(r.IndexOf(delimiter) + 1)
                if dayOfWeek(day) = DateTime.Now.DayOfWeek then Some(task) else None
            | _ -> None )
    
    let tasks = getLine

    tasks
    |> Seq.iter (fun t -> printfn "Description: %s" t)