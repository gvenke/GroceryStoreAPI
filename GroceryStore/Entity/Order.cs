using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GroceryStore.DataBroker;
using System.Linq;

namespace GroceryStore.Entity
{
    [DataContract]
    public class Order : EntityBase
    {

        protected override bool HasBeenChanged()
        {
            var checkPoint = (Order)_checkPoint;
            return CustomerId != checkPoint.CustomerId || !Items.SequenceEqual(checkPoint.Items) || checkPoint.Id != Id || OrderDate != checkPoint.OrderDate;
        }

        protected override EntityBase CreateNewCheckPoint()
        {
            var checkPoint = (Order)MemberwiseClone();
            checkPoint.Id = Id;
            checkPoint.CustomerId = CustomerId;
            checkPoint.OrderDate = OrderDate;

            // items are shallow copies at the moment (not what we want)
            checkPoint.Items = Items.ToList();

            return checkPoint;
        }

        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public DateTime OrderDate { get; set; }

        [DataMember]
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();

        [DataMember]
        public int CustomerId { get; set; }

        public override bool IsNew()
        {
            return Id == null;
        }

        protected override void SaveNew(IDataBroker dataBroker)
        {
            dataBroker.SaveOrder(this);
        }

        protected override void SaveExisting(IDataBroker dataBroker)
        {
            dataBroker.UpdateOrder(this);
        }
    }
}
