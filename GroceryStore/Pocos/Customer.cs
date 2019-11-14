using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GroceryStore.Interfaces;
using GroceryStore.DataBroker;


namespace GroceryStore.Pocos
{
    [DataContract]
    public class Customer : EntityBase
    {
        // limiting ctor access to avoid newing up outside factory method
        public Customer() : base()
        {

        }

        protected override bool HasBeenChanged()
        {
            var checkPoint = (Customer)_checkPoint;
            return Name != checkPoint.Name || checkPoint.Id != Id;
        }

        protected  override EntityBase CreateNewCheckPoint()
        {
            var checkPoint = (Customer)MemberwiseClone();
            if (Name != null)
            {
                checkPoint.Name = string.Copy(Name);
                checkPoint.Id = Id;
            }            
            return checkPoint;
        }

        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        public override bool IsNew()
        {
            return Id == null;
        }

        protected override void SaveNew(IDataBroker dataBroker)
        {
            dataBroker.SaveCustomer(this);
        }

        protected override void SaveExisting(IDataBroker dataBroker)
        {
            dataBroker.UpdateCustomer(this);
        }
    }
}
