using System.ComponentModel.DataAnnotations;

namespace webapp.Models.Dtos;

public class BiblioDto
{
    /* 100 - CREATOR */

    [Display(Name = "100-a. Preferred name of the person (lead/first author)")]
    public required string Lead { get; set; }

    /* 245 - TITLE AND STATEMENT OF REPONSIBILITY */

    [Display(Name = "245-a. Preferred title proper")]
    public required string Title { get; set; }

    [Display(Name = "245-b. Remainder of title (subtitle)")]
    public string? Subtitle { get; set; }

    [Display(Name = "245-c. Statement of responsibility (authors)")]
    public required string Authors { get; set; }

    /* 264: PUBLICATION STATEMENT */

    [Display(Name = "264-1-a. Place of publication")]
    public required string PublishPlace { get; set; }

    [Display(Name = "264-1-b. Publisher's name")]
    public required string Publisher { get; set; }

    [Display(Name = "264-4-c. Date of publication")]
    public required string Date { get; set; }

    /* 520: SUMMARY */

    [Display(Name = "520-a. Summary (abstract)")]
    public required string Summary { get; set; }

    /* 650: SUBJECT ADDED ACCESS POINT - TOPICAL TERM */

    [Display(Name = "650-a. Topical term access point")]
    public required string Topic { get; set; }

    [Display(Name = "650-x. General subdivision")]
    public required string Subdivision { get; set; }

    /* 856: ELECTRONIC LOCATION AND ACCESS */

    [Display(Name = "856-u. Uniform resource identifier")]
    public required string Uri { get; set; }

    [Display(Name = "856-y. Link text")]
    public required string LinkText { get; set; }

    /* 942: ADDED ENTRY ELEMENTS */

    [Display(Name = "942-c. Koha item type")]
    public required string ItemType { get; set; }
}

public class BiblioItemDto
{
    [Display(Name = "Internal id of record this item falls under")]
    public required int  Id { get; set; }

    /* 952: HOLDINGS */

    [Display(Name = "952-a. Home library")]
    public required string HomeLibrary { get; set; }

    [Display(Name = "952-b. Current library")]
    public required string CurrentLibrary { get; set; }

    [Display(Name = "952-c. Shelving location")]
    public required string ShelvingLocation { get; set; }

    [Display(Name = "952-o. Full call number")]
    public required string CallNumber { get; set; }

    [Display(Name = "952-p. Accession number")]
    public required string AccessionNumber { get; set; }

    [Display(Name = "952-t. Copy number")]
    public required string CopyNumber { get; set; }

    [Display(Name = "952-y. Koha item type")]
    public required string KohaItemType { get; set; }
}