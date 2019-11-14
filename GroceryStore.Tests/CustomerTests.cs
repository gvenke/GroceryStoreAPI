using System;
using System.Linq;
using GroceryStore.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroceryStore.Tests
{
    [TestClass]
    public class CustomerTests : EntityTestBase<Customer>
    {
        private static Random _rnd;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _rnd = new Random();
        }

        protected override void EditPoco()
        {
            _poco.Name = Guid.NewGuid().ToString();
        }

        protected override Customer CreateNew()
        {
            return new Customer { Name = "joe blow" };
        }

        protected override Customer CreateExisting()
        {
            return new Customer { Id = 1, Name = "joe blow" };
        }

        [TestMethod]
        public override void Changes()
        {
            const string initialName = "initial name";
            const string changedName = "changed name";
            _poco = new Customer();
            var history = _poco.CheckPointHistory;           
            _poco.Name = initialName;
            _poco.CreateCheckPoint();
            Assert.IsTrue(_poco.Name == ((Customer)_poco.CurrentCheckPoint).Name);
            _poco.Name = changedName;
            Assert.IsTrue(_poco.Name != ((Customer)_poco.CurrentCheckPoint).Name);
            _poco.CreateCheckPoint();
            Assert.IsTrue(_poco.Name == ((Customer)_poco.CurrentCheckPoint).Name);

            var checkPoint1 = (Customer)history[history.First().Key];
            var checkPoint2 = (Customer)history[history.Last().Key];

            Assert.AreEqual(initialName, checkPoint1.Name);
            Assert.AreEqual(changedName, checkPoint2.Name);
        }

        [TestMethod]
        public override void SaveNew()
        {
            SetupDataBroker();

            _poco = new Customer();
            const string name = "joe blow";
            _poco.Name = name;
            _poco.Save(_dataBroker);

            Assert.IsTrue(_poco.Id != null);
            Assert.IsTrue(_dataBroker.CustomerData.Count == 1);
        }

        [TestMethod]
        public override void SaveExisting()
        {
            SetupDataBroker();
            _poco = new Customer { Id = 1, Name = "joe blow" };
            var custId = _poco.Id.GetValueOrDefault();
            _poco.CreateCheckPoint();
            var clone = (Customer)_poco.CurrentCheckPoint;
            _dataBroker.CustomerData.Add(clone.Id.GetValueOrDefault(), clone);

            _poco.Name = "jack black";

            _poco.Save(_dataBroker);
            Assert.IsTrue(_poco.Id == custId);
            Assert.IsTrue(_dataBroker.CustomerData.Count == 1);
        }
    }
}
