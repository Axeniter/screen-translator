using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator.Service
{
    public class OcrLanguage
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsInstalled { get ; set; }
        public long SizeInBytes { get; set; }
        public string DownladUrl { get; set; }

    }

    public class OcrService
    {
    }
}
