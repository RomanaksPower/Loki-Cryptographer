using LOKI.MainFile;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace LOKI;

public partial class QRcodePage : ContentPage
{
	public QRcodePage()
	{
		InitializeComponent();
		BindingContext = new QRcodeViewModel();
	}

	protected override void OnDisappearing()
	{
    	base.OnDisappearing();
    
    	if (BindingContext is QRcodeViewModel qRcodeViewModel)
    	{
        	qRcodeViewModel.ClearData();
    	}
	} 

	private void OnCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs e)
	{
    	var canvas = e.Surface.Canvas;
    	canvas.Clear();
	
    	using var blur = SKImageFilter.CreateBlur(80, 80);
    	using var paint = new SKPaint();

    	paint.ImageFilter = blur;
    	paint.Color = SKColors.Black.WithAlpha(220);
    	canvas.DrawRect(e.Info.Rect, paint);
	}
}