using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterAnalyticsWeb.Controllers;
using System.Threading.Tasks;
using TwitterAnalyticsCommon;
using TwitterAnalyticsWeb.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;
using Moq;
using System.Web;
using System.Collections.Specialized;
using System.Net;
using Microsoft.AspNet.Identity;

namespace TwitterAnalyticsWeb.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {

        public AccountControllerTest()
        {
            SetupLoggers();
        }

        void SetupLoggers()
        {
            LoggerFactory<ILogger>.Register(typeof(WebLogger), () => new WebLogger());
            LoggerFactory<ILogger>.Register(typeof(DALLogger), () => new DALLogger());
        }       
        
        AccountController CreateAccountControllerAs(string userName)
        {

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(userName);
            mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            var controller = new AccountController();
            controller.ControllerContext = mock.Object;

            return controller;
        }
        
        [TestMethod]
        public void Login_View_IsAvailable()
        {            

            // Arrange
            var _controller = CreateAccountControllerAs("Rohit4@gmail.com");
            _controller.ControllerContext = Helpers.GetMvcControllerContextMock(Helpers.GetHttpContextMock().Object).Object;

            // Act
            var _result = _controller.Login("Account/Login") as ViewResult;

            // Assert
            Assert.IsNotNull(_result);
        }
                
        [TestMethod]
        public void Register_View_IsAvailable()
        {

            // Arrange
            var _controller = new AccountController();
            _controller.ControllerContext = Helpers.GetMvcControllerContextMock(Helpers.GetHttpContextMock().Object).Object;

            // Act
            var _result = _controller.Register() as ViewResult;

            // Assert
            Assert.IsNotNull(_result);
        }


        [TestMethod]
        public  void Login_User_Exists()
        {
           
            //Arrange
            var dummyUser = new ApplicationUser() { UserName = "rohit@gmail.com" };
            var mockStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new UserManager<ApplicationUser>(mockStore.Object);
            mockStore.Setup(x => x.CreateAsync(dummyUser))
                        .Returns(Task.FromResult(IdentityResult.Success));

            mockStore.Setup(x => x.FindByNameAsync(dummyUser.UserName))
                        .Returns(Task.FromResult(dummyUser));

            //Act
            Task<IdentityResult> tt = (Task<IdentityResult>)mockStore.Object.CreateAsync(dummyUser);
            var user = userManager.FindByName("rohit@gmail.com");

            //Assert
            Assert.AreEqual("rohit@gmail.com", user.UserName);


        }        

    }



    class MockHttpSession : HttpSessionStateBase
    {
        readonly Dictionary<string, object> _SessionDictionary = new Dictionary<string, object>();
        public override object this[string name]
        {
            get
            {
                object obj = null;
                _SessionDictionary.TryGetValue(name, out obj);
                return obj;
            }
            set { _SessionDictionary[name] = value; }
        }
    }

    class Helpers
    {
        internal static Mock<ControllerContext> GetMvcControllerContextMock(HttpContextBase httpContextBase = null)
        {
            // Set default HttpContextBase
            if (httpContextBase == null)
                httpContextBase = GetHttpContextMock().Object;

            var _controllerContextMock = new Mock<ControllerContext>();

            // Add default HttpContextBase
            _controllerContextMock.Setup(x => x.HttpContext).Returns(httpContextBase);

            return _controllerContextMock;
        }

        internal static Mock<HttpContextBase> GetHttpContextMock(ClaimsPrincipal claimsPrincipal = null, ClaimsIdentity claimsIdentity = null, HttpResponseBase httpResponseBase = null, HttpRequestBase httpRequestBase = null)
        {
            var _httpContextMock = new Mock<HttpContextBase>();

            // Set default ClaimsIdentity
            if (claimsIdentity == null)
                claimsIdentity = GetClaimsIdentityMock().Object;

            // Set default ClaimsPrincipal
            if (claimsPrincipal == null)
                claimsPrincipal = GetClaimsPrincipalMock(claimsIdentity).Object;

            // Set default HttpContextBase
            if (httpResponseBase == null)
                httpResponseBase = GetHttpResponseBaseMock().Object;
            if (httpRequestBase == null)
                httpRequestBase = GetHttpRequestBaseMock().Object;

            // Add Session object to HttpContext
            var _session = new MockHttpSession();

            // Add ClaimsPrincipal to HttpContext.User
            _httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

            // Add UserClaims to HttpContext.Session
            _httpContextMock.Setup(x => x.Session).Returns(_session);

            // Add HttpContextBase object to HttpContext
            _httpContextMock.Setup(x => x.Response).Returns(httpResponseBase);

            _httpContextMock.Setup(x => x.Request).Returns(httpRequestBase);
            return _httpContextMock;
        }

        internal static Mock<HttpResponseBase> GetHttpResponseBaseMock(int? statusCode = 0)
        {
            var _httpResponseMock = new Mock<HttpResponseBase>();

            // Set default StatusCode
            if (statusCode == null)
                statusCode = (int)HttpStatusCode.OK;

            // Add StatusCode to HttpResponse.StatusCode
            _httpResponseMock.Setup(x => x.StatusCode).Returns(statusCode.Value);

            return _httpResponseMock;
        }

        internal static Mock<HttpRequestBase> GetHttpRequestBaseMock()
        {
            var _httpRequestMock = new Mock<HttpRequestBase>();

            // Add StatusCode to HttpResponse.StatusCode
            _httpRequestMock.Setup(x => x.QueryString).Returns(new NameValueCollection());
            _httpRequestMock.Setup(x => x.AppRelativeCurrentExecutionFilePath).Returns(string.Empty);
            _httpRequestMock.Setup(x => x.PathInfo).Returns(string.Empty);
            _httpRequestMock.Setup(x => x.Url).Returns(new Uri("http://site/"));
            return _httpRequestMock;
        }

        internal static Mock<ClaimsPrincipal> GetClaimsPrincipalMock(ClaimsIdentity identity = null)
        {
            // Set default ClaimsIdentity
            if (identity == null)
                identity = GetClaimsIdentityMock().Object;

            var _principalMock = new Mock<ClaimsPrincipal>();
            _principalMock.Setup(x => x.Identity).Returns(identity);

            return _principalMock;
        }

        internal static Mock<ClaimsIdentity> GetClaimsIdentityMock(List<Claim> claimCollection = null, bool isAuthenticated = true)
        {
            // Set default ClaimCollection
            if (claimCollection == null)
            {
                claimCollection = new List<Claim>();
                //claimCollection.AddRange(Models.ValidClaims);
            }

            var _identityMock = new Mock<ClaimsIdentity>();
            _identityMock.Setup(x => x.IsAuthenticated).Returns(isAuthenticated);
            _identityMock.Setup(x => x.Claims).Returns(claimCollection);

            return _identityMock;
        }
    }



}
