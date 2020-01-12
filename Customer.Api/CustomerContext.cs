using Microsoft.EntityFrameworkCore;

namespace Customer.Api
{
    public class CustomerContext : DbContext
    {

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        public DbSet<Model.Customer> Customers { get; set; }
    }
}
