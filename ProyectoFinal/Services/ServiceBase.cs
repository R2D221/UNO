using System;
using System.Data.Entity;

namespace ProyectoFinal.Services
{
	public abstract class ServiceBase<TContext> : IDisposable where TContext : DbContext, new()
	{
		protected TContext db;

		public ServiceBase(TContext db)
		{
			this.db = db ?? new TContext();
		}

		public void Dispose()
		{
			db.Dispose();
		}
	}
}