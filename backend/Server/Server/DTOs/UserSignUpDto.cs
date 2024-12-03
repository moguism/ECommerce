namespace Server.DTOs;

public class UserSignUpDto
{
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
}