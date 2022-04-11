using EcommerceApplication.Domain.DTOs.Response;
using EcommerceApplication.Domain.Entities;

namespace EcommerceApplication.Application.Services
{
    public interface IAuthService
    {
        Task<ResultResponse> CreateUserAsync(Customer user);
        Task<ResultResponse> loginAsync(string email, string password);
        Task<ResultResponse> ChangeUserStatusAsync(Customer user);
        Task<ResultResponse> ChangePasswordAsync(string email, string oldPassword, string newPassword);
    }
}
