using System.Collections.Generic;
using Domain.Models.Crpt.Marking.Response;

namespace Domain.Models.Crpt.Marking.Dto;

public class MarkingListDto
{
    public string? CisesType { get; set; }
    public List<GetCisesResult>? Cises { get; set; }
    
    public required string Gtin { get; set; }
}