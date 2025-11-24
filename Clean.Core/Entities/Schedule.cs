namespace Clean.Core.Entities
{
    public class Schedule
    {
      public int Id { get; set; }
      public List<DateTime> StartWork { get; set; }
      public List<DateTime> EndWork { get;set; }
      public Cleaners Cleaners { get; set; }


    }
}
