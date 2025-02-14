using ShelterManagerRedux.Models;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ShelterManagerRedux.DataAccess
{
    public class ShownInterestContext : DbContext
    {
        public ShownInterestContext() : base("ShownInterestContext")
        {
        }
        public ShownInterestContext(string connString) : base("ShownInterestContext")
        {
            this.Database.Connection.ConnectionString = connString;
        }


        public DbSet<ShownInterest> ShownInterests { get; set; }

        public DbSet<ShelterLocation> ShelterLocations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        internal void Refresh(RefreshMode clientWins, object yourentity)
        {
            throw new NotImplementedException();
        }
    }
}
