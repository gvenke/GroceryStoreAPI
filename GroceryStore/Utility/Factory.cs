using GroceryStore.Entity;

namespace GroceryStore.Utility
{
    public static class Factory
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
