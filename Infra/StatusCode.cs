namespace HridhayConnect_API.Infra
{
    public static class ResponseStatusCode
    {
        public static int Success { get; set; } = 1;
        public static int Error { get; set; } = 0;
        public static int NotFound { get; set; } = 2;
        public static int Exist { get; set; } = 3;
        public static int Failed { get; set; } = 4;
    }

    public static class ResponseStatusMessage
    {
        public static string Success { get { return "Record saved successfully !..."; } }
        public static string Delete { get { return "Record deleted successfully !..."; } }
        public static string Unable_Delete { get { return "Unable to delete record(s)."; } }
        public static string Error { get { return "Opps!... Something went wrong."; } }
        public static string Exist { get { return "Record allready available."; } }
        public static string NotFound { get { return "No any record found."; } }
        public static string UnAuthorize { get { return "You are not authorized to perform this action."; } }
    }
}
