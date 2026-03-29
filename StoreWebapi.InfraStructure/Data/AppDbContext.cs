using InfraStructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreWebapi.Domain.Domain;

namespace StoreWebapi.Infrastructure.Data;

public class AppDbContext: IdentityDbContext<user,IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<comment> Comments { get; set; }
    public DbSet<transaction> Transactions { get; set; }
    public DbSet<coupons> Coupons { get; set; }
    public DbSet<couponUser> CouponUsers { get; set; }
    public DbSet<gridCell> GridCells { get; set; }
    public DbSet<vote> Votes { get; set; }
    public DbSet<voteParticipant> VoteParticipants { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        modelBuilder.ApplyConfiguration(new BookConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new userConfiguration());
        modelBuilder.ApplyConfiguration(new VoteConfiguration());
        modelBuilder.ApplyConfiguration(new VoteParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new CouponsConfiguration());
        modelBuilder.ApplyConfiguration(new CouponUserConfiguration());
        modelBuilder.ApplyConfiguration(new GridCellConfiguration());
    }
}