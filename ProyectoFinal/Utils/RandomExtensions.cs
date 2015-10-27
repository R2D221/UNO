using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ProyectoFinal.Utils
{
	//Implementation based on http://referencesource.microsoft.com/#mscorlib/system/random.cs
	public static class RandomExtensions
	{
		private static int InternalSample(this RandomNumberGenerator rng)
		{
			var bytes = new byte[sizeof(int)];
			rng.GetBytes(bytes);
			return BitConverter.ToInt32(bytes, 0) & ~int.MinValue;
		}
		private static double Sample(this RandomNumberGenerator rng)
		{
			return (rng.InternalSample() * (1.0 / Int32.MaxValue));
		}
		private static double GetSampleForLargeRange(this RandomNumberGenerator rng)
		{
			int result = rng.InternalSample();
			bool negative = (rng.InternalSample() % 2 == 0) ? true : false;
			if (negative)
			{
				result = -result;
			}
			double d = result;
			d += (Int32.MaxValue - 1);
			d /= 2 * (uint)Int32.MaxValue - 1;
			return d;
		}

		public static int Next(this RandomNumberGenerator rng, int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException("minValue");
			}

			long range = (long)maxValue - minValue;
			if (range <= (long)Int32.MaxValue)
			{
				return ((int)(rng.Sample() * range) + minValue);
			}
			else
			{
				return (int)((long)(rng.GetSampleForLargeRange() * range) + minValue);
			}
		}

		public static int Next(this RandomNumberGenerator rng, int maxValue)
		{
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException("maxValue");
			}
			return (int)(rng.Sample() * maxValue);
		}
	}
}