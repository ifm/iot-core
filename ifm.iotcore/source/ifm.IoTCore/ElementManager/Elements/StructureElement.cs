namespace ifm.IoTCore.ElementManager.Elements;

using System.Collections.Generic;
using Common;
using Contracts.Elements;
using Contracts.Elements.Formats;

internal sealed class StructureElement(string identifier,
    Format format,
    IEnumerable<string> profiles,
    string uid,
    bool isHidden) : BaseElement(Identifiers.Structure, identifier, format, profiles, uid, isHidden), IStructureElement;