using NServiceBus;
using SFA.DAS.Apim.Shared.Services;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Apim.Shared.UnitTests.Services;

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