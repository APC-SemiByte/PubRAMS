using System.Xml.Linq;

namespace webapp.Helpers;

public class MarcxmlBuilder
{
    private XElement Record { get; set; } = new("record");

    public MarcxmlBuilder Add(params XElement[] datafield)
    {
        Record.Add(datafield);
        return this;
    }

    public MarcxmlBuilder Add(string tag, params (string, string)[] subfields)
    {
        Record.Add(MarcxmlHelper.CreateDatafield(tag, subfields));
        return this;
    }

    public XDocument Build()
    {
        return new(Record);
    }
}

public static class MarcxmlHelper
{
    public static XElement CreateDatafield(string tag, params XElement[] subfields)
    {
        XElement datafield = new("datafield");
        foreach (XElement subfield in subfields)
        {
            datafield.Add(subfield);
        }
        datafield.SetAttributeValue("tag", tag);
        datafield.SetAttributeValue("ind1", " ");
        datafield.SetAttributeValue("ind2", " ");
        return datafield;
    }

    public static XElement CreateDatafield(string tag, params (string, string)[] subfields)
    {
        XElement datafield = new("datafield");
        foreach ((string, string) subfield in subfields)
        {
            datafield.Add(CreateSubfield(subfield.Item1, subfield.Item2));
        }
        datafield.SetAttributeValue("tag", tag);
        datafield.SetAttributeValue("ind1", " ");
        datafield.SetAttributeValue("ind2", " ");
        return datafield;
    }

    public static XElement CreateSubfield(string code, string value)
    {
        XElement subfield = new("subfield", value);
        subfield.SetAttributeValue("code", code);
        return subfield;
    }
}