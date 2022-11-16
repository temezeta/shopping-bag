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
        public static ControllerContext GetLoggedInControllerContext()
        {
            var claim = new Claim(ClaimTypes.Email, "test@test.com");
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User.FindFirst(ClaimTypes.Email)).Returns(claim);
            var controllerContext = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            return controllerContext;
        }

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

        public static IUrlHelper GetUrlHelper()
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(it => it.ActionContext).Returns(new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor()));
            mockUrlHelper.Setup(url => url.Action(It.IsAny<UrlActionContext>())).Returns("TEST");
            return mockUrlHelper.Object;
        }
    }
}
