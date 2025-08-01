using NiceHandles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace NiceHandles.Controllers
{
    public class ShareController : Controller
    {
        private NHModel db = new NHModel();
        public Account GetCurrentAccount()
        {
            var username = User.Identity.GetUserName();
            return db.Accounts.Where(x => x.UserName.Equals(username)).Single();
        }
    }
}