namespace SFA.DAS.Commitments.Api.Types.ProviderPayment
{
    public class ProviderPaymentPriorityItem
    {
        public string ProviderName { get; set; }
        public long ProviderId { get; set; }
        public int PriorityOrder { get; set; }
    }
}
