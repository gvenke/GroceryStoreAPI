using System;
using GroceryStore.Pocos;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroceryStore.Tests
{
    [TestClass]
    public class OrderTests : EntityTestBase<Order>
    {
        private static Random _rnd;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _rnd = new Random();
        }


        protected override void EditPoco()
        {
            _poco.CustomerId = _rnd.Next();
        }

        protected override Order CreateNew()
        {
            return new Order { OrderDate = DateTime.Parse("1/1/2001"), CustomerId = 1};
        }

        protected override Order CreateExisting()
        {
            return new Order { Id = 1, OrderDate = DateTime.Parse("1/1/2001"), CustomerId = 1 };
        }


        [TestMethod]
        public override void TestChanges()
        {
            _poco = new Order();
            var history = _poco.CheckPointHistory;

            const int initialCustomerId = 1;
            const int changedCustomerId = 2;

            _poco.CustomerId = initialCustomerId;
            _poco.Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1 } };
            
            _poco.CreateCheckPoint();
            var currentCheckPoint = (Order)_poco.CurrentCheckPoint;
            var items1 = _poco.Items.ToList();

            Assert.IsTrue(_poco.CustomerId == currentCheckPoint.CustomerId);
            CollectionAssert.AreEqual(_poco.Items.ToArray(), currentCheckPoint.Items.ToArray());

            _poco.CustomerId = changedCustomerId;
            _poco.Items.Add(new OrderItem { ProductId = 2, Quantity = 2 });
            var items2 = _poco.Items.ToArray();

            Assert.IsFalse(_poco.CustomerId == currentCheckPoint.CustomerId);
            CollectionAssert.AreNotEqual(_poco.Items.ToArray(), currentCheckPoint.Items.ToArray());

            _poco.CreateCheckPoint();
            currentCheckPoint = (Order)_poco.CurrentCheckPoint;

            Assert.IsTrue(_poco.CustomerId == currentCheckPoint.CustomerId);
            CollectionAssert.AreEqual(_poco.Items.ToArray(), currentCheckPoint.Items.ToArray());

            var checkPoint1 = (Order)history[history.First().Key];
            var checkPoint2 = (Order)history[history.Last().Key];

            Assert.AreEqual(initialCustomerId, checkPoint1.CustomerId);
            Assert.AreEqual(changedCustomerId, checkPoint2.CustomerId);

            CollectionAssert.AreEqual(items1, checkPoint1.Items.ToArray());
            CollectionAssert.AreEqual(items2, checkPoint2.Items.ToArray());
        }

        [TestMethod]
        public override void TestSaveNew()
        {
            SetupDataBroker();
            _poco = new Order { CustomerId = 1, Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1 } } };

            _poco.Save(_dataBroker);

            Assert.IsTrue(_poco.Id != null);
            Assert.IsTrue(_dataBroker.OrderData.Count == 1);
        }

        [TestMethod]
        public override void TestSaveExisting()
        {
            SetupDataBroker();
            _poco = new Order { Id = 1, CustomerId = 1, Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1 } } };
            var orderId = _poco.Id.GetValueOrDefault();
            _poco.CreateCheckPoint();
            var clone = (Order)_poco.CurrentCheckPoint;
            _dataBroker.OrderData.Add(clone.Id.GetValueOrDefault(), clone);

            _poco.Items.Add(new OrderItem { ProductId = 2, Quantity = 2 });
            _poco.Save(_dataBroker);

            Assert.IsTrue(_poco.Id == orderId);
            Assert.IsTrue(_dataBroker.OrderData.Count == 1);


        }
    }
}
