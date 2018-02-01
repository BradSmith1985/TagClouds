using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace TagClouds {

	/// <summary>
	/// Represents a tag cloud and the logic required to perform layout and rendering.
	/// </summary>
	public class TagCloud : Component {

		/// <summary>
		/// The base font size used for determining other font sizes.
		/// </summary>
		const float BASE_FONT_SIZE = 8.25f;
		/// <summary>
		/// Format flags used when measuring text (GDI).
		/// </summary>
		const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;
		/// <summary>
		/// Format flags used when rendering text (GDI+).
		/// </summary>
		static readonly StringFormat STRING_FORMAT = new StringFormat(StringFormat.GenericTypographic) {
			HotkeyPrefix = HotkeyPrefix.None
		};

		/// <summary>
		/// Used to temporarily store the rendering properties associated with a <see cref="TagItem"/>.
		/// </summary>
		private class TagRenderInfo {

			/// <summary>
			/// Gets or sets the tag.
			/// </summary>
			public TagItem Item { get; set; }
			/// <summary>
			/// Gets or sets the bounds of the tag.
			/// </summary>
			public RectangleF Bounds { get; set; }
			/// <summary>
			/// Gets or sets the font size for the tag.
			/// </summary>
			public float FontSize { get; set; }
		}

		string _fontFamily;
		float _preferredAspectRatio;
		LinkedList<TagRenderInfo> _renderItems;

		/// <summary>
		/// Gets or sets the ratio of width to height which should be 
		/// considered optimal by the layout algorithm.
		/// </summary>
		/// <remarks>
		/// The actual aspect ratio will be somewhere between this value and 
		/// 1:1 (square).
		/// </remarks>
		[DefaultValue(1f)]
		public float PreferredAspectRatio {
			get {
				return _preferredAspectRatio;
			}
			set {
				if (_preferredAspectRatio != value) {
					if (value == 0) throw new ArgumentException("Aspect ratio cannot be zero.", "value");
					_preferredAspectRatio = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the name of the font used to render the tag cloud.
		/// </summary>
		public string FontFamily {
			get { return _fontFamily; }
			set {
				if (_fontFamily != value) {
					if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Font name cannot be null or empty.", "value");
					_fontFamily = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the style of the font used to render the tag cloud.
		/// </summary>
		[DefaultValue(FontStyle.Regular)]
		public FontStyle FontStyle { get; set; }
		/// <summary>
		/// Gets or sets the colour of the text.
		/// </summary>
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor { get; set; }
		/// <summary>
		/// Gets or sets the gradient of the function that determines the font 
		/// size for each tag.
		/// </summary>
		/// <remarks>
		/// For example: A value of 0 would result in the same font size for 
		/// every tag. A value of 1 would cause the tag with the highest 
		/// frequency to be twice the size of the tag with the lowest 
		/// frequency.
		/// </remarks>
		[DefaultValue(2.5f)]
		public float FontSizeGradient { get; set; }
		/// <summary>
		/// Collection containing the items that make up the tag cloud.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ICollection<TagItem> Items { get; private set; }
		/// <summary>
		/// Gets the calculated bounds of the tag cloud after the last call to 
		/// the <see cref="Arrange"/> method.
		/// </summary>
		[Browsable(false)]
		public RectangleF Bounds { get; private set; }
		/// <summary>
		/// Gets a value that gives a reasonably good metric for determining 
		/// the performance of the layout algorithm (big-O notation).
		/// </summary>
		[Browsable(false)]
		public int CycleCount { get; private set; }

		/// <summary>
		/// Initialises a new instance of the <see cref="TagCloud"/> class using default values.
		/// </summary>
		public TagCloud() {
			Items = new List<TagItem>();
			_renderItems = new LinkedList<TagRenderInfo>();
			FontSizeGradient = 2.5f;
			FontFamily = SystemFonts.DefaultFont.FontFamily.Name;
			FontStyle = FontStyle.Regular;
			TextColor = Color.Black;
			PreferredAspectRatio = 1f;
		}

		/// <summary>
		/// Arranges the tags to form the distinctive tag cloud layout.
		/// </summary>
		public void Arrange() {
			RectangleF totalBounds = RectangleF.Empty;
			Random rnd = new Random();

			_renderItems.Clear();
			CycleCount = 0;

			// sort tags by frequency (highest first)
			var orderedItems = Items.OrderByDescending(x => x.Frequency);

			int maxFrequency = orderedItems.Select(x => x.Frequency).DefaultIfEmpty(0).First();
			int minFrequency = orderedItems.Select(x => x.Frequency).DefaultIfEmpty(0).Last();

			foreach (TagItem tag in orderedItems) {
				RectangleF bestBounds = RectangleF.Empty;
				int bestDist = 0;

				// calculate font size to use
				float scale = (maxFrequency != minFrequency) 
					? ((float)(tag.Frequency - minFrequency) / (float)(maxFrequency - minFrequency))
					: 0;

				float fontSize = BASE_FONT_SIZE + (BASE_FONT_SIZE * (FontSizeGradient * scale));

				// measure text and calculate bounds
				SizeF sz;
				using (Font font = new Font(FontFamily, fontSize, FontStyle)) {
					sz = TextRenderer.MeasureText(tag.Text, font, Size.Empty, TEXT_FORMAT_FLAGS);
				}

				SizeF offset = new SizeF(-(sz.Width / 2f), -(sz.Height / 2f));
				RectangleF tagBounds = new RectangleF(offset.ToPointF(), sz);

				// initialise rendering info with what we know so far
				TagRenderInfo info = new TagRenderInfo() { Item = tag, FontSize = fontSize };

				// try a random subset of the angles between 0 and 360 degrees
				foreach (int angle in Enumerable.Range(0, 360).Shuffle(rnd).Take(90)) {
					int tagDist = 0;

					while (true) {
						// measure outward from the origin
						PointF p = PointF.Empty.GetRadialPoint(angle, tagDist);
						tagBounds.Location = PointF.Add(p, offset);

						// check whether tag would overlap (collide) with previously-placed tags
						bool collision = false;
						foreach (var other in _renderItems) {
							CycleCount++;

							if (other.Bounds.IntersectsWith(tagBounds)) {
								collision = true;
								break;
							}
						}

						// once there are no collisions this location becomes a candidate angle
						if (!collision) break;

						// ...otherwise, increase distance from origin and try again
						tagDist += 5;

						// if we've already exceeded the most optimal distance, we can stop here
						if ((bestDist != 0) && (tagDist > bestDist)) break;
					}

					// determine whether this candidate angle produces the most optimal solution
					RectangleF tryBounds = RectangleF.Union(totalBounds, tagBounds);

					float tryArea = (tryBounds.Width * tryBounds.Height);
					float bestArea = (bestBounds.Width * bestBounds.Height);
					float tryAspectDiff = Math.Abs((tryBounds.Width / tryBounds.Height) - PreferredAspectRatio);
					float bestAspectDiff = Math.Abs((bestBounds.Width / bestBounds.Height) - PreferredAspectRatio);

					bool isBest = (bestBounds.IsEmpty || (tryArea < bestArea))
						&& (bestBounds.IsEmpty || (tryAspectDiff < bestAspectDiff))
						&& ((bestDist == 0) || (tagDist <= bestDist));

					if (isBest) {
						// this becomes the new most optimal solution (until/if a better one is found)
						bestBounds = tryBounds;
						bestDist = tagDist;
						info.Bounds = tagBounds;

						// if the total bounds did not increase at all, skip over the remaining angles
						if (bestBounds == totalBounds) break;
					}
				}

				// commit the current tag
				_renderItems.AddLast(info);
				totalBounds = bestBounds;
			}

			Bounds = totalBounds;
		}

		/// <summary>
		/// Returns the bounds of the tag cloud after scaling and centering within a target rectangle.
		/// </summary>
		/// <param name="preferredBounds"></param>
		/// <param name="sourceBounds"></param>
		/// <returns></returns>
		private static RectangleF CalcActualBounds(RectangleF preferredBounds, RectangleF sourceBounds) {
			double sourceRatio = (double)sourceBounds.Width / (double)sourceBounds.Height;
			double preferredRatio = (double)preferredBounds.Width / (double)preferredBounds.Height;

			float width = (sourceRatio > preferredRatio) ? preferredBounds.Width : (float)(preferredBounds.Width * (sourceRatio / preferredRatio));
			float height = (sourceRatio > preferredRatio) ? (float)(preferredBounds.Height * (preferredRatio / sourceRatio)) : preferredBounds.Height;

			return new RectangleF(
				preferredBounds.X + ((preferredBounds.Width - width) / 2),
				preferredBounds.Y + ((preferredBounds.Height - height) / 2),
				width, 
				height
			);
		}

		/// <summary>
		/// Renders the tag cloud using the specified <see cref="Graphics"/> surface.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		public void Draw(Graphics g, Rectangle bounds) {
			RectangleF actual = CalcActualBounds(bounds, Bounds);
			float scale = (Bounds.Width != 0) ? (actual.Width / Bounds.Width) : 1f;
			if (scale <= 0) return;

			g.SetClip(actual);
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.TranslateTransform(actual.X, actual.Y);
			g.ScaleTransform(scale, scale);
			g.TranslateTransform(-Bounds.X, -Bounds.Y);

			using (Brush b = new SolidBrush(TextColor)) {
				foreach (TagRenderInfo info in _renderItems) {
					using (Font font = new Font(FontFamily, info.FontSize, FontStyle)) {
						g.DrawString(info.Item.Text, font, b, info.Bounds.Location, STRING_FORMAT);
					}
				}
			}

			g.ResetTransform();
		}

		/// <summary>
		/// Renders the tag cloud onto the specified <see cref="Image"/>.
		/// </summary>
		/// <param name="image"></param>
		public void DrawToBitmap(Image image) {
			using (Graphics g = Graphics.FromImage(image)) {
				Draw(g, new Rectangle(Point.Empty, image.Size));
			}
		}

		/// <summary>
		/// Returns the tag at a particular location when the tag cloud is drawn within the specified bounds.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="bounds"></param>
		/// <returns></returns>
		public TagItem HitTest(Point location, Rectangle bounds) {
			RectangleF actual = CalcActualBounds(bounds, Bounds);
			float scale = (Bounds.Width != 0) ? (actual.Width / Bounds.Width) : 1f;
			if (scale <= 0) return null;

			foreach (TagRenderInfo info in _renderItems) {
				RectangleF tagBounds = info.Bounds;

				tagBounds.X = actual.X + scale * (tagBounds.X - Bounds.X);
				tagBounds.Y = actual.Y + scale * (tagBounds.Y - Bounds.Y);
				tagBounds.Width = tagBounds.Width * scale;
				tagBounds.Height = tagBounds.Height * scale;

				if (tagBounds.Contains(location)) return info.Item;
			}

			return null;
		}
	}
}