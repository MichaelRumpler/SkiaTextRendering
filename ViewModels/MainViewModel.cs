using System.Linq;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using SkiaSharp;

namespace SkiaTextRendering.ViewModels;

public partial class MainViewModel : ObservableObject
{
	[ObservableProperty] string? text = "Ascii 🐈‍⬛🐈🏝️ x<=y  👩🏽‍🚒👩🏼‍🎨";

	public FontFamily[] FontFamilies { get; }
	[ObservableProperty] FontFamily fontFamily;

	[ObservableProperty] int fontSize = 80;

	[ObservableProperty] bool showTopLine = true;
	[ObservableProperty] bool showMeasuredAscentLine = true;
	[ObservableProperty] bool showAscentLine = true;
	[ObservableProperty] bool showBaseLine = true;
	[ObservableProperty] bool showDescentLine = true;
	[ObservableProperty] bool showBottomLine = true;


	public MainViewModel()
	{
		FontFamilies = new FontFamily[] {
				new FontFamily("avares://SkiaTextRendering/Assets/AvaloniaFonts#Cascadia Code PL"),
				new FontFamily("avares://SkiaTextRendering/Assets/AvaloniaFonts#Fira Code"),
				new FontFamily("avares://SkiaTextRendering/Assets/AvaloniaFonts#JetBrains Mono"),
				new FontFamily("avares://SkiaTextRendering/Assets/AvaloniaFonts#Monaspace Argon"),
				new FontFamily("avares://SkiaTextRendering/Assets/AvaloniaFonts#Roboto Mono"),
			}
			.Concat(FontManager.Current.SystemFonts.Where(ff => IsMonospace(ff) || ff.Name.Contains("Emoji")))
			.ToArray();
		FontFamily = FontFamilies[0];
	}

	private bool IsMonospace(FontFamily family)
	{
		var tf = SKTypeface.FromFamilyName(family.Name);	// only works with installed system fonts, not with embedded ones
		if (tf?.FamilyName != family.Name)
			return false;

		var font = tf.ToFont(14);
		var i = font.MeasureText("i");
		var W = font.MeasureText("W");
		return i == W;
	}
}
