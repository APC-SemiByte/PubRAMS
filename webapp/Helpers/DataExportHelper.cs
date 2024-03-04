using CsvHelper;
using CsvHelper.Configuration.Attributes;

using System.Globalization;

namespace webapp.Helpers;

public class DataExportBuilder
{
    public List<ProjectData> Projects { get; set; } = new();

    public DataExportBuilder() { Projects = new(); }

    public DataExportBuilder(List<ProjectData> data) { Projects = data; }

    public DataExportBuilder Add(ProjectData data)
    {
        Projects.Add(data);
        return this;
    }

    public DataExportBuilder Add(List<ProjectData> data)
    {
        Projects.AddRange(data);
        return this;
    }

    public void Save(string path)
    {
        using StreamWriter writer = new(path);
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(Projects);
    }
}

public class ProjectData
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required string Tags { get; set; }

    public required string Category { get; set; }

    public required bool Continued { get; set; }

    public required bool Completed { get; set; }

    public required bool Archived { get; set; }

    public required string SoftwareState { get; set; }

    public required string Group { get; set; }

    public required string School { get; set; }

    public required string Adviser { get; set; }

    public DateTime? PublishDate { get; set; }

    public required string Term { get; set; }
}