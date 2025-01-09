namespace Muazzinweb.Models;

public class ApiResponse
{
    public int Code { get; set; }
    public string? Status { get; set; }
    public Data? Data { get; set; }
}
