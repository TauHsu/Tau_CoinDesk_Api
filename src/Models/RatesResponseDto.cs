public class RatesResponseDto
{
    public string UpdatedTime { get; set; } = string.Empty;
    public List<RateItemDto> Rates { get; set; } = new List<RateItemDto>();
}

public class RateItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Rate { get; set; } = string.Empty;
}