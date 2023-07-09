using System.Text.Json;
using ZipTransfer.Models;

namespace ZipTransfer.Services
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
            if (_configuration == null)
            {
                _configuration = await LoadConfiguration();
            }
            return _configuration;
        }

        public async Task<Configuration> GetConfiguration(string configurationPath)
        {
            if (File.Exists(configurationPath))
            {
                _configuration = await LoadConfiguration(configurationPath);
            }
            else
            {
                string errormessage = $"Error: {configurationPath} does not exist";
                _logger.WriteError(errormessage);
                throw new Exception(errormessage);
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

        private async Task<Configuration> LoadConfiguration(string configurationPath)
        {
            try
            {
                Configuration config = await JsonFileReader.ReadAsync<Configuration>(configurationPath);
                return config;
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error loading configuration file: {configurationPath}");
                throw;
            }
        }

        public void AddNewTransfer(Transfer transfer)
        {
            _configuration.Transfers.Add(transfer);

            string jsonString = JsonSerializer.Serialize(_configuration);
            string fileName = _configurationFileName;
            File.WriteAllText(fileName, jsonString);
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
