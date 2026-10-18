using LOKI.MainFile;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;


namespace LOKI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("materialdesignicons-webfont.ttf", "MDI"); 
			});
				
			
			
#if DEBUG
		builder.Logging.AddDebug();
#endif


Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
{
#if WINDOWS
    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
    handler.PlatformView.Background = Microsoft.Maui.Graphics.Colors.Transparent.ToPlatform();
    handler.PlatformView.FocusVisualMargin = new Microsoft.UI.Xaml.Thickness(0);
    handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
#elif ANDROID
    handler.PlatformView.Background = null;
    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
    handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
    handler.PlatformView.SetPadding(0, 0, 0, 0);
#endif
});

Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
{
#if WINDOWS
    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
    handler.PlatformView.Background = Microsoft.Maui.Graphics.Colors.Transparent.ToPlatform();
    handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
#elif ANDROID
    handler.PlatformView.Background = null;
    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
    handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
    handler.PlatformView.SetPadding(0, 0, 0, 0);
#endif
});

		return builder.Build();
	}
}
