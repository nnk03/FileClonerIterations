

using SoftwareEngineeringGroupProject.FileCloner;
using SoftwareEngineeringGroupProject.FileCloner.Logger;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Logger _logger = new("UnitTest");
            FileClonerFactory factory = new FileClonerFactory();
        }
    }
}