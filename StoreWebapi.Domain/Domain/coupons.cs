using Microsoft.EntityFrameworkCore;

namespace StoreWebapi.Domain.Domain;
public class coupons
{
    
    public  Guid couponId { get; set; } = Guid.NewGuid();
    public string code { get; set; }
    public int quantity { get; set; }
    public DateTime  expiryDate { get; set; }
    public DateTime createDate { get; set; } = DateTime.UtcNow;
    public ICollection<couponUser>  couponUsers { get; set; }
    public decimal Discount_percentage { get; set; }
    public bool isExpired { get; set; } = false;
    public bool IsExpired => expiryDate < DateTime.UtcNow;
    

}