namespace LOKI;

public partial class InfoPage : ContentPage
{
	public InfoPage()
	{
		InitializeComponent();
	}

		private async void MainMenu(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}