using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using UT.Data.Extensions;
using UT.Data.IO.Assemblies;
using UT.Data.Modlet;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UT.Data.Efc
{
    public class ServerContext
    {
        #region Members
        private readonly ExtendedDbContext[] contexts;
        #endregion //Members

        #region Constructors
        public ServerContext()
        {
            ExtendedDbContext.Configuration configuration = new(Resources.DbConnectionString, Resources.DbType.AsEnum<ExtendedDbContext.Types>() ?? ExtendedDbContext.Types.Mysql);

            contexts = [.. Loader.GetInstances<ExtendedDbContext>(false, configuration)];

            foreach(ExtendedDbContext context in contexts)
            {
                context.SavingChanges += ExtendedDbContext_SavingChanges;
            }
        }
        #endregion //Constructors

        #region Public Methods
        public void EnsureCreated()
        {
            foreach(ExtendedDbContext context in contexts)
            {
                context.Database.EnsureCreated();
                if (context.Migrate())
                {
                    ExtendedConsole.WriteLine("Updated <red>" + context.GetType().ToString() + "</red>.");
                }
                context.SaveChanges();
            }
        }

        public ExtendedDbContext? Select(IModlet mod)
        {
            return Array.Find(contexts, x => x.GetType().Assembly == mod.GetType().Assembly);
        }
        #endregion //Public Methods

        #region Private Methods
        private void ExtendedDbContext_SavingChanges(object? sender, SavingChangesEventArgs e)
        {
            if (sender is not DbContext context)
            {
                return;
            }

            foreach (EntityEntry entry in context.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
                {
                    continue;
                }

                object entity = entry.Entity;
                Type type = entity.GetType();
                PropertyInfo? transstartdate = type.GetProperty("TransStartDate");
                transstartdate?.SetValue(entity, DateTime.Now);
            }
        }
        #endregion //Private Methods
    }
}
