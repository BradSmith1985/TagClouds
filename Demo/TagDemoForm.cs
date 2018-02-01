using System;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TagClouds;

namespace Demo {

	public partial class TagDemoForm : Form {

		List<TagItem> availableTags;
		TagCloud tagCloud;
		Size oldClientSize;
		StringBuilder stats;
		Random rnd;

		public TagDemoForm() {
			tagCloud = new TagCloud();
			tagCloud.FontFamily = "Cambria";

			rnd = new Random();
			stats = new StringBuilder();

			InitializeComponent();

			DoubleBuffered = true;
			canvas.Paint += Canvas_Paint;
			canvas.MouseMove += Canvas_MouseMove;

			InitTags();
		}

		private void Canvas_MouseMove(object sender, MouseEventArgs e) {
			Rectangle bounds = canvas.ClientRectangle;
			bounds.Inflate(-20, -20);

			TagItem hit = tagCloud.HitTest(e.Location, bounds);
			lblHit.Text = (hit != null) ? hit.Text : "(none)";
		}

		private void InitTags() {
			availableTags = new List<TagItem>();

			using (StreamReader sr = new StreamReader("wikipedia-list-of-colors.txt")) {
				while (!sr.EndOfStream) {
					string tag = sr.ReadLine().Trim().ToLower().Replace(' ', '-');
					availableTags.Add(new TagItem(tag, (int)Math.Pow(rnd.Next(1, 128), 3)));
				}
			}

			nudTags.Maximum = availableTags.Count;
			nudTags.Value = 64;
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
			foreach (var item in availableTags.Shuffle(rnd).Take((int)nudTags.Value)) {
				tagCloud.Items.Add(item);
			}

			tagCloud.FontSizeGradient = (float)nudVariation.Value;
			if (ClientSize.Height != 0) tagCloud.PreferredAspectRatio = (float)ClientSize.Width / (float)ClientSize.Height;
			tagCloud.Arrange();

			stats.AppendFormat("{0}\t{1}", nudTags.Value, tagCloud.CycleCount);
			stats.AppendLine();

			txtCycles.Text = tagCloud.CycleCount.ToString();
			canvas.Invalidate();
		}

		private void nudVariation_ValueChanged(object sender, EventArgs e) {
			ApplySettings();
		}

		private void nudTags_ValueChanged(object sender, EventArgs e) {
			ApplySettings();
		}

		private void btnStats_Click(object sender, EventArgs e) {
			Clipboard.SetText(stats.ToString());
			System.Media.SystemSounds.Beep.Play();
		}

		private async void btnTest_Click(object sender, EventArgs e) {
			// reset stats
			stats.Remove(0, stats.Length);
			for (int i = 512; i >= 1; i--) {
				await Task.Delay(10);
				nudTags.Value = i;
			}
		}
	}
}
