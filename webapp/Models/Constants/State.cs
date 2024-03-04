using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[Index(nameof(Name))]
public class State
{
    public required int Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(128)]
    public required string Desc { get; set; }

    public int AcceptStateId { get; set; }
    public int RejectStateId { get; set; }
}

public enum States
{
    InitialReview = 1,
    InitialRevisions,
    PrfStart,
    PrfReview,
    ExdReview,
    Assignment,
    Proofreading,
    ProofreadingRevisions,
    PrfCompletion,
    PanelReview,
    PanelRevisions,
    Finalizing,
    Publishing,
    Published
}

[Index(nameof(Name))]
public class Completion
{
    public required int Id { get; set; }

    [MaxLength(16)]
    public required string Name { get; set; }
}

public enum Completions
{
    Unfinished = 1,
    Implemented,
    Deployed,
    Donated,
    Archived
}