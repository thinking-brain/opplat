namespace Opplat.Microservices.Shared.Dtos;

public class ResponseDto
{
    public bool Status { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];
}
