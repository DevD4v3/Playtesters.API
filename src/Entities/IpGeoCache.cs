namespace Playtesters.API.Entities;

public class IpGeoCache
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
