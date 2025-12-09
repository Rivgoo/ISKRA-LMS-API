namespace Iskra.Bootstrapper.Options;

public class CorsPolicyDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<string> Origins { get; set; } = [];
    public List<string> Methods { get; set; } = [];
    public List<string> Headers { get; set; } = [];
    public bool AllowCredentials { get; set; }
}