using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models
{
	public class Session
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		public virtual ICollection<CardInPile> Deck { get; set; } = new HashSet<CardInPile>();

		[ForeignKey(nameof(DiscardPileTopId))]
		public virtual Card DiscardPileTop { get; set; }
		[Required]
		public int DiscardPileTopId { get; set; }

		public Direction Direction { get; set; }

		public Color Color { get; set; }

		public DateTime LastPlayed { get; set; }

		[ForeignKey(nameof(HandId1))]
		public virtual Hand Hand1 { get; set; }
		public Guid? HandId1 { get; set; }

		[ForeignKey(nameof(HandId2))]
		public virtual Hand Hand2 { get; set; }
		public Guid? HandId2 { get; set; }

		[ForeignKey(nameof(HandId3))]
		public virtual Hand Hand3 { get; set; }
		public Guid? HandId3 { get; set; }

		[ForeignKey(nameof(HandId4))]
		public virtual Hand Hand4 { get; set; }
		public Guid? HandId4 { get; set; }
	}
}