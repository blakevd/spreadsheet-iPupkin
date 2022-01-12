using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void TestCorrectSyntaxParse()
        {
            Formula a = new Formula("x+y");
            Assert.AreEqual("x+y", a.ToString());

            Formula b = new Formula("x + y");
            Assert.AreEqual("x+y", b.ToString());

            Formula c = new Formula("(x / y)");
            Assert.AreEqual("(x/y)", c.ToString());

            Formula d = new Formula("(x)+(y/3)-78");
            Assert.AreEqual("(x)+(y/3)-78", d.ToString());

            Formula e = new Formula("98274 - x_a * y_car/b + b - (4 + 7)");
            Assert.AreEqual("98274-x_a*y_car/b+b-(4+7)", e.ToString());

            Formula f = new Formula("A1 + B1", s => s.ToLower(), x => !Regex.IsMatch(x, @"[A-Z]"));
            Assert.AreEqual("a1+b1", f.ToString());

            Formula g = new Formula("a_b * (c_d - 8)", s => s.ToUpper(), x => !Regex.IsMatch(x, @"[a-z]"));
            Assert.AreEqual("A_B*(C_D-8)", g.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void RightParRule()
        {
            Formula a = new Formula("(8*k))-b-0");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OnlyOneFails()
        {
            Formula a = new Formula("+");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BlanacedRule()
        {
            Formula a = new Formula("((8*k))- b + ((0)");
        }

        [TestMethod]
        public void StartingAndEndingRule()
        {
            Formula a = new Formula("5 + 5");
            Assert.AreEqual("5+5", a.ToString());

            Formula b = new Formula("a_7 - b_22");
            Assert.AreEqual("a_7-b_22", b.ToString());

            Formula c = new Formula("(8*b)");
            Assert.AreEqual("(8*b)", c.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParOpRule()
        {
            Formula a = new Formula("8 + (-7)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParOpRule2()
        {
            Formula a = new Formula("8 + (");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraRule()
        {
            Formula a = new Formula("6 / a5 n45");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OneTokenRule()
        {
            Formula a = new Formula("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowInIsValid()
        {
            Formula a = new Formula("x2 + x3", n => n, s => { if (s == "x3") throw new ArgumentException(); return true; });
        }

        [TestMethod]
        public void NoValidVariable()
        {
            Formula a = new Formula("x2 + x3", n => n, s => true);
            Assert.IsInstanceOfType(a.Evaluate(x => { throw new ArgumentException(); }), typeof(FormulaError));

        }

        [TestMethod]
        public void GetVars()
        {
            Formula a = new Formula("x2 + x3", n => n, s => true);
            List<string> l = new List<string>(a.GetVariables());
            Assert.AreEqual("x2", l[0]);
        }

        [TestMethod]
        public void testEquals()
        {
            Formula x = new Formula("X1   +   Y2");
            Formula y = new Formula("X1+Y2");
            Assert.IsTrue(x.Equals(y));

            Formula f1 = new Formula("2.0 - 1e2");
            Formula f2 = new Formula("2 - 100");
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f1 != x);
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());

            Formula f3 = new Formula("x1+y2", n => n.ToUpper(), s => true);
            Formula f4 = new Formula("X1           +    Y2");
            Assert.IsTrue(f3 == f4);

            Formula f5 = new Formula("x1+y2");
            Formula f6 = new Formula("x2+y1");

            Assert.IsFalse(f5 == f3);
            Assert.IsFalse(f5 == f6);

            Formula f7 = new Formula("2.0 + x7");
            Formula f8 = new Formula("2.000 + x7");
            Assert.IsTrue(f7 == f8);
            Assert.IsTrue(f7.GetHashCode() == f8.GetHashCode());

            object j = new object();
            Assert.IsFalse(f7.Equals(j));
        }

        [TestMethod]
        public void TestDivByZeroError()
        {
            Formula f1 = new Formula("99/0");
            Assert.IsInstanceOfType(f1.Evaluate(s => 0), typeof(FormulaError));

            Formula f2 = new Formula("x7/x8");
            Assert.IsInstanceOfType(f2.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidOperator()
        {
            Formula f1 = new Formula("9+1=10");
            object x = f1.Evaluate(s => 0);
        }

        [TestMethod]
        public void EvalTest()
        {
            Formula f1 = new Formula("v_car + 7", n => n.ToUpper(), s => true);
            Assert.AreEqual(11.0, f1.Evaluate(a => 4.0));

            Formula f2 = new Formula("9");
            Assert.AreEqual(9.0, (double)f2.Evaluate(s => 0.0), 1e-9);

            Formula f3 = new Formula("A_78");
            Assert.AreEqual(1.0, f3.Evaluate(s => 1.0));

            Formula f4 = new Formula("3*(47+8)/2-12+55");
            Assert.AreEqual(125.5, f4.Evaluate(s => 0));

            Formula f5 = new Formula("(x4/x6)/x2");
            Assert.AreEqual(0.5, f5.Evaluate(s => 2));
        }

        [TestMethod]
        public void CallFormulaError()
        {
            FormulaError e = new FormulaError("test");
            Assert.AreEqual("test", e.Reason);
        }

        [TestMethod]
        public void ps1Tests()
        {
            Assert.AreEqual(5.0, new Formula("5").Evaluate(s => 0));
            Assert.AreEqual(13.0, new Formula("13").Evaluate(s => 13));
            Assert.AreEqual(8.0, new Formula("5+3").Evaluate(s => 0));
            Assert.AreEqual(8.0, new Formula("18-10").Evaluate(s => 0));
            Assert.AreEqual(10.0, new Formula("5*2").Evaluate(s => 0));
            Assert.AreEqual(2.5, (double)new Formula("5/2").Evaluate(s => 0), 1e-9);
            Assert.AreEqual(4.0, new Formula("2+X1").Evaluate(s => 2));
            Assert.AreEqual(15.0, new Formula("2*6+3").Evaluate(s => 0));
            Assert.AreEqual(20.0, new Formula("2+6*3").Evaluate(s => 0));
            Assert.AreEqual(194.0, new Formula("2+3*5+(3+4*8)*5+x").Evaluate(s => 2));
            Assert.AreEqual(26.0, new Formula("2+3*(3+5)").Evaluate(s => 0));
            Assert.AreEqual(0.0, new Formula("(1*1)-2/2").Evaluate(s => 0));
            Assert.AreEqual(16.0, new Formula("2*(3+5)").Evaluate(s => 0));
            Assert.AreEqual(10.0, new Formula("2+(3+5)").Evaluate(s => 0));
            Assert.AreEqual(50.0, new Formula("2+(3+5*9)").Evaluate(s => 0));
            Assert.AreEqual(6.0, new Formula("x1+(x2+(x3+(x4+(x5+x6))))").Evaluate(s => 1));
            Assert.AreEqual(0.0, new Formula("a4-a4*a4/a4").Evaluate(s => 3));
            Assert.AreEqual(12.0, new Formula("((((x1+x2)+x3)+x4)+x5)+x6").Evaluate(s => 2));
        }
    }
}
