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
        public const byte MaxCheckPoints = 3;

        protected SortedDictionary<DateTime, EntityBase> _checkPoints;

        protected EntityBase()
        {
            _checkPoints = new SortedDictionary<DateTime, EntityBase>();
        }

        protected EntityBase _checkPoint;

        public SortedDictionary<DateTime, EntityBase> CheckPointHistory => _checkPoints;

        public EntityBase CurrentCheckPoint => _checkPoint;

        public void CreateCheckPoint()
        {

            _checkPoint = CreateNewCheckPoint();

            bool added = false;
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


        protected abstract EntityBase CreateNewCheckPoint();

        protected abstract bool HasBeenChanged();

        protected abstract void SaveNew(IDataBroker dataBroker);

        protected abstract void SaveExisting(IDataBroker dataBroker);

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
