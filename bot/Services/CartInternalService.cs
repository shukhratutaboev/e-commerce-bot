using bot.Entities;

namespace bot.Services;
public class CartInternalService
{
    private readonly ILogger<CartInternalService> _logger;
    private readonly List<UserCart> _carts;

    public CartInternalService(ILogger<CartInternalService> logger)
    {
        _logger = logger;
        _carts = new List<UserCart>();
    }
    public bool Exists(long chatId)
        => _carts.Any(c => c.UserId == chatId);
    public UserCart GetUserCart(long chatId)
        => _carts.FirstOrDefault(c => c.UserId == chatId);
    public bool UpdateCart(long chatId, string itemId, int quantity)
    {
        try
        {
            var cart = GetUserCart(chatId);
            _carts.Remove(cart);
            var newItem = new Item(itemId, quantity);
            if (cart == default)
            {
                cart = new UserCart(chatId);
                cart.Cart.Add(newItem);
            }
            else
            {
                var item = cart.Cart.FirstOrDefault(i => i.ItemId == itemId);
                if(item != default) cart.Cart.Remove(item);
                if(quantity != 0) cart.Cart.Add(newItem);
            }
            _carts.Add(cart);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
        
    }
    public void UpdateCart(UserCart cart)
    {
        var c = GetUserCart(cart.UserId);
        _carts.Remove(c);
        _carts.Add(cart);
    }
    public void ClearCart(long chatId)
    {
        var c = GetUserCart(chatId);
        _carts.Remove(c);
        c.Cart.Clear();
        _carts.Add(c);
    }
}
