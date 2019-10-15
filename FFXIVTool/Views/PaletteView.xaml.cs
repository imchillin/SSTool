using FFXIVTool.Utility;
using FFXIVTool.ViewModel;
using System.Windows.Controls;

namespace FFXIVTool.Views
{
	/// <summary>
	/// Interaction logic for PaletteView.xaml
	/// </summary>
	public partial class PaletteView : UserControl
	{
		public PaletteView()
		{
			InitializeComponent();
			DataContext = new PaletteSelectorViewModel();
			if (SaveSettings.Default.Theme == "Dark") ThemeButton.IsChecked = true;
		}
	}
}
