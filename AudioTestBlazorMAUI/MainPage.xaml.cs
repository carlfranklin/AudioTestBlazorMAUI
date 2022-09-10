#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace AudioTestBlazorMAUI;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		this.Loaded += MainPage_Loaded;
	}

	private void MainPage_Loaded(object sender, EventArgs e)
	{
#if WINDOWS
        var width = DeviceDisplay.Current.MainDisplayInfo.Width;
        var height = DeviceDisplay.Current.MainDisplayInfo.Height;
        var X = Convert.ToInt32((width / 2) - ((double)Constants.WindowWidth / 2));
        var Y = Convert.ToInt32((height / 2) - ((double)Constants.WindowHeight / 2));
        var point = new PointInt32();
        point.X = X;
        point.Y = Y;
        Constants.AppWindow.Move(point);
        // Constants.AppWindow.Show(); // doesn't work here
#endif
    }
}
