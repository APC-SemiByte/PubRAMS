using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace webapp.Helpers;

public class MarcxmlBuilder
{
    public MarcxmlRecord Record { get; set; } = new();

    public MarcxmlBuilder Add(Datafield datafield)
    {
        Record.Datafields.Add(datafield);
        return this;
    }

    public MarcxmlBuilder Add(string tag, params (string, string)[] subfields)
    {
        List<Subfield> subfields1 = new();
        foreach ((string, string) subfield in subfields)
        {
            subfields1.Add(new() { Code = subfield.Item1, Value = subfield.Item2 });
        }
        Record.Datafields.Add(new() { Tag = tag, Subfields = subfields1 });
        return this;
    }

    public override string ToString()
    {
        XmlSerializer serializer = new(typeof(MarcxmlRecord));
        using Utf8StringWriter stringWriter = new();
        XmlWriterSettings settings = new() { Indent = true };
        using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);
        serializer.Serialize(xmlWriter, Record);
        return stringWriter.ToString()!;
    }
}

[XmlRoot(ElementName = "record")]
public class MarcxmlRecord
{
    [XmlElement(ElementName = "datafield")]
    public List<Datafield> Datafields { get; set; } = new();
}

[XmlRoot(ElementName = "datafield")]
public class Datafield
{
    [XmlAttribute(AttributeName = "tag")]
    public required string Tag { get; set; }

    [XmlAttribute(AttributeName = "ind1")]
    public string Ind1 { get; set; } = " ";

    [XmlAttribute(AttributeName = "ind2")]
    public string Ind2 { get; set; } = " ";

    [XmlElement(ElementName = "subfield")]
    public List<Subfield> Subfields { get; set; } = new();
}

[XmlRoot(ElementName = "subfield")]
public class Subfield
{
    [XmlAttribute(AttributeName = "code")]
    public required string Code { get; set; }

    [XmlText]
    public required string Value { get; set; }
}

public class Utf8StringWriter : StringWriter
{
    private readonly Encoding encoding = Encoding.UTF8;

    public override Encoding Encoding => encoding;
}