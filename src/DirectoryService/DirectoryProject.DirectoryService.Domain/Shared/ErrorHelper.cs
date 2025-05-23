namespace DirectoryProject.DirectoryService.Domain.Shared;

public static class ErrorHelper
{
    public static class General
    {
        private const string DEFAULT_LABEL_VALUE = "value";

        public static Error MethodNotApplicable(string? mes = null)
        {
            return Error.Failure(
                "method.not.applicable",
                mes ?? "Called method is not applicable");
        }

        public static Error ValueIsInvalid(string? name = null)
        {
            var label = name ?? DEFAULT_LABEL_VALUE;
            return Error.Validation("value.is.invalid", $"{label} is invalid", name);
        }

        public static Error ValueIsNullOrEmpty(string? name = null)
        {
            var label = name ?? DEFAULT_LABEL_VALUE;
            return Error.Validation("value.is.null.or.empty", $"{label} is null or empty", name);
        }

        public static Error NotFound(Guid? id = null)
        {
            var label = id == null ? string.Empty : $" for Id: {id}";
            return Error.Validation("record.not.found", $"record not found{label}");
        }

        public static Error NotFound(string? message = null)
        {
            if (message is null)
                return Error.Validation("record.not.found", $"record not found");

            return Error.Validation("record.not.found", message);
        }

        public static Error AlreadyExist(string? name = null)
        {
            var label = name ?? DEFAULT_LABEL_VALUE;
            return Error.Conflict("record.exists", $"{label} exists");
        }
    }
}
