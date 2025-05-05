using System.Runtime.CompilerServices;
using Data;

namespace DataTest
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DataAbstractAPI dataAbstractAPI = DataAbstractAPI.CreateApi();
            Assert.IsNotNull(dataAbstractAPI);
        }
    }
}