using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipTransfer.Models;

namespace ZipTransfer.Services
{
    public class ArgService
    {
        private LoggerService _logger;
        private List<ArgOption> availableArgs;
        private Dictionary<string, string> parsedArgs;


        public ArgService(LoggerService logger)
        {
            availableArgs = new List<ArgOption>();
            parsedArgs = new Dictionary<string, string>();

            DefineAvailableArgs();
            
            _logger = logger;

        }

        private void DefineAvailableArgs()
        {
            availableArgs.Add(new ArgOption("h", "help", "Show help"));

            availableArgs.Add(new ArgOption("s", "source", "Source path", true));
            availableArgs.Add(new ArgOption("d", "destination", "Destination path", true));
            availableArgs.Add(new ArgOption("t", "temp", "Temp path", true));
            availableArgs.Add(new ArgOption("c", "configuration", "Configuration json file path", true));
            availableArgs.Add(new ArgOption("v", "versions", "(Optional) Max number of versions"));
            availableArgs.Add(new ArgOption("da", "delete-after", "(Optional) Delete source files after zip successfully transfers"));
            availableArgs.Add(new ArgOption("zs", "zip-subdirectories", "(Optional) Max number of versions"));
        }

        public string GetHelpText()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Required arguments:");
            foreach (var arg in availableArgs.Where(a => a.IsRequired))
            {
                sb.AppendLine(arg.GetOptionStringWithDescription());
            }

            sb.AppendLine("Available optional arguments:");
            foreach (var arg in availableArgs.Where(a => !a.IsRequired))
            {
                sb.AppendLine(arg.GetOptionStringWithDescription());
            }

            return sb.ToString();
        }
        public string? GetArgValue(string argOption)
        {
            return parsedArgs[argOption];
        }

        public string? GetArgValueByTitle(string argTitle)
        {
            var matchedArg = availableArgs.FirstOrDefault(a => a.Title == argTitle);
            if(matchedArg == null || !parsedArgs.ContainsKey(matchedArg.Option))
            {
                throw new Exception($"Arg with title {argTitle} not found");
            }

            return parsedArgs[matchedArg.Option];
        }

        public void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if(arg.StartsWith("-"))
                {
                    ParseArgValuePair(arg, args[++i]);
                }
            }

            if (!ValidateArgs())
            {
                _logger.WriteError("Invalid arguments");
                return;
            }
        }

        private void ParseArgValuePair(string arg, string value)
        {
            var argNoDash = arg.Substring(1).ToLower();
            if(availableArgs.Any(a => a.Option == argNoDash))
            {
                SetArgValue(argNoDash, value);
            }
        }

        private bool ValidateArgs()
        {
            // check that all of the required args are present in the parsedArgs dictionary
            bool isValid = true;

            foreach(var arg in availableArgs)
            {
                if (arg.IsRequired && !parsedArgs.ContainsKey(arg.Option))
                {
                    _logger.WriteError($"{arg.Option} is required");
                    isValid = false;
                }
            }

            return isValid;
        }

        private void SetArgValue(string argOption, string value)
        {
            // TODO: run validation on the value
            //GetArgOptionByOption(argOption).Validate();

            parsedArgs[argOption.ToLower()] = value;
        }

        //private ArgOption? GetArgOptionByTitle(string title)
        //{
        //    return availableArgs.FirstOrDefault(a => a.Title == title);
        //}

        private ArgOption? GetArgOptionByOption(string option)
        {
            return availableArgs.FirstOrDefault(a => a.Option == option);
        }
    }
}
