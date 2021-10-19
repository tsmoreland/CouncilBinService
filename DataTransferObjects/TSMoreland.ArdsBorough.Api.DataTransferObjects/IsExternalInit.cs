#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Necessary for < NET5.0 builds to support use of init and some of the internals of records
/// </summary>
internal static class IsExternalInit 
{
}
#endif
