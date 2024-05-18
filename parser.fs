open FParsec

type Expr =
    | Number of int
    | Variable of string
    | Bool of bool
    | Let of string * Expr
    | Function of string * string list * Expr list
    | Apply of Expr * Expr list
    | If of Expr * Expr list * Expr list
    | For of Expr * Expr * Expr list
    | Call of Expr
    | Return of string * Expr list
    | Sequence of Expr list

let numberParser : Parser<Expr, unit> =
    pint32 |>> Number

let identifierParser : Parser<string, unit> =
    many1Satisfy isLetter .>> spaces |>> System.String

let rec exprParser input =
    let term = letParser <|> functionOrCallParser <|> forParser <|> ifParser <|> returnParser <|> variableParser <|> numberParser
    chainl1 term opParser input

and letParser : Parser<Expr, unit> =
    pipe2
        (pstring "let" .>> spaces >>. identifierParser)
        (pchar '=' .>> spaces >>. exprParser)
        (fun varName expr -> Let(varName, expr))

and exprSequenceParser : Parser<Expr list, unit> =
    sepEndBy1 exprParser (pchar ';' .>> spaces)

and programParser : Parser<Expr, unit> =
    exprSequenceParser |>> Sequence
and argumentsParser: Parser<Expr list, unit> =
   ((between (pchar '[' .>> spaces) (pchar ']' .>> spaces))(sepBy exprParser (pchar ',' .>> spaces)))
    
and argsParser: Parser<string list, unit> =
   ((between (pchar '[' .>> spaces) (pchar ']' .>> spaces)) (sepBy identifierParser (pchar ',' .>> spaces)))
    
and bodyParser: Parser<Expr list, unit> =
     ((between (pchar '{' .>> spaces) (pchar '}' .>> spaces)) (sepBy exprParser (pchar ';' .>> spaces)))

and ifParser: Parser<Expr, unit> =
    pipe4
        (pstring "if" .>> spaces >>. exprParser)
        (bodyParser)
        (pstring "else")
        (bodyParser)
        (fun expr ifBody _ elseBody -> If(expr, ifBody, elseBody))

and returnParser: Parser<Expr, unit> =
    pipe2
        (pstring "ret" .>> spaces >>. identifierParser)
        (argumentsParser)
        (fun funcName args -> Return(funcName, args))
        
and functionParser : Parser<Expr, unit> =
    pipe3
        (pstring "func" .>> spaces >>. many1Satisfy isLetter .>> spaces |>> System.String)
        (argsParser)
        (bodyParser)
        (fun funcName args body -> Function(funcName, args, body))

and callParser : Parser<Expr, unit> =
    pipe2
        (pstring "call" .>> spaces)
        (exprParser)
        (fun _ ide -> Call(ide))

and functionOrCallParser : Parser<Expr, unit> =
    choice [
        functionParser
        callParser
    ]

and forParser : Parser<Expr, unit> =
    pipe5
        (pstring "for" .>> spaces)
        (identifierParser .>> spaces)
        (pstring "=" .>> spaces >>. exprParser .>> spaces)
        (pstring "to" .>> spaces >>. exprParser .>> spaces)
        (bodyParser)
        (fun _ _ start end_ body -> For(start, end_, body))

and variableParser : Parser<Expr, unit> =
    identifierParser |>> Variable

and opParser : Parser<(Expr -> Expr -> Expr), unit> =
    choice [
        pchar '+' >>% (fun x y -> Apply(Variable("+"), [x; y]))
        pchar '-' >>% (fun x y -> Apply(Variable("-"), [x; y]))
        pchar '*' >>% (fun x y -> Apply(Variable("*"), [x; y]))
        pchar '/' >>% (fun x y -> Apply(Variable("/"), [x; y]))
        pchar '<' >>% (fun x y -> Apply(Variable("<"), [x; y]))
        pchar '>' >>% (fun x y -> Apply(Variable(">"), [x; y]))
        pstring "<=" >>% (fun x y -> Apply(Variable("<="), [x; y]))
        pstring ">=" >>% (fun x y -> Apply(Variable(">="), [x; y]))
        pstring "!=" >>% (fun x y -> Apply(Variable("!="), [x; y]))
        pstring "||" >>% (fun x y -> Apply(Variable("||"), [x; y]))
        pstring "&&" >>% (fun x y -> Apply(Variable("&&"), [x; y]))
        pstring "==" >>% (fun x y -> Apply(Variable("=="), [x; y]))
        ]

let testExpression input =
    printfn "%A" input
    match run programParser input with
    | Success(result, _, _) ->
        printfn "Parsed expression: %A" result
    | Failure(errorMsg, _, _) ->
        printfn "Failed to parse expression: %s" errorMsg

// Тестовые примеры
testExpression "let x = 103;
let e = 105"
testExpression "func double[x, y]{
let k = x*2;
call ret double[x, y-1]*n}"
testExpression "let e = ret double[1, 2]"
testExpression "func factorial[n] {
    if n==0{
        call 1}
    else{call ret factorial[n-1]*n
    }}"
testExpression "if x>0{
        let e=0;
        let t=x+y}
        else{
            let re = ty}"
testExpression "for i = 1 to 10{
let k = 10+5;
let t = i+2}"
testExpression "let e = 10+12+2"