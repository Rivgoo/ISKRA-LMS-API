using System.Net;
using System.Text.Json.Serialization;

namespace Iskra.Application.Results;

/// <summary>
/// Represents a specific error that occurred during an operation, encapsulating details such as a code, description, and type.
/// </summary>
/// <remarks>
/// Instances of this class are immutable after creation and should be constructed using the provided static factory methods
/// (e.g., <see cref="Failure"/>, <see cref="NotFound"/>) to ensure correct categorization with an <see cref="ErrorType"/>.
/// This class is fundamental to the application's railway-oriented programming approach for handling operation outcomes.
/// </remarks>
public class Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with specific details.
    /// This constructor is private to enforce the use of static factory methods for consistent error object creation.
    /// </summary>
    /// <param name="code">The unique, machine-readable code identifying the error (e.g., "User.NotFound").</param>
    /// <param name="description">A human-readable description of the error. Can be a format string if <paramref name="args"/> are provided.</param>
    /// <param name="errorType">The <see cref="ErrorType"/> categorizing the error.</param>
    /// <param name="args">Optional arguments to be formatted into the <paramref name="description"/> string.</param>
    private Error(
        string code,
        string description,
        ErrorType errorType,
        params object[] args
    )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
        Args = args?.Length > 0 ? [.. args] : null;

        Description = FormatDescription(code, description, Args);
    }

    /// <summary>
    /// Gets the unique, machine-readable code identifying the specific error.
    /// </summary>
    /// <value>A string representing the error code (e.g., "Validation.RequiredField", "Resource.Conflict").</value>
    public string Code { get; }

    /// <summary>
    /// Gets a human-readable description of the error, potentially formatted with arguments.
    /// </summary>
    /// <value>A string providing details about what went wrong.</value>
    public string Description { get; }

    /// <summary>
    /// Gets the category or type of the error as defined by <see cref="ErrorType"/>.
    /// </summary>
    /// <value>An <see cref="ErrorType"/> value (e.g., NotFound, BadRequest, Failure).</value>
    public ErrorType ErrorType { get; }

    /// <summary>
    /// Gets the list of arguments used to format the <see cref="Description"/>, if any were provided.
    /// </summary>
    /// <value>A <see cref="List{T}"/> of <see cref="object"/> containing the arguments, or <see langword="null"/> if no arguments were used. This property is ignored during JSON serialization if null.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<object>? Args { get; }

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing a general failure.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A description or format string for the failure.</param>
    /// <param name="args">Optional arguments for formatting the <paramref name="description"/>.</param>
    /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.Failure"/>.</returns>
    public static Error Failure(string code, string description, params object[] args) =>
        new(code, description, ErrorType.Failure, args);

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing a 'resource not found' scenario.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A description or format string for the 'not found' error.</param>
    /// <param name="args">Optional arguments for formatting the <paramref name="description"/>.</param>
    /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.NotFound"/>.</returns>
    public static Error NotFound(string code, string description, params object[] args) =>
        new(code, description, ErrorType.NotFound, args);

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing a validation error or invalid request.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A description or format string for the bad request.</param>
    /// <param name="args">Optional arguments for formatting the <paramref name="description"/>.</param>
    /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.BadRequest"/>.</returns>
    public static Error BadRequest(string code, string description, params object[] args) =>
        new(code, description, ErrorType.BadRequest, args);

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing a conflict, such as a duplicate resource.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A description or format string for the conflict.</param>
    /// <param name="args">Optional arguments for formatting the <paramref name="description"/>.</param>
    /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.Conflict"/>.</returns>
    public static Error Conflict(string code, string description, params object[] args) =>
        new(code, description, ErrorType.Conflict, args);

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing an unauthorized access attempt (authentication required).
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A description or format string for the unauthorized error.</param>
    /// <param name="args">Optional arguments for formatting the <paramref name="description"/>.</param>
    /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.AccessUnAuthorized"/>.</returns>
    public static Error Unauthorized(string code, string description, params object[] args) =>
        new(code, description, ErrorType.AccessUnAuthorized, args);

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing a forbidden access attempt (insufficient permissions).
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A description or format string for the forbidden error.</param>
    /// <param name="args">Optional arguments for formatting the <paramref name="description"/>.</param>
    /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.AccessForbidden"/>.</returns>
    public static Error AccessForbidden(string code, string description, params object[] args) =>
        new(code, description, ErrorType.AccessForbidden, args);

    /// <summary>
    /// Maps the current <see cref="ErrorType"/> to its corresponding <see cref="HttpStatusCode"/>.
    /// </summary>
    /// <returns>The <see cref="HttpStatusCode"/> that best represents the <see cref="ErrorType"/>.</returns>
    /// <remarks>This is useful for translating internal application errors to standard HTTP status codes in API responses.</remarks>
    public HttpStatusCode ToHttpStatusCode()
    {
        return ErrorType switch
        {
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.BadRequest => HttpStatusCode.BadRequest,
            ErrorType.Conflict => HttpStatusCode.Conflict,
            ErrorType.AccessUnAuthorized => HttpStatusCode.Unauthorized,
            ErrorType.AccessForbidden => HttpStatusCode.Forbidden,
            ErrorType.Failure => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
    }

    /// <summary>
    /// Returns a string representation of the error, including its code, description, and type.
    /// </summary>
    /// <returns>A string in the format: "Code: Description (Type: ErrorType)".</returns>
    public override string ToString() => $"{Code}: {Description} (Type: {ErrorType})";

    /// <summary>
    /// Formats the description template with the provided arguments.
    /// Handles potential <see cref="FormatException"/> during string formatting.
    /// </summary>
    /// <param name="code">The error code, used for logging if formatting fails.</param>
    /// <param name="descriptionTemplate">The description string, possibly containing format items (e.g., "{0}").</param>
    /// <param name="args">The list of arguments to insert into the format items. Can be null or empty.</param>
    /// <returns>The formatted description string. If <paramref name="args"/> is null/empty or formatting fails, returns a diagnostic message or the original template.</returns>
    private static string FormatDescription(string code, string descriptionTemplate, List<object>? args)
    {
        if (args == null || args.Count == 0)
            return descriptionTemplate;

        try
        {
            return string.Format(descriptionTemplate, args.ToArray());
        }
        catch (FormatException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting description for code '{code}': {ex.Message}. Description template: '{descriptionTemplate}'. Args: {string.Join(", ", args)}. Using raw description.");
            return $"Error formatting => {descriptionTemplate} (Args: {string.Join(", ", args)})";
        }
    }
}