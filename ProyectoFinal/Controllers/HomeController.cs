using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProyectoFinal.Services;

namespace ProyectoFinal.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			if (Request.IsAuthenticated)
			{
				using (var gamesService = new GamesService())
				{
					var sessions = gamesService.GetActiveSessionsForUser(User.Identity.GetUserId());
					return View("Dashboard", sessions);
				}
			}
			else
			{
				return View();
			}
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
	}
}