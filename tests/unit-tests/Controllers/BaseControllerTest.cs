using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace shopping_bag_unit_tests.Controllers
{
    public class BaseControllerTest
    {
        public BaseControllerTest()
        {
            UnitTestHelper.SetupStaticConfig();
        }

        protected ControllerContext GetLoggedInControllerContext()
        {
            var claim = new Claim(ClaimTypes.Email, "test@test.com");
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User.FindFirst(ClaimTypes.Email)).Returns(claim);
            var controllerContext = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            return controllerContext;
        }

        protected IUrlHelper GetUrlHelper()
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(it => it.ActionContext).Returns(new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor()));
            mockUrlHelper.Setup(url => url.Action(It.IsAny<UrlActionContext>())).Returns("TEST");
            return mockUrlHelper.Object;
        }
    }
}