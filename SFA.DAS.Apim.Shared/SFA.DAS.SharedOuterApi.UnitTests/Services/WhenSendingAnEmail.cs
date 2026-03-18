using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.SharedOuterApi.Services;

namespace SFA.DAS.SharedOuterApi.UnitTests.Services;

public class WhenSendingAnEmail
{
    [Test, MoqAutoData]
    public async Task Then_Sends_Correct_Message_To_Notification_Service(
        SendEmailCommand email,
        [Frozen] Mock<IMessageSession> mockMessageSession,
        NotificationService service)
    {
        await service.Send(email);

        mockMessageSession.Verify(session => session.Send(email, It.IsAny<SendOptions>()));
    }
}