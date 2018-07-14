namespace Framework.Services.Core {
    public static class SharedResource {
        public const string InvalidArgument = "Invalid Argument";
        public const string InvalidArgumentLog = "Invalid Argument in Executing Method {0}";
        public const string UnexpectedError = "Unexpected Error occure";
        public const string UnexpectedErrorLog = "Unexpected Error in Executing Method {0} with Exception {1}";
        public const string BeginMethodExecution = "Begin Method {0} Execution";
        public const string EndMethodExecution = "End Method {0} Execution";
        public const string RecordNotFound = "Record was not found";
        public const string RecordNotFoundLog = "Record was not found in Executing Method {0}";
        public const string RecordAlreadyExist = "Record already exist";
        public const string RecordAlreadyExistLog = "Record already exist in Executing Method {0}";
        public const string UnableToSendEmailLog = "Unable To Send Email in Executing Method {0}";
        public const string UnableToSendSmsLog = "Unable To Send Sms in Executing Method {0}";
        public const string UnableToVerifyToken = "Unable To Verify Token";
        public const string UnableToVerifyTokenLog = "Unable To Verify Token in Executing Method {0}";
        public const string UnableToLookupService = "Unable To Lookup Service";
        public const string UnableToLookupServiceLog = "Unable To Lookup Service in Executing Method {0}";
        public const string UnableToCallService = "Unable To Call Service";
        public const string UnableToCallServiceLog = "Unable To Call Service in Executing Method {0}";

        public const string AttemptToSaveData = "Attempt to Save Data";
        public const string DataSaved = "Data has been saved!";
        public const string ConcurrencyFailure = "Unable to save data changes due to concurrency check violation";
        public const string DbUpdateFailure = "Unable to save data changes";
        public const string TransactionRequired = "A transaction is required!";
    }
}