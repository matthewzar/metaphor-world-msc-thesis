using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CSMetaphorWorld
{
    /// <summary>
    /// This class displays a series of menus, and performs actions when certain keys are pressed (both the action-keypress pairs are determined outside the class
    /// </summary>
    class TextDisplay
    {
        clickableCalculatorSprite calculatorEntity;
        textSprite handEntity;
        textSprite methMechTextEntity;
        textSprite stackEntity;
        textSprite memManEntity;
        textSprite heapEntity;

        List<textSprite> localVariables;

        SpriteFont font;

        public bool drawTextFlag = false;
        public bool enabled = false;


        string[] commandsText = new string[]{"V -> Variable & value interactions.    M -> Memory Manager Instructions.    F -> Function//Method//Procedure control Instructions",
                                        
                                        //variable ops
                                         "D -> Declare a variable     A -> Assign the value in your hand to a variable\n" +
                                         "W -> Write an expression into the calculator (Lone values are just very simple expressions)\n"+
                                         "S -> Substitute a variable name in the calculators expression with the value in your hand\n"+
                                         "E -> Evaluate the calculators expression and place resulting value in hand      R -> Read Variables Value into Hand\n",
        
                                         //Method mechanism ops
                                          "N -> Name the method you want to call    D -> Declare a parameter    C -> Call selected method\n" +
                                          "A -> Assign hands value to a parameter R -> Return from the current frame (with what's in hand)\n",
        
                                        //Heap/MemManager ops 
                                          "A -> Allocate Array space in the heap   G -> Give the memory manager an address and offset C -> Copy value from the heap to MemM\n" +
                                          "W -> Write the value being held by MemMan to heap    R -> Write MM val to stack     O -> Copy a value from the stack by reference\n" +
                                          "S -> Send the value in your hand to the memory manager    L -> Garbage Collect/ cLean the heap    T -> Take value from the MemMan \n" +
                                          "V -> Convert header and offset to absolute address\n"};

        //actions key menu triples
        List<Tuple<Keys, Action, string>> keyActionMenuTriple;

        //This will be used for helping to tell which of the strings in commandsText to display, and therefore which set of commands are active
        enum selectedMenu
        {
            front = 0,
            variables = 1,
            methods = 2,
            memMan = 3
        };

        selectedMenu currentInstructionSet = selectedMenu.front;

        public void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>("myFont");
        }

        public TextDisplay(SpriteFont displayFont, List<Tuple<Keys, Action, string>> listOfActionsForKeyPressAndMenu)
        {
            handEntity = new textSprite("The Hand", 10, 500, 200, 520);
            calculatorEntity = new clickableCalculatorSprite("The Calculator", 10, 20, 530);
            methMechTextEntity = new textSprite("Method Mechanism", 30, 560, 200, 580);
            stackEntity = new textSprite("The Stack", 10, 30, 300, 200);
            memManEntity = new textSprite("Memory Manager", 600, 560, 800, 670);
            heapEntity = new textSprite("The Heap", 600, 10, 800, 500);
            localVariables = new List<textSprite>();
            font = displayFont;
            keyActionMenuTriple = listOfActionsForKeyPressAndMenu;
            
        }

        public void Draw(SpriteBatch spriteBatch, int screenHeight)
        {
            if(drawTextFlag == false || !enabled) //if draw text or the whole overlay isn't enabled then don't draw anything)
                return;
            spriteBatch.DrawString(font, heapEntity.getTextToDisplay(), heapEntity.getPosition(), Color.White);
            spriteBatch.DrawString(font, stackEntity.getTextToDisplay(), stackEntity.getPosition(), Color.White);
            spriteBatch.DrawString(font, handEntity.getTextToDisplay(), handEntity.getPosition(), Color.White);
            spriteBatch.DrawString(font, calculatorEntity.getTextToDisplay(), calculatorEntity.getPosition(), Color.White);
            spriteBatch.DrawString(font, methMechTextEntity.getTextToDisplay(), methMechTextEntity.getPosition(), Color.White);
            spriteBatch.DrawString(font, memManEntity.getTextToDisplay(), memManEntity.getPosition(), Color.White);

            //draw the local variables
            foreach (textSprite variable in localVariables)
            {
                spriteBatch.DrawString(font, variable.getTextToDisplay(), variable.getPosition(), Color.White);
            }


            spriteBatch.DrawString(font, commandsText[(int)currentInstructionSet], new Vector2(10, screenHeight - commandsText[(int)currentInstructionSet].Count(xx => xx == '\n') * 23 - 30), Color.Black);

            
        }

        public void Update(WorldTracker aWorldTracker, KeyboardState oldKeyboardState, KeyboardState currentKeyboardState)
        {
            //This used to have a heap of click checks, but seeing as this is being used for debugging and NOT game play anymore, I removed them

            if (!enabled)
                return; //if the text overlay isn't enabled then don't update anything

            heapEntity.changeText("The Heap\n" + aWorldTracker.globalHeap.ToString());
            memManEntity.changeText(aWorldTracker.theMemManager.ToString());

            stackEntity.changeText("The Stack" + aWorldTracker.globalStack.getNonLocalsAsString());
            if (aWorldTracker.thePlayer.examineHeld() != null)
                handEntity.changeText(string.Format("Hand contains: {0}, which is listed as type {1} and originates from {2}", 
                aWorldTracker.thePlayer.examineHeld().read(), aWorldTracker.thePlayer.examineHeld().readType(),
                aWorldTracker.thePlayer.examineHeld().readOrigin()));
            else
                handEntity.changeText("The Hand is empty");

            if (aWorldTracker.theCalculator.ToString() == "")
                calculatorEntity.changeText("Calculator is blank");
            else
                calculatorEntity.changeText(aWorldTracker.theCalculator.ToString() + " is in the calculator");
            methMechTextEntity.changeText(aWorldTracker.methodMech.ToString());

            //creating the dissasociated local variables that can be clicked:
            localVariables = new List<textSprite>();
            int y = stackEntity.getTextToDisplay().Count(xx => xx == '\n') * 31;
            int x = 40;
            foreach (Tuple<string, string> elem in aWorldTracker.globalStack.getLocalsAsList())
            {

                localVariables.Add(new textSprite(elem.Item2,
                                            new Vector2(x, y),
                                            new Vector2(x + elem.Item2.Length * 9, y + 21),
                                            elem.Item1, false));
                y += 22;
                x += 5;
            } 
            
            //Check whether the disaplyed menu needs to change
            menuSwitching(oldKeyboardState,  currentKeyboardState);



            foreach (Tuple<Keys, Action, string> triple in keyActionMenuTriple)
            {
                if (actionIsSafe(triple.Item1, oldKeyboardState, currentKeyboardState) && inCorrectMenu(triple.Item3))
                {
                    triple.Item2();
                    break; //only one action per update
                }
            }


        }

        private bool inCorrectMenu(string neededMenu)
        {
            if (neededMenu.ToLower() == "front" && currentInstructionSet == selectedMenu.front ||
                neededMenu.ToLower() == "home" && currentInstructionSet == selectedMenu.front || 
                neededMenu.ToLower() == "menu" && currentInstructionSet == selectedMenu.front ||
                neededMenu.ToLower() == "main" && currentInstructionSet == selectedMenu.front) 
                return true;

            if(neededMenu.ToLower() == "memory" && currentInstructionSet == selectedMenu.memMan ||
               neededMenu.ToLower() == "memman" && currentInstructionSet == selectedMenu.memMan) 
                return true;

            if(neededMenu.ToLower() == "methods" && currentInstructionSet == selectedMenu.methods ||
               neededMenu.ToLower() == "functions" && currentInstructionSet == selectedMenu.methods ||
               neededMenu.ToLower() == "method" && currentInstructionSet == selectedMenu.methods ||
               neededMenu.ToLower() == "function" && currentInstructionSet == selectedMenu.methods) 
                return true;

            if (neededMenu.ToLower() == "values" && currentInstructionSet == selectedMenu.variables ||
                neededMenu.ToLower() == "variables" && currentInstructionSet == selectedMenu.variables ||
               neededMenu.ToLower() == "vars" && currentInstructionSet == selectedMenu.variables) 
                return true;

            return false;
        }

        private void menuSwitching(KeyboardState oldState, KeyboardState currentState)
        {
            if (isLegalInstruction((int)selectedMenu.front, Keys.V, oldState, currentState))
            {
                currentInstructionSet = selectedMenu.variables;
            }
            if (isLegalInstruction((int)selectedMenu.front, Keys.M, oldState, currentState))
            {
                currentInstructionSet = selectedMenu.memMan;
            }
            if (isLegalInstruction((int)selectedMenu.front, Keys.F, oldState, currentState))
            {
                currentInstructionSet = selectedMenu.methods;
            }

            //go back to front menu
            if (isLegalInstruction(selectedMenu.variables, Keys.B, oldState, currentState) ||
                isLegalInstruction(selectedMenu.memMan, Keys.B, oldState, currentState) ||
                isLegalInstruction(selectedMenu.methods, Keys.B, oldState, currentState))
            {
                currentInstructionSet = selectedMenu.front;
            }
        }

        /// <summary>
        /// checks that: the key has been pressed, we are not currently recieveing input, and the currentInstructionSet matches what you want done
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="pressedKey"></param>
        /// <returns></returns>
        private bool isLegalInstruction(selectedMenu selectedState, Keys pressedKey, KeyboardState oldState, KeyboardState currentState)
        {
            
            return Guide.IsVisible == false && selectedState == currentInstructionSet &&
                oldState.IsKeyUp(pressedKey) && currentState.IsKeyDown(pressedKey);
        }

        private bool actionIsSafe(Keys theKey, KeyboardState oldState, KeyboardState currentState)
        {
            if (Guide.IsVisible == false && oldState.IsKeyUp(theKey) && currentState.IsKeyDown(theKey))
                return true;
            return false;
        }


        #region menu navigation (all unnecesary in non-textaul versions)
        /// <summary>
        /// Goes through various key presses and states in order to navigate through interactive menues, this is not neccesary in the none textual versions
        /// </summary>

  
        /*
        private void methodMechanismInteractions(KeyboardState oldState, KeyboardState currentState)
        {
            //check if any further checks need to be made about method mechanism operations:
            if (currentInstructionSet != (int)selectedMenu.methods)
                return; //if not then simply return

            if (actionIsSafe(Keys.P, oldState, currentState)) //Pass hand value as paramater
            {
                methodMech.passSingleParameter(thePlayer.takeFromHand());
                assignFunctionCallingSpritesAsAction();
                return; //Only one operation can be done at a time so no need to check more
            }

            if (actionIsSafe(Keys.C, oldState, currentState)) //Call selected method
            {
                //keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Call method", "Enter the name of the method you want to call", "sumFun", callFunction, null);
                globalStack.push(methodMech.callSelectedMethod());
                assignCurrentFrameSpritesAsAction();
                assignFunctionCallingSpritesAsAction();//TODO: change this to also reset the mechanisms content to zero, and place its old content in the new stack frame
                return; //Only one operation can be done at a time so no need to check more
            }
            if (actionIsSafe(Keys.N, oldState, currentState)) //Name the method you want to call
            {

                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Name the method", "Enter the name of the method you want to call", "sumMethod", nameMethod, null);
                return; //Only one operation can be done at a time so no need to check more
            }

            if (actionIsSafe(Keys.R, oldState, currentState)) //Return from the current frame (with what's in hand)
            {
                globalStack.pop();
                return; //Only one operation can be done at a time so no need to check more
            }

            if (actionIsSafe(Keys.U, oldState, currentState)) //Unassigned (top) Parameter to hand
            {
                thePlayer.holdValue(globalStack.getTopFrame().takeFirstUnassignedParameter());
                return; //Only one operation can be done at a time so no need to check more
            }
        }*/
        #endregion

        internal void disable()
        {
            enabled = false;
        }

        internal void enable()
        {
            enabled = true;
        }
    }
}
