using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipTransfer.Models
{
    public class ArgOption
    {
        public string Option { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Example { get; set; }
        public bool IsRequired { get; set; }

        public ArgOption(string option, string title, string description, bool isRequired = false)
        {
            Option = option;
            Title = title;
            Description = description;
            IsRequired = isRequired;
        }

        public virtual string GetOptionString()
        {
            return $"-{Option}";
        }

        public virtual string GetOptionStringWithDescription()
        {
            return $"{GetOptionString()} - {Description}";
        }

    }
}
