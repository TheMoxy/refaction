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
            InMemoryServer.TestDatabasePath = System.IO.Path.GetFullPath(".\\TestData");
        }

        [TestMethod]
        public async Task Product_GetAll()
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

        [TestMethod]
        public async Task Product_Get_By_Id()
        {
            using (var server = new InMemoryServer())
            {
                string id = "de1287c0-4b15-4a7b-9d8a-dd21b3cafec3";

                // Get product by Id
                var product = await server.SendRequest<Product>($"/products/{id}", HttpMethod.Get);

                // Confirm correct product was returned
                Assert.AreEqual("Apple iPhone 6S", product.Name);
                Assert.AreEqual("Newest mobile product from Apple.", product.Description);
                Assert.AreEqual((decimal)1299.99, product.Price);
                Assert.AreEqual((decimal)15.99, product.DeliveryPrice);
            }
        }

        [TestMethod]
        public async Task Product_Get_By_Name()
        {
            using (var server = new InMemoryServer())
            {
                string name = "Samsung Galaxy S7";

                // Get products by name
                var products = await server.SendRequest<Products>($"/products?name={name}", HttpMethod.Get);

                // Confirm correct product was returned
                Assert.AreEqual(1, products.Items.Count);
                var product = products.Items[0];

                Assert.AreEqual("Newest mobile product from Samsung.", product.Description);
                Assert.AreEqual((decimal)1024.99, product.Price);
                Assert.AreEqual((decimal)16.99, product.DeliveryPrice);
            }
        }

        [TestMethod]
        public async Task Product_Post()
        {
            using (var server = new InMemoryServer())
            {
                // Create a product
                var id = new Guid();
                var product = new Product()
                {
                    Id = id,
                    Name = "iPhone X",
                    Description = "The latest and greatest",
                    DeliveryPrice = 2500,
                    Price = 2000
                };

                // Post a new product
                await server.SendRequest($"/products", HttpMethod.Post, product);

                // Get the product back
                var getProduct = await server.SendRequest<Product>($"/products/{id}", HttpMethod.Get);

                // Confirm correct product was returned
                Assert.AreEqual(product.Name, getProduct.Name);
                Assert.AreEqual(product.Price, getProduct.Price);
                Assert.AreEqual(product.DeliveryPrice, getProduct.DeliveryPrice);
            }
        }

        [TestMethod]
        public async Task Product_Delete()
        {
            using (var server = new InMemoryServer())
            {
                // Get all products from server
                var products = await server.SendRequest<Products>("/products", HttpMethod.Get);

                // Delete product
                await server.SendRequest($"/products/{products.Items[0].Id}", HttpMethod.Delete);

                // Get all products from server
                var getProducts = await server.SendRequest<Products>("/products", HttpMethod.Get);

                // Confirm 1 item exists now
                Assert.AreEqual(products.Items.Count-1, getProducts.Items.Count);
                Assert.AreEqual(products.Items[1].Name, getProducts.Items[0].Name);
            }
        }

        [TestMethod]
        public async Task Product_Update()
        {
            using (var server = new InMemoryServer())
            {
                string id = "de1287c0-4b15-4a7b-9d8a-dd21b3cafec3";

                // Get product by Id
                var product = await server.SendRequest<Product>($"/products/{id}", HttpMethod.Get);

                // Confirm correct product was returned
                Assert.AreEqual("Apple iPhone 6S", product.Name);
                product.Name = "iPhone 7";

                // Update product
                await server.SendRequest($"/products/{id}", HttpMethod.Put, product);

                // Get it back
                product = await server.SendRequest<Product>($"/products/{id}", HttpMethod.Get);

                // Confirm new value
                Assert.AreEqual("iPhone 7", product.Name);
            }
        }
        
        //GET /products/{ id}/options - finds all options for a specified product.
        //GET /products/{id}/options/{optionId} - finds the specified product option for the specified product.
        //POST /products/{id}/options - adds a new product option to the specified product.
        //PUT /products/{ id}/options/{optionId} - updates the specified product option.
        //DELETE /products/{id}/options/{optionId} - deletes the specified product option.
    }
}
