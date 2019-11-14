using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GroceryStore.DataBroker;

namespace GroceryStore.Pocos
{
    [DataContract]
    public class Product : EntityBase
    {
        // limiting ctor access to avoid newing up outside factory method
        public Product() : base()
        {

        }

        protected override bool HasBeenChanged()
        {
            var checkPoint = (Product)_checkPoint;
            return Price != checkPoint.Price || Description != checkPoint.Description || checkPoint.Id != Id;
        }

        protected override EntityBase CreateNewCheckPoint()
        {
            var checkPoint = (Product)MemberwiseClone();
            if (Description != null)           
            {
                checkPoint.Description = string.Copy(Description);
                checkPoint.Id = Id;
            }
            checkPoint.Price = Price;
            return checkPoint;
        }

        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Price { get; set; }

        protected override void SaveNew(IDataBroker dataBroker)
        {
            dataBroker.SaveProduct(this);
        }

        public override bool IsNew()
        {
            return Id == null;
        }

        protected override void SaveExisting(IDataBroker dataBroker)
        {
           dataBroker.UpdateProduct(this);
        }
    }
}
