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
            CreateMap<Item, ItemDto>();
            CreateMap<User, ItemAdderUserDto>();
            CreateMap<AddItemDto, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsChecked, opt => opt.Ignore())
                .ForMember(dest => dest.AmountOrdered, opt => opt.Ignore())
                .ForMember(dest => dest.ShoppingList, opt => opt.Ignore())
                .ForMember(dest => dest.ItemAdder, opt => opt.Ignore());
        }
    }
}
