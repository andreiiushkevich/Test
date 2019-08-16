namespace Test.Common.Dtos
{
    public class OperationResult<T>
    {
        public OperationResult(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}