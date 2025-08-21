namespace Application.Dtos;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public List<string> Roles { get; set; } = []!;
    //public string AccessToken { get; set; } = null!;
}
