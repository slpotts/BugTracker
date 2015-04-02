using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Principal;
using BugTracker.Models;
using System.Security.Claims;

namespace BugTracker.Stuff
{
    public static class Extensions
    {
        public static ApplicationDbContext db = new ApplicationDbContext();


        /*public static string GetFirstName(this IIdentity identity)
        {
            var user = db.Users.Find(identity.GetUserId());
            return user != null ? user.FirstName : "";
        }*/

        public static string FullName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "FullName")
                        return claim.Value;
                }
                return "";
            }
            else
            {
                return "";
            }
        }

        public static string Email(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "Email")
                        return claim.Value;
                }
                return "";
            }
            else
            {
                return "";
            }
        }

    }
}