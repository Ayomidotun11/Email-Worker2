using Email_Worker2.DataRepository.Repositories;
using Email_Worker2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetUsersForEmailProcessingAsync();
        Task UpdateUserEmailStatusAsync(int userId, DateTime emailSentTime);
    }
    public class UserService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetUsersForEmailProcessingAsync()
        {
            try
            {
                // Fecth the Users who havent recieved emails 
                var users = await _unitOfWork.Users.FindAsync(u => !u.IsEmailSent);
                _logger.LogInformation("Found {Count} users who need to receive emails", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users for email processing");
                throw;
            }
        }
        public async Task UpdateUserEmailStatusAsync(int userId, DateTime emailSentTime)
        {
            try
            {
                // Access the Users repository through UnitOfWork
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user != null)
                {
                    user.LastEmailSent = emailSentTime;
                    user.IsEmailSent = true;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.CompleteAsync();
                    _logger.LogInformation("Updated email status for user {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user email status for userId: {UserId}", userId);
                throw;
            }
        }

    }
}
