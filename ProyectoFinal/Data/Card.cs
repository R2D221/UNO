using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
	public class Card
	{
		[Key]
		public int Id { get; set; }

		public Color Color { get; set; }

		public Rank Rank { get; set; }

		public virtual ICollection<Hand> Hands { get; set; } = new HashSet<Hand>();

		public override string ToString()
		{
			return $"Color: {Color}, Rank: {Rank}";
		}
	}
}