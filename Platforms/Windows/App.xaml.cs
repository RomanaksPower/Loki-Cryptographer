using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;

namespace LOKI.WinUI;

public partial class App : MauiWinUIApplication
{
	public App()
	{
		this.InitializeComponent();
	}

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
{
    base.OnLaunched(args);

    // окно приложения
    var mauiWindow = Microsoft.Maui.Controls.Application.Current?.Windows?.FirstOrDefault();
    if (mauiWindow != null)
    {
        var nativeWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow != null)
        {
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(nativeWindow.GetWindowHandle());
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = false; // ЗАПРЕЩАЕТ растягивать окно мышкой
                presenter.IsMaximizable = false; // ОТКЛЮЧАЕТ кнопку Развернуть на весь экран
            }
        }
    }
}


	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
