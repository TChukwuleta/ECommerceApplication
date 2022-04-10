namespace EcommerceApplication.Domain.DTOs.Response
{
    public class ResultResponse
    {
        internal ResultResponse(bool succeeded, IEnumerable<string> messages)
        {
            Succeeded = succeeded;
            Messages = messages.ToArray();
        }

        internal ResultResponse(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        internal ResultResponse(bool succeeded, object result)
        {
            Succeeded = succeeded;
            Entity = result;
        }

        public bool Succeeded { get; set; }

        public object Entity { get; set; }

        public string Message { get; set; }

        public string[] Messages { get; set; }

        public static ResultResponse Success()
        {
            return new ResultResponse(true, new string[] { });
        }
        public static ResultResponse Success(string message)
        {
            return new ResultResponse(true, new string[] { message });
        }

        public static ResultResponse Success(string message, object entity)
        {
            return new ResultResponse(true, entity);
        }

        public static ResultResponse Success(object entity)
        {
            return new ResultResponse(true, entity);
        }

        public static ResultResponse Failure(IEnumerable<string> errors)
        {
            return new ResultResponse(false, errors);
        }

        public static ResultResponse Failure(string error)
        {
            return new ResultResponse(false, error);
        }
    }
}

