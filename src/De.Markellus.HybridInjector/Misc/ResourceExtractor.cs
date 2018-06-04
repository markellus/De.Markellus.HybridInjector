using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace De.Markellus.HybridInjector.Misc
{
    internal static class ResourceExtractor
    {
        public static byte[] Extract(Assembly assembly, string resourceName)
        {
            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                IDictionaryEnumerator dict = reader.GetEnumerator();
                while (dict.MoveNext())
                {
                    if ((string)dict.Key == resourceName)
                    {
                        using (Stream streamInner = (Stream)dict.Value)
                        {
                            byte[] bytes = new byte[streamInner.Length];
                            streamInner.Read(bytes, 0, bytes.Length);
                            return bytes;
                        }
                    }
                }
            }

            using (Stream resFilestream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resFilestream == null)
                {
                    return null;
                }

                byte[] bytes = new byte[resFilestream.Length];
                resFilestream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }
    }
}
