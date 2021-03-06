﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    [TestClass]
    public class RcptToVerbTests
    {
        [TestMethod]
        public void EmailAddressOnly()
        {
            TestGoodAddress("<rob@rnwood.co.uk>", "rob@rnwood.co.uk");
        }

        [TestMethod]
        public void EmailAddressWithDisplayName()
        {
            //Should this format be accepted????
            TestGoodAddress("<Robert Wood<rob@rnwood.co.uk>>", "Robert Wood<rob@rnwood.co.uk>");
        }

        private void TestGoodAddress(string address, string expectedAddress)
        {
            Mocks mocks = new Mocks();
            MemoryMessage.Builder messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);

            RcptToVerb verb = new RcptToVerb();
            verb.Process(mocks.Connection.Object, new SmtpCommand("TO " + address));

            mocks.VerifyWriteResponse(StandardSmtpResponseCode.OK);
            Assert.AreEqual(expectedAddress, messageBuilder.To.First());
        }

        [TestMethod]
        public void UnbraketedAddress_ReturnsError()
        {
            TestBadAddress("rob@rnwood.co.uk");
        }

        [TestMethod]
        public void MismatchedBraket_ReturnsError()
        {
            TestBadAddress("<rob@rnwood.co.uk");
            TestBadAddress("<Robert Wood<rob@rnwood.co.uk>");
        }

        [TestMethod]
        public void EmptyAddress_ReturnsError()
        {
            TestBadAddress("<>");
        }

        private void TestBadAddress(string address)
        {
            Mocks mocks = new Mocks();
            MemoryMessage.Builder messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);

            RcptToVerb verb = new RcptToVerb();
            verb.Process(mocks.Connection.Object, new SmtpCommand("TO " + address));

            mocks.VerifyWriteResponse(StandardSmtpResponseCode.SyntaxErrorInCommandArguments);
            Assert.AreEqual(0, messageBuilder.To.Count);
        }
    }
}