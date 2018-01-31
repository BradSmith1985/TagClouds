using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagClouds;

namespace Demo {

	public partial class TagDemoForm : Form {

		List<TagItem> availableTags;
		TagCloud tagCloud;
		Size oldClientSize;

		public TagDemoForm() {
			tagCloud = new TagCloud();
			tagCloud.FontFamily = "Cambria";

			InitializeComponent();

			DoubleBuffered = true;
			canvas.Paint += Canvas_Paint;

			availableTags = new List<TagItem>();

			availableTags.Add(new TagItem("orange", 105));
			availableTags.Add(new TagItem("black", 34));
			availableTags.Add(new TagItem("magenta", 12));
			availableTags.Add(new TagItem("blue", 2));
			availableTags.Add(new TagItem("purple", 3));
			availableTags.Add(new TagItem("green", 16));
			availableTags.Add(new TagItem("white", 34));
			availableTags.Add(new TagItem("silver", 78));
			availableTags.Add(new TagItem("gold", 63));
			availableTags.Add(new TagItem("yellow", 23));
			availableTags.Add(new TagItem("crimson", 1));
			availableTags.Add(new TagItem("violet", 2));
			availableTags.Add(new TagItem("indigo", 7));
			availableTags.Add(new TagItem("cyan", 20));
			availableTags.Add(new TagItem("red", 9));
			availableTags.Add(new TagItem("brown", 6));
			availableTags.Add(new TagItem("beige", 18));
			availableTags.Add(new TagItem("pink", 45));
			availableTags.Add(new TagItem("grey", 30));
			availableTags.Add(new TagItem("maroon", 41));
			availableTags.Add(new TagItem("peach", 22));
			availableTags.Add(new TagItem("turquoise", 1));
			availableTags.Add(new TagItem("charcoal", 1));
			availableTags.Add(new TagItem("navy", 1));
			availableTags.Add(new TagItem("aqua", 87));
			availableTags.Add(new TagItem("lime", 3));

			nudTags.Maximum = nudTags.Value = availableTags.Count;
		}

		private void Canvas_Paint(object sender, PaintEventArgs e) {
			Rectangle bounds = canvas.ClientRectangle;
			bounds.Inflate(-20, -20);

			tagCloud.Draw(e.Graphics, bounds);
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);

			oldClientSize = ClientSize;
			ApplySettings();
		}

		protected override void OnResizeEnd(EventArgs e) {
			base.OnResizeEnd(e);

			if (ClientSize != oldClientSize) {
				ApplySettings();
				oldClientSize = ClientSize;
			}
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);

			canvas.Invalidate();
		}

		private void ApplySettings() {
			tagCloud.Items.Clear();
			foreach (var item in availableTags.Take((int)nudTags.Value)) {
				tagCloud.Items.Add(item);
			}

			tagCloud.FontSizeGradient = (float)nudVariation.Value;
			if (ClientSize.Height != 0) tagCloud.PreferredAspectRatio = (float)ClientSize.Width / (float)ClientSize.Height;
			tagCloud.Arrange();
			txtCycles.Text = tagCloud.CycleCount.ToString();
			canvas.Invalidate();
		}

		private void nudVariation_ValueChanged(object sender, EventArgs e) {
			ApplySettings();
		}

		private void nudTags_ValueChanged(object sender, EventArgs e) {
			ApplySettings();
		}
	}
}
