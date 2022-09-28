using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.Office;
using shopping_bag.Models;

namespace shopping_bag.Services {
    public class OfficeService : IOfficeService {

        private readonly AppDbContext _context;
        public OfficeService(AppDbContext context) {
            _context = context;
        }

        public async Task<ServiceResponse<OfficeDto>> AddOffice(AddOfficeDto office) {
            if(_context.Offices.Any(o => o.Name == office.Name)) {
                return new ServiceResponse<OfficeDto>("Office with that name already exists");
            }
            var addedOffice = await _context.Offices.AddAsync(new Office() { Name = office.Name });
            await _context.SaveChangesAsync();
            return new ServiceResponse<OfficeDto>(MapOfficeToDto(addedOffice.Entity));
        }

        public async Task<ServiceResponse<IEnumerable<OfficeDto>>> GetOffices() {
            var offices = await _context.Offices.ToListAsync();
            return new ServiceResponse<IEnumerable<OfficeDto>>(offices.Select(o => MapOfficeToDto(o)).ToList());
        }

        private OfficeDto MapOfficeToDto(Office office) {
            return new OfficeDto() {
                Name = office.Name
            };
        }
    }
}
