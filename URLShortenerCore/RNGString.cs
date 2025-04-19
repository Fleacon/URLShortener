namespace URLShortenerCore;

public class RNGString
{
    private Random random;
    
    public RNGString()
    {
        random = new Random();    
    }

    public string generateRandomString()
    {
        var charactersets = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        string result = string.Empty; //323

        for (int i = 0; i < 6; i++)
        {
            result += charactersets[random.Next(charactersets.Length)];
        }
        return result;
    }
}