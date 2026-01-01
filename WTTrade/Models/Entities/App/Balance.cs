namespace WTTrade.Models;

public class Balance
{
    public decimal Value { get; set; }
    
    public Balance(decimal value)
    {
        Value = value;
    }
}