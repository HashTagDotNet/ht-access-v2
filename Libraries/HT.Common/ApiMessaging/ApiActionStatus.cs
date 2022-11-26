namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Reflects the outcome of a call to action. Logically simple result of a WebApi controller.action call but often returned from a service layer and thus could refect
    /// the outcome of a service call.
    /// </summary>
    public enum ApiActionStatus
    {
        Success=0,
        Error=1,
        Denied=2
    }
}
