using System.Diagnostics;

namespace WP7HelperFX.Common.Logging
{
    public class DefaultLogger : ILogger
    {
        public void Initialize() { }

        public void DeInitialize() { }

        public void Write(string value)
        {
            Debug.WriteLine(value);
        }
    }
}
