using AutoMapper;
using shopping_bag.DTOs.Office;
using shopping_bag.DTOs.User;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.DTOs.Reminder;

namespace shopping_bag.Utility {
    public class MappingProfile : Profile {

        public MappingProfile() {
            CreateMap<User, UserDto>(); 
            CreateMap<Office, OfficeDto>();
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<ShoppingList, ShoppingListDto>();
            CreateMap<AddOfficeDto, Office>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Removed, opt => opt.Ignore());
            CreateMap<Item, ItemDto>();
            CreateMap<User, RedactedUserDto>();
            CreateMap<AddItemDto, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsChecked, opt => opt.Ignore())
                .ForMember(dest => dest.AmountOrdered, opt => opt.Ignore())
                .ForMember(dest => dest.ShoppingList, opt => opt.Ignore())
                .ForMember(dest => dest.ItemAdder, opt => opt.Ignore())
                .ForMember(dest => dest.UsersWhoLiked, opt => opt.Ignore());
            CreateMap<ModifyItemDto, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ShoppingListId, opt => opt.Ignore())
                .ForMember(dest => dest.ShoppingList, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ItemAdder, opt => opt.Ignore())
                .ForMember(dest => dest.UsersWhoLiked, opt => opt.Ignore());
            CreateMap<ModifyShoppingListDto, ShoppingList>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.OfficeId, opt => opt.Ignore())
                .ForMember(dest => dest.ListDeliveryOffice, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ListCreatorUser, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Removed, opt => opt.Ignore())
                .ForMember(dest => dest.Ordered, opt => opt.Ignore());
            CreateMap<ReminderSettings, ReminderSettingsDto>();
            CreateMap<Reminder, ReminderDto>()
                .ForMember(dest => dest.DueDateRemindersDisabled, opt => opt.ConvertUsing(new ReminderDaysListToDisabledPropertyConverter(), src => src.DueDaysBefore))
                .ForMember(dest => dest.ExpectedRemindersDisabled, opt => opt.ConvertUsing(new ReminderDaysListToDisabledPropertyConverter(), src => src.ExpectedDaysBefore))
                .ForMember(dest => dest.ReminderDaysBeforeDueDate, opt => opt.MapFrom(src => src.DueDaysBefore))
                .ForMember(dest => dest.ReminderDaysBeforeExpectedDate, opt => opt.MapFrom(src => src.ExpectedDaysBefore));
        }
    }

    // Non-empty list -> false (not disabled)
    // Empty list -> true (disabled)
    public class ReminderDaysListToDisabledPropertyConverter : IValueConverter<List<int>, bool> {

        public bool Convert(List<int> source, ResolutionContext context)
            => !source.Any();
    }
}
