using Etammen.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Etammen.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }
    public IActionResult Error(string message = "Request couldn't be processed due to a server error")
    {
        return View("Error", new ErrorViewModel { Message = message });
    }

    [Route("/StatusCodeError/{statusCode}")]
    public IActionResult StatusCodeError(int statusCode = 0)
    {
        if (statusCode == 403)
            return View("Error403", new ErrorViewModel { Message = "unauthorized access", StatusCode = statusCode });

        if (statusCode == 404)
            return View("Error404", new ErrorViewModel { Message = "page not found", StatusCode = statusCode });

        return View("Error", new ErrorViewModel { Message = "Invalid Request" });
    }

}
