using AutoMapper;
using shopping_bag.DTOs.Office;
using shopping_bag.DTOs.User;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.DTOs.ShoppingList;

namespace shopping_bag.Utility {
    public class MappingProfile : Profile {

        public MappingProfile() {
            CreateMap<User, UserDto>();
            CreateMap<Office, OfficeDto>();
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<ShoppingList, ShoppingListDto>();
            CreateMap<AddOfficeDto, Office>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
