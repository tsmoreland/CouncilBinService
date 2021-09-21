using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMoreland.ArdsBorough.Core.ValuesObjects
{
    public record class PostCode
    {

        public PostCode(string value)
        {
            Value = value;
        }
        public string Value { get; }

        public void Decontruct(out string value)
        {
            value = Value;
        }

        public static bool IsValue(string postCode)
        {
            // use regex to check validity
            return false;
        }

    }

}
