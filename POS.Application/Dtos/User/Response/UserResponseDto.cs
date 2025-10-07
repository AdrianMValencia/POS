namespace POS.Application.Dtos.User.Response;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image {  get; set; }
    public string AuthType { get; set; } = null!;
    public DateTime AuditCreateDate { get; set; }
    public int State { get; set; }
    public string StateUser { get; set; } = null!;
}
