using System;

namespace ifm.IoTCore.Common;

/// <summary>
/// Specifies the access permissions.
/// </summary>
[Flags]
public enum AccessPermission
{
    /// <summary>Read access.</summary>
    Read = 0x1,
    /// <summary>Write access.</summary>
    Write = 0x2,
    /// <summary>Execute access.</summary>
    Execute = 0x4
}

