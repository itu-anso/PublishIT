using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishITService;
using PublishITService = PublishITService.PublishITService;

namespace PublishITServiceTests
{
    [TestClass]
    public class PublishITServiceIntegrationTests
    {
        private IPublishITService _service;

        [TestInitialize]
        public void InitTests()
        {
            _service = new global::PublishITService.PublishITService();
        }

        [IntegrationTest]
        [TestMethod]
        public void SuccessfullyRegisterNewUser()
        {
            Assert.AreEqual(2, 1);

            //Assert.AreEqual(responseMessage.Message, "User registered");

            //Assert.IsTrue(responseMessage.IsExecuted);
        }
    }
}
