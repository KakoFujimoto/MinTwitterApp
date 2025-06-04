namespace MinTwitterApp.DTO;

public class ErrorPageDTO
{
    public string? RequestId { get; set; }

    public bool HasRequestId => !string.IsNullOrEmpty(RequestId);
}
