using System;

namespace AzureFunctionsUpdates.Models
{
    public class Toggles
    {
        public const string DoPostUpdateVariableName = "Toggle_DoPostUpdate";

        public static bool DoPostUpdate
        {
            get
            {
                var envDoPostupdate = Environment.GetEnvironmentVariable(DoPostUpdateVariableName);
                if (!string.IsNullOrEmpty(envDoPostupdate) && bool.TryParse(envDoPostupdate, out bool result))
                {
                    return result;
                }

                return false;
            }
        }
    }
}
