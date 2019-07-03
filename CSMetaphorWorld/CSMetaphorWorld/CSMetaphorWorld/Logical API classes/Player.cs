using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class Player
    {
        //Currently bypassing the notepad class for simplicity
        Value inHand;

        /// <summary>
        /// Keeps track of whether the value being held has been read or not, defaults to true as when you start off the first value that the player hold is meaningless and thus can be marked as read
        /// </summary>
        bool beenRead = true;

        public Player()
        {

        }

        /// <summary>
        /// Lets you know if an operation that reads it's value has been read. This allows you to determine if the held value is stale/has been used
        /// </summary>
        /// <returns></returns>
        public bool hasBeenRead()
        {
            return beenRead;
        }


        /// <summary>
        /// returns a reference to the value being held by the player, but does not make the value no longer held
        /// You should only use this method when you don't plan on changing or copying the values content
        /// </summary>
        /// <returns></returns>
        public Value examineHeld()
        {
            return inHand;
        }

        /// <summary>
        /// removes whatever value is being held by the player (or rather it makes said value no longer refered to by the player...it does still exist)
        /// When using a register type operation, this method should not be used, as registers are never actually empty
        /// </summary>
        public void removeHeld(Value replacementValue = null)
        {
            beenRead = true;
            inHand = replacementValue;
        }

        public void holdValue(Value valToHold)
        {
            beenRead = false;
            inHand = valToHold;
        }

        /// <summary>
        /// Returns a reference to the value being held in hand, and then removes reference to that value being in hand
        /// Bassically examineHeld() followed by removeHeld()
        /// </summary>
        /// <returns></returns>
        public Value takeFromHand()
        {
            var tmp = inHand;
            removeHeld();
            return tmp;
        }

        public Value copyFromHand()
        {
            //mark the held value as having been read
            beenRead = true;
            return inHand.clone();
        }

        public bool isHandEmpty()
        {
            return inHand == null;
        }

        public void holdValue(string valueType, string valueAsString, string valueOrigin)
        {
            holdValue(new Value(valueAsString, valueType, valueOrigin));
        }

        /// <summary>
        /// An overload that attempts to infer the value type of the string value being provided, as this sort of inference is not perfect
        /// only use this overload when absolutly neccessary
        /// </summary>
        /// <param name="valueAsString"></param>
        public void holdValue(string valueAsString, string valueOrigin)
        {
            holdValue(new Value(valueAsString, "Expression", valueOrigin));
        }
    }
}
