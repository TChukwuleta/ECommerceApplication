using EcommerceApplication.Data;
using EcommerceApplication.Domain.DTOs.Response;
using EcommerceApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApplication.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ResultResponse> AddOrderAsync(int itemId, string userId)
        {
            try
            {
                var existingItem = await _context.ItemDatas.Where(c => c.Id == itemId).FirstOrDefaultAsync();
                if(existingItem == null)
                {
                    return ResultResponse.Failure("THis is not an item");
                }
                var orderQty  = 0;

                if(existingItem.AddToCart)
                {
                    var CartItem = await _context.CartItems.Where(b => b.ItemDataId == existingItem.Id).FirstOrDefaultAsync();
                    orderQty = CartItem != null ? CartItem.Amount : 1;
                }

                var newOrder = new Order
                {
                    ItemName = existingItem.Name,
                    ItemDataId = existingItem.Id,
                    Quantity = orderQty != 0 ? orderQty : 1,
                    Paid = false,
                    Status = Domain.Enums.OrderStatus.New,
                    StatusDesc = Domain.Enums.OrderStatus.New.ToString(),
                    OrderDate = DateTime.Now,
                    UserId = userId
                };

                newOrder.AmountToBePaid = newOrder.Quantity * existingItem.Amount;

                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                return ResultResponse.Success("Yes", newOrder);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ResultResponse> GetOrderItemsAsync()
        {
            try
            {
                var entity = await _context.Orders.ToListAsync();
                if(entity == null)
                {
                    return ResultResponse.Failure("You haven't made any order");
                }
                return ResultResponse.Success("Here are your orders", entity);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task<ResultResponse> UpdateOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
