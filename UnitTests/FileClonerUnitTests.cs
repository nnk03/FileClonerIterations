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
            logger.Log("HELLO WORLD");
        }
    }
}