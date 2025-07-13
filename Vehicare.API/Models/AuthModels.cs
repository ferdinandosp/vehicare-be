namespace Vehicare.API.Models;

public class LoginPayload
{
    public string? Token { get; set; }
    public User? User { get; set; }
    public string? Error { get; set; }
    public bool Success => !string.IsNullOrEmpty(Token) && User != null;
}

public class RegisterPayload
{
    public User? User { get; set; }
    public string? Error { get; set; }
    public bool Success => User != null && string.IsNullOrEmpty(Error);
}
