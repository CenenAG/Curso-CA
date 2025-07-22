namespace CleanArchitecture.Infrastructure.Email;

public class GmailSettings
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
}

