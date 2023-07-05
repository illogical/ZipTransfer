using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZipTransfer
{
    public class ConfigService
    {
        private const string _configurationFileName = "Configuration.json";
        private Configuration _configuration;
        private LoggerService _logger;

        public ConfigService(LoggerService logger)
        {
            _logger = logger;
        }

        public async Task<Configuration> GetConfiguration()
        {
            if(_configuration == null)
            {
                _configuration = await LoadConfiguration();
            }
            return _configuration;
        }

        private async Task<Configuration> LoadConfiguration()
        {
            try
            {
                Configuration config = await JsonFileReader.ReadAsync<Configuration>(_configurationFileName);
                return config;
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error loading configuration file: {_configurationFileName}");
                throw;
            }
        }

        public static class JsonFileReader
        {
            public static async Task<T> ReadAsync<T>(string filePath)
            {
                using FileStream stream = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<T>(stream);
            }
        }
    }
}
