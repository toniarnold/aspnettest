using Microsoft.EntityFrameworkCore;

namespace apiservice.Model.Db
{
    public partial class AspserviceDbContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Main>(entity =>
            {
                entity.Property(e => e.Changed)
                     .ValueGeneratedOnAddOrUpdate();    // updated by TRG_Main_changed
            });

            modelBuilder.Entity<Accesscode>(entity =>
            {
                entity.Property(e => e.Changed)
                     .ValueGeneratedOnAddOrUpdate();    // updated by TRG_Accesscode_changed
            });
        }
    }
}