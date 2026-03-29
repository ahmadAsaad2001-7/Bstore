using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebapi.Domain.Domain;
public class TransactionConfiguration : IEntityTypeConfiguration<transaction>
{
    public void Configure(EntityTypeBuilder<transaction> builder)
    {
        builder.Property(t => t.amount).HasPrecision(18, 2);
        builder.Property(t => t.currency).HasMaxLength(3);
       builder.HasKey(t => t.id);

        
        builder.HasOne(t => t.buyer)
            .WithMany()
            .HasForeignKey(t => t.vendorId)
            .OnDelete(DeleteBehavior.Restrict); 

       
        builder.HasOne(t => t.vendor)
            .WithMany() 
            .HasForeignKey(t => t.vendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.amount).HasPrecision(18, 2);

        builder.HasOne(t => t.book)
              .WithMany(b => b.Transactions)
              .HasForeignKey(t => t.bookId);
    }
}