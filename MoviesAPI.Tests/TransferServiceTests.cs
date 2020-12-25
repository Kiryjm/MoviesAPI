using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Testing;

namespace MoviesAPI.Tests
{
    [TestClass]
    public class TransferServiceTests
    {
        [TestMethod]
        public void WireTransferWithInsufficientFundsThrowsAnError()
        {
            //Preparation
            Account origin = new Account() {Funds = 0};
            Account destination = new Account() { Funds = 0 };
            decimal amountToTransfer = 5m;
            var service = new TransferService(new WireTransferValidator());
            Exception expectedException = null;

            //Testing
            try
            {
                service.WireTransfer(origin, destination, amountToTransfer);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Verification

            if (expectedException == null)
            {
                Assert.Fail("An exception was expected");
            }

            Assert.IsTrue(expectedException is ApplicationException);
            Assert.AreEqual("The origin account does not have enough funds available", expectedException.Message);
        }

        [TestMethod]
        public void WireTransferCorrectlyEditFunds()
        {
            //Preparation
            Account origin = new Account() {Funds = 10};
            Account destination = new Account() { Funds = 5 };
            decimal amountToTransfer = 7m;
            var service = new TransferService(new WireTransferValidator());

            //Testing
            service.WireTransfer(origin, destination, amountToTransfer);

            //Verification
            Assert.AreEqual(3, origin.Funds);
            Assert.AreEqual(12, destination.Funds);
        }
    }
}
