using System;
using System.Linq;
using System.Reflection;

using SkiaSharp;


namespace SkiaTextRendering.Views;

internal class FontInfo : IDisposable
{
	public string FontFamily { get; }
	public int FontSize { get; }

	public SKTypeface Typeface { get; }
	public SKFont Font { get; }
	public float CharWidth { get; }
	public float LineHeight { get; }
	public float Ascent { get; }
	private bool isSystemFont = false;

	public FontInfo(string fontFamily, int fontSize)
	{
		Typeface = GetTypeface(fontFamily);
		FontFamily = Typeface.FamilyName;
		FontSize = fontSize;

		Font = Typeface.ToFont(fontSize);
		Font.Subpixel = true;


		// check the maximum sizes of those chars:
		// a Powerline Arrow, Unicode Full Block and Ö and y as fallback
		var bigChars = "\uE0B0\u2588Öy";
		var cellSize = new SKRect();
		for (int i = 0; i < bigChars.Length; i++)
		{
			var s = bigChars.AsSpan(i, 1);
			if (Font.ContainsGlyphs(s))    // if the font does not contain the glyph, then skip it
			{
				Font.MeasureText(s, out var rect);
				cellSize.Union(rect);
			}
		}

		CharWidth = Font.MeasureText("W");     // cellSize.Width is too big
		LineHeight = cellSize.Height;
		Ascent = cellSize.Top;
	}

	protected virtual SKTypeface GetTypeface(string fontName)
		=> FindEmbeddedFont(fontName)
		?? FindSystemFont(fontName)
		?? FindEmbeddedFont("Cascadia Code PL")
		?? FindSystemFont("Courier New")
		?? FindSystemFont("Courier")
		?? SKTypeface.Default;

	private SKTypeface? FindEmbeddedFont(string fontName)
	{
		// this is for EmbeddedResources:
		var assembly = Assembly.GetExecutingAssembly();
		var fonts = assembly.GetManifestResourceNames()
			.Where(n => n.EndsWith(".ttf") || n.EndsWith(".otf"))
			.ToArray();

		//// load AvaloniaResource:
		//// cannot use AvaloniaLocator and IAssetLoader
		//var assetLoader = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();
		//var fontAssets = FontFamilyLoader.LoadFontAssets(new Uri($"avares://{assembly.GetName()}/Assets/Fonts"));

		//foreach (var fontAsset in fontAssets)
		//{
		//	var stream = assetLoader.Open(fontAsset);

		//	if (fontManager.TryCreateGlyphTypeface(stream, FontSimulations.None, out var glyphTypeface))
		//	{
		//		AddGlyphTypeface(glyphTypeface);
		//	}
		//}


		// search for a font which contains the name
		var resource = fonts.FirstOrDefault(res => res.Contains(fontName));
		if (resource is not null)
		{
			var stream = assembly.GetManifestResourceStream(resource);
			if (stream != null)
				return SKTypeface.FromStream(stream);
		}

		return null;
	}

	private SKTypeface? FindSystemFont(string fontName)
	{
		var tf = SKTypeface.FromFamilyName(fontName);
		if ((tf?.FamilyName) != fontName)
			return null;
		isSystemFont = true;
		return tf;
	}

	#region IDisposable

	private bool isDisposed;

	protected virtual void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			if (disposing)
			{
				Font?.Dispose();
				Typeface?.Dispose();
			}

			isDisposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion IDisposable
}
