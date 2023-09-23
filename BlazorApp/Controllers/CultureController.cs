using System.Globalization;
using Abotti.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abotti.BlazorApp.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class CultureController : Controller
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;

    public CultureController(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Set(string culture)
    {
        try
        {
            var cultureInfo = new CultureInfo(culture);
            var currentUserName = HttpContext.User.Identity?.Name;
            var userQuery = _userRepository.GetByName(currentUserName);
        }
        catch (CultureNotFoundException e)
        {
            _logger.LogError(e, "Invalid user preferred language {Language}", culture);
            return new BadRequestResult();
        }

        return new OkResult();
    }
}