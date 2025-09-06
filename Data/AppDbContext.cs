using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PreprocessingScriptStore> PreprocessingScriptStores { get; set; }
    public DbSet<ProcessData> ProcessDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName != null)
            {
                entity.SetTableName(tableName.ToLower());
            }

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (columnName != null)
                {
                    property.SetColumnName(columnName.ToLower());
                }
            }
        }

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PreprocessingScriptStore>()
            .HasIndex(p => p.Name)
            .IsUnique();
    }

}
