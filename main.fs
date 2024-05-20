module psp.main

open System.IO
open Parser
open Interpreter

let testFileName = "sample.psp"
let text = File.ReadAllText testFileName
let res1 = testExpression text
let (_, res2) = evalProgram(res1, Map.empty)
