namespace POS.Application.Dtos.User.Response;

public class UserByIdResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
    public string AuthType { get; set; } = null!;
    public int State { get; set; }
}
