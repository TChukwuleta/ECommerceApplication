using EcommerceApplication.Domain.DTOs.Response;
using EcommerceApplication.Domain.Entities;

namespace EcommerceApplication.Application.Services
{
    public interface IOrderService
    {
        Task<ResultResponse> AddOrderAsync(int ItemId, string userId);
        Task<ResultResponse> UpdateOrderAsync(int orderId);
        Task<ResultResponse> GetOrderItemsAsync();
    }
}
