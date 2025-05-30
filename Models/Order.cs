using MineralKingdomApi.Models;

public class Order
{
  public int Id { get; set; }

  public DateTime OrderDate { get; set; }

  public decimal TotalAmount { get; set; }

  // Shipping address snapshot (copied from UserProfile at time of purchase)
  public string ShippingFirstName { get; set; }

  public string ShippingLastName { get; set; }

  public string StreetAddress { get; set; }

  public string City { get; set; }

  public string State { get; set; }

  public string PostalCode { get; set; }

  public string Country { get; set; }

  // Relationship
  public int UserId { get; set; }
  public required User User { get; set; }

  // Future: public List<OrderItem> Items { get; set; } = new();
}
