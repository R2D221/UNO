using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinal.Utils
{
	internal static class ConcurrentDictionaryExtensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> dictionary, TKey key, Func<TKey, TValue> valueFactory)
		{
			return dictionary.GetOrAdd(key, new Lazy<TValue>(() => valueFactory(key))).Value;
		}

		public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> dictionary, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			return dictionary.AddOrUpdate(key,
				new Lazy<TValue>(() => addValueFactory(key)),
				(k, oldValue) => new Lazy<TValue>(() => updateValueFactory(k, oldValue.Value)))
				.Value;
		}
	}
}