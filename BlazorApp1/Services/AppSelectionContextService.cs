using BlazorApp1.Models.Kho;
using BlazorApp1.Models.KhoUser;
using BlazorApp1.Services.Interfaces;

namespace BlazorApp1.Services;

/// <summary>
/// Quan ly trang thai chon user/kho o muc header cho toan bo man hinh.
/// </summary>
public sealed class AppSelectionContextService : IAppSelectionContextService
{
    private readonly IKhoService _khoService;
    private readonly IKhoUserService _khoUserService;
    private readonly ILogger<AppSelectionContextService> _logger;
    private readonly List<KhoListItemVm> _allKhos = [];
    private readonly List<KhoUserListItemVm> _khoUserMappings = [];
    private readonly List<string> _loginOptions = [];
    private readonly List<KhoListItemVm> _khoOptions = [];
    private bool _isInitialized;

    public AppSelectionContextService(
        IKhoService khoService,
        IKhoUserService khoUserService,
        ILogger<AppSelectionContextService> logger)
    {
        _khoService = khoService;
        _khoUserService = khoUserService;
        _logger = logger;
    }

    public IReadOnlyList<string> LoginOptions => _loginOptions;
    public IReadOnlyList<KhoListItemVm> KhoOptions => _khoOptions;
    public string SelectedLogin { get; private set; } = string.Empty;
    public int? SelectedKhoId { get; private set; }
    public string SelectedKhoName => _khoOptions.FirstOrDefault(x => x.Kho_ID == SelectedKhoId)?.Ten_Kho ?? string.Empty;

    public event Action? SelectionChanged;

    public async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            return;
        }

        await ReloadAsync(cancellationToken);
    }

    public async Task SetSelectedLoginAsync(string loginCode, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        var normalizedLogin = NormalizeLogin(loginCode);
        if (!_loginOptions.Contains(normalizedLogin, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        if (string.Equals(SelectedLogin, normalizedLogin, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        SelectedLogin = normalizedLogin;
        RebuildKhoOptionsForSelectedLogin();
        OnSelectionChanged();
    }

    public async Task SetSelectedKhoAsync(int khoId, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        if (_khoOptions.All(x => x.Kho_ID != khoId))
        {
            return;
        }

        if (SelectedKhoId == khoId)
        {
            return;
        }

        SelectedKhoId = khoId;
        OnSelectionChanged();
    }

    private async Task ReloadAsync(CancellationToken cancellationToken)
    {
        try
        {
            _allKhos.Clear();
            _khoUserMappings.Clear();
            _loginOptions.Clear();
            _khoOptions.Clear();

            var getKhoTask = _khoService.GetAllAsync(cancellationToken);
            var getKhoUserTask = _khoUserService.GetAllAsync(cancellationToken);
            await Task.WhenAll(getKhoTask, getKhoUserTask);

            _allKhos.AddRange(getKhoTask.Result.OrderBy(x => x.Ten_Kho));
            _khoUserMappings.AddRange(getKhoUserTask.Result);

            _loginOptions.AddRange(
                _khoUserMappings
                    .Select(x => NormalizeLogin(x.Ma_Dang_Nhap))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x));

            if (_loginOptions.Count == 0)
            {
                _loginOptions.Add("ADMIN");
            }

            if (!_loginOptions.Contains(SelectedLogin, StringComparer.OrdinalIgnoreCase))
            {
                SelectedLogin = _loginOptions[0];
            }

            RebuildKhoOptionsForSelectedLogin();
            _isInitialized = true;
            OnSelectionChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Load app selection context failed.");

            // Fallback toi thieu de UI van su dung duoc khi service kho/kho-user gap loi.
            if (_loginOptions.Count == 0)
            {
                _loginOptions.Add("ADMIN");
            }

            if (_allKhos.Count == 0)
            {
                _allKhos.Add(new KhoListItemVm
                {
                    Kho_ID = 0,
                    Ten_Kho = "Kho mặc định"
                });
            }

            if (string.IsNullOrWhiteSpace(SelectedLogin))
            {
                SelectedLogin = _loginOptions[0];
            }

            _khoOptions.Clear();
            _khoOptions.AddRange(_allKhos);
            SelectedKhoId = _khoOptions.FirstOrDefault()?.Kho_ID;
            _isInitialized = true;
            OnSelectionChanged();
        }
    }

    private void RebuildKhoOptionsForSelectedLogin()
    {
        _khoOptions.Clear();

        if (_khoUserMappings.Count == 0)
        {
            _khoOptions.AddRange(_allKhos);
        }
        else
        {
            var allowedKhoIds = _khoUserMappings
                .Where(x => string.Equals(
                    NormalizeLogin(x.Ma_Dang_Nhap),
                    SelectedLogin,
                    StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Kho_ID)
                .Distinct()
                .ToHashSet();

            _khoOptions.AddRange(_allKhos.Where(x => allowedKhoIds.Contains(x.Kho_ID)));
        }

        if (_khoOptions.Count == 0)
        {
            SelectedKhoId = null;
            return;
        }

        if (!SelectedKhoId.HasValue || _khoOptions.All(x => x.Kho_ID != SelectedKhoId.Value))
        {
            SelectedKhoId = _khoOptions[0].Kho_ID;
        }
    }

    private void OnSelectionChanged()
    {
        SelectionChanged?.Invoke();
    }

    private static string NormalizeLogin(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().ToUpperInvariant();
    }
}
