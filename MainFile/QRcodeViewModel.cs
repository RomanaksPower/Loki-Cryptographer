using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace LOKI.MainFile;

[QueryProperty(nameof(ReceivedBytes), "QrBytesKey")]
public partial class QRcodeViewModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource? _qRcode;
    private byte[]? _receivedBytes;
    public byte[]? ReceivedBytes
    {
        get => _receivedBytes;
        set
        {
            SetProperty(ref _receivedBytes, value);
            
            if (value != null)
            {
                QRcode = ImageSource.FromStream(() => new MemoryStream(value));
            }
        }
    }

    public void ClearData()
    {
        QRcode = null;
        ReceivedBytes = null;
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }
}