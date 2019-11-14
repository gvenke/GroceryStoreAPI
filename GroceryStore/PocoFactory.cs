using System;
using System.Collections.Generic;
using System.Text;
using GroceryStore.Pocos;

namespace GroceryStore
{
    public static class PocoFactory
    {
        public static Product CreateProduct()
        {
            return new Product();
        }

        public static Customer CreateCustomer()
        {
            return new Customer();
        }

        public static Order CreateOrder()
        {
            return new Order();
        }
       
    }
}
