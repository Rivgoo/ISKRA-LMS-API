namespace Iskra.Bootstrapper.Options;

public class SecurityHeadersOptions
{
    /// <summary>
    /// Prevents the browser from "sniffing" the MIME type of a response away from the declared content-type.
    /// Default: "nosniff".
    /// </summary>
    public string XContentTypeOptions { get; set; } = "nosniff";

    /// <summary>
    /// A strict CSP for APIs.
    /// 1. 'default-src none': Blocks all scripts/styles/images (API shouldn't send them).
    /// 2. 'frame-ancestors none': Prevents embedding in iframes (Clickjacking).
    /// Default: "default-src 'none'; frame-ancestors 'none';"
    /// </summary>
    public string ContentSecurityPolicy { get; set; } = "default-src 'none'; script-src 'none'; frame-ancestors 'none';";
}