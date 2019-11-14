using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using GroceryStore.DataBroker;


namespace GroceryStore.Pocos
{
    [DataContract]
    public class OrderItem
    {

        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public int Quantity { get; set; }

    }
}
