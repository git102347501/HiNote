namespace HiNote.Service.Models;

public class GetCurrencyOutput
{
    public decimal Amount { get; set; }

    public decimal usedAmount { get; set; }

    public int status { get; set; }
}