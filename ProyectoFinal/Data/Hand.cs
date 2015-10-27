using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models
{
	public class Hand
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }
		[Required]
		public string UserId { get; set; }

		[ForeignKey(nameof(SessionId))]
		public virtual Session Session { get; set; }
		[Required]
		public Guid SessionId { get; set; }

		public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();

		public bool IsTheirTurn { get; set; }
	}
}