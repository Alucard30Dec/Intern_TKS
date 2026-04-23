using BlazorApp1.Models.Kho;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Luu trang thai chon user/kho o header va phat su kien khi thay doi.
/// </summary>
public interface IAppSelectionContextService
{
    /// <summary>
    /// Danh sach ma dang nhap dang hoat dong co the chon tren header.
    /// </summary>
    IReadOnlyList<string> LoginOptions { get; }

    /// <summary>
    /// Danh sach kho duoc phep theo user dang chon.
    /// </summary>
    IReadOnlyList<KhoListItemVm> KhoOptions { get; }

    /// <summary>
    /// Ma dang nhap dang duoc chon tren header.
    /// </summary>
    string SelectedLogin { get; }

    /// <summary>
    /// Kho dang duoc chon tren header.
    /// </summary>
    int? SelectedKhoId { get; }

    /// <summary>
    /// Ten kho dang chon de hien thi nhanh tren UI.
    /// </summary>
    string SelectedKhoName { get; }

    /// <summary>
    /// Dam bao du lieu context da duoc tai.
    /// </summary>
    Task EnsureInitializedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chon ma dang nhap tren header.
    /// </summary>
    Task SetSelectedLoginAsync(string loginCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chon kho tren header.
    /// </summary>
    Task SetSelectedKhoAsync(int khoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Su kien phat khi lua chon user/kho thay doi.
    /// </summary>
    event Action? SelectionChanged;
}
