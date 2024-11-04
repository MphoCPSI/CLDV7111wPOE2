namespace KhumaloCraft.Data.Entities
{
  public class Order
  {
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public string UserId { get; set; }

    public User User { get; set; }

    public int StatusId { get; set; }

    public Status Status { get; set; }
  }
}