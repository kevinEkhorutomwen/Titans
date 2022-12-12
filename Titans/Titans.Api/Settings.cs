using Titans.Contract.Interfaces;

namespace Titans.Api
{
    public class Settings : ISettings
    {
        private readonly IConfiguration _configuration;
        public string Token => _configuration.GetSection("AppSettings:Token").Value!;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
