namespace ProjectManagement.DTOs
{
    public class ApiResponse
    {
        public object Data { get; set; }
        public List<object> Errors { get; set; }
        public string StatusCode { get; set; }

        public ApiResponse()
        {
            Errors = new List<object>();
        }
    }
}
