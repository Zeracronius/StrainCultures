using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures
{
    public class TagValue
    {
        public string tag = "";
        public float value = 0;
        public void LoadDataFromXmlCustom(System.Xml.XmlNode xmlRoot)
        {
            tag = xmlRoot.Name;
            value = float.Parse(xmlRoot.FirstChild.Value);
        }
    }

    public class TagValueRange
    {
        public string tag = "";
        public FloatRange range = new(0, 1);
        public void LoadDataFromXmlCustom(System.Xml.XmlNode xmlRoot)
        {
            tag = xmlRoot.Name;
            var vRange = xmlRoot.FirstChild.Value.Split('~');
            range = new FloatRange(float.Parse(vRange[0]), float.Parse(vRange[1]));
        }
    }
}
