module psp.main

open System.IO
open Parser
open Interpreter

let testFileName = "sample.psp"
let text = File.ReadAllText testFileName
let res1 = testExpression text

printfn $"%A{res1}"
let res2 = evalProgram(res1, Map.empty)
// printfn $"%A{res2}"