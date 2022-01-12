using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class acts as an extension to the Stack class to help with computations
    /// </summary>
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
        /// Checks if the given int value is on top of the stack
        /// </summary>
        /// <param name="s"></param>
        /// <param name="v">int to check</param>
        /// <returns>True if v is on top of the stack, false if stack is empty or doesnt find it</returns>
        public static bool IsOnTop(this Stack<int> s, int v)
        {
            return s.Count > 0 && s.Peek() == v;
        }

        /// <summary>
        /// Checks if given stack is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns>true if stack is empty, false otherwise</returns>
        public static bool IsEmpty<T>(this Stack<T> s)
        {
            return s.Count == 0;
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

        /// <summary>
        /// Checks if stack has less than two values in it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns>True if stack has less than two values in it</returns>
        public static bool IsLessThanTwo<T>(this Stack<T> s)
        {
            return s.Count < 2;
        }
    }

    /// <summary>
    /// This class evaluates a given mathmatical string expression
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        /// <summary>
        /// Checks if the given denominator is 0, throws a divide by zero error since the denominator
        /// is zero and will devide by zero
        /// </summary>
        /// <param name="den">denominator</param>
        private static void checkDivideByZero(int den)
        {
            if (den == 0)
                throw new ArgumentException("Can't Divide by zero");
        }

        /// <summary>
        /// pops the value stack, pops the operator stack, and applies the popped operator to the popped number and t. 
        /// Pushes the result onto the value stack,
        /// Otherwise, pushes t onto the value stack.
        /// </summary>
        /// <param name="valStack"></param>
        /// <param name="opStack"></param>
        /// <param name="t"></param>
        private static void popAndMultOrDiv(Stack<int> valStack, Stack<char> opStack, int t)
        {
            if (valStack.IsEmpty())
                throw new ArgumentException(); // throws if stack is empty

            int popVal = valStack.Pop();
            char popOp = opStack.Pop();
            int result = -1;

            if (popOp == '*')
            {
                result = popVal * t;
            }
            else if (popOp == '/')
            {
                checkDivideByZero(t); // check for a divide by 0 error
                result = popVal / t;
            }

            valStack.Push(result);

        }

        /// <summary>
        /// Pops the value stack twice and the operator stack once, 
        /// then applies the popped operator to the popped numbers, then pushes the result onto the value stack.
        /// </summary>
        /// <param name="valStack"></param>
        /// <param name="opStack"></param>
        private static void popTwiceAndPush(Stack<int> valStack, Stack<char> opStack)
        {
            if (valStack.IsLessThanTwo())
                throw new ArgumentException(); // throws if something is wrong with expression or doesnt have at least 2 vars

            int x1 = valStack.Pop();
            int x2 = valStack.Pop();

            char operand = opStack.Pop();

            int result = -1;

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
                result = x2 / x1;
            }

            valStack.Push(result);
        }

        /// <summary>
        /// Evaluates the given mathmatical expression, only valid with parenthesis,
        /// variables, integers, add, minus, divide, and multiply
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>the computed value of the given expression</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            // create stacks
            Stack<int> values = new Stack<int>();
            Stack<char> operators = new Stack<char>();

            // split the expression into a list of strings the into chars
            String[] stringExp = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Loop through the expression
            foreach (string s in stringExp)
            {
                if (!s.Equals(" ") && !s.Equals("")) // skips empty strings and spaces
                {
                    String strToken = s.Trim(); // deletes whitespace from strings
                    char charToken = strToken.ToCharArray()[0]; // convert ito a character array to use char values

                    Match findVar = Regex.Match(strToken, "\\b[A-Za-z]+\\d+"); // REGEX : \A[A-Za-z]+\d+ this will find any spreadsheet var

                    if (int.TryParse(strToken, out int val)) // token is an int
                    {
                        if (operators.IsOnTop('*') || operators.IsOnTop('/')) // check operator
                        {
                            popAndMultOrDiv(values, operators, val);
                        }
                        else
                        {
                            values.Push(val);
                        }
                    }
                    else if (findVar.Success) // token is a variable
                    {

                        int var = variableEvaluator(strToken.Substring(findVar.Index, findVar.Length)); // pass in the var we found in the regex

                        if (operators.IsOnTop('*') || operators.IsOnTop('/')) // check operator
                        {
                            popAndMultOrDiv(values, operators, var);
                        }
                        else
                        {
                            values.Push(var);
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

                        if (operators.IsEmpty())
                            throw new ArgumentException();
                        char parenthesis = operators.Pop(); // this should be a '('

                        if (parenthesis == '(')
                        {
                            if (operators.IsOnTop('*') || operators.IsOnTop('/')) // if * or / on top
                            {
                                popTwiceAndPush(values, operators);
                            }
                        }
                    }
                } // end of if s is empty
            } // end of foreach

            if (operators.IsEmpty() && values.HasOneValue()) // no operations left
            {
                return values.Pop();
            }
            else
            {
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
                // throw if we did not meet any conditions
                throw new ArgumentException();
            }
        }
    }
}
