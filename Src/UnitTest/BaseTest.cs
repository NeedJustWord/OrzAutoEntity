using OrzAutoEntity.EncodingProviders;
using OrzAutoEntity.Helpers;

namespace UnitTest
{
    public class BaseTest
    {
        public BaseTest()
        {
            Cp936EncodingProvider.RegisterProvider();
            ConfigHelper.Init("");
        }
    }
}
