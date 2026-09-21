using antigal.server.Controllers;
using antigal.server.Repositories;
using antigal.server.Services;

namespace antigal.server.Tests;

[TestClass]
public class PaymentNotificationSurfaceTests
{
    [TestMethod]
    public void PaymentController_DoesNotExposeLegacyNotificationAction()
    {
        Assert.IsNull(typeof(PaymentController).GetMethod("ReceiveNotification"));
    }

    [TestMethod]
    public void PaymentServiceContract_DoesNotExposeLegacyNotificationHandler()
    {
        Assert.IsNull(typeof(IPaymentService).GetMethod("HandlePaymentNotificationAsync"));
    }

    [TestMethod]
    public void PaymentRepositoryContract_DoesNotExposeUnverifiedStatusMutation()
    {
        Assert.IsNull(typeof(IPaymentRepository).GetMethod("UpdatePaymentStatusAsync"));
    }
}
