using System.Globalization;
using Abotti.Core.Repositories;
using Microsoft.AspNetCore.Localization;
using ILogger = Serilog.ILogger;

namespace Abotti.BlazorApp.Localization;

public class RepositoryCultureProvider : RequestCultureProvider
{
    private readonly CultureInfo _defaultCulture;
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;

    public RepositoryCultureProvider(IUserRepository userRepository, CultureInfo defaultCulture, ILogger logger)
    {
        _userRepository = userRepository;
        _defaultCulture = defaultCulture;
        _logger = logger;
    }

    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var culture = _defaultCulture;
        if (httpContext.User?.Identity?.IsAuthenticated == false)
            return await Task.FromResult(new ProviderCultureResult(culture.Name));

        var userName = httpContext.User.Identity.Name;
        var queryResult = await _userRepository.GetByNameAsync(userName);

        if (queryResult.Ok)
        {
            var user = queryResult.Result;

            try
            {
                if (user != null)
                    culture = new CultureInfo(user.PreferredLanguage);
            }
            catch (CultureNotFoundException e)
            {
                _logger.Warning(e, "Invalid user preferred language {Language}", user.PreferredLanguage);
            }
        }

        return await Task.FromResult(new ProviderCultureResult(culture.Name));
    }
}