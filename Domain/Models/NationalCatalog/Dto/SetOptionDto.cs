namespace Domain.Models.NationalCatalog.Dto;

public class SetOptionDto
{
    public int Id { get; set; }
    public string? SetName { get; set; }
    public string? Gtin { get; set; }
    
    public int? Count { get; set; }
}