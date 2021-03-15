using HW01;
using NUnit.Framework;

namespace HW01Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public string ParseExpression(string s)
        {
            Visitor visitor = new Visitor();
            IExpression expr = Parser.Parse(s);
            expr.Accept(visitor);
            return visitor.ToString();
        }

        [Test]
        public void Test()
        {
            Assert.AreEqual("Literal(1)", ParseExpression("1"));
            Assert.AreEqual("Literal(1)", ParseExpression("((1))"));

            Assert.AreEqual("Binary(Literal(1)+Literal(2))", ParseExpression("1+2"));
            Assert.AreEqual("Binary(Literal(1)+Literal(2))", ParseExpression("(1+2)"));

            Assert.AreEqual("Binary(Literal(1)/Binary(Literal(5)+Variable(y)))", ParseExpression("1/(5+y)"));
            Assert.AreEqual("Binary(Binary(Literal(1)/Literal(5))+Variable(y))", ParseExpression("1/5+y"));

            Assert.AreEqual("Binary(Binary(Binary(Literal(1)+Literal(2))+Literal(4))+Variable(a))", ParseExpression("1+2+4+a"));
            Assert.AreEqual("Binary(Binary(Binary(Literal(1)*Literal(2))/Literal(4))+Variable(a))", ParseExpression("1*2/4+a"));
            Assert.AreEqual("Binary(Literal(1)+Binary(Binary(Literal(1)+Literal(2))*Variable(a)))", ParseExpression("1+(1+2)*a"));

            Assert.AreEqual("Binary(Binary(Literal(1)+Literal(2))+Binary(Binary(Literal(4)*Literal(8))*Literal(9)))", ParseExpression("1+2+4*8*9"));


            Assert.AreEqual("Binary(Binary(Literal(1)+Binary(Literal(2)*Literal(3)))-Variable(a))", ParseExpression("(1+2*3)-a"));
            Assert.AreEqual("Binary(Binary(Literal(1)+Binary(Literal(2)-Variable(b)))-Binary(Variable(a)-Literal(1)))", ParseExpression("(1+(2-b))-(a-1)"));

            Assert.AreEqual("Binary(Binary(Binary(Literal(1)/Variable(x))+Variable(y))-Binary(Binary(Literal(9)/Literal(4))/Literal(3)))", 
                ParseExpression("1/x+((((y))))-9/4/3"));
        }
    }
}