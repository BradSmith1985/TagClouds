using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagClouds {

	public class TagItem {

		public string Text {
			get;
			private set;
		}

		public int Frequency {
			get;
			private set;
		}

		public TagItem(string tag, int frequency) {
			if (tag == null) throw new ArgumentNullException("tag");
			if (frequency <= 0) throw new ArgumentException("Frequency must be greater than zero.", "frequency");

			Text = tag;
			Frequency = frequency;
		}
	}
}
