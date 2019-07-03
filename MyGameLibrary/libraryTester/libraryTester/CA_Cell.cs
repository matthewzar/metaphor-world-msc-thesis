using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libraryTester
{
    class CA_Cell
    {
        public short xCoord_Current;
        public short yCoord_Current;
        public short xCoord_finalDestination;
        public short yCoord_finalDestination;
        public short xCoord_DesiredNextStep;
        public short yCoord_DesiredNextStep;
        short maximumXCoord;
        short maximumYCoord;
        public bool blocked;
        private float chanceOfNonOptimalMove;

        public CA_Cell(short theMaxXCoordinateValue, short theMaxYCoordinateValue)
            : this(-1,-1,-1,-1,-1,-1,theMaxXCoordinateValue,theMaxYCoordinateValue, 0.3f)
        { 
            //NTS: anything here (the body of an overloaded sub-constructor
            //     happens AFTER the main constructor is called
        }

        public CA_Cell(short currentX, short currentY,short theMaxXCoordinateValue, short theMaxYCoordinateValue)
            : this(currentX, currentY, -1, -1, -1, -1, theMaxXCoordinateValue, theMaxYCoordinateValue,0.3f)
        {
        }

        public CA_Cell(short currentX, short currentY, short finalDestinationX, short finalDestinationY, short theMaxXCoordinateValue, short theMaxYCoordinateValue, float chanceOfBadMove)
            : this(currentX, currentY, finalDestinationX, finalDestinationY, -1, -1, theMaxXCoordinateValue, theMaxYCoordinateValue, chanceOfBadMove) 
        {
        }

        public CA_Cell(short currentX, short currentY, short finalDestinationX, short finalDestinationY, short desiredNextStepX, short desiredNextStepY, short theMaxXCoordinateValue, short theMaxYCoordinateValue, float chanceOfBadMove)
        {
            maximumXCoord = theMaxXCoordinateValue;
            maximumYCoord = theMaxYCoordinateValue;
            xCoord_Current = currentX;
            yCoord_Current = currentY;
            xCoord_finalDestination = finalDestinationX;
            yCoord_finalDestination = finalDestinationY;
            xCoord_DesiredNextStep = desiredNextStepX;
            yCoord_DesiredNextStep = desiredNextStepY;
            chanceOfNonOptimalMove = chanceOfBadMove;

            if (chanceOfNonOptimalMove < 0 || chanceOfNonOptimalMove > 1)
                throw new Exception("chanceOfNotMostDirect required range is between 0 and 1");

            calculateDesiredNextStep();
            blocked = false;

        }

        /// <summary>
        /// based on the current location and final destination get the cell closest to the destination
        /// </summary>
        public void calculateDesiredNextStep()
        {
            if (xCoord_Current == xCoord_finalDestination && yCoord_Current == yCoord_finalDestination)
            {
                xCoord_DesiredNextStep = xCoord_Current;
                yCoord_DesiredNextStep = yCoord_Current;
                return;
            }

            Random myRandom = new Random((int)((((Math.Pow(xCoord_Current,yCoord_Current)
                                        + Math.Pow(xCoord_finalDestination,yCoord_finalDestination))
                                        *(1 + Math.Abs(Math.Sin(xCoord_DesiredNextStep*xCoord_finalDestination)))
                                        * System.DateTime.Now.Millisecond * System.DateTime.Now.Second * System.DateTime.Now.Minute))%2147483647));

            checkIfCurrentAndTargetValid();

            int chosenPath;

            //if its not a weird decision choose which of the "fastest paths" is approriate
            if (myRandom.NextDouble() > chanceOfNonOptimalMove)
            {
                #region lined up
                //lined up horizontally, current on left
                if (yCoord_Current == yCoord_finalDestination && xCoord_Current < xCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    chosenPath = myRandom.Next() % 3;

                    if (chosenPath == 1)
                        //if (yCoord_Current+1 >= maximum) y = yCoord_DesiredNextStep else y = (yCoord_Current + 1)
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    else
                        if (chosenPath == 2)
                            yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                        else
                            yCoord_DesiredNextStep = yCoord_Current;
                    return;
                }

                //lined up horizontally, current on right
                if (yCoord_Current == yCoord_finalDestination && xCoord_Current > xCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                    chosenPath = myRandom.Next() % 3;

                    if (chosenPath == 1)
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    else
                        if (chosenPath == 2)
                            yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                        else
                            yCoord_DesiredNextStep = yCoord_Current;

                    return;
                }

                //lined up vertically, current on top
                if (xCoord_Current == xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    chosenPath = myRandom.Next() % 3;

                    if (chosenPath == 1)
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    else
                        if (chosenPath == 2)
                            xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        else
                            xCoord_DesiredNextStep = xCoord_Current;

                    return;
                }

                //lined up vertically, current on bottom
                if (xCoord_Current == xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    chosenPath = myRandom.Next() % 3;

                    if (chosenPath == 1)
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    else
                        if (chosenPath == 2)
                            xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        else
                            xCoord_DesiredNextStep = xCoord_Current;

                    return;
                }
                #endregion

                #region perfectly diagonal
                //lined up diagonally, current at top right
                if (isDifferenceEqual() && xCoord_Current > xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                    yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);

                    return;
                }

                //lined up diagonally, current at top left
                if (isDifferenceEqual() && xCoord_Current < xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);

                    return;
                }

                //lined up diagonally, current at bottom left
                if (isDifferenceEqual() && xCoord_Current < xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);

                    return;
                }

                //lined up diagonally, current at bottom right
                if (isDifferenceEqual() && xCoord_Current > xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                    yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);

                    return;
                }
                #endregion

                #region non-aligned cases
                //lined up diagonally, current at top right
                if (xCoord_Current > xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    chosenPath = myRandom.Next() % 2;

                    if (chosenPath == 0)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    }
                    else if (chosenPath == 1)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (short)(yCoord_Current);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (short)(xCoord_Current);
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    }

                    return;
                }

                //lined up diagonally, current at top left
                if (xCoord_Current < xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    chosenPath = myRandom.Next() % 2;

                    if (chosenPath == 0)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    }
                    else if (chosenPath == 1)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                        yCoord_DesiredNextStep = (short)(yCoord_Current);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (short)(xCoord_Current);
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    }

                    return;
                }

                //lined up diagonally, current at bottom left
                if (xCoord_Current < xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    chosenPath = myRandom.Next() % 2;

                    if (chosenPath == 0)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    }
                    else if (chosenPath == 1)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                        yCoord_DesiredNextStep = (short)(yCoord_Current);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (short)(xCoord_Current);
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    }

                    return;
                }

                //lined up diagonally, current at bottom right
                if (xCoord_Current > xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    chosenPath = myRandom.Next() % 2;

                    if (chosenPath == 0)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    }
                    else if (chosenPath == 1)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (short)(yCoord_Current);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (short)(xCoord_Current);
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    }

                    return;
                }
                #endregion
            }
            else if (true)
            {

                #region lined up
                //lined up horizontally, current on left
                if (yCoord_Current == yCoord_finalDestination && xCoord_Current < xCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = xCoord_Current;

                    if (myRandom.Next() % 2 == 0)
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    else
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    return;
                }

                //lined up horizontally, current on right
                if (yCoord_Current == yCoord_finalDestination && xCoord_Current > xCoord_finalDestination)
                {
                    xCoord_DesiredNextStep = xCoord_Current;

                    if (myRandom.Next() % 2 == 0)
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    else
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    return;
                }

                //lined up vertically, current on top
                if (xCoord_Current == xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    yCoord_DesiredNextStep = yCoord_Current;

                    if (myRandom.Next() % 2 == 0)
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    else
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                    return;
                }

                //lined up vertically, current on bottom
                if (xCoord_Current == xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    yCoord_DesiredNextStep = yCoord_Current;

                    if (myRandom.Next() % 2 == 0)
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    else
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                    return;
                }
                #endregion

                #region perfectly diagonal
                //lined up diagonally, current at top right
                if (isDifferenceEqual() && xCoord_Current > xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                        xCoord_DesiredNextStep = xCoord_Current;
                    }
                    else
                    {
                        yCoord_DesiredNextStep = yCoord_Current;
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                    }
                    return;
                }

                //lined up diagonally, current at top left
                if (isDifferenceEqual() && xCoord_Current < xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                        xCoord_DesiredNextStep = xCoord_Current;
                    }
                    else
                    {
                        yCoord_DesiredNextStep = yCoord_Current;
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    }
                    return;
                }

                //lined up diagonally, current at bottom left
                if (isDifferenceEqual() && xCoord_Current < xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                        xCoord_DesiredNextStep = xCoord_Current;
                    }
                    else
                    {
                        yCoord_DesiredNextStep = yCoord_Current;
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    }
                    return;
                }

                //lined up diagonally, current at bottom right
                if (isDifferenceEqual() && xCoord_Current > xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = yCoord_Current;
                    }
                    else
                    {
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                        xCoord_DesiredNextStep = xCoord_Current;
                    }
                    return;
                }
                #endregion

                #region non-aligned cases
                //lined up diagonally, current at top right
                if (xCoord_Current > xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    }
                    return;
                }

                //lined up diagonally, current at top left
                if (xCoord_Current < xCoord_finalDestination && yCoord_Current < yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    }
                    return;
                }

                //lined up diagonally, current at bottom left
                if (xCoord_Current < xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                    }
                    return;
                }

                //lined up diagonally, current at bottom right
                if (xCoord_Current > xCoord_finalDestination && yCoord_Current > yCoord_finalDestination)
                {
                    if (myRandom.Next() % 2 == 0)
                    {
                        yCoord_DesiredNextStep = (yCoord_Current - 1) < 0 ? yCoord_Current : (short)(yCoord_Current - 1);
                        xCoord_DesiredNextStep = (xCoord_Current + 1) >= maximumXCoord ? xCoord_Current : (short)(xCoord_Current + 1);
                    }
                    else
                    {
                        xCoord_DesiredNextStep = (xCoord_Current - 1) < 0 ? xCoord_Current : (short)(xCoord_Current - 1);
                        yCoord_DesiredNextStep = (yCoord_Current + 1) >= maximumYCoord ? yCoord_Current : (short)(yCoord_Current + 1);
                    }
                    return;
                }
                #endregion

            }
            else
            {
                //move away from target
            }
            

            //unreachable:
            if (xCoord_DesiredNextStep < 0 || xCoord_DesiredNextStep >= maximumXCoord ||
                yCoord_DesiredNextStep < 0 || yCoord_DesiredNextStep >= maximumYCoord)
            {
                Console.WriteLine("Shoot");
            }

        }

        public void blockCell()
        {
            blocked = true;
        }

        private bool isDifferenceEqual()
        {
            if (Math.Abs(xCoord_Current - xCoord_finalDestination) == Math.Abs(yCoord_Current - yCoord_finalDestination))
                return true;
            else
                return false;
        }

        public void moveToDesiredNextStep()
        {
            if (blocked == false)
            {
                checkIfAllValid();
                xCoord_Current = xCoord_DesiredNextStep;
                yCoord_Current = yCoord_DesiredNextStep;
            }
            blocked = false;

            //calculateDesiredNextStep(0.3f);
        }

        private void checkIfAllValid()
        {
            if (xCoord_Current < 0)
                throw new Exception("Undefined X location");
            if (yCoord_Current < 0)
                throw new Exception("Undefined Y location");

            if (xCoord_finalDestination < 0)
                throw new Exception("Invalid final destination X Coordinate");
            if (yCoord_finalDestination < 0)
                throw new Exception("Invalid final destination Y Coordinate");

            if (xCoord_DesiredNextStep < 0)
                throw new Exception("Invalid desired destination X Coordinate");
            if (yCoord_DesiredNextStep < 0)
                throw new Exception("Invalid desired destination Y Coordinate");
        }

        private void checkIfCurrentAndTargetValid()
        {
            if (xCoord_Current < 0)
                throw new Exception("Undefined X location");
            if (yCoord_Current < 0)
                throw new Exception("Undefined Y location");

            if (xCoord_finalDestination < 0)
                throw new Exception("Invalid final destination X Coordinate");
            if (yCoord_finalDestination < 0)
                throw new Exception("Invalid final destination Y Coordinate");
        }
    
    }
}
