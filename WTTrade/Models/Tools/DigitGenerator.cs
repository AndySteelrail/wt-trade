namespace WTTrade.Models;

public class DigitGenerator
{
    private static ThreadLocal<Random> LocalRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

    public static string GenerateTransactId(int length)
    {
        char[] digits = new char[length];
        
        digits[0] = (char)('0' + LocalRandom.Value.Next(1, 10));
        
        for (int i = 1; i < length; i++)
        {
            digits[i] = (char)('0' + LocalRandom.Value.Next(0, 10));
        }

        return new string(digits);
    }
    
    public static int GeneratePlacingOrderDelay()
    {
        return LocalRandom.Value.Next(4105, 6481);
    }
    
    public static int GenerateBeetweenPlacingOrdersDelay()
    {
        return LocalRandom.Value.Next(2006, 2882);
    }
    
    public static int GenerateGetItemDelay()
    {
        return LocalRandom.Value.Next(1347, 2200);
    }
    
    public static int GenerateCancelOrderDelay()
    {
        return LocalRandom.Value.Next(1698, 2304);
    }
}