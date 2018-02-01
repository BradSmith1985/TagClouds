using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using TagClouds;

namespace Demo {

	/// <summary>
	/// Control based on the <see cref="TagCloud"/> component.
	/// </summary>
	[Description("Control based on the TagCloud component.")]
	public class TagCloudControl : Panel {

		Container components;
		TagCloud tagCloud;
		BindingSource items;
		int freezeCount;

		/// <summary>
		/// Gets or sets the gradient of the function that determines the font 
		/// size for each tag.
		/// </summary>
		[Description("The gradient of the function that determines the font size for each tag.")]
		[DefaultValue(2.5f)]
		public float FontSizeGradient {
			get {
				return tagCloud.FontSizeGradient;
			}
			set {
				if (tagCloud.FontSizeGradient != value) {
					tagCloud.FontSizeGradient = value;
					Arrange();
				}
			}
		}
		/// <summary>
		/// Gets a value that gives a reasonably good metric for determining 
		/// the performance of the layout algorithm (big-O notation).
		/// </summary>
		[Browsable(false)]
		public int CycleCount {
			get {
				return tagCloud.CycleCount;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the layout behaviour is random or deterministic.
		/// </summary>
		[Description("Value indicating whether the layout behaviour is random or deterministic.")]
		[DefaultValue(true)]
		public bool IsDeterministic {
			get {
				return tagCloud.IsDeterministic;
			}
			set {
				if (tagCloud.IsDeterministic != value) {
					tagCloud.IsDeterministic = value;
					Arrange();
				}
			}
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="TagCloudControl"/> class, using default values.
		/// </summary>
		public TagCloudControl() {
			components = new Container();

			tagCloud = new TagCloud();
			components.Add(tagCloud);

			items = new BindingSource();
			items.DataSource = tagCloud.Items;
			items.ListChanged += Items_ListChanged;
			components.Add(items);

			// useful for resizable control
			IsDeterministic = true;

			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		protected override void OnFontChanged(EventArgs e) {
			base.OnFontChanged(e);

			// apply font settings
			tagCloud.FontFamily = Font.FontFamily.Name;
			tagCloud.FontStyle = Font.Style;
			tagCloud.TextColor = ForeColor;
			Arrange();
		}

		/// <summary>
		/// Returns the tag at the specified location.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public TagItem HitTest(Point location) {
			return tagCloud.HitTest(location, CalcBounds());
		}

		/// <summary>
		/// Overloaded. Adds the specified tag.
		/// </summary>
		/// <param name="item"></param>
		public void Add(TagItem item) {
			items.Add(item);
		}

		/// <summary>
		/// Overloaded. Adds the specified tag.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="frequency"></param>
		public void Add(string tag, int frequency) {
			items.Add(new TagItem(tag, frequency));
		}

		/// <summary>
		/// Clears all tags.
		/// </summary>
		public void Clear() {
			items.Clear();
		}

		/// <summary>
		/// Removes the specified tag. 
		/// If multiple tags have the same text, all will be removed.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool Remove(string tag) {
			bool any = false;

			foreach (var item in tagCloud.Items.Where(x => x.Text.Equals(tag)).ToList()) {
				items.Remove(item);
				any = true;
			}

			return any;
		}

		/// <summary>
		/// Suspends automatic updating of the tag cloud.
		/// </summary>
		public void BeginUpdate() {
			freezeCount++;
		}

		/// <summary>
		/// Resumes automatic updating of the tag cloud.
		/// </summary>
		public void EndUpdate() {
			freezeCount--;
			if (freezeCount < 0) freezeCount = 0;
			Arrange();
		}

		private void Items_ListChanged(object sender, ListChangedEventArgs e) {
			Arrange();
		}

		private void Arrange() {
			if (freezeCount > 0) return;

			Size sz = ClientSize;
			tagCloud.PreferredAspectRatio = (sz.Height != 0) ? ((float)sz.Width / (float)sz.Height) : 1f;
			tagCloud.Arrange();
			Invalidate();
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if (tagCloud.Items.Any() && !tagCloud.Bounds.IsEmpty) {
				tagCloud.Draw(e.Graphics, CalcBounds());
			}
		}

		private Rectangle CalcBounds() {
			Rectangle bounds = ClientRectangle;

			bounds.X += Padding.Left;
			bounds.Y += Padding.Top;
			bounds.Width -= Padding.Horizontal;
			bounds.Height -= Padding.Vertical;

			return bounds;
		}
	}
}
