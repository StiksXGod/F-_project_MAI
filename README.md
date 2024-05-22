# PCP 1.0 documentation
Welcome, this is official documentation for PCP 1.0. 




# PCP
PCP is a functional language made by students [Kovalenko Alexander](https://github.com/Serohon), [Bachurin Pavel](https://github.com/Sleeps17) and [Mudrov Pavel](https://github.com/StiksXGod) of the Moscow Aviation Institute group M8O-203B-22.
* Turing complete
* Original syntax
* Made using the [FParsec](http://www.quanttec.com/fparsec/) library
* Lazy evaluation implemented

# About PCP team 

## Essay - [GitHub Pages](https://stiksxgod.github.io/fucnProg_githubPages/)<br>
Kovalenko Alexander - Moscow Aviation Institute M80-203B-22 group, parser part<br>
Bachurin Pavel - Moscow Aviation Institute M80-203B-22 group, interpreter part<br>
Mudrov Pavel - Moscow Aviation Institute M80-203B-22 group, GitHub Pages and documentation part<br>


# Table of contents
- [PCP 1.0 documentation](#pcp-10-documentation)
- [PCP](#pcp)
- [About PCP team](#about-pcp-team)
  - [Essay - GitHub Pages](#essay---github-pages)
- [Table of contents](#table-of-contents)
- [Brief Syntax PCP](#brief-syntax-pcp)
- [Syntax PCP](#syntax-pcp)
  - [List of language keywords](#list-of-language-keywords)
  - [Data types](#data-types)
  - [Math operators](#math-operators)
  - [Logical operators](#logical-operators)
  - [Declaration of variables](#declaration-of-variables)
    - [Example:](#example)
  - [If construction](#if-construction)
    - [Example:](#example-1)
  - [Loop  for](#loop--for)
    - [Example:](#example-2)
  - [Declaration function](#declaration-function)
    - [Example:](#example-3)
  - [Call function](#call-function)
  - [Convert to  AST](#convert-to--ast)
  - [Lazy evaluation](#lazy-evaluation)



# Brief Syntax PCP

| Type | Example |
|------------|-------------|
| [declaration](#declaration-of-variables) | let <variable_name\>| 
| [initialization and declaration](#declaration-of-variables) | let <variable_name\> = <expression\>|
| [initialization and declaration function](#declaration-function) | func <function_name> <arg1_name> <arg2_name> ...  {  
||  ..."some expressions"
||  }
| [function call](#call-function)  | let var = call fact 10 | 
| [if statement](#if-construction)  | if <bool_type var name>{
||….”some expressions”;
||}elif <bool_type var name>{
||….”some expressions”;
||}
|[for construction](#loop-for)| for <iterator_variable_name> = <start_value> to <end_value>  {
||..."some expression"    
||}
||
 # Syntax PCP
## List of language keywords

 - ``let`` - used to declare variables
 - ``if`` - used to declare a conditional construct
 - ``for`` - used to declare a loop
 - ``func`` - used to declare a function
 - ``call`` - used to call functions
 - ``parint`` - used for console output

## Data types
```
string, int, float, bool
```

## Math operators
 -  The ``+`` operator - performs the addition of two variables of the same numeric type:
    ```
    let <result_expr> = <expr1> + <expr2>
    ```
 - Operator ``-`` - performs subtraction of two variables of the same numeric type:
    ```
    let <result_expr> = <expr1> - <expr2> 
    ```
 - Operator ``*`` - performs multiplication of two variables of the same numeric type:
    ```
    let <result_expr> = <expr1> * <expr2> 
    ```
 
 - Operator ``/`` - performs division of two variables of the same numeric type:
    ```
    let <result_expr> = <expr1> / <expr2> 
    ```

## Logical operators
 - Operator ``&&`` - performs logical AND:
    ```
    let <result_expr> = <expr1> && <expr2> 
    ```
 - Operator ``||`` - performs logical OR:
    ```
    let <result_expr> = <expr1> || <expr2> 
    ```

## Declaration of variables
```
let <variable_name> = <expression>
```
### Example:
```F#
let var1 = 100
let var2 = "Hello, World!"
let var3 = true 
let var4 = 4 + 5
let var5 = 4.2 + 7.5 + 5.1
```

## If construction
```
if <expression> {
    ...
}
```
### Example:
```
if x == 0{
   x = 1
}
```

## Loop  for
```
for <iterator_variable_name> to <end_value>{
    <start_value> to <end_value>
    ...
}
```
### Example:
```
let sum = 0

for i = 1 to 10 {
    sum = sum + i
}
```

## Declaration function
```
func <function_name> <arg1_name> <arg2_name> ... {
    ..."some expression"
}
```
### Example:
```
func fact[n] {            
    if n==0{
        return 1}
    else{
        return n*call fact[n-1]}     
}

let n = read_int[]
print n

let f = call fact[n]
print f 
```

## Call function
```
let var = call fact[10]
```
## Convert to  AST
Let's look at a simple psp program and its appearance in an abstract syntax tree.
```
func sum a b = a + b

let a = 100
let b = 20

print call sum a b
```
## Lazy evaluation

```
| Let(id, letExpr) ->
        match letExpr with
        | Nothing -> failwith("this function does not return anything")
        | _ -> 
            let newEnv = Map.add id letExpr env
            (Nothing, newEnv)
```
This technique is used to optimize performance and improve program efficiency. It avoids unnecessary calculations and therefore saves resources.Lazy evaluation allows you to work with infinite data structures, such as infinite lists, without fear of memory overflow, since only the necessary part of the data structure will be evaluated.