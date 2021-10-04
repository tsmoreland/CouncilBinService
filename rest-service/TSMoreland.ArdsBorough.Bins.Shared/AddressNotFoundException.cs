using System;
using System.Runtime.Serialization;

namespace TSMoreland.ArdsBorough.Bins.Shared;

[Serializable]
public sealed class AddressNotFoundException : Exception
{

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNotFoundException"/> class
    /// with address details
    /// </summary>
    /// <param name="houseNumber">House or Building number for the address</param>
    /// <param name="postCode">Address Postcode</param>
    public AddressNotFoundException(int houseNumber, PostCode postCode)
        : this(houseNumber, postCode, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNotFoundException"/> class
    /// with address details, a specified error message 
    /// </summary>
    /// <param name="houseNumber">House or Building number for the address</param>
    /// <param name="postCode">Address Postcode</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public AddressNotFoundException(int houseNumber, PostCode postCode, string? message)
        : this(houseNumber, postCode, message, null)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="AddressNotFoundException"/> class
    /// with address details, a specified error message and a reference to the inner
    /// exception that is the cause of this exception.
    /// </summary>
    /// <param name="houseNumber">House or Building number for the address</param>
    /// <param name="postCode">Address Postcode</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference
    /// (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public AddressNotFoundException(int houseNumber, PostCode postCode, string? message, Exception? innerException)
        : base(message, innerException)
    {
        HouseNumber = houseNumber;
        PostCode = postCode;
    }

    /// <summary>
    /// Initializes a new instance of the Exception class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds the serialized object
    /// data about the exception being thrown
    /// .</param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains contextual information
    /// about the source or destination.
    /// </param>
    private AddressNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        HouseNumber = info.GetInt32(nameof(HouseNumber));
        var postCode = info.GetString(nameof(PostCode)) ?? string.Empty;
        PostCode = new PostCode(postCode);
    }

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue(nameof(HouseNumber), HouseNumber);
        info.AddValue(nameof(PostCode), PostCode.Value);
    }

    /// <summary>
    /// House or Building number for the address
    /// </summary>
    public int HouseNumber { get; }

    /// <summary>
    /// Address Postcode
    /// </summary>
    public PostCode PostCode { get; }
}
