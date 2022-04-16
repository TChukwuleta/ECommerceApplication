using EcommerceApplication.Application.Services;
using EcommerceApplication.Data;
using EcommerceApplication.Domain.Entities;
using EcommerceApplication.Domain.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayStack.Net;

namespace EcommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly string token;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly PayStackApi payStack;
        public OrderController(IOrderService orderService, AppDbContext context, IConfiguration config)
        {
            _orderService = orderService;
            _config = config;
            _context = context;
            token = _config["Payment:PaystackTest"];
            payStack = new PayStackApi(token);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(int ItemId, string userid)
        {
            try
            {
                /*var validateUser = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == UserId);
                if(validateUser != null)
                {
                    return new JsonResult("Invalid user details.") { StatusCode = 400 };
                }*/
                var newOrder = await _orderService.AddOrderAsync(ItemId, userid);
                return Ok(newOrder);
            }
            catch (Exception ex)
            {
                return new JsonResult("Something went wrong") { StatusCode = 500 };
            }
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay(PaymentViewModel model)
        {
            try
            {
                var validateUser = await _context.Customers.Where(c => c.Email == model.Email).FirstOrDefaultAsync();
                if(validateUser == null)
                {
                    return BadRequest("Invalid User");
                }

                var validateItem = await _context.ItemDatas.Where(c => c.Id == model.ItemId).FirstOrDefaultAsync();
                if(validateItem == null)
                {
                    return BadRequest("Item does not exist");
                }
                TransactionInitializeRequest request = new()
                {
                    AmountInKobo = model.Amount * 100,
                    Email = validateUser.Email,
                    Reference = Generate().ToString(),
                    Currency = "NGN",
                    CallbackUrl = "http://localhost:7064/order/verify"
                };

                TransactionInitializeResponse response = payStack.Transactions.Initialize(request);
                if (!response.Status)
                {
                    return BadRequest(response.Message);
                }
                var transaction = new Transaction
                {
                    ItemId = model.ItemId,
                    Amount = model.Amount,
                    TransactionReference = request.Reference,
                    Email = validateUser.Email,
                    ItemName = model.Name,
                    BuyerName = validateUser.FirstName + " " + validateUser.LastName,
                    UserId = validateUser.UserId
                };
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                return Ok(response.Data);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify(string reference)
        {
            TransactionVerifyResponse response = payStack.Transactions.Verify(reference);
            if(response.Data.Status == "success")
            {
                var transaction = await _context.Transactions.Where(c => c.TransactionReference == reference).FirstOrDefaultAsync();
                if(transaction != null)
                {
                    transaction.Status = true;
                    _context.Transactions.Update(transaction);
                    await _context.SaveChangesAsync();
                    return Ok(response.Data);
                }
            }
            return BadRequest(response.Data.GatewayResponse);
        }

        [HttpGet("boughtitems")]
        public async Task<IActionResult> BoughtItem(string userid)
        {
            var authTxns = await _context.Transactions.Where(c => c.UserId == userid && c.Status == true).ToListAsync();
            if(authTxns == null)
            {
                return BadRequest("You have not bought any items yet");
            }
            var itemList = new List<ItemData>();
            foreach (var item in authTxns)
            {
                var itemData = await _context.ItemDatas.Where(v => v.Id == item.ItemId).FirstOrDefaultAsync();
                itemList.Add(itemData);
            }
            return Ok(itemList);
        }

        public static int Generate()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            return rand.Next(100000000, 999999999);
        }
    }
}
