using System;
using System.Text;

namespace OrzAutoEntity.EncodingProviders
{
    public class Cp936EncodingProvider : EncodingProvider
    {
        public static void RegisterProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.RegisterProvider(new Cp936EncodingProvider());
        }

        public override Encoding GetEncoding(string name)
        {
            if (string.Equals(name, "cp936", StringComparison.OrdinalIgnoreCase))
            {
                return Encoding.GetEncoding(936);
            }
            return null;
        }

        public override Encoding GetEncoding(int codepage)
        {
            return null;
        }
    }
}
