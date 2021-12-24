namespace CRMHalalBackEnd.Models
{
    public class Response<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int Code { get; set; }

        public override string ToString()
        {
            return $"{nameof(Data)}: {Data}, {nameof(Message)}: {Message}, {nameof(Success)}: {Success}, {nameof(Code)}: {Code}";
        }
    }
}