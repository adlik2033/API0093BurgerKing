namespace API0093BK.DTOs.Common
{
    /// <summary>
    /// Стандартный структурированный ответ API
    /// </summary>
    /// <typeparam name="T">Тип данных в ответе</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ApiResponse()
        {
            Success = true;
        }

        public ApiResponse(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public ApiResponse(string message, bool success = false)
        {
            Success = success;
            Message = message;
        }

        public ApiResponse(List<string> errors, string? message = null)
        {
            Success = false;
            Errors = errors;
            Message = message;
        }
    }

    /// <summary>
    /// Ответ с пагинацией
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// Детальный ответ об ошибке
    /// </summary>
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public List<ValidationError>? ValidationErrors { get; set; }
    }

    /// <summary>
    /// Ошибка валидации поля
    /// </summary>
    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}