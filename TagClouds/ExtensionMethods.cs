using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TagClouds {

	public static class ExtensionMethods {

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) {
			T[] elements = source.ToArray();
			for (int i = elements.Length - 1; i >= 0; i--) {
				int swapIndex = rng.Next(i + 1);
				yield return elements[swapIndex];
				elements[swapIndex] = elements[i];
			}
		}

		public static PointF GetRadialPoint(this PointF origin, float angle, float radius) {
			double radians = angle * (Math.PI / 180.0);

			double x = Math.Sin(radians) * radius;
			double y = Math.Cos(radians) * radius;

			return new PointF((float)(origin.X + x), (float)(origin.Y - y));
		}

		public static Rectangle ToRectangle(this RectangleF rect) {
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
	}
}
