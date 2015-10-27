using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models
{
	public class CardInPile
	{
		[ForeignKey(nameof(SessionId))]
		public virtual Session Session { get; set; }
		[Key, Column(Order = 0), Required]
		public Guid SessionId { get; set; }

		[ForeignKey(nameof(CardId))]
		public virtual Card Card { get; set; }
		[Key, Column(Order = 1), Required]
		public int CardId { get; set; }

		public bool IsDiscarded { get; set; }

		public int Order { get; set; }
	}
}