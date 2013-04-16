﻿namespace Rexster.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rexster.Messages;

    [TestClass]
    public class SessionTests
    {
        private RexsterClient client;

        [TestInitialize]
        public void Initialize()
        {
            client = new RexsterClient("192.168.2.105");
        }

        [TestMethod]
        public void OpenCloseSession()
        {
            var response = client.OpenSession();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Languages);
            Assert.IsTrue(response.Languages.Contains("groovy"));

            response = client.KillSession(response.Session);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void UseSession()
        {
            var response = client.OpenSession();
            var r = Guid.NewGuid().ToByteArray();
            var session = response.Session;

            var request = new ScriptRequest("v = g.addVertex(['name':'foo'])")
            {
                Session = session,
                Meta = { InSession = true, Isolate = false }
            };

            var expected = client.ExecuteScript<Vertex<TestVertex>>(request).Result;

            request = new ScriptRequest("v")
            {
                Session = session,
                Meta = { InSession = true, Isolate = false }
            };

            // TODO: Test does not succeed. Go and learn something about Rexsters sessions!
            var actual = client.ExecuteScript<Vertex<TestVertex>>(request).Result;

            client.KillSession(session);

            Assert.AreEqual(expected, actual);
        }
    }
}