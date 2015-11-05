﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using ProyectoFinal.Services;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace ProyectoFinal.Hubs
{
	public class GameHub : Hub
	{
		public override Task OnConnected()
		{
			var sessionId = Context.QueryString["sessionId"];
			Groups.Add(Context.ConnectionId, sessionId);
			return base.OnConnected();
		}

		public void UseCard(Guid sessionId, int cardId)
		{
			using (var gamesService = new GamesService())
			{
				var update = gamesService.TryUseCard(sessionId, Context.User.Identity.GetUserId(), cardId);
				if (update == null)
				{
					Clients.Caller.denyMove();
				}
				else
				{
					Clients.Caller.acceptMove();
					update.CardHtml = RenderPartialView("Views/Shared/Card.cshtml", "Card", update.Card);
					Clients.Group(sessionId.ToString()).update(update);
				}
			}
		}

		private string RenderPartialView(string view, string key, object model)
		{
			var config = new TemplateServiceConfiguration();
			config.BaseTemplateType = typeof(HtmlSupportTemplateBase<>);
			using (var service = RazorEngineService.Create(config))
			{
				var template = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, view));

				return service.RunCompile(template, key, model.GetType(), model);
			}
		}

		public  class MyHtmlHelper
		{
			public IEncodedString Raw(string rawString)
			{
				return new RawString(rawString);
			}
		}

		public abstract class HtmlSupportTemplateBase<T> : TemplateBase<T>
		{
			public HtmlSupportTemplateBase()
			{
				Html = new MyHtmlHelper();
			}

			public MyHtmlHelper Html { get; set; }
		}
	}
}