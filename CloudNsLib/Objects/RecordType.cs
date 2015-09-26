using System.Diagnostics.CodeAnalysis;

namespace CloudNsLib.Objects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RecordType
    {
        A, AAAA, MX, CNAME, TXT, NS, SRV, SSHFP, SPF
    }
}