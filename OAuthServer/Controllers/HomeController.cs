using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OAuthServer.Models;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2.Messages;

namespace OAuthServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View(); 
        }

        public ActionResult Auth()
        {
            var authSvr = new AuthorizationServer(new AuthServerHost());
            var request = authSvr.ReadAuthorizationRequest(Request);
            Session["request"] = request;
            return View();
        }

        [HttpPost]
        public ActionResult Auth(LoginModel loginData)
        {
            var authSvrHostImpl = new AuthServerHost();
            var ok = (loginData.Username == "Max Muster" && loginData.Password == "test123");
            if (ok)
            {
                var request = Session["request"] as EndUserAuthorizationRequest;
                var authSvr = new AuthorizationServer(authSvrHostImpl);
                var approval = authSvr.PrepareApproveAuthorizationRequest(request, loginData.Username, new[] { "http://localhost/demo" });

                return authSvr
                         .Channel
                         .PrepareResponse(approval)
                         .AsActionResult();
            }

            ViewBag.Message = "Wrong username or password!";
            return View();
        }

        public ActionResult Token()
        {
            var authSvr = new AuthorizationServer(new AuthServerHost());
            var response = authSvr.HandleTokenRequest(Request);
            return response.AsActionResult();
        }

    }
}