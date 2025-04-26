namespace Domain.Models.Crpt.Marking.Response;

public class GetCisesResponse
{
    public bool IsLastPage { get; set; }
    public required GetCisesResult[] Result { get; set; }
}

public class GetCisesResult
{
    public required string Sgtin { get; set; }
    public string? Cis { get; set; }
    public string? CisWithoutBrackets { get; set; }
    public string? Gtin { get; set; }
    public string? ProducerInn { get; set; }
    public string? Status { get; set; }
    public string? StatusExt { get; set; }
    public string? EmissionDate { get; set; }
    public string? ApplicationDate { get; set; }
    public string? ProductionDate { get; set; }
    public string? GeneralPackageType { get; set; }
    public string? OwnerInn { get; set; }
    public string? TnVed { get; set; }
    public string? TnVed10 { get; set; }
    public string? EmissionType { get; set; }
    public string? ProductGroup { get; set; }
    public bool HaveChildren { get; set; }
    public string? Parent { get; set; }
    public string? EliminationReason { get; set; }
    public string? ExpirationDate { get; set; }
    public string? PrVetDocument { get; set; }
    public int Mrp { get; set; }
    public string? IntroducedDate { get; set; }
    public string? ReceiptDate { get; set; }
    public string? TurnoverType { get; set; }
    public string? Color { get; set; }
    public string? ProductSize { get; set; }
    public string? Country { get; set; }
    public bool InGrayZone { get; set; }
    public string? ServiceProviderId { get; set; }
    public string? ServiceProviderType { get; set; }
    public string? ReturnType { get; set; }
    public string? EliminationReasonOther { get; set; }
    public string? ModId { get; set; }
    public int ProductWeight { get; set; }
    public GetCisesExpiration[]? Expiration { get; set; }
    public int EmissionCountryCode { get; set; }
    public int ExportCountryCode { get; set; }
    public string? OrderId { get; set; }
    public string? OperationIntroducedDate { get; set; }
}

public class GetCisesExpiration
{
    public string? ExpirationStorageDate { get; set; }
    public string? StorageConditionId { get; set; }
}

