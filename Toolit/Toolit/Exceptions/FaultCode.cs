namespace Toolit
{
    public enum FaultCode : int
    {
        Unspecified = 0,
        Set = 1,
        ApiLevelNoLongerSupported = 2,
        Throttling = 3,
        Duplicate = 4,
        WrongPassword = 5,
        NotFound = 6,
        Unauthorized = 7,
        Forbidden = 8,
        IllegalArgument = 9,
        IllegalState = 10,
        Ineligible = 11,
        NoData = 12,
        NoExtra = 13
    }
}
