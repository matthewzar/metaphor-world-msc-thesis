using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mastersLibrary;

using Microsoft.CSharp;
using System.CodeDom.Compiler;


namespace testingProject
{
    public partial class Form1 : Form
    {
        inventoryContent myInventory;
        codeTracker ct;

        public Form1()
        {
            myInventory = new inventoryContent();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*Inventory test code:
            myInventory.variables.AddLast(new variable(110, "String", "myString1", DateTime.Now.Second.ToString()));
            myInventory.variables.AddLast(new variable(1410, "String", "another", "Details"));
            myInventory.variables.AddLast(new variable(112410, "String", "temp", "xyz"));
            myInventory.variables.AddLast(new variable(10, "String", "name", "Matthew"));
            myInventory.variables.AddLast(new variable(2, "int", "myInt", "123"));
            myInventory.variables.AddLast(new variable(42, "int", "myInt2", "234"));

            myInventory.assignValueToVariable("myString1", "Hi");
            myInventory.assignValueToVariableFromVars("temp", "name");
            myInventory.assignValueToVariableFromVars("another", "myString1", "name", "+");

            Console.WriteLine(myInventory.variableInteraction("myInt", "myInt2", "-"));
            */

           // myInventory.listVariables();

            ct = new codeTracker(tb_fileName.Text, "1");
            Console.WriteLine("");

            
            richTextBox1.Text = "";
            foreach (var line in ct.getCode())
            {
                richTextBox1.Text += line+"\n";
            }

            
            
        }

        private void bt_forward_Click(object sender, EventArgs e)
        {
            if (ct == null)
                Console.WriteLine("Load some code first");
            else
            {
                int lineCounter = 0;
                foreach (string line in richTextBox1.Lines)
                {
                    //add conditional statement if not selecting all the lines
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(lineCounter), line.Length);
                    
                    if (lineCounter == ct.getCurrentLineNumber())
                        richTextBox1.SelectionBackColor = Color.Yellow;
                    else
                        richTextBox1.SelectionBackColor = Color.White;
                    
                    if (ct.getCurrentLineDescriptor() != "")
                        richTextBox1.SelectionColor = Color.Red;
                    else
                        richTextBox1.SelectionColor = Color.Black;

                    lineCounter++;
                }
                displayVariables(rtb_VariableVals);
                ct.counterAdvance();
            }
        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            if (ct == null)
                Console.WriteLine("Load some code first");
            else
            {
                int lineCounter = 0;
                foreach (string line in richTextBox1.Lines)
                {
                    //add conditional statement if not selecting all the lines
                    if (lineCounter == ct.getCurrentLineNumber())
                    {
                        richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(lineCounter), line.Length);
                        richTextBox1.SelectionBackColor = Color.Yellow;
                    }
                    lineCounter++;
                }
                displayVariables(rtb_VariableVals);
                ct.counterRetreat();
            }
        }


        private void displayVariables(RichTextBox displayBox)
        {
            displayBox.Text = "";
            foreach (var x in ct.getCurrentVariables())
            {
                displayBox.Text += x + "\n";
            }
            if(ct.getCurrentLineDescriptor() != "")
                displayBox.Text += "\n" + ct.getCurrentLineDescriptor();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "foo.exe", true);
            parameters.GenerateExecutable = true;
            CompilerResults results = csc.CompileAssemblyFromSource(parameters,
            @"using System;
                class Program {
                  public static void Main(string[] args) 
                  {
                    Console.WriteLine();
                  }
                }");
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
            

       }

        
        
    }

    //adds methods to already existing classes
    public static class myExtensionMethods
    {
        public static int getSquare(this int num)
        {
            return num * num;
        }
    }

    
    public static class myLINQ
    {
        /*
           var myList = new int[] { 1, 2, 3, 4, 5 };
           foreach (var x in  myList.myWhere(x => x > 3))
                Console.WriteLine(x);
         */
                                    //'this' adds "myWhere" to all IEnumerables
        public static IEnumerable<T> myWhere<T>(this IEnumerable<T> things, Func<T, bool> predicate)
        {
            foreach (var x in things)
            {
                if (predicate(x) == true)
                    yield return x;
            }
        }
    }

    public static class otherThings
    {
        public static Action login(string username, string password)
        {
            /* testing code:
            Func<string, string,Action> a = login;
            var relogin = a("Hi", "There");
            relogin();
             */
            int count = 0;
            //this 'code' gets returned as a 'function' so that the details dont have to be passed around (good for keeping data private)
            Action login = () =>
            {
                count++;
                Console.WriteLine("Hairy login code with details {0}, {1}, attempt number {2}", username, password, count);
            };
            login(); //<--- this line doesnt need to be here unless you want the login code to be executed as soon as this 'function' is created but before its returned
            return login;
        }
    }


}
