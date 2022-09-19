using HT.Access.Admin.Service.LDAP.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HT.Access.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ILdifBuilder _builder;
        private readonly ILdifRunner _runner;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ILdifBuilder builder, ILdifRunner runner)
        {
            _logger = logger;
            _builder = builder;
            _runner = runner;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken=default)
        {

            _builder.AddEntry("do=test domain", "top", "domain");
            await _runner.Run(_builder.Commands).ConfigureAwait(false);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}