using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TagClouds {

	/// <summary>
	/// Miscellaneous extension methods.
	/// </summary>
	public static class ExtensionMethods {

		/// <summary>
		/// Returns a sequence containing the same elements as this sequence, but in a random order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="rng"></param>
		/// <returns></returns>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) {
			T[] elements = source.ToArray();
			for (int i = elements.Length - 1; i >= 0; i--) {
				int swapIndex = rng.Next(i + 1);
				yield return elements[swapIndex];
				elements[swapIndex] = elements[i];
			}
		}

		/// <summary>
		/// Returns a new point at the specified distance and angle from this point.
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="angle"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF GetRadialPoint(this PointF origin, float angle, float radius) {
			double radians = angle * (Math.PI / 180.0);

			double x = Math.Sin(radians) * radius;
			double y = Math.Cos(radians) * radius;

			return new PointF((float)(origin.X + x), (float)(origin.Y - y));
		}
	}
}
