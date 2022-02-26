namespace bot.Entities
{
    public class Item
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public Item(string itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}
