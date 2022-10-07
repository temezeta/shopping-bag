using AutoMapper;
using shopping_bag;

namespace shopping_bag_unit_tests
{
    public class AutoMapperTests
    {
        [Fact]
        public void AutoMapper_AssertConfigurationValid()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddMaps(typeof(Program).Assembly);
            });
            config.AssertConfigurationIsValid();
        }

    }
}
