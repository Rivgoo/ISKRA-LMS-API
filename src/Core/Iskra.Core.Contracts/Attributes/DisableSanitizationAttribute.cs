namespace Iskra.Core.Contracts.Attributes;

/// <summary>
/// Apply this to a string property to bypass automatic HTML sanitization.
/// Use for Passwords, Secrets, or raw JSON strings.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DisableSanitizationAttribute : Attribute
{
}