using System.Net;
using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LOKI.MainFile;

public partial class SendMessege : ObservableObject
{
    private Secret Emails = new Secret();
    
    [ObservableProperty]
    private string? _userMessege;

    [ObservableProperty]
    private bool _yesOrNot;

    [ObservableProperty]
    private bool _goOrNot = false;

    [RelayCommand]
    private async Task ReportMessege()
    {
        NetworkAccess access = Connectivity.Current.NetworkAccess;
        if (access != NetworkAccess.Internet) Application.Current.MainPage.DisplayAlert("Ошибка!", "Отсутствует подключение к сети", "OK");
        if (access != NetworkAccess.Internet) return;


        string Subject = "BugReportMobile";

        if (string.IsNullOrEmpty(UserMessege)) return;
        if (UserMessege.Length < 20) return;

        GoOrNot = true;

        if (YesOrNot == true)
                {
                    Subject = "ProposalReport";
                } else
                {
                    Subject = "BugReportMobile";
                }

        var smtpClient = new SmtpClient("smtp.gmail.com")
{
    Port = 587,
    Credentials = new NetworkCredential(Emails.FromEmail, Emails.EmailPassword),
    EnableSsl = true,
};

var mailMessage = new MailMessage
{
    From = new MailAddress(Emails.FromEmail),
    Subject = Subject,
    Body = UserMessege,
    IsBodyHtml = false,
};
mailMessage.To.Add(Emails.WhereEmail);

smtpClient.Send(mailMessage);
GoOrNot = false;
    }
}
