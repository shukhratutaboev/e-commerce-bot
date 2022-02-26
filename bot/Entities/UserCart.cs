namespace bot.Entities;
public class UserCart
{
    public long UserId { get; set; }
    public int MessageId { get; set; }
    public List<Item> Cart{ get; set; }
    public UserCart(long userId, Item newItem)
    {
        UserId = userId;
        MessageId = 0;
        Cart = new List<Item>() {newItem};
    }
}
