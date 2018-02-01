# TagClouds
A library for creating, arranging and rendering tag clouds.

Related article: https://www.brad-smith.info/blog/archives/931

## Example Usage
```csharp
TagCloud cloud = new TagCloud();

// add some tags
cloud.Items.Add(new TagItem("orange", 2));
cloud.Items.Add(new TagItem("red", 4));
cloud.Items.Add(new TagItem("green", 12));
cloud.Items.Add(new TagItem("pink", 96));
cloud.Items.Add(new TagItem("black", 1));
cloud.Items.Add(new TagItem("brown", 50));
cloud.Items.Add(new TagItem("yellow", 45));
cloud.Items.Add(new TagItem("purple", 32));
cloud.Items.Add(new TagItem("gold", 8));
cloud.Items.Add(new TagItem("silver", 7));

// apply layout
cloud.Arrange();

using (Bitmap bmp = new Bitmap(512, 512)) {
   // render to bitmap
   cloud.DrawToBitmap(bmp);

   // save bitmap
   bmp.Save("test.png", ImageFormat.Png);
}
