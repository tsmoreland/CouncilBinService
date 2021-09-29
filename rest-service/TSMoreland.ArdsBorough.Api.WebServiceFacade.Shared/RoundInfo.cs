namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

/// <summary>
/// Round Info 
/// </summary>
/// <param name="BinName">Bin Name/Type</param>
/// <param name="NextDate">Next Date</param>
/// <param name="Frequency">how often bin is collected</param>
/// <remarks>
/// 
/// </remarks>
public sealed record RoundInfo(string BinName, string NextDate, string Frequency);
