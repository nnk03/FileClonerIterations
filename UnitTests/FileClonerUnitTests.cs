using GroupProjectSE.FileCloner;
using GroupProjectSE.FileCloner.FileClonerLogging;

namespace UnitTests
{
    [TestClass]
    public class FileClonerUnitTests
    {
        [TestMethod]
        public void LoggerTest()
        {
            FileClonerLogger logger = new("UnitTests", true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            FileCloner fileCloner = new();
        }
    }
}