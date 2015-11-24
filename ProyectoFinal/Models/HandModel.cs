using System;
using System.Collections.Generic;

namespace ProyectoFinal.Models
{
	public class HandModel
	{
		public Guid Id { get; set; }

		public UserModel User { get; set; }

		public virtual List<Card> Cards { get; set; } = new List<Card>();

		public bool IsTheirTurn { get; set; }
	
		public bool? IsSafe { get; set; }
	}
}