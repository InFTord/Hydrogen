using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Hydrogen.Database
{
    public sealed class DatabaseContext : DbContext, IDesignTimeDbContextFactory<DatabaseContext>
{
    public DbSet<TagModel> Tags { get; init; } = null!;
    // Required by `dotnet ef`
    public DatabaseContext() { }

    // Standard EFCore convention.
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    // Required by EFCore.
    public DatabaseContext CreateDbContext(string[] args) => new(((DbContextOptionsBuilder<DatabaseContext>)CreateFromArgs(args)).Options);

    public static DbContextOptionsBuilder CreateFromArgs(string[] args, DbContextOptionsBuilder? optionsBuilder = null)
    {
        IConfiguration? configuration = Program.LoadConfiguration(args);
        if (configuration == null)
        {
            Console.WriteLine("Failed to load configuration due to unknown errors.");
            Environment.Exit(1); // Respect the Linux users!
        }

        optionsBuilder ??= new DbContextOptionsBuilder<DatabaseContext>();
        NpgsqlConnectionStringBuilder connectionBuilder = new()
        {
            Host = configuration.GetValue("database:host", "localhost"),
            Port = configuration.GetValue("database:port", 5432),
            Database = configuration.GetValue("database:database", "discord_bot"),
            Username = configuration.GetValue("database:user", "discord_bot"),
            Password = configuration.GetValue<string>("database:password")
        };
        optionsBuilder.UseNpgsql(connectionBuilder.ToString(), options => options.EnableRetryOnFailure(5)); // Attempt to retry 5 times.
        optionsBuilder.UseSnakeCaseNamingConvention(CultureInfo.InvariantCulture); // We use snake_case here to match PostgreSQL convention.

        return optionsBuilder;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => CreateFromArgs(Array.Empty<string>(), optionsBuilder);
}

}