using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            customers = dbContext.Customers.OrderBy(c => c.Name).ToList();

            Assert.AreEqual(696, customers.Count);
            Assert.AreEqual("Abeyatunge, Derek", customers[0].Name);

            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(20);

            Assert.IsNotNull(c);
            Assert.AreEqual(20, c.CustomerId);

            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            customers = dbContext.Customers
                .Where(c => c.StateCode == "OR")
                .OrderBy(c => c.Name)
                .ToList();

            Assert.AreEqual(5, customers.Count);

            PrintAll(customers);
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            c = dbContext.Customers
                .Include(c => c.Invoices)
                .Where(c => c.CustomerId == 20)
                .SingleOrDefault();

            Assert.IsNotNull(c);
            Assert.AreEqual(20, c.CustomerId);
            Assert.IsNotNull(c.Invoices);

            Console.WriteLine(c);

            foreach (var invoice in c.Invoices)
            {
                Console.WriteLine(invoice);
            }
        }

        [Test]
        public void GetWithJoinTest()
        {
            var customers = dbContext.Customers.Join(
               dbContext.States,
               c => c.StateCode,
               s => s.StateCode,
               (c, s) => new { c.CustomerId, c.Name, c.StateCode, s.StateName }).OrderBy(r => r.StateName).ToList();

            Assert.AreEqual(696, customers.Count);

            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }

        [Test]
        public void DeleteTest()
        {
            c = dbContext.Customers.Find(20);

            Assert.IsNotNull(c);

            dbContext.Customers.Remove(c);
            dbContext.SaveChanges();

            Assert.IsNull(dbContext.Customers.Find(20));
        }

        [Test]
        public void CreateTest()
        {
            c = new Customer
            {
                Name = "Test Customer",
                Address = "123 Test Street",
                City = "Test City",
                StateCode = "OR",
                ZipCode = "97601"
            };

            dbContext.Customers.Add(c);
            dbContext.SaveChanges();

            Assert.AreNotEqual(0, c.CustomerId);

            Customer? newCustomer = dbContext.Customers.Find(c.CustomerId);

            Assert.IsNotNull(newCustomer);
            Assert.AreEqual("Test Customer", newCustomer.Name);
        }

        [Test]
        public void UpdateTest()
        {
            c = dbContext.Customers.Find(20);

            Assert.IsNotNull(c);

            c.Name = "Updated Customer";

            dbContext.SaveChanges();

            Customer? updatedCustomer = dbContext.Customers.Find(20);

            Assert.IsNotNull(updatedCustomer);
            Assert.AreEqual("Updated Customer", updatedCustomer.Name);
        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }
    }
}