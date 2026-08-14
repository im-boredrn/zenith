using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;

namespace zenith.Core.Inventory
{
    public class AssimilationInventory : InventoryGeneric
    {

        public AssimilationInventory(int quantitySlots, string invID, ICoreAPI api)
    : base(quantitySlots, invID, api)
        {
            
        }
    }
}
