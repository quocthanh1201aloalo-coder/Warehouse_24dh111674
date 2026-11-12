namespace Warehouse_24dh111674.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class Warehouse_24dh111674Entities : DbContext
    {
        public Warehouse_24dh111674Entities()
            : base("name=Warehouse_24dh111674Entities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Product> Products { get; set; }
    }
}
