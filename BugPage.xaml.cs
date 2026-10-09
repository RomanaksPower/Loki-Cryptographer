using LOKI.MainFile;

namespace LOKI;

public partial class BugPage : ContentPage
{
	public BugPage()
	{
		InitializeComponent();
        BindingContext= new SendMessege();
	}

    private async void MainMenu(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}