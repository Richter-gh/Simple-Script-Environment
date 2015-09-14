using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ScriptCore.Tests
{
    [TestClass()]
    public class ScriptManagerTests
    {
        [TestMethod()]
        public void AddTest()
        {
            var tst = new Dictionary<string, bool>()
            {
                {"some text",true}
            };
            tst.Add("some more text", true);
            ScriptManager sm = new ScriptManager(tst);
            Assert.Fail();
        }
    }
}