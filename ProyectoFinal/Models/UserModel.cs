using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
	public class UserModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public byte[] Photo { get; set; }
	}
}