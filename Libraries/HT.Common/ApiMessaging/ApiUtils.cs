using System;

namespace HT.Common.ApiMessaging
{
    public class ApiUtils
    {
        public static string NewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
