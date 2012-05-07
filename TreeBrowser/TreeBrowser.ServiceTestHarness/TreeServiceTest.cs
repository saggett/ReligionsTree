using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeBrowser.Client.Entities;
using TreeBrowser.SilverlightLib.Proxy;
using Lineage = TreeBrowser.Client.Entities;

namespace TreeBrowser.ServiceTestHarness
{
    /// <summary>
    /// Summary description for TreeServiceTest
    /// </summary>
    [TestClass]
    public class TreeServiceTest
    {

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FetchRootTest()
        {
            var lineages = Lineages.GetLineages(null);
            Assert.AreNotEqual(0, lineages.Count());
        }

        [TestMethod]
        public void SaveLineageTest()
        {
            var service = new Service.TreeService();
            int newLinId =
                service.SaveLineage(new Lineage()
                                        {
                                            EndYear = null,
                                            Name = "UnitTestLineage",
                                            ParentLineageId = 2,
                                            StartYear = 100,
                                            WikipediaArticleName = string.Empty
                                        });
            service.DeleteLineage(newLinId);
        }

        //[TestMethod]
        //public void FetchRootViaWcfTest()
        //{
        //    var result = EndPoint.Proxy.FetchRoot();
        //    Assert.AreNotEqual(0, result.Count);
        //}



    }
}
