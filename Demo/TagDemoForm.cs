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
using System.Threading;

namespace Demo {

	/// <summary>
	/// Demo application demonstrating the <see cref="TagCloud"/> class.
	/// </summary>
	public partial class TagDemoForm : Form {

		List<TagItem> availableTags;
		Size oldClientSize;
		StringBuilder stats;
		Random rnd;
		CancellationTokenSource cts;

		public TagDemoForm() {
			rnd = new Random();
			stats = new StringBuilder();

			InitializeComponent();

			canvas.MouseMove += Canvas_MouseMove;

			InitTags();
		}

		private void Canvas_MouseMove(object sender, MouseEventArgs e) {
			// perform a hit test and display the tag under the mouse pointer as it moves
			TagItem hit = canvas.HitTest(e.Location);
			lblHit.Text = (hit != null) ? hit.Text : "(none)";
		}

		private void InitTags() {
			availableTags = new List<TagItem>();

			// load tags from the text file and assign random frequencies (normal distribution cubed)
			using (StreamReader sr = new StreamReader("wikipedia-list-of-colors.txt")) {
				while (!sr.EndOfStream) {
					string tag = sr.ReadLine().Trim().ToLower().Replace(' ', '-');
					availableTags.Add(new TagItem(tag, (int)Math.Pow(rnd.Next(1, 128), 3)));
				}
			}

			nudTags.Maximum = availableTags.Count;
			nudTags.Value = 64;
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);

			// initial layout
			oldClientSize = ClientSize;
			ApplySettings(true);
		}

		protected override void OnResizeEnd(EventArgs e) {
			base.OnResizeEnd(e);

			if (ClientSize != oldClientSize) {
				// update layout after window is resized
				ApplySettings(false);
				oldClientSize = ClientSize;
			}
		}

		private void ApplySettings(bool resetItems) {
			canvas.BeginUpdate();
			
			// apply user settings
			canvas.FontSizeGradient = (float)nudVariation.Value;

			if (resetItems) {
				// add 'n' tags from the list of available tags
				canvas.Clear();
				foreach (var item in availableTags.Shuffle(rnd).Take((int)nudTags.Value)) {
					canvas.Add(item);
				}
			}

			// perform layout of tag cloud
			canvas.EndUpdate();

			// keep stats
			stats.AppendFormat("{0}\t{1}", nudTags.Value, canvas.CycleCount);
			stats.AppendLine();
			txtCycles.Text = canvas.CycleCount.ToString();

			// repaint
			canvas.Invalidate();
		}

		private void nudVariation_ValueChanged(object sender, EventArgs e) {
			ApplySettings(false);
		}

		private void nudTags_ValueChanged(object sender, EventArgs e) {
			ApplySettings(true);
		}

		private void btnStats_Click(object sender, EventArgs e) {
			// copy CSV data to clipboard
			Clipboard.SetText(stats.ToString());
			System.Media.SystemSounds.Beep.Play();
		}

		private async void btnTest_Click(object sender, EventArgs e) {
			if (cts != null) {
				cts.Cancel();
			}
			else {
				cts = new CancellationTokenSource();

				// reset stats
				stats.Remove(0, stats.Length);

				// count down from n=512
				try {
					for (int i = 512; i >= 1; i--) {
						await Task.Delay(10, cts.Token);
						nudTags.Value = i;
					}
				}
				catch (OperationCanceledException) {
					//...
				}
				finally {
					cts = null;
				}
			}
		}
	}
}
