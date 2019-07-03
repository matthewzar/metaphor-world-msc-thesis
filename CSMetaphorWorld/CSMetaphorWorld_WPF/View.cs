using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;


namespace CSMetaphorWorld_WPF
{
    class View
    {
        Canvas drawingTo;
        public View(Canvas targetCanvas)
        {
            drawingTo = targetCanvas;
        }

        /// <summary>
        /// Adds an (optionlay interactive) image to the canvas.
        /// </summary>
        /// <param name="imageAddress">The directory to locate the image file to use as thr source</param>
        /// <param name="width">The width that the image will be drawn as. Height scales to width keeping a constant aspect ratio</param>
        /// <param name="xCoord">The offset from the left side of the canvas. Negative numbers are acceptable</param>
        /// <param name="yCoord">The offset from the top of the canvas. Negative numbers are acceptable</param>
        /// <param name="mouseLeftUpEvent">The event event/method call to trigger when the mouse is button is released over the image.
        /// Leaving this null will cause no action to be linked. You can also pass lamda functions in, an example of a valid one would be:
        /// (object senderx, EventArgs ex) => { Console.WriteLine("Greetings Human");}</param>
        /// <returns>We return a pointer to the image component added so that you have the option to alter it at a later time</returns>
        public Image addInteractiveImageToCanvas(string imageAddress, double width, double xCoord, double yCoord, Action<object, EventArgs> mouseLeftUpEvent = null)
        {
            if (!System.IO.File.Exists(imageAddress))
                throw new ArgumentException("File does not exist");

            Image simpleImage = new Image();
            simpleImage.Width = width;
            simpleImage.Margin = new Thickness(5);

            // Create source.
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();

            #region getting the bloody source file, and documentation on the trouble it caused
            //  This is what this operation looked like originally, however it couldn't seem to handle any relative addresses (despite saying that it could):
            // bi.UriSource = new Uri(imageAddress, UriKind.RelativeOrAbsolute);


            //This was the second attempt and was based on the idea that perhaps we need to be more careful and explicit when using relative addresses 
            //For some reason when using this technique any relative addresses outside of the bin folder fail..while relative addresses inside succeed...theres something funny going on inside the Uri contructor
            /*if (imageAddress.Contains(@":\")) //all absolute addresses *should* always start with something like "C:\"
                bi.UriSource = new Uri(imageAddress, UriKind.Absolute);
            else
            {
                bi.UriSource = new Uri(new Uri(System.IO.Directory.GetCurrentDirectory()), imageAddress); //read this line as "create an absolute Uri for our current base directory, and then alter it using our relative address
            }*/

            //read this line as: use the Directory class to get the absolute Directory address of our file (relative or not), then add the name of the file to the end 
            string finalAddr = System.IO.Directory.GetParent(imageAddress).ToString() + imageAddress.Substring(imageAddress.LastIndexOf('\\'));
            bi.UriSource = new Uri(finalAddr, UriKind.Absolute);
            
            #endregion
            

            bi.EndInit();

            // Set the image source.
            simpleImage.Source = bi;
            
            //we now want to set the height of the simple image based on the ration between (bi.Width):(parameter width)
            simpleImage.Height = bi.Height * (width / bi.Width);



            ////////////before adding the image to our canvas we want to link an action/event to it (provided the user specifies one and doesn't just give us 'null'
            if(mouseLeftUpEvent != null)
                simpleImage.MouseLeftButtonUp += new MouseButtonEventHandler(mouseLeftUpEvent);
            ////////////

            drawingTo.Children.Add(simpleImage);

            Canvas.SetLeft(simpleImage, xCoord);
            Canvas.SetTop(simpleImage, yCoord);
            return simpleImage;
        }

        public TextBox addTextBox(double width, double xCoord, double yCoord, Action<object, KeyEventArgs> keyPressEvent = null)
        {
            TextBox newTextBox = new TextBox();
            newTextBox.Width = width;
            newTextBox.Margin = new Thickness(5);

            ////////////before adding the image to our canvas we want to link an action/event to it (provided the user specifies one and doesn't just give us 'null'
            if (keyPressEvent != null)
                newTextBox.KeyUp += new KeyEventHandler(keyPressEvent);
            ////////////

            drawingTo.Children.Add(newTextBox);

            Canvas.SetLeft(newTextBox, xCoord);
            Canvas.SetTop(newTextBox, yCoord);
            return newTextBox;

        }

        public Label addLabelToImage(Image image, string labelsText, int relativeXPos, int relativeYPos, bool isHitTestVisible = false)
        {
            Label newLabel = new Label();
           // newLabel.Width = width;
           // newLabel.Margin = new Thickness(5);

            drawingTo.Children.Add(newLabel);
            newLabel.Content = labelsText;
            newLabel.IsHitTestVisible = isHitTestVisible;

            Canvas.SetLeft(newLabel, Canvas.GetLeft(image) + relativeXPos);
            Canvas.SetTop(newLabel, Canvas.GetTop(image) + relativeYPos);
            return newLabel;
        }

        public Menu addMenu(int width, double xPos, double yPos, string[] menuOptions, Action<object, RoutedEventArgs> mouseClickEvent)
        {
            Menu newMenu = new Menu();
            newMenu.Width = width;
            newMenu.Height = menuOptions.Length*21;


            drawingTo.Children.Add(newMenu);

            for (int i = 0; i < menuOptions.Length; i++)
            {
                newMenu.Items.Add(new MenuItem());
                ((MenuItem)newMenu.Items[i]).MinWidth = width - 2;
                ((MenuItem)newMenu.Items[i]).Height = 20;
                ((MenuItem)newMenu.Items[i]).Header = menuOptions[i];
                ((MenuItem)newMenu.Items[i]).Click += new RoutedEventHandler(mouseClickEvent);

            }

            Canvas.SetLeft(newMenu, xPos);
            Canvas.SetTop(newMenu, yPos);
            return newMenu;
        }
            
            

        public void maintainRelativeLabelPosition(Image parentImage, Label labelToMove)
        {
            //the parent image may or may not have moved when we call this...it probably has so any information we glean form the images postion is irrelevant

            //the final position needs to be the offset from the x and y of the previous images position + the current images position
            //TODO: make it so that the new labels position is not always the same as its parent image

            Canvas.SetLeft(labelToMove, Canvas.GetLeft(parentImage));
            Canvas.SetTop(labelToMove, Canvas.GetTop(parentImage));
        }

        /// <summary>
        /// Replaces the image of a sprite with another image
        /// </summary>
        /// <param name="sprite">The sprite to alter</param>
        /// <param name="imageAddress">the address of the new image</param>
        /// <param name="adjustProportion">if 2 images have the same height and different widths, but you want to adjust the final width in order to maintain a ratio set this to true</param>
        internal void setSpriteImageTo(Sprite sprite, string imageAddress, bool adjustProportion = false)
        {
            if (imageAddress == sprite.imageAddress)
                return;

           

            sprite.imageAddress = imageAddress;

            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            string finalAddr = System.IO.Directory.GetParent(imageAddress).ToString() + imageAddress.Substring(imageAddress.LastIndexOf('\\'));
            bi.UriSource = new Uri(finalAddr, UriKind.Absolute);
            
            bi.EndInit();

            if (adjustProportion && bi.SourceRect.Width != sprite.width)
            {
                sprite.width = (sprite.width / sprite.imageComponent.Source.Width) * bi.Width;

                sprite.imageComponent.Width = sprite.width;
            }

            // Set the image source.
            sprite.imageComponent.Source = bi;
        }
    }
}
