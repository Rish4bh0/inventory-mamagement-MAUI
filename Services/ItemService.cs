using Inventory_Management.Data;
using System.Text.Json;

namespace Inventory_Management.Services;

public class ItemService
{
    /* It takes list ItemModel object and serialize to json 
     * and save the serialized JSON to file system */
    private static void SaveItem(List<ItemModel> items)
    {
        string directoryPath = Utils.GetDirectoryPath();
        string itemFilePath = Utils.GetItemsFilePath();

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var serializedUserJson = JsonSerializer.Serialize(items);
        File.WriteAllText(itemFilePath, serializedUserJson);
    }

    public static void SaveItemHistory(List<ItemHistory> itemHistory)
    {
        string directoryPath = Utils.GetDirectoryPath();
        string itemHistoryFilePath = Utils.GetItemsHistoryFilePath();

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var serializedUserJson = JsonSerializer.Serialize(itemHistory);
        File.WriteAllText(itemHistoryFilePath, serializedUserJson);
    }
    /* The FetchItem methods read the contents of the corresponding files 
     * from the filesystem deserialize the JSON back into lists of ItemModel objects
       and return the lists. */
    public static List<ItemModel> FetchItem()
    {
        string itemFilePath = Utils.GetItemsFilePath();

        if (!File.Exists(itemFilePath))
        {
            return new List<ItemModel>();
        }

        var items = File.ReadAllText(itemFilePath);

        return JsonSerializer.Deserialize<List<ItemModel>>(items);
    }

    public static ItemModel FetchItemById(Guid Id)
    {
        List<ItemModel> items = FetchItem();
        ItemModel item = items.FirstOrDefault(item => item.Id == Id);

        if (item == null)
        {
            throw new Exception("Item not found.");
        }

        return item;
    }

    public static List<ItemHistory> FetchItemHistory()
    {
        string itemHistoryFilePath = Utils.GetItemsHistoryFilePath();

        if (!File.Exists(itemHistoryFilePath))
        {
            return new List<ItemHistory>();
        }

        var itemHistory = File.ReadAllText(itemHistoryFilePath);

        return JsonSerializer.Deserialize<List<ItemHistory>>(itemHistory);
    }

    public static List<ItemHistory> FetchRejectedItemHistory()
    {
        List<ItemHistory> records = FetchItemHistory();

        List<ItemHistory> rejectedRecord = records.FindAll(record => record.Action == ActionType.Reject);

        return rejectedRecord;
    }
    /* FetchApprovedItemHistory methods filter the list of item history records by the status of the request 
     * (ActionType) and return only the records that match the specified status.
     */
    public static List<ItemHistory> FetchApprovedItemHistory()
    {
        List<ItemHistory> records = FetchItemHistory();

        List<ItemHistory> approvedRecord = records.FindAll(record => record.Action == ActionType.Approve);

        return approvedRecord;
    }

    public static List<ItemHistory> FetchPendingItemHistory()
    {
        List<ItemHistory> records = FetchItemHistory();

        List<ItemHistory> pendingRecord = records.FindAll(record => record.Action == ActionType.Pending);

        return pendingRecord;
    }

    /*The AddItem methods add a new item or item history record to the list, respectively, and save the updated 
     * list to the corresponding file on the filesystem.
     */
    public static List<ItemModel> AddItem(Guid addedBy, string itemName, int quantity, float unitPrice)
    {
        List<ItemModel> items = FetchItem();

        items.Add(
            new ItemModel
            {
                AddedBy = addedBy,
                Name = itemName,
                Quantity = quantity,
                UnitPrice = unitPrice
            }
        );

        SaveItem(items);
        return items;
    }
    public static List<ItemHistory> AddItemHistory(Guid itemId, Guid takerId, int quantity)
    {
        List<ItemHistory> itemHistory = FetchItemHistory();

        itemHistory.Add(
            new ItemHistory
            {
                ItemId = itemId,
                TakenBy = takerId,
                Quantity = quantity
            }
        );

        SaveItemHistory(itemHistory);
        return itemHistory;
    }
    // The DeleteItem method removes an item from the list based on its ID
    public static List<ItemModel> DeleteItem(Guid Id)
    {
        List<ItemModel> items = FetchItem();
        ItemModel item = items.FirstOrDefault(item => item.Id == Id);

        if (item == null)
        {
            throw new Exception("Item not found.");
        }

        List<ItemHistory> Records = FetchItemHistory();

        Records.RemoveAll(record => item.Id == record.ItemId);
        SaveItemHistory(Records);

        items.Remove(item);
        SaveItem(items);
        return items;
    }

    public static List<ItemModel> UpdateItem(Guid Id, string itemName, int quantity, float unitPrice)
    {
        List<ItemModel> items = FetchItem();
        ItemModel item = items.FirstOrDefault(item => item.Id == Id);

        if (item == null)
        {
            throw new Exception("Item not found.");
        }

        item.UnitPrice = unitPrice;
        item.Quantity = quantity;
        item.Name = itemName;

        SaveItem(items);
        return items;
    }

    public static List<ItemModel> TakeOutItem(Guid itemId, Guid takerId, int quantity)
    {
        if (quantity < 1)
        {
            throw new Exception("Takeout quantity must be atleast 1!");
        }

        List<ItemModel> items = FetchItem();
        ItemModel item = items.FirstOrDefault(item => item.Id == itemId);

        if (item == null)
        {
            throw new Exception("Item not found.");
        }

        if (item.Quantity < quantity)
        {
            throw new Exception("Quantity not available!");
        }

        UserModel user = UserService.FetchUserById(takerId);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        item.Quantity -= quantity;
        SaveItem(items);
        AddItemHistory(itemId, takerId, quantity);
        return items;
    }

    public static List<ItemHistory> HandleTakeAction(Guid RecordId, Guid actionId, ActionType action)
    {
        List<ItemHistory> Records = FetchItemHistory();
        ItemHistory record = Records.FirstOrDefault(record => record.Id == RecordId);


        if (record == null)
        {
            throw new Exception("Records not found.");
        }

        List<ItemModel> items = FetchItem();
        ItemModel item = items.FirstOrDefault(item => item.Id == record.ItemId);

        if (action == ActionType.Reject)
        {
            record.Action = ActionType.Reject;
            item.Quantity += record.Quantity;
        }
       
        if (action == ActionType.Approve)
        {
            string message = "Only able to approve from monday-friday and 9am to 4pm";
            if (DateTime.Parse("9: 00 AM") >= DateTime.Now || DateTime.Parse("4: 00 PM") <= DateTime.Now)
            {
                throw new Exception(message);
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new Exception(message);
            }

            record.Action = ActionType.Approve;
        }
      
        record.ActionedBy = actionId;
        record.DateApproved = DateTime.Now;

        SaveItem(items);
        SaveItemHistory(Records);
        return Records;
    }

    public static List<ItemGraphData> ItemTakenData()
    {
        List<ItemHistory> records = FetchApprovedItemHistory();
        List<ItemGraphData> datas = new();
        foreach (ItemHistory record in records)
        {
            var exist = datas.FirstOrDefault(data => data.ItemId == record.ItemId);
            if (exist != null)
            {
                exist.Quantity += record.Quantity;
                continue;
            }

            datas.Add(
                new ItemGraphData
                {
                    ItemId = record.ItemId,
                    Quantity = record.Quantity,
                }
                );
        }

        return datas;
    }
}
