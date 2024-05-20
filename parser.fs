module Parser

open FParsec

type Id = string

type Expr =
    | Int of int
    | Bool of bool
    | Var of string
    | Let of string * Expr
    | MathOperation of string
    | Function of Id * string list * Expr list
    | Apply of Expr * Expr list
    | If of Expr * Expr list * Expr list
    | For of Expr * Expr * Expr list
    | Return of Expr
    | Call of Id * Expr list
    | Sequence of Expr list
    | Print of string
    | Read_int
and
    env = Map<Id, Expr>
let IntParser : Parser<Expr, unit> =
    pint32 |>> Int

let identifierParser : Parser<string, unit> =
    many1Satisfy isLetter .>> spaces |>> System.String

let rec exprParser input =
    let term = letParser <|> functionOrCallParser <|> forParser <|> ifParser <|> callParser <|> returnParser <|> printParser <|> readIntParser <|> VarParser <|> IntParser
    chainl1 term opParser input

and letParser : Parser<Expr, unit> =
    pipe2
        (pstring "let" .>> spaces >>. identifierParser)
        (pchar '=' .>> spaces >>. exprParser)
        (fun varName expr -> Let(varName, expr))

and printParser: Parser<Expr, unit> =
    pipe2
        (pstring "print" >>. spaces)
        (identifierParser)
        (fun _ varName -> Print(varName))

and readIntParser: Parser<Expr, unit> =
    pstring "read_int()" >>. spaces >>% Read_int

and exprSequenceParser : Parser<Expr list, unit> =
    sepEndBy1 exprParser (spaces)

and programParser : Parser<Expr, unit> =
    exprSequenceParser |>> Sequence
and argumentsParser: Parser<Expr list, unit> =
   (between (pchar '[' .>> spaces) (pchar ']' .>> spaces))(sepBy exprParser (pchar ',' .>> spaces))
    
and argsParser: Parser<string list, unit> =
   ((between (pchar '[' .>> spaces) (pchar ']' .>> spaces)) (sepBy identifierParser (pchar ',' .>> spaces)))
    
and bodyParser: Parser<Expr list, unit> =
     ((between (pchar '{' .>> spaces) (pchar '}' .>> spaces)) (sepBy exprParser (pchar ';' .>> spaces)))

and ifParser: Parser<Expr, unit> =
    pipe4
        (pstring "if" .>> spaces >>. exprParser)
        bodyParser
        (pstring "else")
        bodyParser
        (fun expr ifBody _ elseBody -> If(expr, ifBody, elseBody))

and  callParser: Parser<Expr, unit> =
    pipe2
        (pstring "call" .>> spaces >>. identifierParser)
        argumentsParser
        (fun funcName args -> Call(funcName, args))
        
and functionParser : Parser<Expr, unit> =
    pipe3
        (pstring "func" .>> spaces >>. many1Satisfy isLetter .>> spaces |>> System.String)
        argsParser
        bodyParser
        (fun funcName args body -> Function(funcName, args, body))

and returnParser : Parser<Expr, unit> =
    pipe2
        (pstring "return" .>> spaces)
        exprParser
        (fun _ ide -> Return(ide))

and functionOrCallParser : Parser<Expr, unit> =
    choice [
        functionParser
        returnParser
    ]

and forParser : Parser<Expr, unit> =
    pipe4
        (pstring "for" .>> spaces)
        (exprParser .>> spaces)
        (pstring "to" .>> spaces >>. exprParser .>> spaces)
        (bodyParser)
        (fun _ start end_ body -> For(start, end_, body))
        
and VarParser : Parser<Expr, unit> =
    identifierParser |>> Var

and opParser : Parser<Expr -> Expr -> Expr, unit> =
    choice [
        pchar '+' >>% (fun x y -> Apply(MathOperation("+"), [x; y]))
        pchar '-' >>% (fun x y -> Apply(MathOperation("-"), [x; y]))
        pchar '*' >>% (fun x y -> Apply(MathOperation("*"), [x; y]))
        pchar '/' >>% (fun x y -> Apply(MathOperation("/"), [x; y]))
        pchar '<' >>% (fun x y -> Apply(MathOperation("<"), [x; y]))
        pchar '>' >>% (fun x y -> Apply(MathOperation(">"), [x; y]))
        pstring "<=" >>% (fun x y -> Apply(MathOperation("<="), [x; y]))
        pstring ">=" >>% (fun x y -> Apply(MathOperation(">="), [x; y]))
        pstring "!=" >>% (fun x y -> Apply(MathOperation("!="), [x; y]))
        pstring "||" >>% (fun x y -> Apply(MathOperation("||"), [x; y]))
        pstring "&&" >>% (fun x y -> Apply(MathOperation("&&"), [x; y]))
        pstring "==" >>% (fun x y -> Apply(MathOperation("=="), [x; y]))
        ]
    
let testExpression input =
    match run programParser input with
    | Success(result, _, _) ->
           match result with
           | Sequence(list) -> list
           | _ -> failwith("Пизда всему")
    | Failure(errorMsg, _, _) ->
           failwith("Пизда всему")        

// Тестовые примеры
//testExpression "let x = 103;
//let e = 105;
// print e"
// testExpression "func double[x, y]{
// let k = x*2;
// call ret double[x, y-1]*n}"
// testExpression "let e = ret double[1, 2]"
// testExpression "func factorial[n] {
//     if n==0{
//         call 1}
//     else{call ret factorial[n-1]*n
//     }}"
// testExpression "if x>0{
//         let e=0;
//         let t=x+y}
//         else{
//             let re = ty}"
// testExpression "for i = 1 to 10{
// let k = 10+5;
// let t = i+2}"
// testExpression "let e = 10+12+2"