using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    Int, Float, String, Read, Write, Repeat, Until, If, Elseif,
    Else, Then, Return, Endl, End, Main, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    AndOp, OrOp, AssignmentOp, Constant, Semicolon, Comma, LParanthesis, RParanthesis, LBrace, RBrace,
    Idenifier
}
namespace TINY_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        Dictionary<char, bool> delimiters = new Dictionary<char, bool>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);

            // assignment operators 
            Operators.Add(":=", Token_Class.AssignmentOp);

            // conditional operators
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<>", Token_Class.NotEqualOp);

            // boolean operators
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);

            // arithmatic operators
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);

            // punctuator tokens
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LBrace);
            Operators.Add("}", Token_Class.RBrace);

            // delimiters
            delimiters.Add('<', true);
            delimiters.Add('>', true);
            delimiters.Add('=', true);
            delimiters.Add('&', true);
            delimiters.Add('|', true);
            delimiters.Add('+', true);
            delimiters.Add('-', true);
            delimiters.Add('*', true);
            delimiters.Add('/', true);
            delimiters.Add(':', true);
            delimiters.Add(';', true);
            delimiters.Add(',', true);
            delimiters.Add('(', true);
            delimiters.Add(')', true);
            delimiters.Add('{', true);
            delimiters.Add('}', true);
            // white spaces
            delimiters.Add(' ', true);
            delimiters.Add('\r', true);
            delimiters.Add('\t', true);
            delimiters.Add('\n', true);

        }
        public void StartScanning(string SourceCode)
        {
            // case insensitive
            SourceCode = SourceCode.ToLower();

            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = "";
                // check operators
                string op = SourceCode[i].ToString(); // op of size 1
                string op2 = SourceCode[i].ToString(); // op of size 2
                if (i < SourceCode.Length - 1)
                    op2 += SourceCode[i + 1];
                /* ========================================================================================= */
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\t' || CurrentChar == '\n')
                    continue;
                // string literals
                if (CurrentChar == '\"')
                {
                    CurrentLexeme += SourceCode[j++];
                    while (j < SourceCode.Length && SourceCode[j] != '\"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (j < SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                    }
                    i = j;
                }
                // self-explainatory
                else if (op2 == "/*")
                {
                    while (j < SourceCode.Length - 1 && !(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (j < SourceCode.Length - 1)
                    {
                        CurrentLexeme += SourceCode[j];
                        CurrentLexeme += SourceCode[j + 1];
                    }
                    i = j + 1;
                    continue;
                }
                // identifier, reserved
                else if (CurrentChar >= 'a' && CurrentChar <= 'z')
                {
                    while (j < SourceCode.Length && delimiters.ContainsKey(SourceCode[j]) == false)
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    i = j - 1;
                }
                // constant 
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while (j < SourceCode.Length && delimiters.ContainsKey(SourceCode[j]) == false)
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    i = j - 1;
                }
                // operators of two chars (&&, ||, :=, <>)
                else if (isOperator(op2))
                {
                    CurrentLexeme = op2;
                    i = j + 1;
                }
                // operator of one char 
                else if (isOperator(op))
                {
                    CurrentLexeme = op;
                    i = j;
                }
                else
                {
                    // invalid token 
                    CurrentLexeme += SourceCode[j++];
                    while (j < SourceCode.Length && delimiters.ContainsKey(SourceCode[j]) == false)
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    i = j - 1;
                }
                FindTokenClass(CurrentLexeme);
            }

            TINY_Compiler.TokenStream = Tokens;
        }


        void FindTokenClass(string Lex)
        {
            bool isValidToken = false;
            Token Tok = new Token();
            Tok.lex = Lex;
            if (isReserved(Lex))
            {
                Tok.token_type = ReservedWords[Tok.lex];
            }
            else if (isOperator(Lex))
            {
                Tok.token_type = Operators[Tok.lex];
            }
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
            }
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
            }
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;

            }
            else
            {
                // invalid token
                Errors.Error_List.Add(Lex);
                return;
            }
            Tokens.Add(Tok);
        }
        bool isReserved(string lex)
        {
            return ReservedWords.ContainsKey(lex);
        }
        bool isIdentifier(string lex)
        {
            Regex identifier_regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);
            return identifier_regex.IsMatch(lex);
        }
        bool isString(string lex)
        {
            Regex string_regex = new Regex(@"^\""[^\""]*\""$", RegexOptions.Compiled);
            return string_regex.IsMatch(lex);
        }
        bool isConstant(string lex)
        {
            Regex constant_regex = new Regex(@"^(\+|-)?[0-9]+(\.[0-9]+)?((E|e)(\+|-)?[0-9]+)?$", RegexOptions.Compiled);
            return constant_regex.IsMatch(lex);
        }
        bool isOperator(string lex)
        {
            return Operators.ContainsKey(lex);
        }
        bool isLetter(char c)
        {
            return (c >= 'a' && c <= 'z');
        }
        bool isDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }
    }
}