namespace Iskra.Application.Errors;

public sealed class ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base("Validation.General", "One or more validation errors occurred.", ErrorType.BadRequest)
    {
        Errors = errors;
    }

    /// <summary>
    /// Collection of granular errors (e.g. per field).
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Creates a ValidationError from a collection of specific errors.
    /// </summary>
    public static ValidationError FromErrors(IEnumerable<Error> errors) => new([.. errors]);
}