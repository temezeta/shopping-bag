using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using AutoMapper;
using shopping_bag;

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

        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddMaps(typeof(Program).Assembly);
            });
            return new Mapper(config);
        }
    }
}
