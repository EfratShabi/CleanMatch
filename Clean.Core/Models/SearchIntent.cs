namespace Clean.Core.Models
{
    public record SearchIntent(DateOnly? Date, TimeOnly? Start, double? Hours, int? MaxPrice)
    {
        public SearchField? NextMissingField =>
            Date is null ? SearchField.Date :
            Start is null ? SearchField.Start :
            Hours is null ? SearchField.Hours :
            null;
    }
}
