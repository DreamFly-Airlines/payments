using Microsoft.EntityFrameworkCore;

namespace Payments.Infrastructure.Persistence;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext()
    {
    }

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}