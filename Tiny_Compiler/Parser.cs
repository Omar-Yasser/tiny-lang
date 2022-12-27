using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

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

        //Program → Functions Main_Function
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Functions());
            program.Children.Add(Main_Function());
            return program;
        }

        //Statements → State Statements
        Node Statements()
        {
            // base case
            if (!(InputPointer < TokenStream.Count)) return null;
            Node statements = new Node("Statements");
            statements.Children.Add(State());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Return && TokenStream[InputPointer].token_type != Token_Class.Until && TokenStream[InputPointer].token_type != Token_Class.Elseif && TokenStream[InputPointer].token_type != Token_Class.Else && TokenStream[InputPointer].token_type != Token_Class.End)
            {
                statements.Children.Add(Statements());
            }
            return statements;
        }

        //State → 1 Write_Statement | 2 Read_Statement |
        //  7 Declaration_Statement | 4 Assignment_Statement |
        //  3 If_Statement | 2 Repeat_Statement |          
        //  3 Function_Call | ε
        Node State()
        {
            if(!(InputPointer < TokenStream.Count)) return null;

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
                case Token_Class.Idenifier:
                    //Function_Call → identifier (Args)
                    //Assignment_Statement → identifier := Expression
                    if(InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.LParanthesis)
                        state.Children.Add(Function_Call());
                    else
                        state.Children.Add(Assignment_Statement());
                    break;
                default:
                    state.Children.Add(Declaration_Statement());
                    break;
                    //default:
                    //    if(!isDatatype()) return null;
                    //state.Children.Add(Declaration_Statement());
            }

            return state;
        }

        //Main_Function → Datatype main() Function_Body
        Node Main_Function()
        {
            Node mainFunction = new Node("Main_Function");
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match(Token_Class.Main));
            mainFunction.Children.Add(match(Token_Class.LParanthesis));
            mainFunction.Children.Add(match(Token_Class.RParanthesis));
            mainFunction.Children.Add(Function_Body());
            return mainFunction;
        }

        //Functions → Function_Statement Functions | ε
        Node Functions()
        {
            Node functions = new Node("Functions");
            if (InputPointer < TokenStream.Count && isDatatype() && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
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
            functionDeclaration.Children.Add(match(Token_Class.Idenifier));
            functionDeclaration.Children.Add(match(Token_Class.LParanthesis));
            functionDeclaration.Children.Add(Parameters());
            functionDeclaration.Children.Add(match(Token_Class.RParanthesis));
            return functionDeclaration;
        }

        //Parameters → Parameter Param | ε
        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            if(InputPointer < TokenStream.Count && isDatatype())
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
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                param.Children.Add(match(Token_Class.Comma));
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
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }

        //Function_Body → { Statements Return_Statement }
        Node Function_Body()
        {
            Node functionBody = new Node("Function_Body");
            functionBody.Children.Add(match(Token_Class.LBrace));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Return)
                functionBody.Children.Add(Statements());
            functionBody.Children.Add(Return_Statement());
            functionBody.Children.Add(match(Token_Class.RBrace));
            return functionBody;
        }

        //Return_Statement → return Expression;
        Node Return_Statement()
        {
            Node returnStatement = new Node("Return_Statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));
            return returnStatement;
        }

        //Function_Call → identifier (Args)
        Node Function_Call()
        {
            Node functionCall = new Node("Function_Call");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LParanthesis));
            if (TokenStream[InputPointer].token_type != Token_Class.RParanthesis)
                functionCall.Children.Add(Args());
            functionCall.Children.Add(match(Token_Class.RParanthesis));
            return functionCall;
        }

        //Args → IdList | ε
        Node Args()
        {
            Node args = new Node("Args");
            if (InputPointer < TokenStream.Count && isTerm())
            {
                args.Children.Add(ArgList());
                return args;
            }
            else
            {
                return null;
            }
        }

        Node ArgList()
        {
            Node idList = new Node("ArgList");
            if (isFunctionCall())
                idList.Children.Add(Args());
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                idList.Children.Add(match(Token_Class.Idenifier));
            else if (TokenStream[InputPointer].token_type == Token_Class.Constant)
                idList.Children.Add(match(Token_Class.Constant));
            idList.Children.Add(Arg());
            return idList;
        }

        //Id → , identifier Id | ε
        Node Arg()
        {
            Node arg = new Node("Arg");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arg.Children.Add(match(Token_Class.Comma));
                if (isFunctionCall())
                    arg.Children.Add(Args());
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                    arg.Children.Add(match(Token_Class.Idenifier));
                else if (TokenStream[InputPointer].token_type == Token_Class.Constant)
                    arg.Children.Add(match(Token_Class.Constant));
                
                arg.Children.Add(Arg());
                return arg;
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
            idList.Children.Add(match(Token_Class.Idenifier));
            idList.Children.Add(Id());
            return idList;
        }

        //Id → , identifier Id | ε
        Node Id()
        {
            Node id = new Node("Id");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                id.Children.Add(match(Token_Class.Comma));
                id.Children.Add(match(Token_Class.Idenifier));
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
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(Write_Expression());
            writeStatement.Children.Add(match(Token_Class.Semicolon)); 
            return writeStatement;
        }

        //  Write_Expression → Expression | endl
        Node Write_Expression()
        {
            Node writeExpression = new Node("Write_Expression");

            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                writeExpression.Children.Add(match(Token_Class.Endl));
            }
            else 
            {
                writeExpression.Children.Add(Expression());
            }
            return writeExpression;
        }

        // Read_Statement → read identifier;
        Node Read_Statement()
        {
            Node readStatement = new Node("Read_Statement");

            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            readStatement.Children.Add(match(Token_Class.Semicolon)); 
            return readStatement;
        }

        // Declaration_Statement → Datatype DecState;
        Node Declaration_Statement()
        {
            Node declarationStatement = new Node("Declaration_Statement");
            declarationStatement.Children.Add(Datatype());
            declarationStatement.Children.Add(DecState());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            return declarationStatement;
        }

        // Datatype → int | float | string
        Node Datatype()
        {
            Node datatype = new Node("DataType");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else
            {
                datatype.Children.Add(match(Token_Class.String));
            }
            return datatype;
        }

        // DeclarationStatement → DataType VarsDeclartion;
        // VarsDeclartion → identifier Initialization Declartions
        // Initialization → := Expression | ε
        // Declartions → , identifier Initialization Declartions | ε
        Node DecState()
        {
            Node decState = new Node("DecState");
            //Debug.Assert(InputPointer < TokenStream.Count);
            if (!(InputPointer < TokenStream.Count) || TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                return decState;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
            {
                decState.Children.Add(AssignState());    
            }
            else
            {
                decState.Children.Add(IdList());
            }
            decState.Children.Add(DecState());
            return decState;
        }

        Node AssignState()
        {
            Node assignmentStatement = new Node("Assignment_Statement");
            assignmentStatement.Children.Add(match(Token_Class.Idenifier));
            assignmentStatement.Children.Add(match(Token_Class.AssignmentOp));
            assignmentStatement.Children.Add(Expression());
            return assignmentStatement;
        }

        // Assignment_Statement → identifier := Expression
        Node Assignment_Statement()
        {
            Node assignmentStatement = AssignState();
            assignmentStatement.Children.Add(match(Token_Class.Semicolon));
            return assignmentStatement;
        }
        bool chk()
        {
            int read = 1;
            for(int i = InputPointer; i < TokenStream.Count; ++i)
            {
                if (TokenStream[i].token_type == Token_Class.Semicolon) break;
                if (isFunctionCall(i)) read = 0;
                if (read == 0 && TokenStream[i].token_type == Token_Class.RParanthesis) read = 1;
                if (read == 1 && isArithmaticOperator(i)) return true;
            }
            return false;
        }
        // Expression → string | Term | Equation
        Node Expression()
        {
            Node expression = new Node("Expression");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else if(chk())
            {
                expression.Children.Add(Equation());
            }
            else
            {
                expression.Children.Add(Term());
            }
            return expression;
        }

        // Term → number | identifier | Function_Call
        Node Term()
        {
            Node term = new Node("Term");
            if (isFunctionCall())
            {
                term.Children.Add(Function_Call());
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                term.Children.Add(match(Token_Class.Constant));
            }
            else
            {
                term.Children.Add(match(Token_Class.Idenifier));
            }
            return term;
        }

        // Arithmatic_Operator → + | - | * | /
        Node Arithmatic_Operator()
        {
            Node arithmaticOperator = new Node("Arithmatic_Operator");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.PlusOp));
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.MinusOp));
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arithmaticOperator.Children.Add(match(Token_Class.MultiplyOp));
            }
            else
            {
                arithmaticOperator.Children.Add(match(Token_Class.DivideOp));
            }
            return arithmaticOperator;
        }

        // Equation → Operand Equations
        Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(Operand());
            equation.Children.Add(Equations());

            return equation;
        }

        // Operand → Term SubEq | (Equation) SubEq 
        Node Operand()
        {
            Node operand = new Node("Operand");

            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                operand.Children.Add(match(Token_Class.LParanthesis));
                operand.Children.Add(Equation());
                operand.Children.Add(match(Token_Class.RParanthesis));
                operand.Children.Add(SubEq ());
            }
            else
            {
                operand.Children.Add(Term());
                operand.Children.Add(SubEq());
            }

            return operand;
        }

        //SubEq → Arithmatic_Operator Eq SubEq | ε
        Node SubEq()
        {
            Node subEq = new Node("SubEq");
            if(InputPointer < TokenStream.Count && isArithmaticOperator())
            {
                subEq.Children.Add(Arithmatic_Operator());
                subEq.Children.Add(Eq());
                subEq.Children.Add(SubEq());
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

            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                eq.Children.Add(match(Token_Class.LParanthesis));
                eq.Children.Add(Equation());
                eq.Children.Add(match(Token_Class.RParanthesis));
            }
            else
            {
                eq.Children.Add(Term());
            }

            return eq;
        }

        // Equations → Arithmatic_Operator Operand Equations | ε 
        Node Equations()
        {
            Node equations = new Node("Equations");

            if(InputPointer < TokenStream.Count && isArithmaticOperator())
            {
                equations.Children.Add(Arithmatic_Operator());
                equations.Children.Add(Operand());
                equations.Children.Add(Equations());
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
            IfStatement.Children.Add(match(Token_Class.If));
            IfStatement.Children.Add(Condition_Statement());
            IfStatement.Children.Add(match(Token_Class.Then));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Return && TokenStream[InputPointer].token_type != Token_Class.Until && TokenStream[InputPointer].token_type != Token_Class.Elseif && TokenStream[InputPointer].token_type != Token_Class.Else && TokenStream[InputPointer].token_type != Token_Class.End)
                IfStatement.Children.Add(Statements());
            IfStatement.Children.Add(IfState());

            return IfStatement;
        }

        // IfState → Else_If_Statment | Else_Statment | end
        Node IfState()
        {
            Node  IfState = new Node("IfState");

            if(InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Elseif))
            {
                IfState.Children.Add(Else_If_Statment());
            }
            else if(InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Else))
            {
                IfState.Children.Add(Else_Statment());
            }
            else 
            {
                IfState.Children.Add(match(Token_Class.End));
            }

            return IfState;
        }

        //  Else_Statment → else Statements end
        Node Else_Statment()
        {
            Node ElseStatment = new Node("Else_Statment");
            ElseStatment.Children.Add(match(Token_Class.Else));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Return && TokenStream[InputPointer].token_type != Token_Class.Until && TokenStream[InputPointer].token_type != Token_Class.Elseif && TokenStream[InputPointer].token_type != Token_Class.Else && TokenStream[InputPointer].token_type != Token_Class.End)
                ElseStatment.Children.Add(Statements());
            ElseStatment.Children.Add(IfState());
            return ElseStatment;
        }

        // Else_If_Statment → elseif Condition_Statement then Statements IfState
        Node Else_If_Statment()
        {
            Node ElseIfStatment = new Node("Else_If_Statment");
            ElseIfStatment.Children.Add(match(Token_Class.Elseif));
            ElseIfStatment.Children.Add(Condition_Statement());
            ElseIfStatment.Children.Add(match(Token_Class.Then));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Return && TokenStream[InputPointer].token_type != Token_Class.Until && TokenStream[InputPointer].token_type != Token_Class.Elseif && TokenStream[InputPointer].token_type != Token_Class.Else && TokenStream[InputPointer].token_type != Token_Class.End)
                ElseIfStatment.Children.Add(Statements());
            ElseIfStatment.Children.Add(IfState());

            return ElseIfStatment;
        }

        // Repeat_Statement → repeat Statements until Condition_Statement
        Node Repeat_Statement()
        {
            Node RepeatStatement = new Node("Repeat_Statement");
            RepeatStatement.Children.Add(match(Token_Class.Repeat));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Until)
            {
                RepeatStatement.Children.Add(Statements());
            }
            RepeatStatement.Children.Add(match(Token_Class.Until));
            RepeatStatement.Children.Add(Condition_Statement());

            return RepeatStatement;
        }

        // Condition_Statement → Condition Condition_Expression
        Node Condition_Statement()
        {
            Node conditionStatement = new Node("Condition_Statement");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(Condition_Expression());

            return conditionStatement;
        }

        // Condition → identifier Condition_Operator Term 
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());

            return condition;
        }

        // Condition_Expression → Boolean_Operator Condition Condition_Expression| ε 
        Node Condition_Expression()
        {
            Node ConditionExpression= new Node("Condition_Expression");

            if(InputPointer < TokenStream.Count && isBooleanOperator())
            {
                ConditionExpression.Children.Add(Boolean_Operator());
                ConditionExpression.Children.Add(Condition());
                ConditionExpression.Children.Add(Condition_Expression());
                return ConditionExpression;
            }
            else
            {
                return null;
            }
        }

        // Boolean_Operator  → && | ||
        Node Boolean_Operator()
        {
            Node BooleanOperator = new Node("Boolean_Operator");

            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                BooleanOperator.Children.Add(match(Token_Class.AndOp));
            }
            else
            {
                BooleanOperator.Children.Add(match(Token_Class.OrOp));
            }

            return BooleanOperator;
        }

        // Condition_Operator → < | > | = | <>
        Node Condition_Operator()
        {
            Node ConditionOperator = new Node("Condition_Operator");

            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                ConditionOperator.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                ConditionOperator.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                ConditionOperator.Children.Add(match(Token_Class.EqualOp));
            }
            else
            {
                ConditionOperator.Children.Add(match(Token_Class.NotEqualOp));
            }

            return ConditionOperator;
        }

        bool isDatatype()
        {
           return (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Float);
        }

        bool isArithmaticOperator(int x = -1)
        {
            if (x == -1) x = InputPointer;
            return (TokenStream[x].token_type == Token_Class.PlusOp || 
                    TokenStream[x].token_type == Token_Class.MinusOp ||
                    TokenStream[x].token_type == Token_Class.MultiplyOp ||
                    TokenStream[x].token_type == Token_Class.DivideOp);
        }

        //  && | ||
        bool isBooleanOperator()
        {
            return (TokenStream[InputPointer].token_type == Token_Class.OrOp || TokenStream[InputPointer].token_type == Token_Class.AndOp );
        }

        // number | identifier | Function_Call
        bool isTerm()
        {
            return (TokenStream[InputPointer].token_type == Token_Class.Constant || 
                    TokenStream[InputPointer].token_type == Token_Class.Idenifier || isFunctionCall());
        }
        
        bool isFunctionCall(int x = -1)
        {
            if (x == -1) x = InputPointer;
            return (x < TokenStream.Count && TokenStream[x].token_type == Token_Class.Idenifier && x + 1 < TokenStream.Count && TokenStream[x + 1].token_type == Token_Class.LParanthesis);
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
