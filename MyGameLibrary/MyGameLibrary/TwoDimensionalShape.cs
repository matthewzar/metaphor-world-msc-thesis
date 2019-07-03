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
    public class TwoDimensionalShape
    {
        public VertexPositionColor[] userPrimitives;
        public VertexPositionColor[] originalVertexPositions;
        public short[] userPrimitivesIndices;
        
        public Vector3 mDirection;
        public Vector3 mSpeed;

        

        public int mTotalTriangles;
        public float mScale;

        public void SetUpVertices(ContentManager theContentManager, float[] shapeVertices, Color[] theVertexColours)
        {
            userPrimitives = new VertexPositionColor[theVertexColours.Length];

            for (int i = 0; i < theVertexColours.Length; i++)
            {
                userPrimitives[i].Position = vertexAndMatricManipulation.floatArrayToVector3(shapeVertices, i);
                userPrimitives[i].Color = theVertexColours[i];
            }

            originalVertexPositions = (VertexPositionColor[])userPrimitives.Clone();

        }
         



        public void LoadContent(ContentManager theContentManager, short[] shapeIndecesDrawOrder, float[] shapeVertices, Color[] shapeColours, Vector3 startingPosition, int totalTriangles,  float scale)
        {
            mScale = scale;
            mSpeed = Vector3.Zero;
            mDirection = Vector3.Zero;

            mTotalTriangles = totalTriangles;

            userPrimitivesIndices = shapeIndecesDrawOrder;
            
            SetUpVertices(theContentManager, shapeVertices, shapeColours);
                    
            setPosition(startingPosition);
        }


        private void setPosition(Vector3 newPosition)
        {
            for (int i = 0; i < userPrimitives.Length; i++)
            {
                userPrimitives[i].Position = (originalVertexPositions[i].Position + newPosition) * mScale;
            }
        }

        


        public virtual void Draw(GraphicsDevice theGraphicsDevice, BasicEffect theBasicEffect)
        {
            // Start using the BasicEffect
            theBasicEffect.CurrentTechnique.Passes[0].Apply();
            // Draw the primitives
            theGraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList,
                userPrimitives, 0, userPrimitives.Length, userPrimitivesIndices, 0, mTotalTriangles);
            
            
            
        }

        public void Update(GameTime theGameTime)
        {
            vertexAndMatricManipulation.moveShapePosition(userPrimitives, mSpeed, mDirection, (float)theGameTime.ElapsedGameTime.TotalSeconds);
        }

    }
}
