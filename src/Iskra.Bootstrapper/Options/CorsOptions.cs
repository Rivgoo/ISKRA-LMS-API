namespace Iskra.Bootstrapper.Options;

public class CorsOptions
{
    public string DefaultPolicyName { get; set; } = string.Empty;
    public List<CorsPolicyDefinition> Policies { get; set; } = [];
}