namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Reflects how important the creator of this message considers the message in context
    /// </summary>
    public enum ApiEventStatus
    {
        /// <summary>
        /// No message status set
        /// </summary>
        None = 0,

        /// <summary>
        /// Debug level
        /// </summary>
        Verbose = 1,

        /// <summary>
        /// Information somebody may want to pay attention to
        /// </summary>
        Info = 2,

        /// <summary>
        /// Very important and should be noted by the caller
        /// </summary>
        Warning = 4,

        /// <summary>
        /// A bad condition occured, often a business error rather than a system error (e.g. 'Person already exists in this course')
        /// </summary>
        Error = 8,

        /// <summary>
        /// An unexpected error occured, often a system exception or activity that might have left the system or data in an unstable state
        /// </summary>
        Critical = 16
    }
}
