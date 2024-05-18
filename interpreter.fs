module Interpreter

open Parser
open System.IO

let funof = function
    | "+" -> 
        (function 
            | [Int(a); Int(b)] -> Int(a+b)
            | _ -> failwith "Invalid arguments for +"
        )
    | "-" -> 
        (function 
            | [Int(a); Int(b)] -> Int(a-b)
            | _ -> failwith "Invalid arguments for -"
        )
    | "*" -> 
        (function 
            | [Int(a); Int(b)] -> Int(a*b)
            | _ -> failwith "Invalid arguments for *"
        )
    | "/" -> 
        (function 
            | [Int(a); Int(b)] -> Int(a/b)
            | _ -> failwith "Invalid arguments for /"
        )
    | "<" -> 
        (function 
            | [Int(a); Int(b)] -> Bool(a<b)
            | _ -> failwith "Invalid arguments for <"
        )
    | ">" -> 
        (function 
            | [Int(a); Int(b)] -> Bool(a>b)
            | _ -> failwith "Invalid arguments for >"
        )
    | "<=" -> 
        (function 
            | [Int(a); Int(b)] -> Bool(a<=b)
            | _ -> failwith "Invalid arguments for <="
        )
    | ">=" -> 
        (function 
            | [Int(a); Int(b)] -> Bool(a>=b)
            | _ -> failwith "Invalid arguments for >="
        )
    | "!=" -> 
        (function 
            | [Int(a); Int(b)] -> Bool(a<>b)
            | _ -> failwith "Invalid arguments for !="
        )
    | "==" -> 
        (function 
            | [Int(a); Int(b)] -> Bool(a=b)
            | _ -> failwith "Invalid arguments for =="
        )
    | "||" -> 
        (function 
            | [Bool(a); Bool(b)] -> Bool(a||b)
            | _ -> failwith "Invalid arguments for ||"
        )
    | "&&" -> 
        (function 
            | [Bool(a); Bool(b)] -> Bool(a&&b)
            | _ -> failwith "Invalid arguments for &&"
        )
    | _ -> failwith "Unknown operation"

let rec eval (expr: Expr, env: Map<Id, Expr>): (Expr * Map<Id, Expr>) =
    // printfn $"%A{expr}"
    match expr with
    | Int(x) -> (Int(x), env)
    
    | Bool(x) -> (Bool(x), env)
    
    | Var(id) -> 
        match Map.tryFind id env with
        | Some(value) -> eval (value, env)
        | None -> failwith $"Variable '%s{id}' not found"
        
    | Let(id, letExpr) ->
        match Map.tryFind id env with
        | Some(_) -> failwith("redeclaration of variable")
        | None ->
            let (value, env) = eval(letExpr, env)
            let newEnv = Map.add id value env
            // Не важно, что будет в типе Expr возвращаемого значение
            // От Let ожидается, только обновление окружения
            (value, newEnv)
            
    | Function(id, argsNames, body) ->
        match Map.tryFind id env with
        | Some(value) -> failwith("redeclaration of function") 
        | None ->
            failwith("NOT IMPLEMENTED")
            
    | Apply(ex1, args) ->
        let evaluatedArgs = (List.map (fun arg -> eval(arg, env)) args) |> List.map fst 
        let result = apply (ex1, env, evaluatedArgs)
        (result, env)
        
    | If(condExpr, thenExprs, elseExprs) ->
        let (flag, _) = eval(condExpr, env)
        match flag with
        // TODO: add nothing returning value
        | Bool(true) -> (condExpr, evalProgram(thenExprs, env))
        | Bool(false) -> (condExpr, evalProgram(elseExprs, env)) 
        | _ -> failwith "Condition must evaluate to a boolean value"
        
    | For(initExpr, condExpr, bodyExprs) ->
        failwith "For not implemented yet"
        
    | Call(id, args) ->
        match Map.tryFind id env with
        | Some(value) ->
            failwith("")
        | None -> failwith("unknown name of function")
        
    | Return(ex) ->
        failwith "Return not implemented yet"
        
    | _ -> failwith("unknown key word")
    

and apply (statement: Expr, env: Map<Id, Expr>, args: Expr list): (Expr) =
    match statement with
    | MathOperation(op) -> (funof op) args
    | _ -> failwith("unknown operation")
    

and evalProgram (exprs: Expr list, env: Map<Id, Expr>) : Map<Id, Expr> =
    match exprs with
    | [] -> env
    | expr :: rest ->
        let (_, newEnv) = eval (expr, env)
        evalProgram (rest, newEnv)
    

       
    