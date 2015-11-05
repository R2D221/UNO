using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Controllers
{
	[Authorize]
	public class GamesController : Controller
	{
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(CreateGameViewModel model)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Hay un error con los correos. Veríficalos y vuelve a intentarlo");
				return View(model);
			}

			var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

			using (var gamesService = new GamesService())
			{
				string first;
				string second;
				string third;
				string fourth;

				try
				{
					first = User.Identity.GetUserId();
					second = userManager.FindByEmail(model.Email2).Id;
					third = userManager.FindByEmail(model.Email3).Id;
					fourth = userManager.FindByEmail(model.Email4).Id;
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Hay un error con los correos. Veríficalos y vuelve a intentarlo");
					return View(model);
				}

				var set = new HashSet<string>(new[] { first, second, third, fourth });
				if (set.Count != 4)
				{
					ModelState.AddModelError("", "No puedes invitar a un amigo más de una vez");
					return View(model);
				}

				var sessionId = gamesService.Create(first, second, third, fourth);

				return RedirectToAction("Play", new { id = sessionId });
			}
		}

		public ActionResult Play(Guid id)
		{
			using (var gamesService = new GamesService())
			{
				var session = gamesService.GetSession(id, User.Identity.GetUserId());
				return View(session);
			}
		}
	}
}