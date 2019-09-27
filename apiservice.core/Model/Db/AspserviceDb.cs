//usng Microsoft.EntityFrameworkCore.Infrastructure;
/*
Re-generate the AspserviceDbContext from the project directory with:

dotnet ef dbcontext scaffold -o Db -c AspserviceDbContext -f "Data Source=HP;Initial Catalog=APISERVICE_DB;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer

Afterwards delete the protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) method
to remove the hard-coded connection string.
 */

namespace apiservice.Model.Db
{
    public class AspserviceDb : IAspserviceDb
    {
        public const int ACCESSCODE_LENGTH = 6;    // dbo.Accesscode.accesscode

        internal AspserviceDbContext _DbContext;

        public AspserviceDb(AspserviceDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public AspserviceDb()
        {
        }

        public long Insert(Accesscode accesscode)
        {
            _DbContext.Accesscode.Add(accesscode);
            _DbContext.SaveChanges();
            return accesscode.Accesscodeid;
        }
    }
}