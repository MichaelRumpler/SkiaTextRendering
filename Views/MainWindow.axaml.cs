using Avalonia.Controls;

namespace SkiaTextRendering.Views
{
	public partial class MainWindow : Window
	{
		public static MainWindow Current { get; private set; }

		public MainWindow()
		{
			Current = this;

			InitializeComponent();
		}
	}
}