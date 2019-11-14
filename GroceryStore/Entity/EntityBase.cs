using System;
using System.Collections.Generic;
using GroceryStore.DataBroker;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GroceryStore.Tests")]

namespace GroceryStore.Entity
{
    public abstract class EntityBase
    {
        // a completely arbitrary number I just pulled out of a hat
        public const byte MaxCheckPoints = 3;

        protected SortedDictionary<DateTime, EntityBase> _checkPoints;

        protected EntityBase()
        {
            _checkPoints = new SortedDictionary<DateTime, EntityBase>();
        }

        protected EntityBase _checkPoint;

        public SortedDictionary<DateTime, EntityBase> CheckPointHistory => _checkPoints;

        public EntityBase CurrentCheckPoint => _checkPoint;

        /// <summary>
        /// the checkpoint system uses the "Memento" design pattern to track changes
        /// </summary>
        public void CreateCheckPoint()
        {

            _checkPoint = CreateNewCheckPoint();

            bool added = false;

            // looping here in event of a dupe key - which could happen during multiple calls in quick succession
            while(!added)
            {
                try
                {
                    _checkPoints.Add(DateTime.UtcNow, _checkPoint);
                    added = true;
                }
                catch (ArgumentException)
                {
                    // continue
                }
            }
    

            

            if (_checkPoints.Count > MaxCheckPoints)
            {
                _checkPoints.Remove(_checkPoints.Keys.First());
            }

        }
            
        public bool IsDirty()
        {
            if (_checkPoint == null)
            {
                throw new InvalidOperationException("IsDirty cannot be performed without a checkpoint");
            }
            return HasBeenChanged();
        }

        public abstract bool IsNew();

        public void ClearCheckPoints()
        {
            _checkPoint = null;
            _checkPoints = new SortedDictionary<DateTime, EntityBase>();
        }

        /// <summary>
        /// implementations should create deep copies of the current object
        /// </summary>
        /// <returns></returns>
        protected abstract EntityBase CreateNewCheckPoint();

        protected abstract bool HasBeenChanged();

        protected abstract void SaveNew(IDataBroker dataBroker);

        protected abstract void SaveExisting(IDataBroker dataBroker);

        /// <summary>
        /// limiting the access to prevent saving outside the GrocvceryStore class. We're using
        /// the InternalsVisibleTo to allow an exemption for unit tests
        /// </summary>
        /// <param name="dataBroker"></param>
        internal void Save(IDataBroker dataBroker)
        {
            if (IsNew()) {
                SaveNew(dataBroker);
            } else
            {
                SaveExisting(dataBroker);
            }
        }

    }
}
