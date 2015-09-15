using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScriptCore.Tests
{
    [TestClass]
    public class ScriptManagerTests
    {
        #region Code Examples
        private string _correctCode = @"
using ScriptCore;
namespace ScriptCore.Tests{
    public class Test:IExecutable
    {
        public string Author { get {return " + "\"" + "SomeAuthor" + "\"" + @";} }
        public string Name { get {return " + "\"" + "SomeAuthor" + "\"" + @";} }
        public string Version { get {return " + "\"" + "SomeAuthor" + "\"" + @";} }
        public void Execute()
        {}
    }}";
                
        private string _incorrectCode = @"
namespace ScriptCore.Tests{
    public class Test
    {
        public string Author { get {return " + "\"" + "SomeAuthor" + "\"" + @";} }
        public string Name { get {return " + "\"" + "SomeAuthor" + "\"" + @";} }
        public string Version { get {return " + "\"" + "SomeAuthor" + "\"" + @";} }
        public void Execute()
        {}
    }}
";
        #endregion

        [TestMethod]
       // [ExpectedException(typeof(NullReferenceException))]
        public void ScriptManagerTest_1()
        {
            var dic = new Dictionary<string, bool>()
            {
                { "test",false }
            };
            ScriptManager sm = new ScriptManager(dic);
            Assert.IsNotNull(sm.ErrorMessage);
        }

        [TestMethod]
        public void ScriptManagerTest_2()
        {
            ScriptManager sm = new ScriptManager();
            Assert.IsNull(sm.ErrorMessage);
        }
        
        [TestMethod]
        public void AddTest_1()
        {            
            string temp;
            ScriptManager sm = new ScriptManager();
            bool result = sm.Add("text", true, out temp);
            Assert.IsNotNull(temp);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddTest_2()
        {
            string temp;
            ScriptManager sm = new ScriptManager();
            bool result = sm.Add(_correctCode, true, out temp);
            Assert.AreEqual(temp, "");
            Assert.IsTrue(result);
        }

        [TestMethod]        
        public void AddTest_3()
        {
            string temp;
            ScriptManager sm = new ScriptManager();
            bool result = sm.Add(_incorrectCode, true, out temp);
            Assert.AreNotEqual(temp, "");
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Test adding correct code from file
        /// </summary>
        [TestMethod]
        public void AddTest_4()
        {
            string temp;
            ScriptManager sm = new ScriptManager();
            File.WriteAllText("AddTest_4.cs", _correctCode);
            bool result = sm.Add("AddTest_4.cs", true, out temp);
            File.Delete("AddTest_4.cs");
            Assert.AreEqual(temp, "");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemoveTest_1()
        {
            ScriptManager sm = new ScriptManager();
            bool result = sm.Remove("test");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveTest_2()
        {
            string temp;
            ScriptManager sm = new ScriptManager();
            File.WriteAllText("RemoveTest_2.cs", _correctCode);
            bool result = sm.Add("RemoveTest_2.cs", true, out temp);
            result = sm.Remove("RemoveTest_2.cs");
            Assert.IsTrue(result);
            File.Delete("RemoveTest_2.cs");
        }

        [TestMethod]
        public void ExecuteTest()
        {
            string temp;
            ScriptManager sm = new ScriptManager();
            File.WriteAllText("RemoveTest_2.cs", _correctCode);
            bool result = sm.Add("RemoveTest_2.cs", true, out temp);
            File.Delete("RemoveTest_2.cs");
            try
            {
                sm.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
    }
}