using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using AutoMapper;
using shopping_bag;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using shopping_bag.Config;

namespace shopping_bag_unit_tests
{
    public static class UnitTestHelper
    {
        public static void SetupStaticConfig()
        {
            var testConfiguration = new Dictionary<string, string>
            {
                {"VerificationEmail:BodyText", "Something sensible"},
                {"Jwt:Token", "superlongssecretwritesomethinghere"},
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(testConfiguration).Build();
            StaticConfig.Setup(configuration);
        }

        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddMaps(typeof(Program).Assembly);
            });
            return new Mapper(config);
        }
    }
}
