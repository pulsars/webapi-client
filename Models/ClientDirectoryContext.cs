using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace ClientDirectory.Models
{
    public partial class ClientDirectoryContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValue(true);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar");

                entity.Property(e => e.JobTitle)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar");

                entity.Property(e => e.PhoneNumbers)
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar");

                entity.HasOne(d => d.IdClientRoleNavigation).WithMany(p => p.Client).HasForeignKey(d => d.IdClientRole).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ClientRole>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar");
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Login).HasForeignKey(d => d.IdClient).OnDelete(DeleteBehavior.Restrict);
            });
        }

        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<ClientRole> ClientRole { get; set; }
        public virtual DbSet<Login> Login { get; set; }
    }
}