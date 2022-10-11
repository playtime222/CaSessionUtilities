using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace RdeMessagingDemo.Server.Tests
{
    public class RouteTesting
    {
        //[InlineData("/", "~/", "Home", "Index")]
        //[InlineData("/", "api/command", "Other", "command")]
        //[Theory]
        //public void stub_mocker(string apppath, string route, string expected_controller, string expected_action)
        //{
        //    var rc = new RouteCollection();
        //    Infrastructure.RouteRegistry.RegisterRoutes(rc);

        //    var httpmock = new StubHttpContextForRouting(appPath: apppath, requestUrl: route);

        //    // this always returns null for everything but the Index case.
        //    var routeData = rc.GetRouteData(httpmock);

        //    var controller = routeData.Values["controller"];
        //    var action = routeData.Values["action"];
        //    Assert.Equal(expected_controller, controller);
        //    Assert.Equal(expected_action, action);
        //}

        //[Fact]
        //public void kick_the_tires()
        //{
        //    var rc = new RouteCollection();

        //    Infrastructure.RouteRegistry.RegisterRoutes(rc);

        //    // get the route corresponding to name.
        //    var got = rc["name"];

        //    var expected = //What? foo is an internal type that can't be instantiated.

        //    Assert.Equal(foo, frob);
        //}


    }
}
