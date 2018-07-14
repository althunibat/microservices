namespace Framework.Common
{
    public class Error
    {
        public static readonly Error ConcurrencyFailure = new Error(1001, "ConcurrencyFailure");
        public static readonly Error DbUpdateFailure = new Error(1002, "DbUpdateFailure");
        public static readonly Error GenericFailure = new Error(1003, "GenericFailure");

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }
        public string Message { get; }
        public string Description { get; set; }
    }
}