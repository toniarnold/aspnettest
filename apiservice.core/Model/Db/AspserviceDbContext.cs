using Microsoft.EntityFrameworkCore;

namespace apiservice.Model.Db
{
    public partial class AspserviceDbContext : DbContext
    {
        public AspserviceDbContext()
        {
        }

        public AspserviceDbContext(DbContextOptions<AspserviceDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accesscode> Accesscode { get; set; }
        public virtual DbSet<Main> Main { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=HP;Initial Catalog=APISERVICE_DB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accesscode>(entity =>
            {
                entity.HasKey(e => e.Accesscodeid)
                    .IsClustered(false);

                entity.HasIndex(e => e.Phonenumber);

                entity.Property(e => e.Accesscodeid).HasColumnName("accesscodeid");

                entity.Property(e => e.Accesscode1)
                    .IsRequired()
                    .HasColumnName("accesscode")
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Changed)
                    .HasColumnName("changed")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Phonenumber)
                    .IsRequired()
                    .HasColumnName("phonenumber")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Session)
                    .HasColumnName("session")
                    .HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.SessionNavigation)
                    .WithMany(p => p.Accesscode)
                    .HasForeignKey(d => d.Session)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accesscode_Main");
            });

            modelBuilder.Entity<Main>(entity =>
            {
                entity.HasKey(e => e.Session)
                    .IsClustered(false);

                entity.HasIndex(e => e.Clsid);

                entity.HasIndex(e => e.Mainid)
                    .HasName("CIX_Main_mainid")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.Session)
                    .HasColumnName("session")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Changed)
                    .HasColumnName("changed")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Clsid).HasColumnName("clsid");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Main1)
                    .IsRequired()
                    .HasColumnName("main");

                entity.Property(e => e.Mainid)
                    .HasColumnName("mainid")
                    .ValueGeneratedOnAdd();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}