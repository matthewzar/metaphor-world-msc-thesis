using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CSMetaphorWorld
{
    public class clickableCalculatorSprite
    {
        public string expression;
        int pixelsPerChar;
        Vector2 clickableRegionTopLeft;
        Vector2 clickableRegionBottonRight;

        //new textSprite("The Calculator", 5, 20, 530);
        public clickableCalculatorSprite(string startExpression, int pixelsPerCharachter, int topLeftX, int topLeftY)
        {
            expression = startExpression;
            pixelsPerChar = pixelsPerCharachter;
            clickableRegionTopLeft = new Vector2(topLeftX, topLeftY);
            clickableRegionBottonRight = new Vector2((startExpression.Length + 1)* pixelsPerCharachter, topLeftY + 20);
        }

        public void changeText(string newText)
        {
            expression = newText;
        }

        public string getTextToDisplay()
        {
            return expression;
        }

        public Vector2 getPosition()
        {
            return clickableRegionTopLeft;
        }

        

        public clickableCalculatorSprite() : this("", 5, 0, 0) { }

        public clickableCalculatorSprite(string startExpression) : this(startExpression, 5, 0, 0) { }

        public clickableCalculatorSprite(int pixelsPerCharachter) : this("", pixelsPerCharachter, 0, 0) { }


        internal bool isClicked(int mouseXCoord, int mouseYCoord)
        {
            return mouseXCoord >= clickableRegionTopLeft.X && mouseXCoord <= clickableRegionBottonRight.X &&
                    mouseYCoord >= clickableRegionTopLeft.Y && mouseYCoord <= clickableRegionBottonRight.Y;
        }

        internal string getClickedWordifVariable()
        {
            return null;
        }

        internal string getClickedWord(int xPixelOffset)
        {
            int clickedCharIndex = xPixelOffset / pixelsPerChar;
            
            //assume that by this stage the exact charachter index clicked has been calculated
            string clickedWord = extractVariableAtIndex(expression,clickedCharIndex);
            
            //TODO: add checking to ensure that the selected word is in a variable format
            if(isLegalVariable(clickedWord))    
                return clickedWord;
            else
                return "";
        }

        private bool isLegalVariable(string testName)
        {
            if (testName == "Math")
                return false;
            else
                return true;
        }

        private string extractVariableAtIndex(string extractionExpression, int index)
        {
            //TODO consider whether Regexes would be more appropriate
            //look at the charachter at the index is it is a letter/number/underscore and shift left until a valid variable end is found
            if (index > extractionExpression.Length - 1)
                index = extractionExpression.Length - 1;

            while (!isValidVariableChar(extractionExpression[index]))
            {
                if (index == 0)
                    return null; //theres nothing left on the left to search
                else
                    index--;
            }

            // then determine left and right boundaries
            int leftBoundary = index;
            while (true)
            {
                if (leftBoundary == 0)
                    break; //theres nothing more to search for on the left

                if (isValidVariableChar(extractionExpression[leftBoundary - 1])) //is the one on the left legal
                    leftBoundary--; //if it is then make it the new left boundary
                else
                    break; //if not then you've found the left border
            }

            int rightBoundary = index;
            while (true)
            {
                if (rightBoundary >= extractionExpression.Length-1)
                    break; //theres nothing more to search for on the right

                if (isValidVariableChar(extractionExpression[rightBoundary + 1])) //is the one on the left legal
                    rightBoundary++; //if it is then make it the new left boundary
                else
                    break; //if not then you've found the left border
            }

            //now have left and right boundaries of the word in question
            return expression.Substring(leftBoundary, rightBoundary - leftBoundary + 1);
        }

        private bool isValidVariableChar(char charachterToCheck)
        {
            //TODO, change this to use regexes, the current technique is really messy (I was just too tired when writting it to do a neat job [I was looking for functionality over neatness])
            return ((charachterToCheck >= 48 && charachterToCheck <= 57) || //is it a number
                   (charachterToCheck >= 65 && charachterToCheck <= 90) || //uppercase letter
                   (charachterToCheck >= 97 && charachterToCheck <= 122) || //lowercase letter
                    charachterToCheck == 95); //is it an underscore
            
        }
    }
}
