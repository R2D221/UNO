using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Models
{
	public partial class ApplicationDbContext
	{
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Hand> Hands { get; set; }
		public DbSet<Card> Cards { get; set; }
	}
}