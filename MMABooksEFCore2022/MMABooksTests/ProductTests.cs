using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;
using MMABooksEFClasses.MarisModels;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        MMABooksContext dbContext;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            var products = dbContext.Products
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.AreEqual(16, products.Count);

            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            var p = dbContext.Products.Find("A4CS");

            Assert.IsNotNull(p);

            Console.WriteLine(p);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the products that have a unit price of 56.50
            var products = dbContext.Products
                .Where(p => p.UnitPrice == 56.50m)
                .ToList();

            foreach (var p in products)
            {
                Assert.AreEqual(56.50m, p.UnitPrice);
                Console.WriteLine(p);
            }
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            // get a list of objects that include the productcode,
            // unitprice, quantity and inventoryvalue
            var products = dbContext.Products.Select(
                p => new
                {
                    p.ProductCode,
                    p.UnitPrice,
                    p.OnHandQuantity,
                    Value = p.UnitPrice * p.OnHandQuantity
                })
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.AreEqual(16, products.Count);

            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void DeleteTest()
        {
            var p = dbContext.Products.Find("A4Cs");

            Assert.IsNotNull(p);

            dbContext.Products.Remove(p);
            dbContext.SaveChanges();

            Assert.IsNull(dbContext.Products.Find("A4CS"));
        }

        [Test]
        public void CreateTest()
        {
            var p = new Products
            {
                ProductCode = "TEST01",
                Description = "Test Product",
                UnitPrice = 25.00m,
                OnHandQuantity = 10
            };

            dbContext.Products.Add(p);
            dbContext.SaveChanges();

            var newProduct = dbContext.Products.Find("TEST01");

            Assert.IsNotNull(newProduct);
            Assert.AreEqual("Test Product", newProduct.Description);
        }

        [Test]
        public void UpdateTest()
        {
            var p = dbContext.Products.Find("A4CS");

            Assert.IsNotNull(p);

            p.Description = "Updated Product";
            dbContext.SaveChanges();

            var updatedProduct = dbContext.Products.Find("A4CS");

            Assert.IsNotNull(updatedProduct);
            Assert.AreEqual("Updated Product", updatedProduct.Description);
        }
    }
}