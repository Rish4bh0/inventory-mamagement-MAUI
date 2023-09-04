using System;

namespace Inventory_Management.Data;

public class ItemModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int Quantity { get; set; }
    public float UnitPrice { get; set; }
    public Guid AddedBy { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.Now;
}
