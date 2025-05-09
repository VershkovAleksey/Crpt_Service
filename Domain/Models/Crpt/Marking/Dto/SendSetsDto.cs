using Domain.Models.Crpt.Marking.Request;

namespace Domain.Models.Crpt.Marking.Dto;

public class SendSetsDto
{
    public required string Token { get; set; }
    public required CreateDocumentBodyRequest Request { get; set; }
}