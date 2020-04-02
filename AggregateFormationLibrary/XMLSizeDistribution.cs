using System;
using System.Xml.Serialization;

namespace AggregateFormation
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class XMLSizeDistribution<T>
    {

        [XmlElement(ElementName = "size")]
        public Size<T>[] Sizes { get; set; }
    }

    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class Size<T>
    {
        [XmlElement(ElementName = "value")]
        public T Value { get; set;}

        [XmlElement(ElementName = "probability")]
        public double Probability { get; set; }
    }

}
