using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flour.WebKitPDF.Commons
{
    public interface IConverter
    {
        byte[] Convert(IDocument document);
    }
}
