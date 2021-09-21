using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMoreland.ArdsBorough.Core.Models
{
    public sealed class CollectionAddress
    {

        public CollectionAddress(string postcode, int houseNumber)
        {
            Postcode = postcode;
            HouseNumber = houseNumber;
        }

        public string Postcode { get; }
        public int HouseNumber { get; }
    }
}
