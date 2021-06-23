using System;

namespace Sefeko.AzureFunctionsTest.Common
{
    public class EnvironmentSettingsHelper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        }
    }
}
