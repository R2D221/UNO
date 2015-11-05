using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ProyectoFinal.Models
{
	public class GameUpdateModel
	{
		public Direction Direction { get; set; }
		[JsonIgnore]
		public Card Card { get; set; }
		public string CardHtml { get; set; }
		public string PreviousUserId { get; set; }
		public string NextUserId { get; set; }
	}
}