using System.Diagnostics.CodeAnalysis;

namespace CloudNsLib.Objects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SshfpAlgorithm
    {
        RSA = 1,
        DSA = 2,
        ECDSA = 3
    }
}