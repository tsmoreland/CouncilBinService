using TSMoreland.ArdsBorough.Core.ValuesObjects;

namespace TSMoreland.ArdsBorough.Core.Models
{
    public sealed class CollectionAddress
    {

        public CollectionAddress(PostCode postcode, int houseNumber)
        {
            Postcode = postcode;
            HouseNumber = houseNumber;
        }

        public PostCode Postcode { get; }
        public int HouseNumber { get; }
    }
}
