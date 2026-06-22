namespace FileStorage.Domain.Constants
{
    public static class ErrorCodes
    {
        public const string ValidationError = "ERR_4000";

        public const string QuotaExceeded = "ERR_4001";
        public const string FileTooLarge = "ERR_4002";
        public const string InvalidFileType = "ERR_4003";

        public const string Unauthorized = "ERR_4011";
        public const string InvalidCredentials = "ERR_4012";
        public const string TokenExpired = "ERR_4013";

        public const string Forbidden = "ERR_4031";

        public const string UserNotFound = "ERR_4041";
        public const string FileNotFound = "ERR_4042";

        public const string FileAlreadyExists = "ERR_4091";

        public const string InternalError = "ERR_5001";
    }
}