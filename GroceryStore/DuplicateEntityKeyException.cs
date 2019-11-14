using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryStore.Utilities
{
    public class DuplicateEntityKeyException : Exception
    {
        public DuplicateEntityKeyException() : base() { }

        public DuplicateEntityKeyException(string message, object key) : base(message)
        {
            DuplicateKey = key;
        }

        public object DuplicateKey { get; private set; }
    }
}
