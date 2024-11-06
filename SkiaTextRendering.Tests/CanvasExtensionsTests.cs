using System.Collections.Concurrent;
using System.Reflection;

using SkiaSharp;
using SkiaSharp.HarfBuzz;

using SkiaTextRendering.Extensions;

namespace SkiaTextRendering.Tests;

public class CanvasExtensionsTests
{
	private SKCanvas canvas;
	private SKFont font;
	private SKPaint paint;

	#region Access private fields

	FieldInfo? shaperCacheField = null;
	public Dictionary<int, (SKShaper shaper, DateTime cachedAt)> ShaperCache
	{
		get
		{
			if(shaperCacheField is null)
				shaperCacheField = typeof(Extensions.CanvasExtensions).GetField("shaperCache", BindingFlags.NonPublic | BindingFlags.Static);

			return (Dictionary<int, (SKShaper shaper, DateTime cachedAt)>)shaperCacheField!.GetValue(null)!;
		}
	}

	FieldInfo? shapeResultCacheField = null;
	public Dictionary<int, (SKShaper.Result shapeResult, DateTime cachedAt)> ShapeResultCache
	{
		get
		{
			if (shapeResultCacheField is null)
				shapeResultCacheField = typeof(Extensions.CanvasExtensions).GetField("shapeResultCache", BindingFlags.NonPublic | BindingFlags.Static);

			return (Dictionary<int, (SKShaper.Result shapeResult, DateTime cachedAt)>)shapeResultCacheField!.GetValue(null)!;
		}
	}

    #endregion Access private fields

    public CanvasExtensionsTests()
    {
		canvas = new SKCanvas(new SKBitmap(800, 600));
		var familyName = SKFontManager.Default.FontFamilies.First();
		font = SKFontManager.Default.MatchFamily(familyName).ToFont(20);
		paint = new SKPaint() { Color = SKColors.LimeGreen, IsAntialias = true, IsStroke = false };
	}

	[Fact]
	public void CacheGetsFilled()
	{
		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
		Extensions.CanvasExtensions.SetShaperCacheDuration(30_000);

		Assert.Empty(ShaperCache);
		Assert.Empty(ShapeResultCache);

		Extensions.CanvasExtensions.DrawShapedText(canvas, "Hello world!", 0, 0, font, paint);

		Assert.Single(ShaperCache);
		Assert.Single(ShapeResultCache);

		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
	}

	[Fact]
	public async Task CacheGetsClearedWhenSettingDurationToZero()
	{
		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
		Extensions.CanvasExtensions.SetShaperCacheDuration(30_000);

		Extensions.CanvasExtensions.DrawShapedText(canvas, "Hello world!", 0, 0, font, paint);

		Assert.Single(ShaperCache);
		Assert.Single(ShapeResultCache);

		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
		await Task.Delay(10);

		Assert.Empty(ShaperCache);
		Assert.Empty(ShapeResultCache);
	}

	[Fact]
	public async Task CacheGetsClearedAutomatically()
	{
		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
		Extensions.CanvasExtensions.SetShaperCacheDuration(30);

		Extensions.CanvasExtensions.DrawShapedText(canvas, "Hello world!", 0, 0, font, paint);

		Assert.Single(ShaperCache);
		Assert.Single(ShapeResultCache);

		await Task.Delay(50);

		Assert.Empty(ShaperCache);
		Assert.Empty(ShapeResultCache);

		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
	}

	[Fact]
	public void TwoStringsGetBothCached()
	{
		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
		Extensions.CanvasExtensions.SetShaperCacheDuration(30_000);

		Assert.Empty(ShaperCache);
		Assert.Empty(ShapeResultCache);

		Extensions.CanvasExtensions.DrawShapedText(canvas, "Hello", 0, 0, font, paint);
		Extensions.CanvasExtensions.DrawShapedText(canvas, "world!", 100, 0, font, paint);

		Assert.Single(ShaperCache);
		Assert.Equal(2, ShapeResultCache.Count);

		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
	}

	[Fact]
	public void ShaperGetsCachedForDifferentFonts()
	{
		var fontFamily2 = SKFontManager.Default.FontFamilies.Skip(1).First();
		var font2 = SKFontManager.Default.MatchFamily(fontFamily2).ToFont(20);

		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
		Extensions.CanvasExtensions.SetShaperCacheDuration(30_000);

		Assert.Empty(ShaperCache);
		Assert.Empty(ShapeResultCache);

		Extensions.CanvasExtensions.DrawShapedText(canvas, "Hello", 0, 0, font, paint);

		Assert.Single(ShaperCache);
		Assert.Single(ShapeResultCache);

		Extensions.CanvasExtensions.DrawShapedText(canvas, "world!", 100, 0, font2, paint);

		Assert.Equal(2, ShaperCache.Count);
		Assert.Equal(2, ShapeResultCache.Count);

		Extensions.CanvasExtensions.SetShaperCacheDuration(0);
	}
}