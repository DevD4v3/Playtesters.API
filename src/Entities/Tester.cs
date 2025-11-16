namespace Playtesters.API.Entities;

public class Tester
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = Guid.NewGuid().ToString();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
