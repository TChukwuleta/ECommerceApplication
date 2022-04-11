using EcommerceApplication.Domain.DTOs.Response;
using EcommerceApplication.Domain.Entities;

namespace EcommerceApplication.Application.Services
{
    public interface ICartService
    {   
        Task<ResultResponse> AddToCartAsync(int cartItemId, string userId, CartItem cartItem);
        Task<ResultResponse> RemoveFromCartAsync(int itemId);
        Task<ResultResponse> UpdateCartAsync(int cartId);
        Task<ResultResponse> GetCartItemsAsync();
    }
}
