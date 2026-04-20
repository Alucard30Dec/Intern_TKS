namespace BlazorApp1.Models.Common;

/// <summary>
/// Ket qua xu ly khong can tra ve du lieu.
/// </summary>
public sealed class ServiceResult
{
    /// <summary>
    /// Danh dau thao tac co thanh cong hay khong.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Thong diep nghiep vu de hien thi cho nguoi dung.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Tao ket qua thanh cong khong kem du lieu.
    /// </summary>
    public static ServiceResult Ok(string message = "") => new()
    {
        Success = true,
        Message = message
    };

    /// <summary>
    /// Tao ket qua that bai khong kem du lieu.
    /// </summary>
    public static ServiceResult Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}

/// <summary>
/// Ket qua xu ly co kem du lieu.
/// </summary>
public sealed class ServiceResult<T>
{
    /// <summary>
    /// Danh dau thao tac co thanh cong hay khong.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Thong diep nghiep vu de hien thi cho nguoi dung.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Du lieu tra ve khi thao tac thanh cong.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Tao ket qua thanh cong co kem du lieu.
    /// </summary>
    public static ServiceResult<T> Ok(T data, string message = "") => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    /// <summary>
    /// Tao ket qua that bai.
    /// </summary>
    public static ServiceResult<T> Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}
