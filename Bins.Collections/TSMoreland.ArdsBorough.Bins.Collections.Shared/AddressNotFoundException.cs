//
// Copyright (c) 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Runtime.Serialization;

namespace TSMoreland.ArdsBorough.Bins.Collections.Shared;

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
