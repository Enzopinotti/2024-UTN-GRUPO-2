using antigal.server.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace antigal.server.Tests;

[TestClass]
public class UnitOfWorkContractAuthorityTests
{
    [TestMethod]
    public void Contract_DoesNotExposeSaveChangesAsync()
    {
        Assert.IsNull(typeof(IUnitOfWork).GetMethod("SaveChangesAsync"));
        Assert.IsNull(typeof(UnitOfWork).GetMethod("SaveChangesAsync"));
    }

    [TestMethod]
    public void Contract_PreservesExplicitTransactionBoundary()
    {
        var interfaceMethod = typeof(IUnitOfWork).GetMethod("BeginTransactionAsync");
        var implementationMethod = typeof(UnitOfWork).GetMethod("BeginTransactionAsync");

        Assert.IsNotNull(interfaceMethod);
        Assert.IsNotNull(implementationMethod);
        Assert.AreEqual(typeof(Task<IDbContextTransaction>), interfaceMethod.ReturnType);
        Assert.AreEqual(typeof(Task<IDbContextTransaction>), implementationMethod.ReturnType);
    }
}
