namespace Test.Common.Contracts
{
    public interface IOperationResult<T>
    {
        T Data { get; set; }
    }
}