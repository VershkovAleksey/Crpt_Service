using Domain.Models.Crpt.Marking.Enums;

namespace Domain.Models.NationalCatalog.Dto;

public class CreatedSetsDto
{
    public int Id { get; set; }
    public required string SetName { get; set; }
    public required string Gtin { get; set; }
    public required int Count { get; set; }
    public string? Response { get; set; }
    
    public string Date { get; set; }
    public string Status { get; set; }
}