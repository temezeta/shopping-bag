using shopping_bag.DTOs.Office;
using shopping_bag.Models;

namespace shopping_bag.Services {
    public interface IOfficeService {

        Task<ServiceResponse<IEnumerable<OfficeDto>>> GetOffices();
        Task<ServiceResponse<OfficeDto>> AddOffice(AddOfficeDto office);
    }
}
