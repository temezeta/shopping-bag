using shopping_bag.DTOs.Office;
using shopping_bag.Models;

namespace shopping_bag.Services {
    public interface IOfficeService {

        Task<ServiceResponse<IEnumerable<Office>>> GetOffices();
        Task<ServiceResponse<Office>> AddOffice(AddOfficeDto office);

        Task<ServiceResponse<Office>> ModifyOffice(AddOfficeDto office, long officeId);

        Task<ServiceResponse<bool>> RemoveOffice(long officeId);
    }
}
