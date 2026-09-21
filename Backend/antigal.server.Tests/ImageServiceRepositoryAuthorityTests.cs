using antigal.server.Repositories;
using antigal.server.Services;
using CloudinaryDotNet;

namespace antigal.server.Tests;

[TestClass]
public class ImageServiceRepositoryAuthorityTests
{
    [TestMethod]
    public void Constructor_UsesCloudinaryAndUnitOfWorkOnly()
    {
        var constructors = typeof(ImageService).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        CollectionAssert.AreEqual(
            new[] { typeof(Cloudinary), typeof(IUnitOfWork) },
            constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }
}
