using System.Text;

public class TelegramMessageSplitter
{
    public static List<string> SplitMessage(string text, int maxLength = 4096)
    {
        string[] lines = text.Split('\n');
        List<string> parts = new List<string>();
        StringBuilder currentPart = new StringBuilder();
        int currentLength = 0;

        foreach (string line in lines)
        {
            if (line.Length > maxLength)
            {
                throw new ArgumentException(
                    $"Строка длиной {line.Length} превышает максимальный размер {maxLength} символов.");
            }

            int potentialLength = currentLength == 0 ? line.Length : currentLength + 1 + line.Length;

            if (potentialLength > maxLength)
            {
                parts.Add(currentPart.ToString());
                currentPart.Clear();
                currentPart.Append(line);
                currentLength = line.Length;
            }
            else
            {
                if (currentLength > 0)
                {
                    currentPart.Append('\n');
                    currentLength++;
                }

                currentPart.Append(line);
                currentLength += line.Length;
            }
        }

        if (currentLength > 0)
        {
            parts.Add(currentPart.ToString());
        }

        return parts;
    }
}