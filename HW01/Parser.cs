using System;
using System.Collections.Generic;
using System.Text;


namespace HW01
{
    public interface IExpressionVisitor
    {
        void Visit(Literal expression);
        void Visit(Variable expression);
        void Visit(BinaryExpression expression);
        void Visit(ParenExpression expression);
    }

    public interface IExpression
    {
        void Accept(IExpressionVisitor visitor);
    }

    public class Literal : IExpression
    {
        public Literal(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Variable : IExpression
    {
        public Variable(string name)
        {
            Name = name;
        }

        public readonly string Name;
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class BinaryExpression : IExpression
    {
        public readonly IExpression FirstOperand;
        public readonly IExpression SecondOperand;
        public readonly string Operator;

        public BinaryExpression(IExpression firstOperand, IExpression secondOperand, string @operator)
        {
            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            Operator = @operator;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ParenExpression : IExpression
    {
        public ParenExpression(IExpression operand)
        {
            Operand = operand;
        }

        public readonly IExpression Operand;
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Visitor : IExpressionVisitor
    {
        private readonly StringBuilder myBuilder;

        public Visitor()
        {
            myBuilder = new StringBuilder();
        }

        public void Visit(Literal expression)
        {
            myBuilder.Append("Literal(" + expression.Value + ")");
        }

        public void Visit(Variable expression)
        {
            myBuilder.Append("Variable(" + expression.Name + ")");
        }

        public void Visit(BinaryExpression expression)
        {
            myBuilder.Append("Binary(");
            expression.FirstOperand.Accept(this);
            myBuilder.Append(expression.Operator);
            expression.SecondOperand.Accept(this);
            myBuilder.Append(")");
        }

        public void Visit(ParenExpression expression)
        {
            myBuilder.Append("Paren(");
            expression.Operand.Accept(this);
            myBuilder.Append(")");
        }

        public override string ToString()
        {
            return myBuilder.ToString();
        }
    }


    public class Parser
    {
        public static IExpression Parse(string text)
        {
            Stack<IExpression> parsedExpressions = new Stack<IExpression>();
            Stack<char> parsedOperators = new Stack<char>();

            List<char> binOps = new List<char> { '+', '-', '*', '/' };
            List<char> brackets = new List<char> { '(', ')' };
            char lastBinOp = '\0';
            int closeBracketExpected = 0;

            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (binOps.Contains(ch))
                {
                    if ((lastBinOp == '*' || lastBinOp == '/') && ch != '(')
                    {
                        parsedOperators.Pop();
                        var right = parsedExpressions.Pop();
                        var left = parsedExpressions.Pop();

                        parsedExpressions.Push(new BinaryExpression(left, right, lastBinOp.ToString()));
                    }

                    lastBinOp = ch;
                    parsedOperators.Push(ch);
                }
                else if (brackets.Contains(ch))
                {
                    if (ch == '(')
                    {
                        closeBracketExpected += 1;
                        parsedOperators.Push(ch);
                    } else
                    {
                        closeBracketExpected -= 1;

                        if (closeBracketExpected  < 0)
                        {
                            throw new Exception("Invalid brackets");
                        }

                        var lastOp = parsedOperators.Peek();
                        while (lastOp != '(')
                        {
                            parsedOperators.Pop();
                            var right = parsedExpressions.Pop();
                            var left = parsedExpressions.Pop();

                            parsedExpressions.Push(new BinaryExpression(left, right, lastOp.ToString()));

                            lastOp = parsedOperators.Peek();

                        }
                        parsedOperators.Pop();
                    }
                    lastBinOp = '\0';
                }
                else if (char.IsDigit(ch))
                {
                    parsedExpressions.Push(new Literal(ch.ToString()));
                }
                else if (char.IsLetter(ch))
                {
                    parsedExpressions.Push(new Variable(ch.ToString()));
                } 
                else
                {
                    throw new Exception("Invalid expression");
                }
            }

            if (lastBinOp == '*' || lastBinOp == '/')
            {
                parsedOperators.Pop();
                var right = parsedExpressions.Pop();
                var left = parsedExpressions.Pop();

                parsedExpressions.Push(new BinaryExpression(left, right, lastBinOp.ToString()));
            }

            if (closeBracketExpected != 0)
            {
                throw new Exception("Invalid brackets");
            }

            Stack<IExpression> reversedExpressions = new Stack<IExpression>();
            Stack<char> reversedOperators = new Stack<char>();

            // reverse for associativity
            while (parsedExpressions.Count > 0)
            {
                reversedExpressions.Push(parsedExpressions.Pop());
            }

            while (parsedOperators.Count > 0)
            {
                reversedOperators.Push(parsedOperators.Pop());
            }

            // we eliminated all * and / expressions and brackets expressions
            while (reversedExpressions.Count > 1)
            {
                if (reversedOperators.Count == 0)
                {
                    throw new Exception("Invalid expression");
                }

                var op = reversedOperators.Pop();
                var left = reversedExpressions.Pop();
                var right = reversedExpressions.Pop();

                reversedExpressions.Push(new BinaryExpression(left, right, op.ToString()));
            }

            
            return reversedExpressions.Pop();
        }

    }

}
