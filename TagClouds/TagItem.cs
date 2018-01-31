using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagClouds {

	/// <summary>
	/// Represents an item in a tag cloud.
	/// </summary>
	public class TagItem {

		/// <summary>
		/// Gets the text of the tag.
		/// </summary>
		public string Text {
			get;
			private set;
		}
		/// <summary>
		/// Gets the frequency of the tag.
		/// </summary>
		public int Frequency {
			get;
			private set;
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="TagItem"/> class using the specified values.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="frequency"></param>
		public TagItem(string tag, int frequency) {
			if (tag == null) throw new ArgumentNullException("tag");
			if (frequency <= 0) throw new ArgumentException("Frequency must be greater than zero.", "frequency");

			Text = tag;
			Frequency = frequency;
		}
	}
}
