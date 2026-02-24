namespace Oda.HospitalManagement.Application
{
    public enum ResultType
    {
        Success,
        NotFound,
        Conflict,
        Error
    }

    public class ServiceResult<T>
    {
        public ServiceResult(T? data, ResultType type)
        {
            Data = data;
            Type = type;
        }

        public ServiceResult(string message, ResultType type)
        {
            Message = message;
            Type = type;
        }

        public ServiceResult(T? data, ResultType type, string? message = null)
            : this(data, type)
        {
            Message = message;
        }

        /// <summary>
        /// Data in response. Might be null in case of error. Should not be null if <see cref="ResultType.Success"/> but can be empty
        /// </summary>
        public T? Data { get; }

        /// <summary>
        /// Define status of result.
        /// </summary>
        public ResultType Type { get; }

        /// <summary>
        /// Contains additional data or error in case <see cref="ServiceResult{T}.Type"/> is not  <see cref="ResultType.Success" />
        /// </summary>
        public string? Message { get; set; }
    }
}
