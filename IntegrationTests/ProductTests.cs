using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using refactor_me.Models;

namespace IntegrationTests
{
    [TestClass]
    [DeploymentItem("TestData", "TestData")]
    public class ProductTests
    {
        [TestInitialize]
        public void Init()
        {
            // Set directory for the test DB
            Helpers.DataDirectory = System.IO.Path.GetFullPath(".\\TestData");
        }

        [TestMethod]
        public async Task GetAll()
        {
            using (var server = new InMemoryServer())
            {
                // Get all products from server
                var products = await server.SendRequest<Products>("/products", HttpMethod.Get);

                // Confirm 2 items returned and check some values
                Assert.AreEqual(2, products.Items.Count);
                Assert.AreEqual(new Guid("8f2e9176-35ee-4f0a-ae55-83023d2db1a3"), products.Items[0].Id);
                Assert.AreEqual("Apple iPhone 6S", products.Items[1].Name);
            }
        }
    }
}
