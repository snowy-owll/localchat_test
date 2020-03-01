using System.Data.Entity;

namespace DBLocalChat
{
    public class DBLocalChatContext : DbContext
    {
        public DBLocalChatContext(string connectionStringName)
            :base($"name={connectionStringName}") { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.Client)
                .HasForeignKey(e => e.IdClient)
                .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);
        }
    }
}
