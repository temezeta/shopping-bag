using shopping_bag.Models;
using shopping_bag.Models.Email;

namespace shopping_bag.Services
{
    public interface IEmailService
    {
        ServiceResponse<bool> SendEmail(Email email);
    }
}