using antigal.server.Mapping;
using antigal.server.Models.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class UserRegistrationMapperTests
{
    [TestMethod]
    public void ToUser_PreservesRegistrationIdentityFields()
    {
        var source = new UserForRegistrationDto
        {
            FirstName = "Ana",
            LastName = "Gomez",
            Email = "ana@example.test",
            Password = "not-mapped",
            ConfirmPassword = "not-mapped",
            ClientUri = "https://example.test/confirm"
        };

        var user = UserRegistrationMapper.ToUser(source);

        Assert.AreEqual(source.FirstName, user.FirstName);
        Assert.AreEqual(source.LastName, user.LastName);
        Assert.AreEqual(source.Email, user.Email);
        Assert.AreEqual(source.Email, user.UserName);
    }

    [TestMethod]
    public void ToUser_NullSource_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => UserRegistrationMapper.ToUser(null!)
        );
    }
}
