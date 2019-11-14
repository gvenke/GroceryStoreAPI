using GroceryStore;
using GroceryStore.Entity;
using GroceryStore.Utility;
using System;


namespace SandBox
{
    class Program
    {

        static void Main(string[] args)
        {
            
            
            var dataBroker = Factory.GetDataBroker();
            var store = new GroceryStoreManager(dataBroker);
            var entity = store.GetOrder(1);
            entity.CustomerId = 3;
            entity.OrderDate = DateTime.Now;
            entity.Items.Clear();
            entity.Items.Add(new OrderItem { ProductId = 1, Quantity = 1 });

            store.Save(entity);

            entity.CustomerId = 4;
            entity.OrderDate = DateTime.Now;
            entity.Items.Add(new OrderItem { ProductId = 2, Quantity = 2 });
            store.Save(entity);



            Console.ReadLine();
        }
    }
}
