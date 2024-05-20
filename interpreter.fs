module Interpreter

open Parser

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

let printTypeofExpr (expr: Expr, env: Map<Id, Expr>) =
    match Map.tryFind "n" env with
    | Some(value) -> printf $"%A{value} "
    | None -> printf "none n "
    match expr with
    | Int(x) -> printfn $"Type: int %A{x}"
    | Bool _ -> printfn "Type: bool"
    | Nothing -> printfn "Type: Nothing"
    | Var id -> printfn $"Type: Var %A{id}"
    | Let (id, _) -> printfn $"Type: Let %A{id}"
    | MathOperation id -> printfn $"Type: MathOperation %A{id}"
    | Function _ -> printfn "Type: Function"
    | Apply (id, args) -> printfn $"Type: Apply %A{id} %A{args}"
    | If _ -> printfn "Type: If"
    | For _ -> printfn "Type: For"
    | Return _ -> printfn "Type: Return"
    | Call _ -> printfn "Type: Call"
    | Sequence _ -> printfn "Type: Sequence"
    | Print _ -> printfn "Type: Print"
    | Read_int -> printfn "Type: Read_int" 

let rec eval (expr: Expr, env: Map<Id, Expr>): Expr * Map<Id, Expr> =
    // printTypeofExpr(expr, env)
    match expr with
    | Int(x) -> (Int(x), env)
    
    | Bool(x) -> (Bool(x), env)
    
    | Var(id) -> 
        match Map.tryFind id env with
        | Some(value) -> eval (value, env)
        | None -> failwith $"Variable '%s{id}' not found"
        
    | Let(id, letExpr) ->
        let value, _ = eval(letExpr, env)
        match value with
        | Nothing -> failwith("this function does not return anything")
        | _ -> 
            let newEnv = Map.add id value env
            (Nothing, newEnv)
            
    | Function(id, argsNames, body) ->
        match Map.tryFind id env with
        | Some(value) -> failwith("redeclaration of function") 
        | None ->
            let f = Function(id, argsNames, body)
            let newEnv = Map.add id f env
            (Nothing, newEnv)            
    | Apply(ex1, args) ->
        let evaluatedArgs = (List.map (fun arg -> eval(arg, env)) args) |> List.map fst 
        let result = apply (ex1, env, evaluatedArgs)
        (result, env)
        
    | If(condExpr, thenExprs, elseExprs) ->
        let flag, _ = eval(condExpr, env)
        match flag with
        | Bool(true) -> evalProgram(thenExprs, env)
        | Bool(false) -> evalProgram(elseExprs, env) 
        | _ -> failwith "Condition must evaluate to a boolean value"
        
    | For(startExpr, endExpr, bodyExprs) ->
        match startExpr with
        | Let(varName, value) ->
            let (evaluatedStartExpr, _) = eval(value, env)
            let (evaluatedEndExpr, _) = eval(endExpr, env)
            let newEnv = Map.add varName evaluatedStartExpr env
            let rec evalFor(_startExpr: Expr, _endExpr: Expr, _env: Map<Id, Expr>) =
                let (condition, _) = eval(Apply(MathOperation("<="), [_startExpr; _endExpr]), _env)
                match condition with
                | Bool(false) -> (Nothing, _env)
                | Bool(true) ->
                    let _, _newEnv = evalProgram(bodyExprs, _env)
                    match Map.tryFind varName _newEnv with
                    | Some(value) -> evalFor(value, _endExpr, _newEnv)
                    | None -> failwith("Опять какая-то залупа случилась")
                | _ -> failwith("Хуйню какую-то передали")
            let _, envForReturn = evalFor(evaluatedStartExpr, evaluatedEndExpr, newEnv)
            let lastEnv = Map.remove varName envForReturn
            (Nothing, lastEnv)
        | _ ->
            failwith("Unknown expression in for initialization")
            
    | Call(id, args) ->
        match Map.tryFind id env with
        | Some(func) ->
            match func with
            | Function(_, argsNames, body) ->
                if argsNames.Length <> args.Length then
                    failwith("incorrect number of arguments")
                else
                    let evaluatedArgs = args |> List.map (fun expr -> eval(expr, env)) |> List.map (fun (expr, _) -> expr)
                    let newEnv = List.zip argsNames evaluatedArgs |> List.fold (fun map (key, value) -> Map.add key value map) env                                          
                    let returnedValue, lastEnv = evalFunction(body, newEnv)
                    (returnedValue, lastEnv)
            | _ -> failwith("unknown type of function")
        | None -> failwith("unknown name of function")
        
    | Return(ex) ->
        let (evaluatedEx, _) = eval(ex, env)
        (Return(evaluatedEx), env) 
    
    | Print(varName) ->
        match Map.tryFind varName env with
        | Some(value) ->
            match value with
            | Int(v) ->
                printfn $"%d{v}"
            | Bool(v) ->
                printf $"%b{v}"
            | _ ->
                failwith("unknown type to print")
        | None ->
            failwith("unknown variable name")
        
        (Nothing, env)
    | Nothing -> (Nothing, env)
    | _ -> failwith("unknown key word")
    
and evalFunction(body: Expr list, env: Map<Id, Expr>): Expr * Map<Id, Expr> =
    match body with
    | [] -> (Nothing, env)
    | expr::tail ->
        let (ex, newEnv) = eval(expr, env)
        match ex with
        | Return(value) -> (value, env)
        | _ -> evalFunction(tail, newEnv) 

and apply (statement: Expr, env: Map<Id, Expr>, args: Expr list): (Expr) =
    match statement with
    | MathOperation(op) -> (funof op) args
    | _ -> failwith("unknown operation")

and evalProgram (exprs: Expr list, env: Map<Id, Expr>) : Expr * Map<Id, Expr> =
    match exprs with
    | [] ->
        Nothing, env
    | expr :: rest ->
        let (ex, newEnv) = eval (expr, env)
        match ex with
        | Return(value) -> (Return(value), env)
        | _ -> evalProgram (rest, newEnv)
           