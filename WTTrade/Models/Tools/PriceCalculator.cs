namespace WTTrade.Models;

public class PriceCalculator
{
    private Dictionary<int, decimal> _sellPrices;
    private Dictionary<int, decimal> _buyPrices;
    private readonly decimal _exchangeCommission;
    private readonly decimal _sellerCommission;
    private readonly decimal _discount;
    private readonly decimal _profitRate;

    public PriceCalculator()
    {
        _sellPrices = new ();
        _buyPrices = new ();
        _exchangeCommission = 0.15m;
        _sellerCommission = 1 - _exchangeCommission;
        _discount = 100m; // >= 100
        _profitRate = 0.005m;
    }

    public void ClearSellPriceDictionary()
    {
        _sellPrices.Clear();
    }

    public void ClearBuyPriceDictionary()
    {
        _buyPrices.Clear();
    }

    public decimal CalcSellPrice(WTOrderBrief wtOrderBrief, int value)
    {
        if (_sellPrices.ContainsKey(value))
        {
            return _sellPrices[value];
        }
        
        decimal bestSellPrice = 1000000m;
        if (wtOrderBrief.response.sell.Count > 0)
        {
            bestSellPrice = wtOrderBrief.response.sell[0][0];
        }
                        
        decimal secondSellPrice = bestSellPrice;
        if (wtOrderBrief.response.sell.Count > 1)
        {
            secondSellPrice = wtOrderBrief.response.sell[1][0];
        }
                        
        decimal bestBuyPrice = 1000m;
        if (wtOrderBrief.response.buy.Count > 0)
        {
            bestBuyPrice = wtOrderBrief.response.buy[0][0];
        }
        
        decimal minSalesThreshold = Math.Ceiling(bestBuyPrice / 0.85m / 100) * 100;

        if (bestSellPrice < minSalesThreshold)
        {
            bestSellPrice = Math.Max(secondSellPrice, minSalesThreshold + _discount);
        }
        _sellPrices[value] = bestSellPrice;

        return bestSellPrice;
    }

    public decimal CalcBuyPrice(WTOrderBrief wtOrderBrief, int value)
    {
        if (_buyPrices.ContainsKey(value))
        {
            return _buyPrices[value];
        }
        
        decimal bestBuyPrice = 1000m;
        if (wtOrderBrief.response.buy.Count > 0)
        {
            bestBuyPrice = wtOrderBrief.response.buy[0][0];
        }
                        
        decimal secondBuyPrice = bestBuyPrice;
        if (wtOrderBrief.response.buy.Count > 1)
        {
            secondBuyPrice = wtOrderBrief.response.buy[1][0];
        }
                        
        decimal bestSellPrice = 1000000m;
        if (wtOrderBrief.response.sell.Count > 0)
        {
            bestSellPrice = wtOrderBrief.response.sell[0][0];
        }
        
        decimal maxBuyThreshold = Math.Floor(bestSellPrice * 0.85m / 100) * 100;

        if (bestBuyPrice > maxBuyThreshold)
        {
            bestBuyPrice = Math.Min(secondBuyPrice, maxBuyThreshold - _discount);
        }
        _buyPrices[value] = bestBuyPrice;

        return bestBuyPrice;
    }

    public decimal CalcDiscountPrice(decimal price, string type)
    {
        if (type == "на продажу")
        {
            return price - _discount;
        }
        return price + _discount;
    }

    public decimal CalcAfterCommissionPrice(decimal price)
    {
        decimal priceAfterCommission = Math.Floor( (price * _sellerCommission) / 100) * 100;
        return priceAfterCommission;
    }

    public int CalcAllowableAmount(int amount, decimal price, decimal balance)
    {
        int possibleAmount = (int)Math.Floor( balance / price );
        return Math.Min(amount, possibleAmount);
    }
}