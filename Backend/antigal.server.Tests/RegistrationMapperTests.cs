using antigal.server.Mapping;
using antigal.server.Models.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class RegistrationMapperTests
{
    [TestMethod]
    public void ToUser_MapsRegistrationIdentityFields()
    {
        var source = new UserForRegistrationDto
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.test"
        };

        var user = RegistrationMapper.ToUser(source);

        Assert.AreEqual(source.FirstName, user.FirstName);
        Assert.AreEqual(source.LastName, user.LastName);
        Assert.AreEqual(source.Email, user.Email);
    }

    [TestMethod]
    public void ToUser_UsesEmailAsUserName()
    {
        var source = new UserForRegistrationDto
        {
            Email = "user@example.test"
        };

        var user = RegistrationMapper.ToUser(source);

        Assert.AreEqual(source.Email, user.UserName);
    }
}
