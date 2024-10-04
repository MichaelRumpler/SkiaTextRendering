﻿using BenchmarkDotNet.Attributes;

using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace SkiaTextRendering.Benchmarks;

//[ShortRunJob]
[MemoryDiagnoser]
public class DrawShapedTextBenchmarks
{
	SKCanvas canvas;
	SKFont cascadiaFont;
	SKFont emojiFont;
	SKShaper cascadiaShaper;
	SKShaper emojiShaper;
	SKPaint paint;

	string ascii = "The big brown fox jumps over the lazy dog"; // 41 glyphs
	string ligatures = @"<= >= == === != !== www /\ \/ <~ ~> <| |>";
	string emojis = "🐈‍⬛🐈👩🏽‍🚒👩🏼‍🎨🙂😉😍😶‍🌫️🤡🥸👧🏾🤢🤮🤬👩🏽‍🦰🎅🏽👨🏽‍🦽";    // 17 glyphs


	public DrawShapedTextBenchmarks()
	{
		canvas = new SKCanvas(new SKBitmap(800, 600));
		cascadiaFont = SKFontManager.Default.MatchFamily("Cascadia Code PL").ToFont(20);		// I installed that font locally to get comparable results
		emojiFont = SKFontManager.Default.MatchFamily("Segoe UI Emoji").ToFont(20);				// this was installed on my win11 machine by default
		paint = new SKPaint() { Color = SKColors.LimeGreen, IsAntialias = true, IsStroke = false };
		cascadiaShaper = new(cascadiaFont.Typeface);
		emojiShaper = new(emojiFont.Typeface);

		Extensions.CanvasExtensions.SetShaperCacheDuration(null, 30_000);
	}

	[Benchmark] public void DrawAscii() => DrawText(ascii, cascadiaFont);
	[Benchmark] public void DrawShapedAscii() => DrawShapedText(ascii, cascadiaFont);
	[Benchmark] public void DrawAsciiWithShaper() => DrawShapedText(ascii, cascadiaFont, cascadiaShaper);
	[Benchmark] public void DrawAsciiWithCache() => DrawShapedWithCacheText(ascii, cascadiaFont);

	[Benchmark] public void DrawLigatures() => DrawText(ligatures, cascadiaFont);
	[Benchmark] public void DrawShapedLigatures() => DrawShapedText(ligatures, cascadiaFont);
	[Benchmark] public void DrawLigaturesWithShaper() => DrawShapedText(ligatures, cascadiaFont, cascadiaShaper);
	[Benchmark] public void DrawLigaturesWithCache() => DrawShapedWithCacheText(ligatures, cascadiaFont);

	[Benchmark] public void DrawEmojis() => DrawText(emojis, cascadiaFont);
	[Benchmark] public void DrawShapedEmojis() => DrawShapedText(emojis, emojiFont);
	[Benchmark] public void DrawEmojisWithShaper() => DrawShapedText(emojis, emojiFont, emojiShaper);
	[Benchmark] public void DrawEmojisWithCache() => DrawShapedWithCacheText(emojis, emojiFont);

	private void DrawText(string str, SKFont font)
	{
		canvas.DrawText(str, 0, 20, font, paint);
	}

	private void DrawShapedText(string str, SKFont font)
	{
		canvas.DrawShapedText(str, 0, 20, font, paint);
	}

	private void DrawShapedText(string str, SKFont font, SKShaper shaper)
	{
		canvas.DrawShapedText(shaper, str, 0, 20, font, paint);
	}

	private void DrawShapedWithCacheText(string str, SKFont font)
	{
		Extensions.CanvasExtensions.DrawShapedText(canvas, str, 0, 20, font, paint);
	}
}