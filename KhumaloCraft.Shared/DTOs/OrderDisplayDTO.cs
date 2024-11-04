namespace KhumaloCraft.Shared.DTOs;

public class OrderDisplayDTO
{
  public string? OrderId { get; set; }
  public string UserId { get; set; }
  public DateTime? OrderDate { get; set; }
  public int StatusId { get; set; }
  public string StatusName { get; set; }
  public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
  public UserDTO User { get; set; }
}

