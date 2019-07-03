using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CSMetaphorWorld
{
    public class interactiveRegion
    {
        int topLeftX = 0;
        int bottomRightX = 0;
        int topLeftY = 0;
        int bottomRightY = 0;

        interactionModes interactionMode = interactionModes.mouse_left_press;
        public enum interactionModes
        {
            keyboard_press,
            keyboard_release,
            keyboard_hold,

            mouse_right_press,
            mouse_right_release,
            mouse_right_hold,

            mouse_left_press,
            mouse_left_release,
            mouse_left_hold,

            mouse_over
        }

        Action whatToDo;

        /// <summary>
        /// When the mode is keyboard-something this is the button that will be compared to
        /// </summary>
        Keys keyboardButtonInQuestion = Keys.None;

        public interactiveRegion(int xCoordUpper, int yCoordUpper, int xCoordLower, int yCoordLower, Action actionToPerform,  interactionModes actionMode, Keys keyboardButton = Keys.None)
        {
            interactionMode = actionMode;
            whatToDo = actionToPerform;
            topLeftX = xCoordUpper;
            bottomRightX = xCoordLower;
            topLeftY = yCoordUpper;
            bottomRightY = yCoordLower;
            keyboardButtonInQuestion = keyboardButton;
        }

        public void addPositionalOffset(int xOffset, int yOffset)
        {
            topLeftX += xOffset;
            bottomRightX += xOffset;
            topLeftY += yOffset;
            bottomRightY += yOffset;
        }

        public bool mouseClicked(MouseState oldMouseState, MouseState currentMouseState)
        {
            return interactionMode == interactionModes.mouse_right_press && oldMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released ||
                   interactionMode == interactionModes.mouse_left_press && oldMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
        }

        public bool mouseInRegion(MouseState currentMouseState)
        {
            return currentMouseState.X >= topLeftX && currentMouseState.X <= bottomRightX &&
                   currentMouseState.Y >= topLeftY && currentMouseState.Y <= bottomRightY;
        }



        public bool keyPressed(KeyboardState oldKeyboardState, KeyboardState currentKeyboardState)
        {
            return interactionMode == interactionModes.keyboard_press && currentKeyboardState.IsKeyUp(keyboardButtonInQuestion) && oldKeyboardState.IsKeyDown(keyboardButtonInQuestion);
        }

        public bool attemptInteraction(MouseState oldMouseState, MouseState currentMouseState, KeyboardState oldKeyboardState,  KeyboardState currentKeyboardState)
        {
            switch(interactionMode)
            {
                case(interactionModes.mouse_over):
                    if (mouseInRegion(currentMouseState))
                    {
                        whatToDo();
                        return true;
                    }  
                    break;
                case(interactionModes.keyboard_press):
                    if (keyPressed(oldKeyboardState, currentKeyboardState))
                    {
                        whatToDo();
                        return true;
                    }
                    break;
                case(interactionModes.keyboard_release):
                    throw new NotImplementedException();
                case (interactionModes.keyboard_hold):
                    throw new NotImplementedException();


                case (interactionModes.mouse_left_press):
                case (interactionModes.mouse_right_press):
                    if (mouseClicked(oldMouseState, currentMouseState) &&
                        mouseInRegion(currentMouseState))
                    {
                        whatToDo();
                        return true;
                    }    
                    break;


                case (interactionModes.mouse_right_release):
                    throw new NotImplementedException();
                case (interactionModes.mouse_right_hold):
                    throw new NotImplementedException();

                
                case (interactionModes.mouse_left_release):
                    throw new NotImplementedException();
                case (interactionModes.mouse_left_hold):
                    throw new NotImplementedException();

            }
            return false;
        }


    }
}
