using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
	public class SessionInfoModel
	{
		public Guid Id { get; set; }
		public Card DiscardPileTop { get; set; }
		public IEnumerable<UserModel> Players { get; set; } = new List<UserModel>();
	}
}