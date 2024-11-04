using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Data.Entities
{
  public class Status
  {
    public int StatusId { get; set; }

    [Required]
    [MaxLength(50)]
    public string StatusName { get; set; }
  }
}