namespace ifm.IoTCore.Profile.DeviceTag;

using System;
using System.Collections.Generic;
using DataStore.Contracts;
using ElementManager.Contracts;
using ElementManager.Contracts.Elements;

/// <summary>
/// Implements the device tag profile.
/// </summary>
/// <param name="elementManager">The element manager.</param>
/// <param name="parentElement">The parent element.</param>
/// <param name="dataStore">The data store.</param>
public class DeviceTagProfile(IElementManager elementManager, IBaseElement parentElement, IDataStore dataStore)
{
    private readonly IElementManager _elementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));
    private readonly IBaseElement _parentElement = parentElement ?? throw new ArgumentNullException(nameof(parentElement));
    private readonly IDataStore _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));

    private class Settings
    {
        public string ApplicationTag { get; set; }
        public string FunctionTag { get; set; }
        public string LocationTag { get; set; }
    }
    private Settings _settings;

    private const string ProfileName = "devicetag";
    private readonly List<string> _parameterProfile = ["parameter"];

    private const string DataStoreSection = "devicetag";
    private const string DataStoreKey = "settings";

    /// <summary>Gets the application tag.</summary>
    public string ApplicationTag => _settings.ApplicationTag;

    /// <summary>Gets the function tag.</summary>
    public string FunctionTag => _settings.FunctionTag;

    /// <summary>Gets the location tag.</summary>
    public string LocationTag => _settings.LocationTag;

    /// <summary>
    /// Builds the profile elements.
    /// </summary>
    public void Build()
    {
        _settings = _dataStore.Get<Settings>(DataStoreSection, DataStoreKey) ?? new Settings();

        var profileElement = _parentElement.GetElementByIdentifier(ProfileName) ?? _elementManager.CreateStructureElement(null, ProfileName, profiles: new List<string> { ProfileName });

        _elementManager.CreateDataElement(profileElement,
            "applicationtag",
            _ => _settings.ApplicationTag,
            (_, v) =>
            {
                _settings.ApplicationTag = v;
                _dataStore.Set(DataStoreSection, DataStoreKey, _settings);
            },
            profiles: _parameterProfile);

        _elementManager.CreateDataElement(profileElement,
            "functiontag",
            _ => _settings.FunctionTag,
            (_, v) =>
            {
                _settings.FunctionTag = v;
                _dataStore.Set(DataStoreSection, DataStoreKey, _settings);
            },
            profiles: _parameterProfile);

        _elementManager.CreateDataElement(profileElement,
            "locationtag",
            _ => _settings.LocationTag,
            (_, v) =>
            {
                _settings.LocationTag = v;
                _dataStore.Set(DataStoreSection, DataStoreKey, _settings);
            },
            profiles: _parameterProfile);

        _elementManager.AddElement(_parentElement, profileElement);
    }
}