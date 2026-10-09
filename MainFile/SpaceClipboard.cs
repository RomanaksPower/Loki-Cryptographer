namespace LOKI.MainFile;

public static class SpaceClipboard
{
    private static string? LocalText;

    public static void CopyLocalText(string input)
    {
        LocalText = input;
    }

    public static string? PasteLocalText()
    {
        return LocalText;
    }

    public static bool HasText => !string.IsNullOrEmpty(LocalText);
}