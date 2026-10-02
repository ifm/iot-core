namespace ifm.IoTCore.ElementManager.Elements;

using System.Collections.Generic;
using Common;
using Contracts.Elements;
using Contracts.Elements.Formats;

internal sealed class TypeElement(string identifier,
    Format format,
    IEnumerable<string> profiles,
    string uid,
    bool isHidden) : BaseElement(Identifiers.Type, identifier, format, profiles, uid, isHidden), ITypeElement; 
