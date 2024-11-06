using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Threading;

using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace SkiaTextRendering.Views;

public class TextInfo : Control
{
	#region Bindable Properties


	/// <summary>
	/// Text DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, string> TextProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, string>(nameof(Text),
			o => o.Text,
			(o, v) => o.Text = v);

	private string _Text = "Text";
	/// <summary>
	/// Gets or sets the Text property. This DirectProperty 
	/// indicates the text.
	/// </summary>
	public string Text
	{
		get => _Text;
		set => SetAndRaise(TextProperty, ref _Text, value);
	}


	/// <summary>
	/// FontFamily DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, string> FontFamilyProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, string>(nameof(FontFamily),
			o => o.FontFamily,
			(o, v) => o.FontFamily = v);

	private string _FontFamily = default;
	/// <summary>
	/// Gets or sets the FontFamily property. This DirectProperty 
	/// indicates the font family.
	/// </summary>
	public string FontFamily
	{
		get => _FontFamily;
		set => SetAndRaise(FontFamilyProperty, ref _FontFamily, value);
	}



	/// <summary>
	/// FontSize DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, int> FontSizeProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, int>(nameof(FontSize),
			o => o.FontSize,
			(o, v) => o.FontSize = v);

	private int _FontSize = 30;
	/// <summary>
	/// Gets or sets the FontSize property. This DirectProperty 
	/// indicates the font size.
	/// </summary>
	public int FontSize
	{
		get => _FontSize;
		set => SetAndRaise(FontSizeProperty, ref _FontSize, value);
	}


	/// <summary>
	/// ShowTopLine DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, bool> ShowTopLineProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, bool>(nameof(ShowTopLine),
			o => o.ShowTopLine,
			(o, v) => o.ShowTopLine = v);

	private bool _ShowTopLine = false;
	/// <summary>
	/// Gets or sets the ShowTopLine property. This DirectProperty 
	/// indicates whether to show the top line.
	/// </summary>
	public bool ShowTopLine
	{
		get => _ShowTopLine;
		set => SetAndRaise(ShowTopLineProperty, ref _ShowTopLine, value);
	}


	/// <summary>
	/// ShowMeasuredAscentLine DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, bool> ShowMeasuredAscentLineProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, bool>(nameof(ShowMeasuredAscentLine),
			o => o.ShowMeasuredAscentLine,
			(o, v) => o.ShowMeasuredAscentLine = v);

	private bool _ShowMeasuredAscentLine = false;
	/// <summary>
	/// Gets or sets the ShowMeasuredAscentLine property. This DirectProperty 
	/// indicates whether to show the measured ascent line.
	/// </summary>
	public bool ShowMeasuredAscentLine
	{
		get => _ShowMeasuredAscentLine;
		set => SetAndRaise(ShowMeasuredAscentLineProperty, ref _ShowMeasuredAscentLine, value);
	}


	/// <summary>
	/// ShowAscentLine DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, bool> ShowAscentLineProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, bool>(nameof(ShowAscentLine),
			o => o.ShowAscentLine,
			(o, v) => o.ShowAscentLine = v);

	private bool _ShowAscentLine = false;
	/// <summary>
	/// Gets or sets the ShowAscentLine property. This DirectProperty 
	/// indicates whether to show the ascent line.
	/// </summary>
	public bool ShowAscentLine
	{
		get => _ShowAscentLine;
		set => SetAndRaise(ShowAscentLineProperty, ref _ShowAscentLine, value);
	}


	/// <summary>
	/// ShowBaseLine DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, bool> ShowBaseLineProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, bool>(nameof(ShowBaseLine),
			o => o.ShowBaseLine,
			(o, v) => o.ShowBaseLine = v);

	private bool _ShowBaseLine = false;
	/// <summary>
	/// Gets or sets the ShowBaseLine property. This DirectProperty 
	/// indicates whether to show the base line.
	/// </summary>
	public bool ShowBaseLine
	{
		get => _ShowBaseLine;
		set => SetAndRaise(ShowBaseLineProperty, ref _ShowBaseLine, value);
	}


	/// <summary>
	/// ShowDescentLine DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, bool> ShowDescentLineProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, bool>(nameof(ShowDescentLine),
			o => o.ShowDescentLine,
			(o, v) => o.ShowDescentLine = v);

	private bool _ShowDescentLine = false;
	/// <summary>
	/// Gets or sets the ShowDescentLine property. This DirectProperty 
	/// indicates whether to show the descent line.
	/// </summary>
	public bool ShowDescentLine
	{
		get => _ShowDescentLine;
		set => SetAndRaise(ShowDescentLineProperty, ref _ShowDescentLine, value);
	}


	/// <summary>
	/// ShowBottomLine DirectProperty definition
	/// </summary>
	public static readonly DirectProperty<TextInfo, bool> ShowBottomLineProperty =
		AvaloniaProperty.RegisterDirect<TextInfo, bool>(nameof(ShowBottomLine),
			o => o.ShowBottomLine,
			(o, v) => o.ShowBottomLine = v);

	private bool _ShowBottomLine = false;
	/// <summary>
	/// Gets or sets the ShowBottomLine property. This DirectProperty 
	/// indicates whether to show the bottom line.
	/// </summary>
	public bool ShowBottomLine
	{
		get => _ShowBottomLine;
		set => SetAndRaise(ShowBottomLineProperty, ref _ShowBottomLine, value);
	}


	#endregion Bindable Properties

	private readonly SKColor topColor = SKColors.LightCoral;
	private readonly SKColor measuredAscentColor = SKColors.LightBlue;
	private readonly SKColor ascentColor = SKColors.LightCyan;
	private readonly SKColor baselineColor = SKColors.Red;
	private readonly SKColor descentColor = SKColors.LightPink;
	private readonly SKColor bottomColor = SKColors.LightSkyBlue;

	private readonly SKColor[] backgroundColors = new[] { SKColors.DarkBlue, SKColors.DarkMagenta, SKColors.DarkOrange, SKColors.DarkRed, SKColors.DarkGray, SKColors.DarkSlateGray };
	private int nextBackgroundColorIndex = 0;
	private Dictionary<string, SKColor> fontColors = new();

	private SKPaint background = new SKPaint() { Color = SKColors.Black };
	private SKPaint foreground = new SKPaint() { Color = SKColors.LimeGreen, IsAntialias = true, IsStroke = false };
	private FontInfo? fontInfo;
	private SKFont headerFont = SKTypeface.Default.ToFont(20);
	private SKFont captionFont = SKTypeface.Default.ToFont(14);


    public TextInfo()
    {
		Extensions.CanvasExtensionsConcurrentDict.SetShaperCacheDuration(30_000);
		Extensions.CanvasExtensionsRuntimeCache.SetShaperCacheDuration(30_000);
	}

    private SKColor GetFontColor(SKFont font)
	{
		if(fontColors.TryGetValue(font.Typeface.FamilyName, out SKColor color))
			return color;

		color = backgroundColors[fontColors.Count % backgroundColors.Length];
		fontColors[font.Typeface.FamilyName] = color;
		return color;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == FontFamilyProperty || change.Property == FontSizeProperty)
			ChangeFontInfo();
	}

	private void ChangeFontInfo()
	{
		lock (this)
		{
			fontInfo?.Dispose();
			fontInfo = new FontInfo(FontFamily, FontSize);
		}
	}

	private void Render(SKCanvas canvas, Rect rect)
	{
		lock (this)
		{
			if (fontInfo is null || Text is null) return;

			canvas.Clear(SKColors.Black);

			float x = 0;
			float y = 10;

			canvas.DrawText("SkiaSharp DrawText:", 0, y + 20, headerFont, foreground);
			y += 30;

			x = DrawText(canvas, x, y, Text, fontInfo.Font, false);
			DrawLineCaption(canvas, x, y, fontInfo);
			y += fontInfo.LineHeight;

			x = 0;
			y += 50;
			canvas.DrawText("HarfBuzzSharp DrawShapedText:", 0, y + 20, headerFont, foreground);
			y += 30;

			x = DrawText(canvas, x, y, Text, fontInfo.Font, true);
			DrawLineCaption(canvas, x, y, fontInfo);
		}
	}

	private float DrawText(SKCanvas canvas, float x, float y, string text, SKFont font, bool useHarfBuzz)
	{
		// text may contain characters which have no glyph in the current font
		// in these instances a fallback font must be used

		float width = 0;

		if (font.ContainsGlyphs(text))
		{
			// font contains glyphs for the whole text. No fallback font is needed.

			return DrawTextNoFallback(canvas, x, y, text, font, useHarfBuzz);
		}

		// there are some characters in text which have no glyph in font

		// iterate over all text elements
		int start = 0;
		TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(text);
		bool notAtEnd;
		while (notAtEnd = enumerator.MoveNext())
		{
			var textElement = enumerator.GetTextElement();
			if (!font.ContainsGlyphs(textElement))
			{
				// render previous text elements with current font
				if (start != enumerator.ElementIndex)
				{
					var regularText = text.Substring(start, enumerator.ElementIndex - start);
					width += DrawTextNoFallback(canvas, x + width, y, regularText, font, useHarfBuzz);
					start = enumerator.ElementIndex;
				}

				// find next element which can be rendered with current font again
				while ((notAtEnd = enumerator.MoveNext())
					&& !font.ContainsGlyphs(enumerator.GetTextElement()))
					;

				// get text which has no glyphs in font
				var subtext = notAtEnd
					? text.Substring(start, enumerator.ElementIndex - start)
					: text.Substring(start);

				// unfortunately MatchCharacter only takes a char or codepoint - I cannot pass it a string which is a grapheme cluster
				// so I just find a fallback for the first codepoint (still better than a single char)
				var firstCodepoint = subtext.EnumerateRunes().First().Value;

				// find a fallback font
				var fallback = SKFontManager.Default.MatchCharacter(
					font.Typeface.FamilyName,
					font.Typeface.FontStyle,
					null,
					firstCodepoint);

				if (fallback is null)
					width += DrawTextNoFallback(canvas, x + width, y, subtext, font, useHarfBuzz);    // no fallback found, then just use the given font
				else
					width += DrawText(canvas, x + width, y, subtext, fallback.ToFont(font.Size), useHarfBuzz);    // this searches for fallback fonts again if necessary

				start = notAtEnd ? enumerator.ElementIndex : text.Length;
			}
		}

		if (start < text.Length)
			width += DrawTextNoFallback(canvas, x + width, y, text.Substring(start), font, useHarfBuzz);

		return width;
	}

	/// <summary>
	/// Draw some text.
	/// Does not search for a fallback font. The given <paramref name="font"/> must contain all glyphs.
	/// </summary>
	/// <returns>The width of the rendered text.</returns>
	private float DrawTextNoFallback(SKCanvas canvas, float x, float y, string text, SKFont font, bool useHarfBuzz)
	{
		var width = useHarfBuzz
			? HarfbuzzMeasure(text, font)           // use HarfBuzz to measure
			: font.MeasureText(text);               // Skia does not measure grapheme clusters correctly but it is MUCH faster

		{
			// this block is just used in this sample to draw the caption. it is not needed for the actual text rendering.

			background.Color = GetFontColor(font);
			canvas.DrawRect(x, y, width, fontInfo.LineHeight, background);

			canvas.DrawText(font.Typeface.FamilyName, x + width / 2, y + fontInfo.LineHeight + 15, SKTextAlign.Center, captionFont, background);
			canvas.DrawText($"width={width}", x + width / 2, y + fontInfo.LineHeight + 30, SKTextAlign.Center, captionFont, background);

			DrawLines(canvas, x, y, width, fontInfo);
		}

		if (useHarfBuzz)
			Extensions.CanvasExtensions.DrawShapedText(canvas, text, x, y - fontInfo.Ascent, font, foreground);          // HarfBuzz
		else
			canvas.DrawText(text, x, y - fontInfo.Ascent, font, foreground);                // Skia

		return width;
	}

	private static float HarfbuzzMeasure(string text, SKFont font)
	{
		// code from https://github.com/mono/SkiaSharp/issues/1810
		// updated for v3.0 and set UnitsPerEm which should work around the bug in that issue

		using (var blob = font.Typeface.OpenStream().ToHarfBuzzBlob())
		using (var hbface = new HarfBuzzSharp.Face(blob, 0) { UnitsPerEm = font.Typeface.UnitsPerEm })
		{
			using (var hbFont = new HarfBuzzSharp.Font(hbface))
			{
				using (var buffer = new HarfBuzzSharp.Buffer())
				{
					buffer.AddUtf16(text);
					buffer.GuessSegmentProperties();
					hbFont.Shape(buffer);

					hbFont.GetScale(out var xScale, out _);
					var scale = font.Size / xScale;
					return buffer.GlyphPositions.Sum(x => x.XAdvance) * scale;
				}
			}
		}
	}


	private void DrawLines(SKCanvas canvas, float x, float y, float width, FontInfo fontInfo)
	{
		// draw lines
		y -= fontInfo.Ascent;
		var paint = new SKPaint();

		if (ShowTopLine)
		{
			paint.Color = topColor;
			canvas.DrawLine(x, y + fontInfo.Font.Metrics.Top, x + width, y + fontInfo.Font.Metrics.Top, paint);
		}
		if (ShowMeasuredAscentLine)
		{
			paint.Color = measuredAscentColor;
			canvas.DrawLine(x, y + fontInfo.Ascent, x + width, y + fontInfo.Ascent, paint);
		}
		if (ShowAscentLine)
		{
			paint.Color = ascentColor;
			canvas.DrawLine(x, y + fontInfo.Font.Metrics.Ascent, x + width, y + fontInfo.Font.Metrics.Ascent, paint);
		}
		if (ShowBaseLine)
		{
			paint.Color = baselineColor;
			canvas.DrawLine(x, y, x + width, y, paint);
		}
		if (ShowDescentLine)
		{
			paint.Color = descentColor;
			canvas.DrawLine(x, y + fontInfo.Font.Metrics.Descent, x + width, y + fontInfo.Font.Metrics.Descent, paint);
		}
		if (ShowBottomLine)
		{
			paint.Color = bottomColor;
			canvas.DrawLine(x, y + fontInfo.Font.Metrics.Bottom, x + width, y + fontInfo.Font.Metrics.Bottom, paint);
		}
	}

	private void DrawLineCaption(SKCanvas canvas, float x, float y, FontInfo fontInfo)
	{
		// draw captions for each visible line
		y -= fontInfo.Ascent + captionFont.Metrics.Ascent / 2;
		var paint = new SKPaint();

		if (ShowTopLine)
		{
			paint.Color = topColor;
			canvas.DrawText("Top", x, y + fontInfo.Font.Metrics.Top, captionFont, paint);
		}
		if (ShowMeasuredAscentLine)
		{
			paint.Color = measuredAscentColor;
			canvas.DrawText("Measured Ascent", x, y + fontInfo.Ascent, captionFont, paint);
		}
		if (ShowAscentLine)
		{
			paint.Color = ascentColor;
			canvas.DrawText("Ascent", x, y + fontInfo.Font.Metrics.Ascent, captionFont, paint);
		}
		if (ShowBaseLine)
		{
			paint.Color = baselineColor;
			canvas.DrawText("Baseline", x, y, captionFont, paint);
		}
		if (ShowDescentLine)
		{
			paint.Color = descentColor;
			canvas.DrawText("Descent", x, y + fontInfo.Font.Metrics.Descent, captionFont, paint);
		}
		if (ShowBottomLine)
		{
			paint.Color = bottomColor;
			canvas.DrawText("Bottom", x, y + fontInfo.Font.Metrics.Bottom, captionFont, paint);
		}
	}


	#region Get SKCanvas and call Render(SKCanvas, Rect)

	public override void Render(DrawingContext context)
	{
		context.Custom(new CustomDrawOp(Render, new Rect(0, 0, Bounds.Width, Bounds.Height)));
		Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
	}

	class CustomDrawOp : ICustomDrawOperation
	{
		private readonly Action<SKCanvas, Rect> render;

		public Rect Bounds { get; }

		public CustomDrawOp(Action<SKCanvas, Rect> render, Rect bounds)
		{
			this.render = render;
			Bounds = bounds;
		}

		public void Render(ImmediateDrawingContext context)
		{
			var leaseFeature = context.TryGetFeature<Avalonia.Skia.ISkiaSharpApiLeaseFeature>();
			if (leaseFeature == null)
				return;

			using var lease = leaseFeature.Lease();
			var canvas = lease.SkCanvas;
			canvas.Save();

			render(canvas, Bounds);

			canvas.Restore();
		}

		#region interface implementation (not used)
		public void Dispose() { }
		public bool Equals(ICustomDrawOperation? other) => false;
		public bool HitTest(Point p) => false;
		#endregion interface implementation (not used)
	}

	#endregion Get SKCanvas and call Render(SKCanvas, Rect)
}
