using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Security;

namespace LOKI.MainFile;

public partial class MainCode : ObservableObject
{
    [ObservableProperty]
    private string? _userTextIn;
    
    [ObservableProperty]
    private string? _userTextOut;

    [ObservableProperty]
    private string? _ephemeralKeyUser;
    
    [ObservableProperty]
    private string? _someoneEphemeralKey;

    [ObservableProperty]
    private bool _cryptFalse;

    [ObservableProperty]
    public string _labelText = "Шифрование";

    [ObservableProperty]
    private string _currentVersion = $"{AppInfo.Current.VersionString}";

    [ObservableProperty]
    private bool _hideText = true;

    [ObservableProperty]
    private bool _localOrGlobal = false;

    [ObservableProperty]
    public string _globalLabelText = "Буфер LOKI";

    private bool theonlyone = false;


    [RelayCommand]
    private async Task CryptMessege()
    {
        try 
        {
            if ( UserTextIn == null || UserTextIn.Length == 0) return;

            byte[] MessegeToByte = Encoding.UTF8.GetBytes(UserTextIn);
            byte[] CryptMessege = new byte[MessegeToByte.Length];
            byte[] Nonce = new byte[12];//рандомное число
            SecureRandom random = new SecureRandom();//рандомное число для кривой
            byte[] RandomForBytes = new byte[32];

            // Ассиметрия
            random.NextBytes(RandomForBytes);
            X25519PrivateKeyParameters MyPrivateKey = new X25519PrivateKeyParameters(RandomForBytes);// кривая X25519
            X25519PublicKeyParameters MyPublicKey = MyPrivateKey.GeneratePublicKey();

            string? a1 = await SecureStorage.Default.GetAsync("MyNowPrivateKey");// Приватный ключ
            if (string.IsNullOrEmpty(a1))
            {
                await SecureStorage.Default.SetAsync("MyNowPrivateKey", Convert.ToBase64String(RandomForBytes));
            }
            else
            {
                SecureStorage.Default.Remove("MyNowPrivateKey");
                await SecureStorage.Default.SetAsync("MyNowPrivateKey", Convert.ToBase64String(RandomForBytes));
            }

            if (string.IsNullOrEmpty(EphemeralKeyUser)) //если ключ пользователя null
            {
                byte[] MyEphemeralBytes = MyPublicKey.GetEncoded(); //промежуточный ключ пользователя
                EphemeralKeyUser = Convert.ToBase64String(MyEphemeralBytes);
                // сохранение своего первого ключа

                string a = Preferences.Default.Get("MyEphemeralKey", string.Empty);
                if (!string.IsNullOrEmpty(a))
                {
                    Preferences.Default.Clear("MyEphemeralKey");
                }

                Preferences.Default.Set("MyEphemeralKey", Convert.ToBase64String(MyEphemeralBytes));
            }
                
                byte[] SomeoneEphemeralBytes = new byte[32]; // массив под чужой помежуточный ключ
                if (!string.IsNullOrEmpty(SomeoneEphemeralKey))
                {
                    SomeoneEphemeralBytes = Convert.FromBase64String(SomeoneEphemeralKey);
                }
                else
                {// если не ввел ключ собеседника
                    SomeoneEphemeralKey = "нет промежуточного ключа собеседника";
                    return;
                }

                if (SomeoneEphemeralBytes == null || SomeoneEphemeralBytes.Length == 0) return; //подстраховка на промежуточный ключ собеседника
                X25519PublicKeyParameters SomeoneRestoredEphemeral = new X25519PublicKeyParameters(SomeoneEphemeralBytes, 0);

                byte[] GeneralKey = new byte[32]; // общий ключ
                MyPrivateKey.GenerateSecret(SomeoneRestoredEphemeral, GeneralKey, 0);

            //создание второго промеж ключа для следующего сообщения
                SecureRandom random2 = new SecureRandom();
                byte[] RandomForBytes2 = new byte[32];
                random2.NextBytes(RandomForBytes2);

                X25519PrivateKeyParameters MyPrivateKey2 = new X25519PrivateKeyParameters(RandomForBytes2);
                X25519PublicKeyParameters MyPublicKey2 = MyPrivateKey.GeneratePublicKey();
                byte[] MyEphemeralBytes2 = MyPublicKey2.GetEncoded();
                EphemeralKeyUser = Convert.ToBase64String(MyEphemeralBytes2);
                
            // Симметрия
                    try
                    {
                        RandomNumberGenerator.Fill(Nonce);
                        byte[] Gamma = HKDF.DeriveKey(HashAlgorithmName.SHA256, GeneralKey, MessegeToByte.Length, Nonce);

                        try
                        {
                            for (int i = 0; i < MessegeToByte.Length; i++)
                            {
                                CryptMessege[i] = (byte)(MessegeToByte[i] ^ Gamma[i]);
                            }
                        }
                        finally
                        {
                            CryptographicOperations.ZeroMemory(Gamma);
                        }
                    }
                    finally
                    {
                        CryptographicOperations.ZeroMemory(GeneralKey);
                    }
            

            // Вывод инфы на интерфейс
            try
            {
                string CryptMessegeBase64 = Convert.ToBase64String(CryptMessege);
                string NonceBase64 = Convert.ToBase64String(Nonce);
                string MyEphemeralBytes2Base64 = Convert.ToBase64String(MyEphemeralBytes2);
                string Result = $"{CryptMessegeBase64}✶{MyEphemeralBytes2Base64}{NonceBase64}";

                UserTextOut = Result;
                
            }
            finally
            {
                CryptographicOperations.ZeroMemory(MessegeToByte); // очистка остального
            }

        } catch (Exception) {}
        
    }



    [RelayCommand]
    private async Task DecryptMessege()
    {
        try
        {
            if ( UserTextIn == null || UserTextIn.Length == 0 || UserTextIn.Length <= 16 || string.IsNullOrEmpty(SomeoneEphemeralKey)) return;
            if (SomeoneEphemeralKey == EphemeralKeyUser) return;

        //Ассиметрия

            string UserMessege = new string(UserTextIn);
            string NonceBase64 = UserMessege[^16..]; 
            string LeftPart = UserMessege[..^16];

            int PositionStar = LeftPart.IndexOf("✶");//позиция символа
            if (PositionStar == -1) return;

            string cryptMessageBase64 = LeftPart.Substring(0, PositionStar);// разделение по символу
            string someoneEphemeralBase64 = LeftPart.Substring(PositionStar + 1);

            byte[] CryptMessege = Convert.FromBase64String(cryptMessageBase64);
            byte[] Nonce = Convert.FromBase64String(NonceBase64);
            byte[] SomeoneEphemeralBytesForFuture = Convert.FromBase64String(someoneEphemeralBase64);
            byte[] SomeoneEphemeralBytesNow = Convert.FromBase64String(SomeoneEphemeralKey);
            byte[] DeCryptMessege = new byte[CryptMessege.Length];

            string? PrivateKey64 = await SecureStorage.Default.GetAsync("MyNowPrivateKey");
            if (string.IsNullOrEmpty(PrivateKey64)) return;
            byte[] RandomForBytes = Convert.FromBase64String(PrivateKey64);

            SomeoneEphemeralKey = Convert.ToBase64String(SomeoneEphemeralBytesForFuture);// заменяем ключ на новый

            string UserEphemeralKey = Preferences.Default.Get("MyEphemeralKey", string.Empty);// промежуточный ключ
            if (string.IsNullOrEmpty(UserEphemeralKey)) return;

            X25519PrivateKeyParameters MyPrivateKey = new X25519PrivateKeyParameters(RandomForBytes, 0);//кривая
            X25519PublicKeyParameters SomeoneRestoredEphemeral = new X25519PublicKeyParameters(SomeoneEphemeralBytesNow, 0);

            byte[] GeneralKey = new byte[32]; // общий ключ
            MyPrivateKey.GenerateSecret(SomeoneRestoredEphemeral, GeneralKey, 0);

        //Симметрия

            byte[] Gamma = HKDF.DeriveKey(HashAlgorithmName.SHA256, GeneralKey, CryptMessege.Length, Nonce);
            try
            {
                for (int i = 0; i < CryptMessege.Length; i++)
                {
                    DeCryptMessege[i] = (byte)(CryptMessege[i] ^ Gamma[i]);
                }
                char[] Output = Encoding.UTF8.GetChars(DeCryptMessege);

                if (Output.Contains('�') || Output.Contains('┐') || Output.Contains('╜')) return;// если два раза нажал дешифровать то это не даст стереть текст
                UserTextOut = new string(Output);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(Gamma);
                CryptographicOperations.ZeroMemory(DeCryptMessege);
                CryptographicOperations.ZeroMemory(GeneralKey);
            }

        } catch (Exception) {}
    }



    [RelayCommand]
    private void Clear()
    {
        if (!string.IsNullOrEmpty(UserTextIn))
        {
            UserTextIn = "";
        }

        if (!string.IsNullOrEmpty(UserTextOut))
        {
            UserTextOut = "";
        }
    }


    [RelayCommand]
    private async Task CopyKey()
    {
        if (string.IsNullOrEmpty(EphemeralKeyUser)) return;

        if (LocalOrGlobal == false)
        {
            SpaceClipboard.CopyLocalText(EphemeralKeyUser);
        }
        else
        {
            await Clipboard.Default.SetTextAsync(EphemeralKeyUser);
        }
    }

    [RelayCommand]
    private async Task CopyToOut()
    {
        if (string.IsNullOrEmpty(UserTextOut)) return;

        if (LocalOrGlobal == false)
        {
            SpaceClipboard.CopyLocalText(UserTextOut);
        }
        else
        {
            await Clipboard.Default.SetTextAsync(UserTextOut);
        }
    }

    [RelayCommand]
    private async Task Paste()
    {
        if (LocalOrGlobal == false)
        {
            SomeoneEphemeralKey = SpaceClipboard.PasteLocalText();
        }
        else
        {
            SomeoneEphemeralKey = await Clipboard.Default.GetTextAsync();
        }
    }

    [RelayCommand]
    private async Task PasteToIn()
    {
        if (LocalOrGlobal == false)
        {
            UserTextIn = SpaceClipboard.PasteLocalText();
        }
        else
        {
            UserTextIn = await Clipboard.Default.GetTextAsync();
        }
    }

    [RelayCommand]
    private async Task Distribution()
    {
        if (CryptFalse == false)
        {
            _ = CryptMessege();
        } else
        {
            _ = DecryptMessege();
        }
    }

    public async void TheOnlyOneBanner()//сработает ток 1 раз
    {
        theonlyone = Preferences.Default.ContainsKey("HowDoYouDo");
        if (theonlyone == true) return;

        Routing.RegisterRoute(nameof(OnlyBannerPage), typeof(OnlyBannerPage));
        await Shell.Current.GoToAsync(nameof(OnlyBannerPage));

        Preferences.Default.Set("HowDoYouDo", true);
    }

    [RelayCommand]
    private async Task SendText()
    {
        if ( UserTextOut == null || UserTextOut.Length == 0) return;  

        bool star = UserTextOut.Contains('✶');
        if (star == false) return;

        await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = UserTextOut.ToString(),
                Title = "Куда отправить сообщение?"
            });
    }
}