namespace ProyectoFinal.Migrations
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using Models;

	internal sealed class Configuration : DbMigrationsConfiguration<ProyectoFinal.Models.ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(ProyectoFinal.Models.ApplicationDbContext context)
		{
			//  This method will be called after migrating to the latest version.

			if (!context.Cards.Any())
			{
				var cards = new List<Card>(108);

				foreach (var color in (Color[])Enum.GetValues(typeof(Color)))
				{
					foreach (var rank in (Rank[])Enum.GetValues(typeof(Rank)))
					{
						cards.Add(new Card { Color = color, Rank = rank });
						if (!new Rank[] { Rank._0, Rank.Wild, Rank.WildDrawFour }.Contains(rank))
						{
							cards.Add(new Card { Color = color, Rank = rank });
						}
					}
				}

				context.Cards.AddRange(cards);
				context.SaveChanges();
			}
		}
	}
}
