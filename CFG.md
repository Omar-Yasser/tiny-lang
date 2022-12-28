# Tiny CFG

## Terminals
 
`(`, `)`, `{`, `}`, `;` , `,` , `+`, `-`, `*`, `/` , `:=` , `<` , `>` , `=` , `<>` , `&&` , `||` , `return identifier`, `main()`, `read`, `endl`, `write`, `int`, `float`, `string`, `number`, `if`, `then`, `end`, `else`, `elseif`, `repeat`, `until`
 
## Non Terminals
 
`Program`,`Functions`,`Main_Function`,`Statements`,`State`,`Write_Statement`,`Read_Statement`,`Declaration_Statement`, 
`Assignment_Statement`,`If_Statement`,`Repeat_Statement `,`Function_Call `,`Datatype`,`Function_Statement` `Function_Declaration`,`Parameters`,`Parameter`,`Function_Body`,`Return_Statement`,`Args `,`IdList `,
`Write_Expression`,`Expression `,`Datatype `,`DecState `,`Expression`,`Term `,`Arithmatic_Operator`,`Equation`,
`Operand`,`SubEq`,`Eq`,`Equations `,`Condition_Statement`,`IfState `,`Else_Statment`,`Else_If_Statment `,`Condition `,`condition_operator `,`Condition_Expression`,`Boolean_Operator`


## Rules

#### Program
 
1. Program → Functions Main_Function
2. Statements → State Statements
3. State → Write_Statement |Read_Statement | Declaration_Statement | Assignment_Statement |  \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; If_Statement | Repeat_Statement |  Function_Call | ε
 
#### Functions
 
4. Main_Function → Datatype main() Function_Body
5. Functions → Function_Statement Functions | ε
6. Function_Statement  → Function_Declaration Function_Body
7. Function_Declaration  → Datatype identifier (Parameters)
	
8. Parameters → Parameter Param | ε
9.  Param → , Parameter Param | ε
10. Parameter → Datatype identifier
 
#### Function Body
 
11. Function_Body → { Statements Return_Statement }
12. Return_Statement → return Expression;
 
#### Function Call
 
13. Function_Call → identifier (Args)
14. Args → IdList | ε
15. IdList → identifier Id
16.	Id → , identifier Id | ε


#### Write & read
 
17. Write_Statement → write Write_Expression;
18. Write_Expression→ Expression | endl
19. Read_Statement → read identifier;

#### Variables
 
20. Declaration_Statement → Datatype Vars_Declartion;
21. Datatype → int | float | string
22. Vars_Declartion → identifier Initialization 
23. Initialization → := Expression | ε 
24. Declarations → , identifier Initialization Declarations | ε
25. Assignment_Statement → identifier := Expression
26. Expression → string | Term | Equation
27. Term → number | identifier | Function_Call
28. Arithmatic_Operator → + | - | * | /
 
#### Equations
 
29. Equation → Operand Equations 
30. Operand → Term SubEq | (Equation) SubEq 
31. SubEq → Arithmatic_Operator Eq SubEq | ε
32. Eq → Term | (Equation)
33. Equations → Arithmatic_Operator Oprand Equations | ε
 
 
#### If, If else, else if
 
34. If_Statement → if Condition_Statement then Statements IfState
35. IfState → Else_If_Statment | Else_Statment | end
36. Else_Statment → else Statements end
37. Else_If_Statment → elseif Condition_Statement then Statements IfState
 
#### Repeat
 
38. Repeat_Statement → repeat Statements until Condition_Statement
 
#### Conditions
 
39. Condition_Statement → Condition Condition_Expression
40. Condition → identifier Condition_Operator Term
41. Condition_Expression → Boolean_Operator Condition Condition_Expression| ε
42. Condition_Operator → < | > | = | <>
43. Boolean_Operator  → && | ||