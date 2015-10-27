using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
	public class CreateGameViewModel
	{
		[EmailAddress, Required]
		public string Email2 { get; set; }

		[EmailAddress, Required]
		public string Email3 { get; set; }

		[EmailAddress, Required]
		public string Email4 { get; set; }
	}
}