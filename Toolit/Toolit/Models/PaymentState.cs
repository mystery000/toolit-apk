namespace Toolit
{
    public enum PaymentState : int
    {
        Initialized = 1,
        Refunded = 2,
        RefundFailed = 3,
        Failed = 4,
        PaidToEscrow = 5,
        Finalized = 6,
        PaidToCraftsman = 7,
        Error = 8,
    }
}
