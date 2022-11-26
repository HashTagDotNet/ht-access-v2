namespace HT.Access.Admin.Service.Schema.Models
{
    public class CommandOptions
    {
        public bool ContinueOnError { get; set; }

        /// <summary>
        /// If true and system encounters conflict, then exit command with success status.  Usefully when seeding directory with potentially existing data
        /// </summary>
        public bool IgnoreIfExists { get; set; }
    }
}
