using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace webapp.Helpers;

/// <summary>
/// Simplifies the creation of a MARCXML document.
/// </summary>
public class MarcxmlBuilder
{
    public MarcxmlRecord Record { get; set; } = new();

    /// <summary>
    /// Add an existing <see cref="Datafield" />
    /// </summary>
    public MarcxmlBuilder Add(Datafield datafield)
    {
        Record.Datafields.Add(datafield);
        return this;
    }

    /// <summary>
    /// Create and add a <see cref="Datafield" />. See params for more details.
    /// </summary>
    /// <param name="tag">MARC tag</param>
    /// <param name="subfields">variadic number of tuples of format (code, value)</param>
    /// <example>
    /// Example usage:
    /// <code>
    /// builder.Add("110", ("a", "PubRAMS: A project document publishing system"));
    /// </code>
    /// </example>
    public MarcxmlBuilder Add(
        string tag,
        (string, string)[] subfields,
        string ind1 = " ",
        string ind2 = " "
    )
    {
        List<Subfield> subfields1 = new();
        foreach ((string, string) subfield in subfields)
        {
            subfields1.Add(new() { Code = subfield.Item1, Value = subfield.Item2 });
        }
        Record.Datafields.Add(new() {
            Tag = tag,
            Ind1 = ind1,
            Ind2 = ind2,
            Subfields = subfields1
        });
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