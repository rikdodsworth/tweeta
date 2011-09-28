
namespace Tweeta.Common.Logging
{
    public interface ILogger
    {
        void Initialize();

        void DeInitialize();

        void Write(string value);
    }

}
