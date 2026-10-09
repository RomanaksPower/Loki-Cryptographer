using LOKI.MainFile;

namespace LOKI;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
        BindingContext= new MainCode();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainCode mainCode)
        {
            mainCode.TheOnlyOneBanner(); 
        }
    }

    private async void Tgk(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://t.me/LOKICryptographerDevelop"));
    }

    private async void BugClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new BugPage());
    }


    private async void Information(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new InfoPage());      
    }

    private void TransmissionText(object sender, ToggledEventArgs e)
    {
        if (BindingContext is MainCode mainCode)
        {
            if (mainCode.LabelText == "Шифрование")
            {
                mainCode.LabelText = "Дешифрование";
            } else
            {
                mainCode.LabelText = "Шифрование";
            }
        }
    }

    private void TransmissionLocalText(object sender, ToggledEventArgs e)
    {
        if (BindingContext is MainCode mainCode)
        {
            if (mainCode.GlobalLabelText == "Буфер LOKI")
            {
                mainCode.GlobalLabelText = "Буфер Android";
            } else
            {
                mainCode.GlobalLabelText = "Буфер LOKI";
            }
        }
    }

    private async void DeleteAlert(object sender, EventArgs e)
    {
        bool DeleteOrNot = await DisplayAlertAsync("Очистка полей ввода и вывода", "Очистить поля?", "Да", "Нет");  

        if (BindingContext is MainCode mainCode)
        {
            if (DeleteOrNot == false) {} else
            {
                mainCode.ClearCommand.Execute(null);
            }
        }
    }

        private async void HelpAuthor(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://pay.cloudtips.ru/p/68a611ea"));
    }
}
