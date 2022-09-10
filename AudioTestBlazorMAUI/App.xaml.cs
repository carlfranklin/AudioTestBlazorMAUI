﻿#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace AudioTestBlazorMAUI;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            Constants.AppWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            Constants.AppWindow.Resize(new SizeInt32(Constants.WindowWidth, Constants.WindowHeight));
            // Constants.AppWindow.Hide(); // doesn't work here
#endif
        });

        MainPage = new MainPage();
	}
}
