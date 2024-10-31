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
            // logs will be in the path
            // C:\Users\<username\AppData\Local\GroupProjectSE\FileCloner
            FileClonerLogger logger = new("UnitTests", true);
            logger.Log("HELLO WORLD");
        }

        [TestMethod]
        public void FileClonerConfigFileCreation()
        {
            // config files are in the path
            // C:\Users\<username\AppData\Local\GroupProjectSE\FileCloner
            FileCloner fileCloner = new();
        }
    }
}