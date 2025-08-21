namespace Application.Dtos
{
    public class LoginResult
    {
        public UserResponseDto User { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

}
