using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSMetaphorWorld;
using System.IO;


namespace CSMetaphorWorld_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Control Controller;
        

        public MainWindow()
        {
            //Initialize all the basics BEFORE we try to alter them
            InitializeComponent();
            Controller = new Control(this);
            
        }

        private void canvas_Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //GetPosition is known to misbehave at times, if it starts misbehaving then we can the workaround descibed here:
            //http://tech.pro/tutorial/893/wpf-snippet-reliably-getting-the-mouse-position

            MainWindow.GetWindow(this).Title = string.Format("last X = {0}  Last Y = {1}",Mouse.GetPosition(canvas_Main).X,Mouse.GetPosition(canvas_Main).Y);

        }

        private void btn_SpriteAdder_Click(object sender, RoutedEventArgs e)
        {
            addNewSprite(tb_SpriteName.Text, tb_FileName.Text, Convert.ToInt32(tb_width.Text),  Convert.ToDouble(tb_XPos.Text),Convert.ToDouble(tb_YPos.Text));
        }

        public void addNewSprite(string key, string filename, int width, double xPos, double yPos)
        {
            if (Controller.theMainSpritesList.ContainsKey(key))
            {
                Controller.theMainSpritesList.Remove(key);
            }

            while (canvas_Main.Children.Count > 0)
            {
                canvas_Main.Children.RemoveAt(0);
            }
            
            if(filename.Contains('\\') ||  filename.Contains("..."))
                Controller.theMainSpritesList.Add(key, new Sprite(filename, width, xPos, yPos));
            else
                Controller.theMainSpritesList.Add(key, new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\"+filename,width,xPos,yPos));
            
            Controller.theView = new View(Controller.parentWindow.canvas_Main);

            Controller.LoadNewLevel("empty", ref Controller.Model);

            displaySpriteDetailsToConsole();
        }

        public void displaySpriteDetailsToConsole()
        {
            Console.WriteLine("\n");
            foreach (var sprtPair in Controller.theMainSpritesList)
            {
                Console.WriteLine("Key = {0}  Width = {1}   X = {2}   Y = {3}", sprtPair.Key, sprtPair.Value.width, sprtPair.Value.xCoord, sprtPair.Value.yCoord);
            }
           
            Console.WriteLine("AsCode:");

            foreach (var sprtPair in Controller.theMainSpritesList)
            {
                Console.WriteLine("theMainSpritesList.Add(\"{0}\", new Sprite(@\"{1}\", {2}, {3}, {4}, {5}));",
                    sprtPair.Key, sprtPair.Value.imageAddress, sprtPair.Value.width, sprtPair.Value.xCoord, sprtPair.Value.yCoord, "\n\t(object senderx, EventArgs ex) => { }"); 
              //Console.WriteLine("theMainSpritesList.Add(\"{0}\", new Sprite(@\"{1}\", {2}, {3}, {4}, (object senderx, EventArgs ex) => { }));", sprtPair.Key); 
                                   // sprtPair.Key, sprtPair.Value.imageAddress, sprtPair.Value.width, sprtPair.Value.xCoord, sprtPair.Value.yCoord);
            }
            

        }

        private void canvas_Main_Drop(object sender, DragEventArgs e)
        {
            //more tutorial on drag and drop here: http://wpftutorial.net/DragAndDrop.html
            if (e.Data.GetDataPresent("FileDrop"))
            {
                string[] x = (string[])e.Data.GetData("FileDrop");
                if(x[0].Contains(".png") || x[0].Contains(".bmp") || x[0].Contains(".jpg"))
                {
                    string input = "";
                    if (InputBox("Input", "Please enter a name for use as an identification key", ref input) == System.Windows.Forms.DialogResult.OK)
                    {
                        addNewSprite(input, x[0], 100, 10, 10);
                    }
                }
            }
              
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!");
        }


        public static System.Windows.Forms.DialogResult InputBox(string title, string promptText, ref string value)
        {


            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
            System.Windows.Forms.Button buttonOk = new System.Windows.Forms.Button();
            System.Windows.Forms.Button buttonCancel = new System.Windows.Forms.Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | System.Windows.Forms.AnchorStyles.Right;
            buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 107);
            form.Controls.AddRange(new System.Windows.Forms.Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            System.Windows.Forms.DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {

        }

        private void canvas_Main_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
     
        }


    }
}
