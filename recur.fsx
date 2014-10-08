// Copyright 2014 Scott Banwart
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

//match fsi.CommandLineArgs with
//    | [| scriptname; msg; |] ->
//        printfn "running script: %s" scriptname
//        printfn "messages: %s" msg
//    | _ ->
//        printfn "USAGE: [message]"
open System
open System.IO

let recurFile = @"C:\Work\todo.txt-extras\recur.txt"
let todoFile = @"C:\Work\todo.txt-extras\todo.txt"
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

    let rec processDateMacro (input : string, macrostart : int) = 
        if macrostart > 0 then
            let macroend = input.IndexOf("%", macrostart + 1)
            let macro = input.Substring(macrostart, (macroend - macrostart) + 1)

            let operator = input.Substring(macrostart + 5, 1)
            if operator = "+" then
                let additionalDays = Double.Parse(input.Substring(input.IndexOf(operator, macrostart) + 1, macroend - macrostart - 6))
                let newTask = input.Replace(macro, DateTime.Now.AddDays(additionalDays).ToString("yyyy-MM-dd"))
                let nextMacro = newTask.IndexOf("%date")
                processDateMacro(newTask, nextMacro)
            else
                let newTask = input.Replace(macro, DateTime.Now.ToString("yyyy-MM-dd"))
                let nextMacro = newTask.IndexOf("%date")
                processDateMacro(newTask, nextMacro)
        else
            input
        
    let getLine =
        let lines = fileReader recurFile
        lines
        |> Seq.map (fun line ->
            let task = line.Substring(line.IndexOf(delimiter) + 1)
            let macrostart = task.IndexOf("%date")
            line.Substring(0, line.IndexOf(delimiter)), processDateMacro(task, macrostart))
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
    |> Seq.choose(fun t ->
        let mutable keep = true
        use fileReader = new StreamReader(File.OpenRead(todoFile))
        while not fileReader.EndOfStream do
            let line = fileReader.ReadLine()
            if t = line then 
                keep <- false
        match keep with
        | true -> Some(t)
        | false -> None)
    |> Seq.iter (fun t -> 
        use fileWriter = new StreamWriter(File.Open(todoFile, FileMode.Append))
        fileWriter.WriteLine(t))
