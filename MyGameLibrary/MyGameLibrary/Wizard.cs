using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyGameLibrary
{

    public class Wizard : Sprite
    {
        const string WIZARD_ASSETNAME = "WizardSquare";
        const int START_POSITION_X = 125;
        const int START_POSITION_Y = 245;
        const int WIZARD_SPEED = 160;
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;
        const int JUMP_HEIGHT = 100;

        enum State
        {
            Walking,
            Jumping,
            Ducking
        }
        State mCurrentState = State.Walking;

        Vector2 mDirection = Vector2.Zero;
        Vector2 mSpeed = Vector2.Zero;
        Vector2 mStartingPosition = Vector2.Zero;

        List<Fireball> mFireballs = new List<Fireball>();
        ContentManager mContentManager;

        KeyboardState mPreviousKeyboardState;

        public void LoadContent(ContentManager theContentManager)
        {
            mContentManager = theContentManager;

            foreach (Fireball aFireball in mFireballs)
            {
                aFireball.LoadContent(theContentManager);
            }

            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(theContentManager, WIZARD_ASSETNAME);
            Source = new Rectangle(0, 0, 200, Source.Height);
        }

        public void Update(GameTime theGameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            UpdateMovement(aCurrentKeyboardState);
            UpdateJump(aCurrentKeyboardState);
            UpdateDuck(aCurrentKeyboardState);
            UpdateFireball(theGameTime, aCurrentKeyboardState);

            mPreviousKeyboardState = aCurrentKeyboardState;

            base.Update(theGameTime, mSpeed, mDirection);
        }

        #region fireball methods
        private void UpdateFireball(GameTime theGameTime, KeyboardState aCurrentKeyboardState)
        {
            foreach (Fireball aFireball in mFireballs)
            {
                aFireball.Update(theGameTime);
            }

            if (aCurrentKeyboardState.IsKeyDown(Keys.RightControl) == true && mPreviousKeyboardState.IsKeyDown(Keys.RightControl) == false)
            {
                ShootFireball();
            }
        }

        private void ShootFireball()
        {
            if (mCurrentState == State.Walking)
            {
                bool aCreateNew = true;
                foreach (Fireball aFireball in mFireballs)
                {
                    if (aFireball.Visible == false)
                    {
                        aCreateNew = false;
                        aFireball.Fire(Position+ new Vector2(Size.Width/2,Size.Height/2),
                            new Vector2(200,0), new Vector2(1,0));
                        break;
                    }
                }

                if (aCreateNew == true)
                {
                    Fireball aFireball = new Fireball();
                    aFireball.LoadContent(mContentManager);
                    aFireball.Fire(Position+ new Vector2(Size.Width/2,Size.Height/2),
                            new Vector2(200,200), new Vector2(1,0));
                    mFireballs.Add(aFireball);
                }
            }
        }

        #endregion

        #region duck methods
        private void UpdateDuck(KeyboardState aCurrentKeyboardState)
        {
            if (aCurrentKeyboardState.IsKeyDown(Keys.RightShift) == true)
                Duck();
            else
                StopDucking();
        }
       

        private void Duck()
        {
            if (mCurrentState == State.Walking)
            {
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;

                Source = new Rectangle(200, 0, 200, Source.Height);
                mCurrentState = State.Ducking;
            }
        }

        private void StopDucking()
        {
            if (mCurrentState == State.Ducking)
            {
                Source = new Rectangle(0, 0, 200, Source.Width);
                mCurrentState = State.Walking;
            }
        }
        #endregion

        #region jump methods
        private void UpdateJump(KeyboardState aCurrentKeyboardState)
        {
            if (mCurrentState == State.Walking)
            {
                if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true && mPreviousKeyboardState.IsKeyDown(Keys.Space) == false)
                    Jump();
            }

            if (mCurrentState == State.Jumping)
            {
                if(mStartingPosition.Y - Position.Y > JUMP_HEIGHT)
                {
                    mDirection.Y = MOVE_DOWN;
                }

                if(Position.Y > mStartingPosition.Y)
                {
                    Position.Y = mStartingPosition.Y;
                    mCurrentState = State.Walking;
                    mDirection = Vector2.Zero;
                }
            }
        }

        private void Jump()
        {
            if (mCurrentState != State.Jumping)
            {
                mCurrentState = State.Jumping;
                mStartingPosition = Position;
                mDirection.Y = MOVE_UP;
                mSpeed = new Vector2(WIZARD_SPEED, WIZARD_SPEED);
            }
        }
        #endregion

        private void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            if (mCurrentState == State.Walking)
            {
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;

                //move left or right
                if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
                {
                    mSpeed.X = WIZARD_SPEED;
                    mDirection.X = MOVE_LEFT;
                }
                else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
                {
                    mSpeed.X = WIZARD_SPEED;
                    mDirection.X = MOVE_RIGHT;
                }

                //move up or down
                if (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true)
                {
                    mSpeed.Y = WIZARD_SPEED;
                    mDirection.Y = MOVE_UP;
                }
                else if (aCurrentKeyboardState.IsKeyDown(Keys.Down) == true)
                {
                    mSpeed.Y = WIZARD_SPEED;
                    mDirection.Y = MOVE_DOWN;
                }
            }
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (Fireball aFireball in mFireballs)
            {
                aFireball.Draw(theSpriteBatch);
            }
            base.Draw(theSpriteBatch);
        }

    }
}
