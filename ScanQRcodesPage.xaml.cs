namespace LOKI;

public partial class ScanQRcodesPage : ContentPage
{
	public ScanQRcodesPage()
	{
		InitializeComponent();
	}

	private async void CameraBarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var firstResult = e.Results?.FirstOrDefault();
        if (firstResult != null)
        {
             MainThread.BeginInvokeOnMainThread(() =>
            {
                QRCodeScan.IsDetecting = false;
                QRCodeScan.IsVisible = false;
            });

            var navigationParameter = new Dictionary<string, object>
            {
                { "SomeoneKey", firstResult.Value }
            };
            await Shell.Current.GoToAsync("..", navigationParameter);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var CameraOn = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (CameraOn != PermissionStatus.Granted)
        {
            CameraOn = await Permissions.RequestAsync<Permissions.Camera>();
        }

        if (CameraOn == PermissionStatus.Granted)
        {
            await Task.Delay(2000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                QRCodeScan.IsDetecting = true;
                QRCodeScan.IsVisible = true;
            });
        } else
        {
            await DisplayAlertAsync("Камера недоступна", "Нет разрешения на использование камеры.", "ОК");
            await Shell.Current.GoToAsync("..");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            QRCodeScan.IsDetecting = false;
            QRCodeScan.IsVisible = false;
        });
    }
}