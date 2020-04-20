using System;
using System.ComponentModel;
using System.Xml.Serialization;

using ANPaX.AggregateFormation.interfaces;

namespace ANPaX.AggregateFormation
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class XMLSizeDistribution<T> : IFileSizeDistribution<T>
    {

        [XmlElement(ElementName = "size")]
        public Size<T>[] Sizes { get; set; }
    }

    [Serializable()]
    [XmlType(AnonymousType = true)]
    public partial class Size<T>
    {
        [XmlElement(ElementName = "value")]
        public T Value { get; set; }

        [XmlElement(ElementName = "probability")]
        public double Probability { get; set; }
    }

}
