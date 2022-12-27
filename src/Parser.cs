using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Header());
            program.Children.Add(DeclSec());
            program.Children.Add(Block());
            program.Children.Add(match(Token_Class.Dot));
            MessageBox.Show("Success");
            return program;
        }

        // Node Header()
        // {
        //     Node header = new Node("Header");
        //     // write your code here to check the header sructure
        //     return header;
        // }
        // Node DeclSec()
        // {
        //     Node declsec = new Node("DeclSec");
        //     // write your code here to check atleast the declare sturcure 
        //     // without adding procedures
        //     return declsec;
        // }
        // Node Block()
        // {
        //     Node block = new Node("block");
        //     // write your code here to match statements
        //     return block;
        // }

        // Implement your logic here

        //Statements → State Statements
        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(State());
            statements.Children.Add(Statements());
            return statements;
        }

        //State → Write_Statement |Read_Statement |
        //  7 Declaration_Statement | 4 Assignment_Statement |
        //  1 If_Statement | 2 Repeat_Statement |          
        //  3 Function_Call | ε
        Node State()
        {
            if(!(InputPointer < TokenStream.size)) return null;

            Node state =new Node("State");
            switch (TokenStream[InputPointer].token_type)
            {
                case Token_Class.Write:
                    state.Children.Add(Write_Statement());
                    break;
                case Token_Class.Read:
                    state.Children.Add(Read_Statement());
                    break;
                case Token_Class.If:
                    state.Children.Add(If_Statement());
                    break;
                case Token_Class.Repeat:
                    state.Children.Add(Repeat_Statement());
                    break;
                case Token_Class.Identifier:
                    //Function_Call → identifier (Args)
                    //Assignment_Statement → identifier := Expression
                    if(InputPointer + 1 < TokenStream && TokenStream[InputPointer+1].token_type == Token_Class.LParanthesis)
                        state.Children.Add(Function_Call());
                    else
                        state.Children.Add(Assignment_Statement());
                    break;
                case Token_Class.Write:
                    state.Children.Add(Declaration_Statement());
                    break;
                default:
                    if(!isDatatype()) return null;
                    state.Children.Add(Declaration_Statement());
            }

            return state;
        }

        //Main_Function → Datatype main() Function_Body
        Node Main_Function()
        {
            Node mainFunction = new Node("Main_Function");
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match.Token_Class.Main);
            mainFunction.Children.Add(match.Token_Class.LParanthesis);
            mainFunction.Children.Add(match.Token_Class.RParanthesis);
            mainFunction.Children.Add(Function_Body());
            return mainFunction;
        }

        //Functions → Function_Statement Functions | ε
        Node Functions()
        {
            Node functions = new Node("Functions");
            if (InputPointer < TokenStream.size && isDatatype())
            {
                functions.Children.Add(Function_Statement());
                functions.Children.Add(Functions());
                return functions;
            }
            else
            {
                return null;
            }
        }

        //Function_Statement → Function_Declaration Function_Body
        Node Function_Statement()
        {
            Node functionStatement = new Node("Function_Statement");
            functionStatement.Children.Add(Function_Declaration());
            functionStatement.Children.Add(Function_Body());
            return functionStatement;
        }

        //Function_Declaration → Datatype identifier (Parameters)
        Node Function_Declaration()
        {
            Node functionDeclaration = new Node("Function_Declaration");
            functionDeclaration.Children.Add(Datatype());
            functionDeclaration.Children.Add(match.Token_Class.Identifier);
            functionDeclaration.Children.Add(match.Token_Class.LParanthesis);
            functionDeclaration.Children.Add(Parameters());
            functionDeclaration.Children.Add(match.Token_Class.RParanthesis);
            return functionDeclaration;
        }

        //Parameters → Parameter Param | ε
        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            if(InputPointer < TokenStream.size && isDatatype())
            {
            parameters.Children.Add(Parameter());
            parameters.Children.Add(Param());
            return parameters;
            }
            else
            {
                return null;
            }
        }

        //Program → Functions Main_Function
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Functions());
            program.Children.Add(Main_Function());
        }

        //Statements → State Statements
        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(State());
            statements.Children.Add(Statements());
            return statements;
        }

        //State → Write_Statement |Read_Statement |
        //  7 Declaration_Statement | 4 Assignment_Statement |
        //  1 If_Statement | 2 Repeat_Statement |          
        //  3 Function_Call | ε
        Node State()
        {
            if(!(InputPointer < TokenStream.size)) return null;

            Node state =new Node("State");
            switch (TokenStream[InputPointer].token_type)
            {
                case Token_Class.Write:
                    state.Children.Add(Write_Statement());
                    break;
                case Token_Class.Read:
                    state.Children.Add(Read_Statement());
                    break;
                case Token_Class.If:
                    state.Children.Add(If_Statement());
                    break;
                case Token_Class.Repeat:
                    state.Children.Add(Repeat_Statement());
                    break;
                case Token_Class.Identifier:
                    //Function_Call → identifier (Args)
                    //Assignment_Statement → identifier := Expression
                    if(InputPointer + 1 < TokenStream && TokenStream[InputPointer+1].token_type == Token_Class.LParanthesis)
                        state.Children.Add(Function_Call());
                    else
                        state.Children.Add(Assignment_Statement());
                    break;
                case Token_Class.Write:
                    state.Children.Add(Declaration_Statement());
                    break;
                default:
                    if(!isDatatype()) return null;
                    state.Children.Add(Declaration_Statement());
            }

            return state;
        }

        //Main_Function → Datatype main() Function_Body
        Node Main_Function()
        {
            Node mainFunction = new Node("Main_Function");
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match.Token_Class.Main);
            mainFunction.Children.Add(match.Token_Class.LParanthesis);
            mainFunction.Children.Add(match.Token_Class.RParanthesis);
            mainFunction.Children.Add(Function_Body());
            return mainFunction;
        }

        //Functions → Function_Statement Functions | ε
        Node Functions()
        {
            Node functions = new Node("Functions");
            if (InputPointer < TokenStream.size && isDatatype())
            {
                functions.Children.Add(Function_Statement());
                functions.Children.Add(Functions());
                return functions;
            }
            else
            {
                return null;
            }
        }

        //Function_Statement → Function_Declaration Function_Body
        Node Function_Statement()
        {
            Node functionStatement = new Node("Function_Statement");
            functionStatement.Children.Add(Function_Declaration());
            functionStatement.Children.Add(Function_Body());
            return functionStatement;
        }

        //Function_Declaration → Datatype identifier (Parameters)
        Node Function_Declaration()
        {
            Node functionDeclaration = new Node("Function_Declaration");
            functionDeclaration.Children.Add(Datatype());
            functionDeclaration.Children.Add(match.Token_Class.Identifier);
            functionDeclaration.Children.Add(match.Token_Class.LParanthesis);
            functionDeclaration.Children.Add(Parameters());
            functionDeclaration.Children.Add(match.Token_Class.RParanthesis);
            return functionDeclaration;
        }

        //Parameters → Parameter Param | ε
        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            if(InputPointer < TokenStream.size && isDatatype())
            {
            parameters.Children.Add(Parameter());
            parameters.Children.Add(Param());
            return parameters;
            }
            else
            {
                return null;
            }
        }

        //Param → , Parameter Param | ε
        Node Param()
        {
            Node param = new Node("Param");
            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                param.Children.Add(match.Token_Class.Comma);
                param.Children.Add(Parameter());
                param.Children.Add(Param());
                return param;
            }
            else
            {
                return null;
            }
        }

        //Parameter → Datatype identifier
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match.Token_Class.Identifier);
            return parameter;
        }

        //Function_Body → { Statements Return_Statement }
        Node Function_Body()
        {
            Node functionBody = new Node("Function_Body");
            functionBody.Children.Add(match.Token_Class.LBrace);
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(Return_Statement());
            functionBody.Children.Add(match.Token_Class.RBrace);
            return functionBody;
        }

        //Return_Statement → return Expression;
        Node Return_Statement()
        {
            Node returnStatement = new Node("Return_Statement");
            returnStatement.Children.Add(match.Token_Class.Return);
            returnStatement.Children.Add(Expression());
            return returnStatement;
        }

        //Function_Call → identifier (Args)
        Node Function_Call()
        {
            Node functionCall = new Node("Function_Call");
            functionCall.Children.Add(match.Token_Class.Identifier);
            functionCall.Children.Add(match.Token_Class.LParanthesis);
            functionCall.Children.Add(Args());
            functionCall.Children.Add(match.Token_Class.RParanthesis);
        }

        //Args → IdList | ε
        Node Args()
        {
            Node args = new Node("Args");
            if (InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                args.Children.Add(IdList);
                return args;
            }
            else
            {
                return null;
            }
        }

        //IdList → identifier Id
        Node IdList()
        {
            Node idList = new Node("IdList");
            idList.Children.Add(match.Token_Class.Idenifier);
            idList.Children.Add(Id());
            return idList;
        }

        //Id → , identifier Id | ε
        Node Id()
        {
            Node id = new Node("Id");
            if (InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                id.Children.Add(match.Token_Class.Comma);
                id.Children.Add(match.Token_Class.Idenifier);
                id.Children.Add(Id());
                return id;
            }
            else
            {
                return null;
            }
        }

        // Write_Statement → write Write_Expression;
        Node Write_Statement()
        {
            Node writeStatement = new Node("Write_Statement");
            functionCall.Children.Add(match.Token_Class.Write);
            functionCall.Children.Add(Write_Expression());
            functionCall.Children.Add(match.Token_Class.Semicolon); 
            return writeStatement;
        }

        //  Write_Expression → Expression | endl
        Node Write_Expression()
        {
            Node writeExpression = new Node("Write_Expression");

            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                functionCall.Children.Add(match.Token_Class.Endl);
            }
            else 
            {
                functionCall.Children.Add(Expression());
            }
            return writeExpression;
        }

        // Read_Statement → read identifier;
        Node Read_Statement()
        {
            Node readStatement = new Node("Read_Statement");

            functionCall.Children.Add(match.Token_Class.Read);
            functionCall.Children.Add(match.Token_Class.Idenifier); 
            functionCall.Children.Add(match.Token_Class.Semicolon); 
            return readStatement;
        }

        // Declaration_Statement → Datatype DecState;
        Node Declaration_Statement()
        {
            Node declarationStatement = new Node("Declaration_Statement");
            functionCall.Children.Add(Datatype());
            functionCall.Children.Add(DecState());
            functionCall.Children.Add(match.Token_Class.Semicolon);
            return declarationStatement;
        }

        // Datatype → int | float | string
        Node Datatype()
        {
            Node datatype = new Node("DataType");
            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                functionCall.Children.Add(match.Token_Class.Int);
            }
            else if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                 functionCall.Children.Add(match.Token_Class.Float);
            }
            else
            {
                functionCall.Children.Add(match.Token_Class.String);
            }
            return datatype;
        }

        // DecState → IdList, DecState | Assignment_Statement, DecState
        Node DecState()
        {
            Node decState = new Node("DecState");
            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                functionCall.Children.Add(Assignment_Statement());
                functionCall.Children.Add(match.Token_Class.Comma);
                functionCall.Children.Add(DecState());
            }
            else
            {
                functionCall.Children.Add(IdList());
                functionCall.Children.Add(match.Token_Class.Comma);
                functionCall.Children.Add(DecState());
            }
            return decState;
        }

        // Assignment_Statement → identifier := Expression
        Node Assignment_Statement()
        {
            Node assignmentStatement = new Node("Assignment_Statement");
            functionCall.Children.Add(match.Token_Class.Idenifier);
            functionCall.Children.Add(match.Token_Class.AssignmentOp);
            functionCall.Children.Add(Expression());
            return assignmentStatement;
        }

        // Expression → string | Term | Equation
        Node Expression()
        {
            Node expression = new Node("Expression");
            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                functionCall.Children.Add(match.Token_Class.String);
            }
            else if(InputPointer < TokenStream.size &&  isTerm())
            {
                functionCall.Children.Add(Term());
            }
            else
            {
               functionCall.Children.Add(Equation);            
            }
            return expression;
        }

        // Term → number | identifier | Function_Call
        Node Term()
        {
            Node term = new Node("Term");
            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                functionCall.Children.Add(match.Token_Class.Number);
            }
            else if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                functionCall.Children.Add(match.Token_Class.Identifier);
            }
            else
            {
               functionCall.Children.Add(Function_Call());            
            }
            return term;
        }

        // Arithmatic_Operator → + | - | * | /
        Node Arithmatic_Operator()
        {
            Node arithmaticOperator = new Node("Arithmatic_Operator");
            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                functionCall.Children.Add(match.Token_Class.PlusOp);
            }
            else if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                functionCall.Children.Add(match.Token_Class.MinusOp);
            }
            else if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                functionCall.Children.Add(match.Token_Class.MultiplyOp);
            }
            else
            {
                functionCall.Children.Add(match.Token_Class.DivideOp);
            }
            return arithmaticOperator;
        }

        // Equation → Operand Equations
        Node Equation()
        {
            Node equation = new Node("Equation");
            functionCall.Children.Add(Operand());
            functionCall.Children.Add(Equations());

            return equation;
        }

        // Operand → Term SubEq | (Equation) SubEq 
        Node Operand()
        {
            Node operand = new Node("Operand");

            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                functionCall.Children.Add(match.Token_Class.LParanthesis);
                functionCall.Children.Add(Equation());
                functionCall.Children.Add(match.Token_Class.RParanthesis);
                functionCall.Children.Add(SubEq ());
            }
            else
            {
                functionCall.Children.Add(Term());
                functionCall.Children.Add(SubEq());
            }

            return operand;
        }

        //SubEq → Arithmatic_Operator Eq SubEq | ε
        Node SubEq()
        {
            Node subEq = new Node("SubEq");
            if(InputPointer < TokenStream.size && isArithmaticOperator())
            {
                functionCall.Children.Add(Arithmatic_Operator());
                functionCall.Children.Add(Eq());
                functionCall.Children.Add(SubEq());
                return subEq;
            }
            else
            {
                return null;
            }
        }

        // Eq → Term | (Equation)
        Node Eq()
        {
            Node eq = new Node("Eq");

            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                functionCall.Children.Add(match.Token_Class.LParanthesis);
                functionCall.Children.Add(Equation());
                functionCall.Children.Add(match.Token_Class.RParanthesis);
            }
            else
            {
                functionCall.Children.Add(Term());
            }

            return eq;
        }

        // Equations → Arithmatic_Operator Oprand Equations | ε 
        Node Equations()
        {
            Node equations = new Node("Equations");

            if(InputPointer < TokenStream.size && isArithmaticOperator())
            {
                functionCall.Children.Add(Arithmatic_Operator());
                functionCall.Children.Add(Oprand());
                functionCall.Children.Add(Equations());
                return equations;
            }
            else
            {
                return null;
            }
        }

        // If_Statement  → if Condition_Statement then Statements IfState
        Node If_Statement()
        {
            Node IfStatement = new Node("If_Statement");
            functionCall.Children.Add(match.Token_Class.if);
            functionCall.Children.Add(Condition_Statement());
            functionCall.Children.Add(match.Token_Class.then);
            functionCall.Children.Add(Statements());
            functionCall.Children.Add(IfState());

            return IfStatement;
        }

        // IfState → Else_If_Statment | Else_Statment | end
        Node IfState()
        {
            Node  IfState = new Node("IfState");

            if(InputPointer < TokenStream.size && (TokenStream[InputPointer].token_type == Token_Class.Elseif))
            {
                functionCall.Children.Add(Else_If_Statment());
            }
            else if(InputPointer < TokenStream.size && (TokenStream[InputPointer].token_type == Token_Class.Else))
            {
                functionCall.Children.Add(Else_Statment());
            }
            else 
            {
                functionCall.Children.Add(match.Token_Class.end);
            }

            return IfState;
        }

        //  Else_Statment → else Statements end
        Node Else_Statment()
        {
            Node ElseStatment = new Node("Else_Statment");
            functionCall.Children.Add(match.Token_Class.else);
            functionCall.Children.Add(Statements());
            functionCall.Children.Add(match.Token_Class.end);
            return ElseStatment;
        }

        // Else_If_Statment → elseif Condition_Statement then Statements IfState
        Node Else_If_Statment()
        {
            Node ElseIfStatment = new Node("Else_If_Statment");
            functionCall.Children.Add(match.Token_Class.Elseif)
            functionCall.Children.Add(Condition_Statement());
            functionCall.Children.Add(match.Token_Class.then);
            functionCall.Children.Add(Statements());
            functionCall.Children.Add(IfState());

            return ElseIfStatment;
        }

        // Repeat_Statement → repeat Statements until Condition_Statement
        Node Repeat_Statement()
        {
            Node RepeatStatement = new Node("Repeat_Statement");
            functionCall.Children.Add(match.Token_Class.repeat);
            functionCall.Children.Add(Statements());
            functionCall.Children.Add(match.Token_Class.until);
            functionCall.Children.Add(Condition_Statement());

            return RepeatStatement;
        }

        // Condition_Statement → Condition Condition_Expression
        Node Condition_Statement()
        {
            Node conditionStatement = new Node("Condition_Statement");
            functionCall.Children.Add(Condition());
            functionCall.Children.Add(Condition_Expression());

            return conditionStatement;
        }

        // Condition → identifier Condition_Operator Term 
        Node Condition()
        {
            Node condition = new Node("Condition");
            functionCall.Children.Add(match.Token_Class.identifier);
            functionCall.Children.Add(Condition_Operator());
            functionCall.Children.Add(Term());

            return condition;
        }

        // Condition_Expression → Boolean_Operator Condition Condition_Expression| ε 
        Node Condition_Expression()
        {
            Node ConditionExpression= new Node("Condition_Expression");

            if(InputPointer < TokenStream.size && isBooleanOperator())
            {
                functionCall.Children.Add(Boolean_Operator());
                functionCall.Children.Add(Condition());
                functionCall.Children.Add(Condition_Expression());
                return ConditionExpression;
            }
            else
            {
                return NULL;
            }
        }

        // Boolean_Operator  → && | ||
        Node Boolean_Operator()
        {
            Node BooleanOperator = new Node("Boolean_Operator");

            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.And)
            {
                functionCall.Children.Add(match.Token_Class.And);
            }
            else
            {
                functionCall.Children.Add(match.Token_Class.Or);
            }

            return BooleanOperator;
        }

        // Condition_Operator → < | > | = | <>
        Node Condition_Operator()
        {
            Node ConditionOperator = new Node("Condition_Operator");

            if(InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                functionCall.Children.Add(match.Token_Class.LessThanOp);
            }
            else if (InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                functionCall.Children.Add(match.Token_Class.GreaterThanOp);
            }
            else if (InputPointer < TokenStream.size && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                functionCall.Children.Add(match.Token_Class.EqualOp);
            }
            else
            {
                functionCall.Children.Add(match.Token_Class.NotEualOp);
            }

            return ConditionOperator;
        }

        bool isDatatype()
        {
           return (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Float);
        }

        bool isArithmaticOperator()
        {
            return (TokenStream[InputPointer].token_type == Token_Class.PlusOp || 
                    TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                    TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                    TokenStream[InputPointer].token_type == Token_Class.DivideOp);
        }

        //  && | ||
        bool isBooleanOperator()
        {
            return (TokenStream[InputPointer].token_type == Token_Class.Or || TokenStream[InputPointer].token_type == Token_Class.And );
        }

        // number | identifier | Function_Call
        bool isTerm()
        {
            return (TokenStream[InputPointer].token_type == Token_Class.Number || 
                    TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                    TokenStream[InputPointer].token_type == Token_Class.Function_Call);
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
