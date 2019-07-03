using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Diagnostics;

using System.Numerics;

namespace CSMetaphorWorld
{
    class RuntimeCompiler
    {

        //string currentEquation;
        private static string dynamicCalc(string theEquation)
        {
            //this method needs these usings:
            /*
             * using System.CodeDom.Compiler;
             * using Microsoft.CSharp;
             * using System.Reflection;
             * using System.Diagnostics;
             */

            //C sharp has a bit of an inconsistency regarding bool.ToString() and the code represention of bools
            //false.ToString() outputs -> "False" ....which is then not recognisable as valid code. Thus I include this check:
            if (theEquation == "False" || theEquation == "True")
                theEquation = theEquation.ToLower();
            //this alteration does not mess with strings that contain true or false as they are supposed to be passed in as "\"False\""


            //all about the compiler parameters:
            CompilerParameters CompilerParams = new CompilerParameters();
            string outputDirectory = Directory.GetCurrentDirectory();

            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.CompilerOptions = "/optimize";

            string[] references = { "System.dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);
            /////////

            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerResults compile;

            int attemptCount = 0;
            do
            {
                if (attemptCount > 1)
                {
                    theEquation = theEquation.Substring(2, theEquation.Length - 4);//remove the extra quotation marks that where added
                    Console.WriteLine("We where unable to compile your statement of <{0}>", theEquation);
                    Console.WriteLine("So we are going to assume it is a string in its simplest form and return that to you");
                    theEquation = theEquation.Remove(theEquation.IndexOf('"'), 1);
                    theEquation = theEquation.Remove(theEquation.LastIndexOf('"'), 1);
                    return '"' + theEquation + '"';
                    //throw new Exception("We where unable to correct your bad input and thus the code could not compile");
                }

                string[] code =         {("using System;\n" +
                                            "namespace DynaCore\n" +
                                            "{" +
                                            "   public class DynaCore\n" +
                                            "   {" +
                                            "       static public string Main()\n" +
                                            "       {\n" +
                                            "            try\n" +
                                            "            {\n" +
                              string.Format("                object x = {0};\n",theEquation)+
                                            "                Type theType = x.GetType();\n"+
                                            "                string typeName = theType.Name;\n"+
                                            "                if (typeName == \"String\")\n"+
                              string.Format("                    return '\"' + ({0}).ToString() + '\"';\n",theEquation)+
                                            "                else\n"+
                              string.Format("                    return ({0}).ToString();\n",theEquation)+
                                            "            }\n" +
                                            "            catch (Exception e)\n" +
                                            "            {\n" +
                                            "                return \"Your Input To The Calculator Was Malformed and threw this exception:\"+e.ToString();\n" +
                                            "            }\n" +
                                            "       }\n" +
                                            "   }\n" +
                                            "}\n")};



                compile = provider.CompileAssemblyFromSource(CompilerParams, code); //this is where the code is actually compiled

                if (compile.Errors.HasErrors)
                {
                    Console.WriteLine("CODE CONTAINED ERRORS, ARE YOU PERHAPS ATTEMPTING TO COMPILE A 'PLAIN' STRING WITHOUT QUOTES?");
                    Console.WriteLine("We will attempt to correct the error by adding quotes to the begining and end of your input");
                    theEquation = '"' + theEquation + '"';
                    attemptCount++;
                }

            } while (compile.Errors.HasErrors);

            //ExpoloreAssembly(compile.CompiledAssembly);

            Module module = compile.CompiledAssembly.GetModules()[0];
            Type mt = null;
            MethodInfo methInfo = null;

            if (module != null)
            {
                mt = module.GetType("DynaCore.DynaCore");
            }

            if (mt != null)
            {
                methInfo = mt.GetMethod("Main");
            }

            if (methInfo != null)
            {
                //this line invokes the methInfo method (which in this case is "Main") 
                //any parameters would be placed inside "new object[] {XXX}"
                return methInfo.Invoke(null, new object[] { }).ToString();
            }
            return "ERROR!?!"; //you really shouldn't be able to get here......I think
        }

        /// <summary>
        /// Takes the result of some expression, and compiles the result as an object. The objects type is then extracted via codedom and reflection
        /// </summary>
        /// <param name="answerAsString"></param>
        /// <returns></returns>
        public static string dynamicTypeInference(string answerAsString)
        {
            //this method needs these usings:
            /*
             * using System.CodeDom.Compiler;
             * using Microsoft.CSharp;
             * using System.Reflection;
             * using System.Diagnostics;
             */

            //C sharp has a bit of an inconsistency regarding bool.ToString() and the code represention of bools
            //false.ToString() outputs -> "False" ....which is then not recognisable as valid code. Thus I include this check:
            if (answerAsString == "False" || answerAsString == "True")
                answerAsString = answerAsString.ToLower();
            //this alteration does not mess with strings that contain true or false as they are supposed to be passed in as "\"False\""


            //all about the compiler parameters:
            CompilerParameters CompilerParams = new CompilerParameters();
            string outputDirectory = Directory.GetCurrentDirectory();

            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.CompilerOptions = "/optimize";

            string[] references = { "System.dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);
            /////////

            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerResults compile;

            int attemptCount = 0;
            do
            {
                if (attemptCount > 1)
                {
                    Console.WriteLine("We where unable to interpret the type of your answer <{0}>", answerAsString);
                    Console.WriteLine("So we are going to assume it is a string");
                    return answerAsString.GetType().Name;
                }

                string[] code =         {("using System;\n" +
                                            "namespace DynaCore\n" +
                                            "{" +
                                            "   public class DynaCore\n" +
                                            "   {" +
                                            "       static public string Main()\n" +
                                            "       {\n" +
                                            "            try\n" +
                                            "            {\n" +
                              string.Format("                object x = {0};\n",answerAsString)+
                                            "                return (x).GetType().Name;\n" +
                                            "            }\n" +
                                            "            catch (Exception e)\n" +
                                            "            {\n" +
                                            "                return \"Your Input To The Calculator Was Malformed and threw this exception:\"+e.ToString();\n" +
                                            "            }\n" +
                                            "       }\n" +
                                            "   }\n" +
                                            "}\n")};



                compile = provider.CompileAssemblyFromSource(CompilerParams, code); //this is where the code is actually compiled

                if (compile.Errors.HasErrors)
                {
                    Console.WriteLine("CODE CONTAINED ERRORS, ARE YOU PERHAPS ATTEMPTING TO INFER THE TYPE OF A 'PLAIN' STRING WITHOUT QUOTES?");
                    Console.WriteLine("We will attempt to correct the error by adding quotes to the beginning and end of your input");
                    answerAsString = '"' + answerAsString + '"';
                    attemptCount++;
                }

            } while (compile.Errors.HasErrors);

            //ExpoloreAssembly(compile.CompiledAssembly);

            Module module = compile.CompiledAssembly.GetModules()[0];
            Type mt = null;
            MethodInfo methInfo = null;

            if (module != null)
            {
                mt = module.GetType("DynaCore.DynaCore");
            }

            if (mt != null)
            {
                methInfo = mt.GetMethod("Main");
            }

            if (methInfo != null)
            {
                //this line invokes the methInfo method (which in this case is "Main") 
                //any parameters would be placed inside "new object[] {XXX}"
                return methInfo.Invoke(null, new object[] { }).ToString();
            }
            return "ERROR!?!"; //you really shouldn't be able to get here......I think
        }



        /// <summary>
        /// this does a best guess based on the format of the string
        /// DEPRECATED, rather use dynamicTypeInference (it avoids guessing and uses runtime compiling and reflection to ensure the correct answer)
        /// </summary>
        /// <param name="theEquation"></param>
        /// <returns></returns>
        private static string guessEquationResultType(string theAnswer)
        {
            //TODO add support for nagative numbers
            //TODO add support for more than just bool, int, double, and string
            if (theAnswer == "true" || theAnswer == "false")
            {
                return "bool";
            }

            //getting here means we know its not a bool



            if (!(theAnswer.Contains('0') || theAnswer.Contains('1') || theAnswer.Contains('2')
                || theAnswer.Contains('3') || theAnswer.Contains('4') || theAnswer.Contains('5')
                || theAnswer.Contains('6') || theAnswer.Contains('7') || theAnswer.Contains('8') || theAnswer.Contains('9')))
            {//non-numeric
                //if its not a bool and contains no numbers it must be a string or something much more complicated like an object
                //but seeing as this is meant to guess the type that is returned by a calculator it is safe to assume that it can only return bool, numeric or string types
                return "string";
            }
            else
            {   //contains numbers...
                //thus it could be negative, which we handle here:
                bool isNegative = theAnswer[0] == '-';
                if (isNegative)
                    theAnswer = theAnswer.Substring(1); //remember that if its a number it needs to be treated as negative


                if (theAnswer[0] == '.' || theAnswer[theAnswer.Length - 1] == '.' || theAnswer.Count(x => x == '.') > 1) //multiple decimals or a decimal at the begining or end would be a malfored number and thus it must be a string
                    return "string";

                //getting here it could still contain letters AND numbers
                foreach (char x in theAnswer)
                {
                    if (x != '0' && x != '1' && x != '2' && x != '3' && x != '4' && x != '5' &&
                       x != '6' && x != '7' && x != '8' && x != '9' && x != '.') //is it a non-numeric character?
                        return "string"; //then return it as a string
                }

                if (theAnswer.Count(x => x == '.') == 1)
                {//then it is either float or double
                    return "double"; //assuming double for the moment, the difference between float and double would be in the number of significant digits
                    //One can say for certain that a number is a double if it is outside the bounds of a float however it doesn't work the other way, for example 1.0, 0.5 and 3.333 could be either doubles or floats
                    //therefore it is safest to always return double so as to not lose acurracy
                }
                else
                    if (theAnswer.Count(x => x == '.') == 0)
                    {
                        //numbers but no decimal points...must be an int/long/byte etc (depending on size)
                        if (isNegative == false && System.Numerics.BigInteger.Parse(theAnswer) >= ulong.MaxValue)//it contains no decimals but is to long for an ulong, it must either be a string or something more complex like a BigInteger (which I don't want to include yet)
                            return "string";

                        //if it is not negative but it is between long and ulong then return ulong
                        if (isNegative == false && ulong.Parse(theAnswer) > long.MaxValue && ulong.Parse(theAnswer) < ulong.MaxValue)
                            return "ulong";

                        //is it too small for a long, but negative so innaproopriate for an ulong
                        if (isNegative && ulong.Parse(theAnswer) > long.MaxValue)
                            return "string";

                        //regardless of whether the number is negative or not check if its value is greater than that of the largest int value
                        if (double.Parse(theAnswer) > int.MaxValue)
                            return "long";

                        return "int";//getting here probably means its an int, however smaller values like byte and short can still be added (if you want them)
                    }
                    else //getting here means something is probably wrong...
                    {
                        Console.WriteLine("guessEquationResultType was unable to match a type to your input of {0}", theAnswer);
                        return "string";
                    }
            }
        }

        /// <summary>
        /// A publicly accessible version of guessEquationResultType().
        /// DEPRECATED, rather use dynamicTypeInference (it avoids guessing and uses runtime compiling and reflection to ensure the correct answer)
        /// </summary>
        /// <param name="valueAsString"></param>
        /// <returns></returns>
        public static string attemptValueTypeInference(string valueAsString)
        {
            //TODO add support for more than just bool, int, ulong, long, double, and string
            return guessEquationResultType(valueAsString);
        }

        /// <summary>
        /// A publicly accessible version of dynamicCalc() that takes an equation and evaluates its results
        /// </summary>
        /// <param name="theEquation"></param>
        /// <returns></returns>
        public static string calculateToString(string theEquation)
        {
            //validity checking of theEquation
            //error checking
            //edge cases:
            if (theEquation == null || theEquation.Length == 0 ) //this is the default behaviour of most calculators
                return "";

            try
            {
                return dynamicCalc(theEquation);
            }
            catch
            {
                Console.WriteLine("WARNING: That Appears to be a malformed expression we're assuming its a string");
                return '"'+ theEquation+'"';
            }
        }

        /// <summary>
        /// Takes an answer in the form of a string, infers its runtime type and returns the compiler equivalent. For example
        /// "42" -> Int32 -> int  |  Boolean -> bool   |  "\"42\"" -> String -> string  | Int64 -> long  
        /// </summary>
        /// <param name="theAnswer"></param>
        /// <returns></returns>
        public static string dynamicTypeInferenceToCompilerFormat(string theAnswer)
        {
            string answerType = dynamicTypeInference(theAnswer);
            switch (answerType)
            {
                case ("Int16"):
                    return "short";
                case ("Int32"):
                    return "int";
                case ("Int64"):
                    return "long";
                case ("UInt64"):
                    return "ulong";
                case ("String"):
                    return "string";
                case ("Boolean"):
                    return "bool";
                case ("Double"):
                    return "double";
                case ("Float"):
                    return "float";
                case ("Char"):
                    return "char";
                default:
                    return answerType;

            }
        }

        /// <summary>
        /// Returns a tuple in the format <Type,Answer> for example "1+2+3" -> <"6","int">
        /// </summary>
        /// <param name="theEquation"></param>
        /// <returns></returns>
        public static Tuple<string, string> getAnswerAndTypeOfEquation(string theEquation)
        {
            string theAnswer = calculateToString(theEquation);
            return new Tuple<string, string>(dynamicTypeInferenceToCompilerFormat(theAnswer), theAnswer);
        }


    }
}

