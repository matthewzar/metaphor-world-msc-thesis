using MyGameLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace testTheFunctions
{
    
    
    /// <summary>
    ///This is a test class for vertexAndMatricManipulationTest and is intended
    ///to contain all vertexAndMatricManipulationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class vertexAndMatricManipulationTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for maxPosition
        ///</summary>
        [TestMethod()]
        public void maxPositionTest()
        {
            float[] theArray = {0f,2f,4f,2f,6f,3f,2.3f,6f}; 
            int expected = 4; 
            int actual;
            actual = vertexAndMatricManipulation.maxPosition(theArray);
            Assert.AreEqual(expected, actual, "Bad index");
            
        }

        /// <summary>
        ///A test for returnValue
        ///</summary>
        [TestMethod()]
        public void returnValueTest()
        {
            for (int x = -10; x < 10; x++)
            {
                int val = x;
                int expected = x;
                int actual;
                actual = vertexAndMatricManipulation.returnValue(val);
                Assert.AreEqual(expected, actual, "Bad return value");
            }
            
            
        }

        /// <summary>
        ///A test for gridToWorldConverter
        ///</summary>
        [TestMethod()]
        public void gridToWorldConverterTest()
        {
            int x = 10;
            int y = 10;
            int maxX = 10;
            int maxY = 10;
            float worldArea = 4F;
            Vector3 expected = new Vector3(2, -2,0);
            Vector3 actual;
            actual = vertexAndMatricManipulation.gridToWorldConverter(x, y, maxX, maxY, worldArea);
            Assert.AreEqual(expected, actual);

            x = 5;
            y = 5;
            maxX = 10;
            maxY = 10;
            worldArea = 4F;
            expected = new Vector3(0, 0, 0);
            actual = vertexAndMatricManipulation.gridToWorldConverter(x, y, maxX, maxY, worldArea);
            Assert.AreEqual(expected, actual);

            x = 0; 
            y = 0; 
            maxX = 10; 
            maxY = 10; 
            worldArea = 4F;
            expected = new Vector3(-2, 2,0); 
            actual = vertexAndMatricManipulation.gridToWorldConverter(x, y, maxX, maxY, worldArea);
            Assert.AreEqual(expected, actual);
        }
    }
}
