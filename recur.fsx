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

open System
open System.IO

let delimiter = " "

match fsi.CommandLineArgs with
| [| scriptname; todoFile; recurFile |] ->
    let fileReader fileName = 
        seq { 
            use fileReader = new StreamReader(File.OpenRead(fileName))
            while not fileReader.EndOfStream do
                yield fileReader.ReadLine()
        }

    let dayOfWeek (input : string) =
        let day = input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower()
        Enum.Parse(typeof<DayOfWeek>, day) :?> DayOfWeek

    let getMonthAsNumber (input : string) =
        match input.ToLower() with
        | "january" -> 1
        | "february" -> 2
        | "march" -> 3
        | "april" -> 4
        | "may" -> 5
        | "june" -> 6
        | "july" -> 7
        | "august" -> 8
        | "september" -> 9
        | "october" -> 10
        | "november" -> 11
        | "december" -> 12
        | _ -> 0

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

    let rec findFirstDay (workingDate : DateTime, day : string) : int =
        if workingDate.DayOfWeek = dayOfWeek(day) then workingDate.Day else findFirstDay(workingDate.AddDays(1.0), day)

    let rec findLastDay (workingDate : DateTime, day : string) : int =
        if workingDate.DayOfWeek = dayOfWeek(day) then workingDate.Day else findFirstDay(workingDate.AddDays(-1.0), day)

    let dayMatches occurrence day =
        let currentDate = System.DateTime.Now
        match occurrence with
        | "first" ->
            let workingDate = new System.DateTime(currentDate.Year, currentDate.Month, 1)
            let dayOfMonth = findFirstDay(workingDate, day)
            if currentDate.Day = dayOfMonth then true else false
        | "second" ->
            let workingDate = new System.DateTime(currentDate.Year, currentDate.Month, 1)
            let dayOfMonth = findFirstDay(workingDate, day)
            if currentDate.Day = dayOfMonth + 7 then true else false
        | "third" ->
            let workingDate = new System.DateTime(currentDate.Year, currentDate.Month, 1)
            let dayOfMonth = findFirstDay(workingDate, day)
            if currentDate.Day = dayOfMonth + 14 then true else false
        | "fourth" ->
            let workingDate = new System.DateTime(currentDate.Year, currentDate.Month, 1)
            let dayOfMonth = findFirstDay(workingDate, day)
            if currentDate.Day = dayOfMonth + 21 then true else false
        | "last" ->
            let workingDate = new System.DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
            let dayOfMonth = findFirstDay(workingDate, day)
            if currentDate.Day = dayOfMonth then true else false 
        | _ -> false 
        
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
            | (f, r) when f = "weekdays" ->
                let today = DateTime.Now
                if today.DayOfWeek <> DayOfWeek.Saturday && today.DayOfWeek <> DayOfWeek.Sunday then Some(r) else None
            | (f, r) when f = "weekends" ->
                let today = DateTime.Now
                if today.DayOfWeek = DayOfWeek.Saturday || today.DayOfWeek = DayOfWeek.Sunday then Some(r) else None
            | (f, r : string) when f = "weekly" ->
                let day = r.Substring(0, r.IndexOf(delimiter))
                let task = r.Substring(r.IndexOf(delimiter) + 1)
                if dayOfWeek(day) = DateTime.Now.DayOfWeek then Some(task) else None
            | (f, r) when f = "monthly" -> 
                let occurrence = r.Substring(0, r.IndexOf(delimiter))
                let temp = r.Substring(r.IndexOf(delimiter) + 1)
                let day = temp.Substring(0, temp.IndexOf(delimiter))
                let task = temp.Substring(temp.IndexOf(delimiter) + 1)
                if dayMatches occurrence day then Some(task) else None
            | (f, r) when f = "yearly" ->
                let currentDate = System.DateTime.Now
                let month = getMonthAsNumber(r.Substring(0, r.IndexOf(delimiter)))
                let temp = r.Substring(r.IndexOf(delimiter) + 1)
                let day = Int32.Parse(temp.Substring(0, temp.IndexOf(delimiter)))
                let task = temp.Substring(temp.IndexOf(delimiter) + 1)
                if month = currentDate.Month && day = currentDate.Day then Some(task) else None
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

| _ ->
    printfn "recur.fsx 1.0.0"
    printfn "usage: recur.fsx <path to todo.txt> <path to recur.txt>"