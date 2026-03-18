namespace SFA.DAS.SharedOuterApi.Common;

public enum RequestStatus : short
{
    New,
    Sent,
    Accepted,
    Declined,
    Expired,
    Deleted
}
