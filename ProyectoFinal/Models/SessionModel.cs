using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
	public class SessionModel
	{
		public Guid Id { get; set; }

		public Direction Direction { get; set; }

		public int DeckCount { get; set; }

		public int DiscardPileCount { get; set; }

		public Card DiscardPileTop { get; set; }

		public List<HandModel> Hands { get; set; }
	}
}