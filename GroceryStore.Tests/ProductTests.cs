using System;
using GroceryStore.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroceryStore.Tests
{
    [TestClass]
    public class ProductTests : EntityTestBase<Product>
    {
        private static Random _rnd;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _rnd = new Random();
        }


        protected override void EditPoco()
        {
            _poco.Price= _rnd.NextDouble();
        }

        protected override Product CreateNew()
        {
            return new Product { Price = 1.99, Description = "test desc" };
        }

        protected override Product CreateExisting()
        {
            return new Product { Id = 1, Price = 1.99, Description = "test desc" };
        }

        [TestMethod]
        public override void Changes()
        {
            const string initialDesc = "initial desc";
            const string changedDesc = "changed desc";
            const double initialPrice = 10.25;
            const double changedPrice = 11.50;

            _poco = new Product();
            var history = _poco.CheckPointHistory;

            _poco.Description = initialDesc;
            _poco.Price = initialPrice;
            _poco.CreateCheckPoint();

            Assert.IsTrue(_poco.Description == ((Product)_poco.CurrentCheckPoint).Description);
            Assert.IsTrue(_poco.Price == ((Product)_poco.CurrentCheckPoint).Price);


            _poco.Description = changedDesc;
            _poco.Price = changedPrice;
            Assert.IsTrue(_poco.Description != ((Product)_poco.CurrentCheckPoint).Description);
            Assert.IsTrue(_poco.Price != ((Product)_poco.CurrentCheckPoint).Price);

            _poco.CreateCheckPoint();
            Assert.IsTrue(_poco.Description == ((Product)_poco.CurrentCheckPoint).Description);
            Assert.IsTrue(_poco.Price == ((Product)_poco.CurrentCheckPoint).Price);

            var checkPoint1 = (Product)history[history.First().Key];
            var checkPoint2 = (Product)history[history.Last().Key];

            Assert.AreEqual(initialDesc, checkPoint1.Description);
            Assert.AreEqual(changedDesc, checkPoint2.Description);

            Assert.AreEqual(initialPrice, checkPoint1.Price);
            Assert.AreEqual(changedPrice, checkPoint2.Price);
        }

        [TestMethod]
        public override void SaveNew()
        {
            SetupDataBroker();

            _poco = new Product();
  
            _poco.Description = "test desc";
            _poco.Price = 12.99;
            _poco.Save(_dataBroker);

            Assert.IsTrue(_poco.Id != null);
            Assert.IsTrue(_dataBroker.ProductData.Count == 1);
        }

        [TestMethod]
        public override void SaveExisting()
        {
            SetupDataBroker();
            _poco = new Product { Id = 1, Description = "test desc" };
            var productId = _poco.Id.GetValueOrDefault();
            _poco.CreateCheckPoint();
            var clone = (Product)_poco.CurrentCheckPoint;
            _dataBroker.ProductData.Add(clone.Id.GetValueOrDefault(), clone);

            _poco.Description = "changed desc";

            _poco.Save(_dataBroker);
            Assert.IsTrue(_poco.Id == productId);
            Assert.IsTrue(_dataBroker.ProductData.Count == 1);
        }

    }
}
