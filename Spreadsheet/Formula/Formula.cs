using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private String finalFormula; // normalized formula
        private List<string> varList; // List to keep track of things

        private Regex splitRegex = new Regex("(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
        private Regex varRegex = new Regex(@"^[a-zA-Z_][a-zA-Z_0-9]*$");

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            string normFormula = normalize(formula); // normalize
            varList = new List<string>();

            if (isValidSyntax(normFormula, isValid, out string output)) // call private helper that checks syntax
            {
                finalFormula = output;
            }
        }

        /// <summary>
        /// private helper method with all the logic for the constructor
        /// </summary>
        /// <param name="formula">a formula</param>
        /// <param name="isValid"></param>
        /// <returns>True if it passed all tests</returns>
        private bool isValidSyntax(string formula, Func<string, bool> isValid, out string outputFormula)
        {
            int lp = 0; // tracker for left parenthesis
            int rp = 0; // tracker for right parenthesis
            int tCount = 0; // token counter

            string result = "";
            string prev = null;
            string varPattern = @"^[a-zA-Z_][a-zA-Z_0-9]*$";
            string opPattern = @"^[\+\-*/]$";
            string parPattern = @"[()]";
            List<string> tokens = new List<string>(GetTokens(formula));

            if (tokens.Count() < 1) // checks if it has at least 1 token
                throw new FormulaFormatException("There is les than 1 token");

            foreach (string t in tokens) // loop through tokens
            {
                tCount++; // show what token we are on

                bool tIsOperator = Regex.IsMatch(t, opPattern);
                bool tIsVariable = isValid(t) && Regex.IsMatch(t, varPattern);
                bool tIsNumber = Double.TryParse(t, out double doubleOfT);
                bool tIsPar = Regex.IsMatch(t, parPattern);

                if (tIsVariable && !varList.Contains(t)) // add to our lists for later use
                    varList.Add(t);

                if (tCount == 1) // check first token
                {
                    if (t != "(" && !tIsNumber && !tIsVariable)
                        throw new FormulaFormatException("First token is not a valid number, variable or left parenthesis");
                }
                else if (tCount == tokens.Count()) // check if its the last token
                {
                    if (t != ")" && !tIsNumber && !tIsVariable)
                        throw new FormulaFormatException("Last token is not a valid number, variable or right parenthesis");
                }

                if (!tIsOperator && !tIsPar && !tIsNumber && !tIsVariable) // if doesnt meet parsing rule
                    throw new FormulaFormatException("Invalid token in formula, it is either an invalid variable, decimal number, or not a (,),+,-,*,/");
                else if (t == "(")
                    lp++;
                else if (t == ")")
                    rp++;

                if (rp > lp) // too many right par;
                    throw new FormulaFormatException("Extra right parenthesis found");

                if (prev != null)
                {
                    if (Regex.IsMatch(prev, opPattern) || prev == "(") // check if prev is operator
                    {
                        if (!tIsNumber && !tIsVariable && t != "(") // if it is then the next t should be these
                            throw new FormulaFormatException("Invalid token preceding a (, it should be a number, variable, or a (");
                    }
                    else if (Double.TryParse(prev, out double y) || (isValid(prev) && Regex.IsMatch(prev, varPattern)) || prev == ")") // check if prev is var or num
                        if (t != ")" && !tIsOperator) // if it is then the next should be one of these
                            throw new FormulaFormatException("Invalid token preceding a number, variable, or ), it must be a +,-,*,/,)");
                }

                if (tIsNumber) // add a normalized number to our result string
                    result += doubleOfT.ToString();
                else
                    result += t;

                prev = t;
            }

            if (rp != lp) // check balanced par;
                throw new FormulaFormatException("Extra left parenthesis found");

            outputFormula = result;
            return true; // passed all checks
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // create stacks
            Stack<double> values = new Stack<double>();
            Stack<char> operators = new Stack<char>();

            // split the expression into a list of strings the into chars
            String[] stringExp = splitRegex.Split(finalFormula);

            // Loop through the expression
            foreach (string s in stringExp)
            {
                if (!s.Equals(" ") && !s.Equals("")) // skips empty strings and spaces
                {
                    String strToken = s.Trim(); // deletes whitespace from strings
                    char charToken = strToken.ToCharArray()[0]; // convert ito a character array to use char values

                    Match findVar = varRegex.Match(strToken);

                    if (Double.TryParse(strToken, out double val)) // token is a double
                    {
                        if (operators.IsOnTop('*') || operators.IsOnTop('/')) // check operator
                        {
                            bool success = PopAndMultOrDiv(values, operators, val);
                            if (!success)
                                return new FormulaError("Can't Divide by zero");
                        }
                        else
                        {
                            values.Push(val);
                        }
                    }
                    else if (findVar.Success) // token is a variable
                    {
                        try
                        {
                            double var = lookup(findVar.ToString());

                            if (operators.IsOnTop('*') || operators.IsOnTop('/')) // check operator
                            {
                                bool success = PopAndMultOrDiv(values, operators, var);
                                if (!success)
                                    return new FormulaError("Can't Divide by zero");
                            }
                            else
                            {
                                values.Push(var);
                            }
                        }
                        catch 
                        {
                            return new FormulaError("Variable does not have a value assigned to it");
                        }
                    }
                    else if (charToken == '+' || charToken == '-') // token is + or -
                    {
                        if (operators.IsOnTop('+') || operators.IsOnTop('-')) // + or - is on top of stack
                        {
                            popTwiceAndPush(values, operators);
                        }

                        operators.Push(charToken); // push token onto stack

                    }
                    else if (charToken == '*' || charToken == '/') // push t onto stack if * or /
                    {
                        operators.Push(charToken);
                    }
                    else if (charToken == '(') // token is a left parenthesis
                    {
                        operators.Push(charToken);
                    }
                    else if (charToken == ')') // token is a right parenthesis
                    {
                        if (operators.IsOnTop('+') || operators.IsOnTop('-')) // if + or - on top
                        {
                            popTwiceAndPush(values, operators);
                        }

                        char parenthesis = operators.Pop(); // this should be a '('

                        if (parenthesis == '(')
                        {
                            if (operators.IsOnTop('*') || operators.IsOnTop('/')) // if * or / on top
                            {
                               bool success = popTwiceAndPush(values, operators);
                                if (!success)
                                    return new FormulaError("Can't Divide by zero");
                            }
                        }
                    }
                } // end of if s is empty
            } // end of foreach

            if (operators.HasOneValue()) // operators must have 1 left
            {
                if (operators.IsOnTop('+') || operators.IsOnTop('-')) // that value must be + or -
                {
                    if (values.Count == 2) // values must have 2 left
                    {
                        popTwiceAndPush(values, operators);
                        return values.Pop();
                    }
                }
            }
            // if we did not meet any conditions
            return values.Pop();

        }

        /// <summary>
        /// checks if the given denominator is 0
        /// </summary>
        /// <param name="v"></param>
        /// <returns>true if v is 0, false otherwise</returns>
        private static bool CheckDivByZero(double v)
        {
            return v == 0.0;
        }

        /// <summary>
        /// pops the value stack, pops the operator stack, and applies the popped operator to the popped number and t. 
        /// Pushes the result onto the value stack,
        /// Otherwise, pushes t onto the value stack.
        /// </summary>
        /// <param name="valStack"></param>
        /// <param name="opStack"></param>
        /// <param name="t"></param>
        private static bool PopAndMultOrDiv(Stack<double> valStack, Stack<char> opStack, double t)
        {
            double popVal = valStack.Pop();
            char popOp = opStack.Pop();
            double result = -1;

            if (popOp == '*')
            {
                result = popVal * t;
            }
            else if (popOp == '/')
            {
                if (CheckDivByZero(t))
                    return false;
                result = popVal / t;
            }

            valStack.Push(result);
            return true; //success
        }

        /// <summary>
        /// Pops the value stack twice and the operator stack once, 
        /// then applies the popped operator to the popped numbers, then pushes the result onto the value stack.
        /// </summary>
        /// <param name="valStack"></param>
        /// <param name="opStack"></param>
        private static bool popTwiceAndPush(Stack<double> valStack, Stack<char> opStack)
        {
            double x1 = valStack.Pop();
            double x2 = valStack.Pop();

            char operand = opStack.Pop();

            double result = 0;

            if (operand == '+')
            {
                result = x2 + x1;
            }
            else if (operand == '-')
            {
                result = x2 - x1;
            }
            else if (operand == '*')
            {
                result = x2 * x1;
            }
            else if (operand == '/')
            {
                if (CheckDivByZero(x1))
                    return false;
                result = x2 / x1;
            }

            valStack.Push(result);
            return true;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return varList;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return finalFormula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            try
            {
                return obj.ToString() == this.ToString();
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            try
            {
                return f1.Equals(f2);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            try
            {
                return !f1.Equals(f2);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return finalFormula.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    public static class StackExtensions
    {
        /// <summary>
        /// Checks if the given char value is on top of the stack
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c">character to check</param>
        /// <returns>True if c is on top of the stack, false if stack is empty or doesnt find it</returns>
        public static bool IsOnTop(this Stack<char> s, char c)
        {
            return s.Count > 0 && s.Peek() == c;
        }

        /// <summary>
        /// Checks if stack has exactly 1 value in it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns>True if stack has only one value in it, false otherwise</returns>
        public static bool HasOneValue<T>(this Stack<T> s)
        {
            return s.Count == 1;
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}