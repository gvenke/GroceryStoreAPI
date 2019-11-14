using System;
using GroceryStore.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GroceryStore.Tests
{
    /// <summary>
    /// Summary description for PocoTestBase
    /// </summary>
    [TestClass]
    public abstract class EntityTestBase<T> where T : EntityBase, new()
    {
        protected T _poco;
        protected UTDataBroker _dataBroker;



        [TestMethod]
        public void CheckPointSingle()
        {
            _poco = new T();
            _poco.CreateCheckPoint();
            Assert.IsNotNull(_poco.CurrentCheckPoint, "checkpoint should not be null");
            Assert.IsNotNull(_poco.CheckPointHistory, "checkpoint history should not be null");
            Assert.AreEqual(1, _poco.CheckPointHistory.Count);
            Assert.AreNotEqual(_poco.CheckPointHistory.Last(), _poco.CurrentCheckPoint, "current checkpoint should not match the previous one");         
        }

        [TestMethod]
        public void CheckPointMultiple()
        {
            _poco = new T();
            _poco.CreateCheckPoint();
            var history = _poco.CheckPointHistory;
            //System.Threading.Thread.Sleep(1000);
           _poco.CreateCheckPoint();


            Assert.AreEqual(2, history.Count);
            Assert.IsTrue(history.First().Key < history.Last().Key, "checkpoint history logged out of order");

            bool invalid = false;
            EntityBase prevCheckPoint = null;
            foreach(var key in history.Keys)
            {
                var curCheckPoint = history[key];
                invalid = prevCheckPoint != null && curCheckPoint.Equals(prevCheckPoint);
                if (invalid)
                {
                    break;
                }
                prevCheckPoint = curCheckPoint;
            }
            Assert.IsFalse(invalid, "all checkpoints should be separate references");
        }

        [TestMethod]
        public void CheckPointHistoryLimit()
        {  
            _poco = new T();
            for (int i = 1; i <= 8; i++)
            {
                EditPoco();
                _poco.CreateCheckPoint();
            }

            Assert.AreEqual(EntityBase.MaxCheckPoints, _poco.CheckPointHistory.Count);
        }

        public void ClearCheckPoints()
        {
            _poco = new T();
            _poco.CreateCheckPoint();
            _poco.ClearCheckPoints();

            Assert.IsNull(_poco.CurrentCheckPoint);
            Assert.IsTrue(_poco.CheckPointHistory.Count == 0);
        }

        [TestMethod]
        public void CheckPointDirty()
        {
            _poco = new T();           
            _poco.CreateCheckPoint();
            Assert.IsFalse(_poco.IsDirty());
            EditPoco();
            Assert.IsTrue(_poco.IsDirty());
            _poco.CreateCheckPoint();
            Assert.IsFalse(_poco.IsDirty());
        }

        [TestMethod]
        public void CheckPointsNotCloned()
        {
            _poco = new T();
            _poco.CreateCheckPoint();
            _poco.CreateCheckPoint();
            _poco.CreateCheckPoint();
            foreach(var checkPoint in _poco.CheckPointHistory.Values)
            {
                Assert.AreEqual(0, checkPoint.CheckPointHistory.Count);
                Assert.IsNull(checkPoint.CurrentCheckPoint);
            }
        }

        [TestMethod]
        public void IsDirtyWithNoCheckPoint()
        {
            _poco = new T();
            Assert.ThrowsException<InvalidOperationException>(() => _poco.IsDirty());
        }

        [TestMethod]
        public void IsNew()
        {
            _poco = CreateNew();
            Assert.IsTrue(_poco.IsNew());
        }

        [TestMethod]
        public void IsNotNew()
        {
            _poco = CreateExisting();
            Assert.IsFalse(_poco.IsNew());
        }



        protected abstract T CreateNew();

        protected abstract T CreateExisting();

        public abstract void Changes();

        public abstract void SaveNew();

        public abstract void SaveExisting();

        protected abstract void EditPoco();

        protected  void SetupDataBroker()
        {
            _dataBroker = new UTDataBroker();
        }

    }
}
