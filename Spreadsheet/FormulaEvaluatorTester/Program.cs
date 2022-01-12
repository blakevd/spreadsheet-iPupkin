using System;
using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string exp = "2+5*7*";
            int test = Evaluator.Evaluate(exp, LookupTest);
            Console.WriteLine("Ans: " + test);
        }

        public static int LookupTest(string v)
        {
        
            if (v == "A6")
                return 100;
            else if (v == "BH7")
                return 200;
            return 0;
        }
    }
}
