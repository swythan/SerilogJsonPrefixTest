using Serilog;
using SerilogJsonPrefixTest;

Console.WriteLine("Hello, World!");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(formatter: new TypePrefixedJsonFormatter())
    .CreateLogger();

Log.Information("String: {Value}", "Serilog");
Log.Warning("Integer: {Value}", 52);
Log.Information("Double: {Value}", Math.PI);
Log.Information("Boolean: {Value}", true);
Log.Warning("DateTime: {Value}", DateTime.UtcNow);
//Log.Information("User: {User}", new Person<Cat>("James", "Chaldecott", new DateTime(1975, 2, 21)));
//Log.Information("User: {@User}", new Person<Cat>("James", "Chaldecott", new DateTime(1975, 2, 21), new[] {new Cat("Gizmo", true)}));

var user = new Person<Pet>(
    "James",
    "Chaldecott",
    new DateTime(1975, 2, 21), 
    new Pet[] 
    {
        new Cat("Gizmo", true),
        new Dog("Ludo", "bubbles"),
    });

using (Serilog.Context.LogContext.PushProperty("UserDetails", user, true))
{
    Log.Warning("User: {User}", user);
}

Log.CloseAndFlush();

public record Person<TPet>(string FirstName, string LastName, DateTime BirthDate, IReadOnlyCollection<TPet>? Pets = default) where TPet : Pet
{
    double AgeYears => (DateTime.Now - BirthDate).TotalDays / 365.25;

    public override string? ToString()
    {
        return $"{FirstName} {LastName} ({AgeYears:F1})";
    }
}

public abstract record Pet(string Name);

public record Cat(string Name, bool IsRatKiller) : Pet(Name);
public record Dog(string Name, string FavouriteToy) : Pet(Name);
