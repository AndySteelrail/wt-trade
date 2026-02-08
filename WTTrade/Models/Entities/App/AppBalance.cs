namespace WTTrade.Models;

public class AppBalance
{
    public decimal Value { get; set; }
    
    public AppBalance(decimal value)
    {
        Value = value;
    }
}