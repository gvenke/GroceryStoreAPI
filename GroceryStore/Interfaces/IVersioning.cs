using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryStore.Interfaces
{
    interface IVersioning
    {
        bool IsDirty { get; set; }     
    }
}
