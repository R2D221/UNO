using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
	public class EditProfileModel
	{
		[Required]
		public string Name { get; set; }

		public HttpPostedFileBase Photo { get; set; }
	}
}