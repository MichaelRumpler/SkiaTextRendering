using Avalonia.Media;

using SkiaSharp;
using SkiaSharp.HarfBuzz;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SkiaTextRendering.Extensions;

public static class CanvasExtensionsRuntimeCache
{
	public static void DrawShapedText(this SKCanvas canvas, string text, float x, float y, SKFont font, SKPaint paint) =>
		DrawShapedText(canvas, text, x, y, SKTextAlign.Left, font, paint);

	public static void DrawShapedText(this SKCanvas canvas, string text, float x, float y, SKTextAlign textAlign, SKFont font, SKPaint paint)
	{
		if (string.IsNullOrEmpty(text))
			return;

		var shaper = GetShaper(font.Typeface);
		DrawShapedText(canvas, shaper, text, x, y, textAlign, font, paint);
	}

	public static void DrawShapedText(this SKCanvas canvas, SKShaper shaper, string text, float x, float y, SKFont font, SKPaint paint) =>
		DrawShapedText(canvas, shaper, text, x, y, SKTextAlign.Left, font, paint);

	public static void DrawShapedText(this SKCanvas canvas, SKShaper shaper, string text, float x, float y, SKTextAlign textAlign, SKFont font, SKPaint paint)
	{
		if (string.IsNullOrEmpty(text))
			return;

		if (canvas == null)
			throw new ArgumentNullException(nameof(canvas));
		if (shaper == null)
			throw new ArgumentNullException(nameof(shaper));
		if (paint == null)
			throw new ArgumentNullException(nameof(paint));

		font.Typeface = shaper.Typeface;

		// shape the text
		var result = GetShapeResult(shaper, text, font);

		// create the text blob
		using var builder = new SKTextBlobBuilder();
		var run = builder.AllocateRawPositionedRun(font, result.Codepoints.Length, null);

		// copy the glyphs
		var g = run.Glyphs;
		var p = run.Positions;
		for (var i = 0; i < result.Codepoints.Length; i++)
		{
			g[i] = (ushort)result.Codepoints[i];
			p[i] = result.Points[i];
		}

		// build
		using var textBlob = builder.Build();

		// adjust alignment
		var xOffset = 0f;
		if (textAlign != SKTextAlign.Left)
		{
			var width = result.Width;
			if (textAlign == SKTextAlign.Center)
				width *= 0.5f;
			xOffset -= width;
		}

		// draw the text
		canvas.DrawText(textBlob, x + xOffset, y, paint);
	}

	private static uint cacheDuration = 0;

	public static void SetShaperCacheDuration(this SKCanvas? canvas, uint milliseconds) => SetShaperCacheDuration(milliseconds);
	public static void SetShaperCacheDuration(uint milliseconds)
	{
		cacheDuration = milliseconds;
	}

	private static SKShaper GetShaper(SKTypeface typeface)
	{
		if(cacheDuration == 0)
			return new SKShaper(typeface);

		var b = typeface.IsBold ? 'B' : ' ';
		var i = typeface.IsItalic ? 'I' : ' ';
		var key = $"{typeface.FamilyName}|{b}|{i}";

		var shaper = System.Runtime.Caching.MemoryCache.Default.Get(key) as SKShaper
			?? new SKShaper(typeface);

		System.Runtime.Caching.MemoryCache.Default.Set(key, shaper, DateTimeOffset.Now.AddMilliseconds(cacheDuration));
		return shaper;
	}

	private static SKShaper.Result GetShapeResult(SKShaper shaper, string text, SKFont font)
	{
		if(cacheDuration == 0)
			return shaper.Shape(text, 0, 0, font);

		var b = font.Typeface.IsBold ? 'B' : ' ';
		var i = font.Typeface.IsItalic ? 'I' : ' ';
		var key = $"{font.Typeface.FamilyName}|{font.Size}|{b}|{i}|{text}";

		var result = System.Runtime.Caching.MemoryCache.Default.Get(key) as SKShaper.Result
			?? shaper.Shape(text, 0, 0, font);

		System.Runtime.Caching.MemoryCache.Default.Set(key, result, DateTimeOffset.Now.AddMilliseconds(cacheDuration));
		return result;
	}
}
