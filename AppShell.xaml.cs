namespace LOKI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("QRCodePage", typeof(QRcodePage));
		Routing.RegisterRoute("ScanningPage", typeof(ScanQRcodesPage));
	}
}
