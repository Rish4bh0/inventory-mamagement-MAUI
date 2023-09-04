namespace Inventory_Management.Data;

public class ItemHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public Guid TakenBy { get; set; }
    public Guid ActionedBy { get; set; }
    public ActionType Action { get; set; } = ActionType.Pending;
    public int Quantity { get; set; }
    public DateTime DateRequested { get; set; } = DateTime.Now;
    public DateTime DateApproved { get; set; }
}
