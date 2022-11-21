using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.Office;
using shopping_bag.Models;

namespace shopping_bag.Services {
    public class OfficeService : IOfficeService {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public OfficeService(AppDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Office>> AddOffice(AddOfficeDto office) {
            var findOffice = await _context.Offices.FirstOrDefaultAsync(o => o.Name == office.Name);

            if (findOffice != null && !findOffice.Removed) {
                return new ServiceResponse<Office>("Office with that name already exists");
            }
            if (findOffice != null && findOffice.Removed)
            {
                findOffice.Removed = false;
                await _context.SaveChangesAsync();
                return new ServiceResponse<Office>(_mapper.Map<Office>(findOffice));
            }

            var addedOffice = await _context.Offices.AddAsync(_mapper.Map<Office>(office));
            await _context.SaveChangesAsync();
            return new ServiceResponse<Office>(addedOffice.Entity);
        }

        public async Task<ServiceResponse<Office>> ModifyOffice(AddOfficeDto office, long officeId)
        {
            var modifyOffice = await _context.Offices.Where(o => o.Id == officeId).FirstOrDefaultAsync();

            if (modifyOffice == null || modifyOffice.Removed)
            {
                return new ServiceResponse<Office>(error: "Invalid office id");
            }

            var findOffice = _context.Offices.Any(o => o.Name == office.Name);

            if (findOffice)
            {
                return new ServiceResponse<Office>(error: "Office with that name already exists");
            }

            modifyOffice.Name = office.Name;
            await _context.SaveChangesAsync();

            return new ServiceResponse<Office>(data: modifyOffice);
        }

        public async Task<ServiceResponse<bool>> RemoveOffice(long officeId)
        {
            var office = await _context.Offices.Where(o => o.Id == officeId).FirstOrDefaultAsync();

            if (office == null || office.Removed)
            {
                return new ServiceResponse<bool>(error: "Invalid office id");
            }

            office.Removed = true;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(true);
        }

        public async Task<ServiceResponse<IEnumerable<Office>>> GetOffices() {
            var offices = await _context.Offices.Where(o => !o.Removed).ToListAsync();
            return new ServiceResponse<IEnumerable<Office>>(offices);
        }
    }
}
