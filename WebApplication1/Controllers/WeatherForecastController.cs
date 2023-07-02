using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetExampleViewModel")]
    public ExampleVm GetExampleViewModel()
    {
        return new ExampleVm();
    }

    [HttpGet(Name = "GetExampleDomainModel")]
    public ExampleDomainModel GetExampleDomainModel()
    {
        return new ExampleDomainModel();
    }
}