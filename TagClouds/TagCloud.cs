using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TagClouds {

	public class TagCloud {

		const float BASE_FONT_SIZE = 8.25f;
		const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.NoPrefix;

		static readonly StringFormat STRING_FORMAT = new StringFormat(StringFormat.GenericTypographic) {
			HotkeyPrefix = HotkeyPrefix.None
		};

		private class TagRenderInfo {

			public TagItem Item { get; set; }
			public RectangleF Bounds { get; set; }
			public float FontSize { get; set; }
		}

		private string _fontFamily;
		private float _preferredAspectRatio;

		private List<TagRenderInfo> RenderItems { get; set; }

		/// <summary>
		/// Gets or sets the ratio of width to height which should be 
		/// considered optimal by the layout algorithm.
		/// </summary>
		/// <remarks>
		/// The actual aspect ratio will be somewhere between this value and 
		/// 1:1 (square).
		/// </remarks>
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
		public FontStyle FontStyle { get; set; }
		/// <summary>
		/// Gets or sets the colour of the text.
		/// </summary>
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
		public float FontSizeGradient { get; set; }
		/// <summary>
		/// Collection containing the items that make up the tag cloud.
		/// </summary>
		public ICollection<TagItem> Items { get; private set; }
		/// <summary>
		/// Gets the calculated bounds of the tag cloud after the last call to 
		/// the <see cref="Arrange"/> method.
		/// </summary>
		public RectangleF Bounds { get; private set; }

		public int CycleCount { get; private set; }

		public TagCloud() {
			Items = new List<TagItem>();
			RenderItems = new List<TagRenderInfo>();
			FontSizeGradient = 4f;
			FontFamily = SystemFonts.DefaultFont.FontFamily.Name;
			FontStyle = FontStyle.Regular;
			TextColor = Color.Black;
			PreferredAspectRatio = 1f;
		}

		public void Arrange() {
			CycleCount = 0;
			RenderItems.Clear();

			RectangleF totalBounds = RectangleF.Empty;
			Random rnd = new Random();

			int minFrequency = Items.Select(x => x.Frequency).DefaultIfEmpty(0).Min();
			int maxFrequency = Items.Select(x => x.Frequency).DefaultIfEmpty(0).Max();

			foreach (TagItem tag in Items.OrderByDescending(x => x.Frequency)) {
				RectangleF bestBounds = RectangleF.Empty;
				int bestDist = 0;

				float scale = (maxFrequency != minFrequency) ? ((float)(tag.Frequency - minFrequency) / (float)(maxFrequency - minFrequency)) : 0;
				float fontSize = BASE_FONT_SIZE + (BASE_FONT_SIZE * (FontSizeGradient * scale));

				SizeF sz;
				using (Font font = new Font(FontFamily, fontSize, FontStyle)) {
					sz = TextRenderer.MeasureText(tag.Text, font, Size.Empty, TEXT_FORMAT_FLAGS);
				}

				TagRenderInfo info = new TagRenderInfo() { Item = tag, FontSize = fontSize };

				foreach (int angle in Enumerable.Range(0, 360).Shuffle(rnd).Take(90)) {
					RectangleF tagBounds = new RectangleF(PointF.Empty, sz);
					int tagDist = 0;

					while (true) {
						PointF p = PointF.Empty.GetRadialPoint(angle, tagDist);
						tagBounds.Location = new PointF(p.X - (sz.Width / 2f), p.Y - (sz.Height / 2f));

						bool collision = false;
						foreach (var other in RenderItems) {
							CycleCount++;

							if (other.Bounds.IntersectsWith(tagBounds)) {
								collision = true;
								break;
							}
						}

						if (collision)
							tagDist += 5;
						else
							break;
					}

					RectangleF tryBounds = RectangleF.Union(totalBounds, tagBounds);
					bool isBest = (bestBounds.IsEmpty || ((tryBounds.Width * tryBounds.Height) < (bestBounds.Width * bestBounds.Height)))
						&& (bestBounds.IsEmpty || (Math.Abs((tryBounds.Width / tryBounds.Height) - PreferredAspectRatio) < Math.Abs((bestBounds.Width / bestBounds.Height) - PreferredAspectRatio)))
						&& ((bestDist == 0) || (tagDist < bestDist));

					if (isBest) {
						bestBounds = tryBounds;
						bestDist = tagDist;
						info.Bounds = tagBounds;						
					}
				}

				RenderItems.Add(info);
				totalBounds = bestBounds;
			}

			Bounds = totalBounds;
		}

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
				foreach (TagRenderInfo info in RenderItems) {
					using (Font font = new Font(FontFamily, info.FontSize, FontStyle)) {
						g.DrawString(info.Item.Text, font, b, info.Bounds.Location, STRING_FORMAT);
					}
				}
			}

			g.ResetTransform();
		}
	}
}