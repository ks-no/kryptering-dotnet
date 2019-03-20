using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KS.Fiks.Crypto.Tests
{
    public static class TestDataUtil
    {
        public static string GetContentFromResource(string resource)
        {
            using (var stream = GetContentStreamFromResource(resource))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream GetContentStreamFromResource(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str =>
                str.EndsWith(resource, StringComparison.CurrentCulture));
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}