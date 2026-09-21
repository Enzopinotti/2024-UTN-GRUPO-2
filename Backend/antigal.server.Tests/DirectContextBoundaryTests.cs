using antigal.server.Data;

namespace antigal.server.Tests;

[TestClass]
public class DirectContextBoundaryTests
{
    [TestMethod]
    public void NonRepositoryAppDbContextConsumers_AreExplicitlyBounded()
    {
        var assembly = typeof(Program).Assembly;

        var consumers = assembly
            .GetTypes()
            .Where(type => type.IsClass)
            .Where(type => type.Namespace?.StartsWith("antigal.server.Repositories", StringComparison.Ordinal) != true)
            .Where(type => type
                .GetConstructors()
                .Any(constructor => constructor
                    .GetParameters()
                    .Any(parameter => parameter.ParameterType == typeof(AppDbContext))))
            .Select(type => type.FullName)
            .Where(name => name is not null)
            .Cast<string>()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(Array.Empty<string>(), consumers);
    }
}
