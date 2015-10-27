using System;
using System.Collections.Generic;
using System.Linq;
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
				return View(model);
			}

			var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

			using (var gamesService = new GamesService())
			{
				var first = User.Identity.GetUserId();
				var second = userManager.FindByEmail(model.Email2).Id;
				var third = userManager.FindByEmail(model.Email3).Id;
				var fourth = userManager.FindByEmail(model.Email4).Id;

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

		public ActionResult UseCard(Guid sessionId, int cardId)
		{
			using (var gamesService = new GamesService())
			{
				var newUserTurn = gamesService.TryUseCard(sessionId, User.Identity.GetUserId(), cardId);
				if (newUserTurn == null)
				{
					return new HttpStatusCodeResult(400, "Jugada inválida");
				}
				else
				{
					return Content(newUserTurn);
				}
			}
		}
	}
}