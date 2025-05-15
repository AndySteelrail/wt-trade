namespace WTTrade.Models;

public abstract class TraderMessages
{
    public static string CancelOrder(string orderName, string type, bool result)
    {
        if (result)
        {
            return  $"{orderName} - заявка {type} отменена успешно"; 
        }
        else
        {
            return  $"{orderName} - заявка {type} не отменена. Возникли проблемы";
        }
    }
    
    public static string CreateOrder(string orderName, string type, decimal price, int amount, bool result)
    {
        if (result)
        {
            if (amount != 1)
            {
                return $"{orderName} - выставлена заявка {type} {amount} шт. Цена за шт: {price} gjn"; 
            }
            return $"{orderName} - выставлена заявка {type}. Цена: {price} gjn";
        }
        else
        {
            return $"{orderName} - не выставлена заявка {type}. Возникли проблемы";
        }
    }
}