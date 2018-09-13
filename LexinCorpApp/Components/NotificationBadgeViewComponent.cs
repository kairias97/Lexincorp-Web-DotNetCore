using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Components
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(0);
        }
    }
}
