using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
	public partial class ApplicationUser
	{
		[Required]
		public string Name { get; set; }

		public byte[] Photo { get; set; }

		public virtual ICollection<Hand> Hands { get; set; } = new HashSet<Hand>();
	}
}