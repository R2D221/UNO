using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ProyectoFinal.Utils
{
	public static class ListExtensions
	{
		private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		public static void Shuffle<T>(this IList<T> list)
		{
			for (var i = 0; i < list.Count; i++)
				list.Swap(i, rng.Next(i, list.Count));
		}

		private static void Swap<T>(this IList<T> list, int i, int j)
		{
			var temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
}