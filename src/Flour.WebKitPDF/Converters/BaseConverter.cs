using Flour.WebKitPDF.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flour.WebKitPDF.Converters
{
    internal class BaseConverter : IConverter
    {
        private readonly IUtils _utils;

        public BaseConverter(IUtils utils)
        {
            _utils = utils;
        }

        public byte[] Convert(IDocument document)
        {
            //var currentDocument = document ?? throw new ArgumentNullException(nameof(document));
            //var content = currentDocument.GetObjects();

            //if (content is null || !content.Any())
            //    throw new InvalidOperationException("Cannot process document with empty content");

            //IntPtr converter = PrepareConverter(document);

            return null;
        }

        //private IntPtr PrepareConverter(IDocument document)
        //{
        //    IntPtr converter;
        //    {
        //        IntPtr settings = _utils.CreateGlobalSettings();
        //        ApplyConfig(settings, document, true);
        //        converter = _utils.CreateConverter(settings);
        //    }
        //}

        //private void ApplyConfig(IntPtr config, ISettings settings, bool isGlobal)
        //{
        //    if (settings == null)
        //        return

        //    var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        //    var props = settings.GetType().GetProperties(bindingFlags);

        //    foreach (var prop in props)
        //    {
        //        Attribute[] attrs = (Attribute[])prop.GetCustomAttributes();
        //        object propValue = prop.GetValue(settings);

        //        if (propValue == null)
        //        {
        //            continue;
        //        }
        //        else if (attrs.Length > 0 && attrs[0] is WkHtmlAttribute)
        //        {
        //            var attr = attrs[0] as WkHtmlAttribute;

        //            Apply(config, attr.Name, propValue, isGlobal);
        //        }
        //        else if (propValue is ISettings)
        //        {
        //            ApplyConfig(config, propValue as ISettings, isGlobal);
        //        }

        //    }
        //}
    }
}
