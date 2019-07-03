using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Text;

namespace CSMetaphorWorld
{
    //public class multiSpriteCollection<TKey, TValue> : IDictionary<string, Tuple<Dictionary<string, dynamicSprite>,
    //                                                                             Dictionary<string, dynamicSprite>, 
    //                                                                             Dictionary<string, Tuple<Color, Color, Color, Color, Color>>,
    //                                                                             Dictionary<string, Tuple<Color, Color, Color, Color, Color>>>>
    public class multiSpriteCollection
    {
        public int Count { get { return primarySprites.Count; } }

        /// <summary>
        /// The appearance and behaviour of these sprites is linked to those of the primary sprites: 
        /// any changes to these sprites are caused by the primary sprites being interacted with in some way
        /// </summary>
        Dictionary<string,dynamicSprite> ancillarySprites;
        
        /// <summary>
        /// The primary sprites are there to be interacted with and displayed.
        /// </summary>
        Dictionary<string, dynamicSprite> primarySprites;
        
        /// <summary>
        /// Each sprite has 5 possible tinting colours, in order from lowest to highest priority they are tinting colour when:
        /// Not howevered over, Hovered over, pressed, clicked, special.
        /// Leaving a colour as null will result in the colour defaulting to whatever colour is set as the default, unless
        /// one of the other colour conditions is met
        /// </summary>
        Dictionary<string,Tuple<Color,Color,Color,Color,Color>> primaryDrawTintColours;

        /// <summary>
        /// Much the same as the primaryDrawTintColours, except behaviour is determined by interaction with the primary sprites,
        /// while colour changes affect the acillary sprites.
        /// priority - Not howevered over, Hovered over, pressed, clicked, special.
        /// </summary>
        Dictionary<string, Tuple<Color, Color, Color, Color, Color>> ancillaryDrawTintColours;

        Dictionary<string, Predicate<string>> specialColoursConditionals;

        private Predicate<string> alwaysFalse = delegate(string key) { return false; };

        Color defaultColour = Color.White;

        enum drawOrder
        {
            allPrimariesFirst=1,
            allAncillariesFirst=2,
            primary_Ancillary_Repeat=3,
            ancillary_Primary_Repeat=4
        }
        drawOrder spriteDrawOrder = drawOrder.allPrimariesFirst;

        /// <summary>
        /// Item 1 -> primary spritess
        /// Item 2 -> ancillary sprites
        /// Item 3 -> primary tints
        /// Item 4 -> ancilarry tints
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tuple<dynamicSprite, dynamicSprite, Tuple<Color, Color, Color, Color, Color>, Tuple<Color, Color, Color, Color, Color>> getTupleSetFromKey(string key)
        {
            return new Tuple<dynamicSprite, dynamicSprite, Tuple<Color, Color, Color, Color, Color>, Tuple<Color, Color, Color, Color, Color>>(
                        primarySprites[key], ancillarySprites[key], primaryDrawTintColours[key], ancillaryDrawTintColours[key]);
        }

        public void changeAllPrimaryImagesTo(string key, string newImageName)
        {
            primarySprites[key].changeAllImagesTo(newImageName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newPrimarySprite"></param>
        /// <param name="newAncillarySprite"></param>
        /// <param name="newPrimarySpritesTintColours">NotHovered - Hovered Over - Pressed - Clicked - specialCondition</param>
        /// <param name="newAncillarySpritesTintColours"></param>
        /// <param name="specialColourConditional"></param>
        public void Add(string key, dynamicSprite newPrimarySprite, dynamicSprite newAncillarySprite,
                        Tuple<Color, Color, Color, Color, Color> newPrimarySpritesTintColours, Tuple<Color, Color, Color, Color, Color> newAncillarySpritesTintColours,
                        Predicate<string> specialColourConditional)
        {
            primarySprites.Add(key, newPrimarySprite);
            ancillarySprites.Add(key, newAncillarySprite);
            primaryDrawTintColours.Add(key, newPrimarySpritesTintColours);
            ancillaryDrawTintColours.Add(key, newAncillarySpritesTintColours);
            
            if(specialColourConditional == null)
                specialColoursConditionals.Add(key, alwaysFalse);
            else
                specialColoursConditionals.Add(key, specialColourConditional);
        }

        public bool ContainsKey(string key)
        {
            return primarySprites.ContainsKey(key);
        }

        public dynamicSprite getPrimaryDynamicSprite(string key)
        {
            return primarySprites[key];
        }

        public void Clear()
        {
            primarySprites.Clear();
            ancillarySprites.Clear();
            primaryDrawTintColours.Clear();
            ancillaryDrawTintColours.Clear();
        }

        public bool Contains(object key)
        {
            //becuase the same key is used for all 4 class dictionaries, we only need to check if it is it one of them
            return primarySprites.ContainsKey((string)key);
        }

        //private IEnumerable<string> getPermutations(string elements)
        //{
        //    if (elements.Length <= 1)
        //        yield return elements;
        //    else
        //    {
        //        foreach (string perm in getPermutations(elements.Substring(1)))
        //            for (int i = 0; i < elements.Length; i++)
        //            {
        //                yield return perm.Substring(0, i) + elements.Substring(0, 1) + perm.Substring(i);
        //            }
        //    }
        //}

        public IEnumerable<Tuple<int, string ,dynamicSprite,dynamicSprite, Tuple<Color, Color, Color, Color, Color>, Tuple<Color, Color, Color, Color, Color>>> getEnumerator()
        {
            //come back and tinker with this
            int index = 0;
            foreach (var keyValPair in primarySprites)
            {
                yield return new Tuple<int, string ,dynamicSprite, dynamicSprite, Tuple<Color, Color, Color, Color, Color>, Tuple<Color, Color, Color, Color, Color>>(index, keyValPair.Key,
                                 keyValPair.Value, ancillarySprites[keyValPair.Key], primaryDrawTintColours[keyValPair.Key], ancillaryDrawTintColours[keyValPair.Key]);
                index++;
            }
            //for (int i = 0; i < primarySprites.Count; i++)
            //{
            //    yield return new tprimarySprites.GetEnumerator().CurrentMoveNext();
            //}
        }


        public multiSpriteCollection() :
            this(Color.White) { }

        public multiSpriteCollection(Color defaultColour) :
            this(new Dictionary<string, dynamicSprite>(), new Dictionary<string, dynamicSprite>(), new Dictionary<string, Tuple<Color, Color, Color, Color, Color>>(),
                 new Dictionary<string, Tuple<Color, Color, Color, Color, Color>>(), new Dictionary<string, Predicate<string>>(), defaultColour) { }

        public multiSpriteCollection(Dictionary<string, dynamicSprite> primarySprites, Dictionary<string, dynamicSprite> ancillarySprites,
                                        Dictionary<string, Tuple<Color, Color, Color, Color, Color>> primaryDrawTintColours,
                                        Dictionary<string, Tuple<Color, Color, Color, Color, Color>> ancillaryDrawTintColours,
                                        Dictionary<string, Predicate<string>> specialColoursConditionals,
                                        Color defaultColour)
        {

            this.primarySprites = primarySprites;
            this.ancillarySprites = ancillarySprites;
            this.primaryDrawTintColours = primaryDrawTintColours;
            this.ancillaryDrawTintColours = ancillaryDrawTintColours;
            this.specialColoursConditionals = specialColoursConditionals;
            this.defaultColour = defaultColour;
        }

        public void setCustomPrimarySpriteTint(string key, Tuple<Color, Color, Color, Color, Color> newPalette)
        {
            primaryDrawTintColours[key] = newPalette;
        }

        public void setCustomPrimarySprite(string key, dynamicSprite newSprite)
        {
            primarySprites[key] = newSprite;
        }

        private Color getPrimaryCurrentColour(string spriteKey)
        {
            
            Color temp = defaultColour;
            
            //TODO: include the special condition colour check
            if (primaryDrawTintColours[spriteKey].Item5 != null &&
                primarySprites[spriteKey] != null &&
                specialColoursConditionals[spriteKey](spriteKey))
                return primaryDrawTintColours[spriteKey].Item5;

            if (primaryDrawTintColours[spriteKey].Item4 != null &&
                primarySprites[spriteKey] != null &&
                primarySprites[spriteKey].isSpriteClicked())
                return primaryDrawTintColours[spriteKey].Item4;

            if (primaryDrawTintColours[spriteKey].Item3 != null &&
                primarySprites[spriteKey] != null &&
                primarySprites[spriteKey].isSpritePressed())
                return primaryDrawTintColours[spriteKey].Item3;

            //check if the sprite IS howevered over
            if (primaryDrawTintColours[spriteKey].Item2 != null &&
                primarySprites[spriteKey] != null &&
                primarySprites[spriteKey].isSpriteHoveredOver())
                return primaryDrawTintColours[spriteKey].Item2;

            //check iif the sprite NOT hovered over
            if (primaryDrawTintColours[spriteKey].Item1 != null &&
                primarySprites[spriteKey] != null &&
                !primarySprites[spriteKey].isSpriteHoveredOver())
                return primaryDrawTintColours[spriteKey].Item1;

            return defaultColour;
        }

        private Color getAncillaryCurrentColour(string spriteKey)
        {
            //TODO: include the special condition colour check

            //We check for all the nulls, becuase in order to allow for flexibility we ASLO allow for things to be marked as non-existent (using null)
            //If something is null we should do anything used it or to it

            if (ancillaryDrawTintColours[spriteKey].Item5 != null &&
                ancillarySprites[spriteKey] != null &&
                specialColoursConditionals[spriteKey](spriteKey))
                return ancillaryDrawTintColours[spriteKey].Item5;

            if (ancillaryDrawTintColours[spriteKey].Item4 != null &&
                primarySprites[spriteKey] != null &&
                primarySprites[spriteKey].isSpriteClicked())
                return ancillaryDrawTintColours[spriteKey].Item4;

            if (ancillaryDrawTintColours[spriteKey].Item3 != null &&
                primarySprites[spriteKey] != null &&
                primarySprites[spriteKey].isSpritePressed())
                return ancillaryDrawTintColours[spriteKey].Item3;

            //check if the sprite IS howevered over
            if (ancillaryDrawTintColours[spriteKey].Item2 != null &&
                primarySprites[spriteKey] != null &&
                primarySprites[spriteKey].isSpriteHoveredOver())
                return ancillaryDrawTintColours[spriteKey].Item2;

            //check if the sprite NOT hovered over
            if (ancillaryDrawTintColours[spriteKey].Item1 != null &&
                primarySprites[spriteKey] != null &&
                !primarySprites[spriteKey].isSpriteHoveredOver())
                return ancillaryDrawTintColours[spriteKey].Item1;

            return defaultColour;
        }

        public void LoadSingleSpriteContent(string key, ContentManager Content)
        {
            primarySprites[key].LoadContent(Content);
        }

        public void LoadContent(ContentManager Content)
        {
            //Split these two loops from a single one, simply becuase performing a dictionary lookup of the ancillary sprites inside a loop for the primary sprites,
            //is likely to be slightly less efficient than taking advantage of the IEnumerables super efficient iterator 
            foreach (var keypair in primarySprites)
            {
                if (keypair.Value != null)
                    keypair.Value.LoadContent(Content);
            }
            foreach (var keypair in ancillarySprites)
            {
                if (keypair.Value != null)
                    keypair.Value.LoadContent(Content);
            }
        }

        /// <summary>
        /// Draws all the sprites
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public void Draw(SpriteBatch theSpriteBatch)
        {
            //error check
            if(primarySprites.Count != ancillarySprites.Count || primarySprites.Count != primaryDrawTintColours.Count || (primarySprites.Count != ancillaryDrawTintColours.Count))
                throw new Exception("The 4 lists in the multiSpriteCollection need to all be the same length");

            Draw(new List<string>(),theSpriteBatch);
        }

        /// <summary>
        /// Draws all the sprites (primary and ancillary) except the ones whos keys appear in the exlusion list parameter
        /// </summary>
        /// <param name="exclusionKeys"></param>
        public void Draw(List<string> exclusionKeys, SpriteBatch theSpriteBatch)
        {
            switch (spriteDrawOrder)
            {
                case (drawOrder.allPrimariesFirst):
                    #region
                    foreach (var keyPair in primarySprites)
                    {
                        if (keyPair.Value != null && !exclusionKeys.Contains(keyPair.Key))
                            keyPair.Value.Draw(theSpriteBatch);
                    }
                    foreach (var keyPair in ancillarySprites)
                    {
                        if (keyPair.Value != null && !exclusionKeys.Contains(keyPair.Key))
                            keyPair.Value.Draw(theSpriteBatch);
                    }
                    #endregion
                    break;
                case (drawOrder.allAncillariesFirst):
                    #region
                    foreach (var keyPair in ancillarySprites)
                    {
                        if (keyPair.Value != null && !exclusionKeys.Contains(keyPair.Key))
                            keyPair.Value.Draw(theSpriteBatch);
                    }
                    foreach (var keyPair in primarySprites)
                    {
                        if (keyPair.Value != null && !exclusionKeys.Contains(keyPair.Key))
                            keyPair.Value.Draw(theSpriteBatch);
                    }
                    #endregion
                    break;
                case (drawOrder.primary_Ancillary_Repeat):
                    #region
                    foreach (var keyPair in primarySprites)
                    {
                        if (keyPair.Value != null && !exclusionKeys.Contains(keyPair.Key))
                            keyPair.Value.Draw(theSpriteBatch);
                        if (ancillarySprites[keyPair.Key] != null && !exclusionKeys.Contains(keyPair.Key))
                            ancillarySprites[keyPair.Key].Draw(theSpriteBatch);
                    }
                    #endregion
                    break;
                case (drawOrder.ancillary_Primary_Repeat):
                    #region
                    foreach (var keyPair in primarySprites)
                    {
                        if (ancillarySprites[keyPair.Key] != null && !exclusionKeys.Contains(keyPair.Key))
                            ancillarySprites[keyPair.Key].Draw(theSpriteBatch);
                        if (keyPair.Value != null && !exclusionKeys.Contains(keyPair.Key))
                            keyPair.Value.Draw(theSpriteBatch);
                    }

                    #endregion
                    break;
            }
        }

        internal void drawAllAncillaries(SpriteBatch theSpriteBatch) 
        {
            foreach (var sprt in ancillarySprites)
                if (sprt.Value != null)
                    sprt.Value.Draw(theSpriteBatch);
        }

        internal void drawAllPrimaries(SpriteBatch theSpriteBatch) 
        {
            foreach (var sprt in primarySprites)
                if (sprt.Value != null)
                    sprt.Value.Draw(theSpriteBatch);
        }

        internal void drawSinglePrimary(string key, SpriteBatch theSpriteBatch)
        {
            primarySprites[key].Draw(theSpriteBatch);
        }

        internal void drawSingleAncillary(string key, SpriteBatch theSpriteBatch)
        {
            ancillarySprites[key].Draw(theSpriteBatch);
        }

        internal void drawPrimariesWithExclusions(List<string> exclusionKeys, SpriteBatch theSpriteBatch)
        {
            foreach (var sprt in primarySprites)
                if(!exclusionKeys.Contains(sprt.Key))
                    sprt.Value.Draw(theSpriteBatch);
        }
      
        internal void drawAncillariesWithExclusions(List<string> exclusionKeys, SpriteBatch theSpriteBatch)
        {
            foreach (var sprt in ancillarySprites)
                if (!exclusionKeys.Contains(sprt.Key))
                    sprt.Value.Draw(theSpriteBatch);
        }


        /// <summary>
        /// Draws the two pared sprites based off a key you provide
        /// </summary>
        /// <param name="p"></param>
        /// <param name="spriteBatch"></param>
        internal void DrawSpecificSpriteByKey(string key, SpriteBatch theSpriteBatch)
        {
            switch (spriteDrawOrder)
            {
                case (drawOrder.allPrimariesFirst):
                case (drawOrder.primary_Ancillary_Repeat):
                    if (primarySprites[key] != null)
                        primarySprites[key].Draw(theSpriteBatch);
                    if(ancillarySprites[key] != null)
                        ancillarySprites[key].Draw(theSpriteBatch);
                    return;

                case (drawOrder.allAncillariesFirst):
                case (drawOrder.ancillary_Primary_Repeat):
                    if (ancillarySprites[key] != null)
                        ancillarySprites[key].Draw(theSpriteBatch);
                    if (primarySprites[key] != null)
                        primarySprites[key].Draw(theSpriteBatch);

                    return;
            }
        }

        public void Update(GameTime theGameTime, KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            //update each sprite, and then its tint colour appropriately
            foreach (var keyPair in primarySprites)
            {
                if (keyPair.Value != null)
                {
                    keyPair.Value.Update(theGameTime, oldKeyboard, newKeyboard, oldMouse, newMouse);
                    keyPair.Value.spriteTintColour = getPrimaryCurrentColour(keyPair.Key);
                    //keyPair.Value.textColour = Color.Red;
                    //keyPair.Value.updateText(0, 0, 1.0f, "None");
                }
            }
            foreach (var keyPair in ancillarySprites)
            {
                if (keyPair.Value != null)
                {
                    keyPair.Value.Update(theGameTime, oldKeyboard, newKeyboard, oldMouse, newMouse);
                    keyPair.Value.spriteTintColour = getAncillaryCurrentColour(keyPair.Key);
                    //keyPair.Value.updateText(0, 0, 1.0f, "None");
                }
            }
        }



        
    }
}
