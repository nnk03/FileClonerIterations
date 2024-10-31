

using SoftwareEngineeringGroupProject.FileCloner;
using SoftwareEngineeringGroupProject.FileCloner.Logger;

namespace UnitTests
{
    [TestClass]
    public class FileClonerUnitTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Logger _logger = new("UnitTest");
            FileClonerFactory factory = new FileClonerFactory();
        }
    }
}