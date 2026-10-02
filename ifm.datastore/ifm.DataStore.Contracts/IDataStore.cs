namespace ifm.DataStore.Contracts;

/// <summary>
/// Provides functionality to interact with a data store.
/// </summary>
public interface IDataStore
{
    /// <summary>
    /// Gets a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the value exists the value; otherwise the default value of T.</returns>
    T Get<T>(string section, string key);

    /// <summary>
    /// Sets a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void Set<T>(string section, string key, T value);

    /// <summary>
    /// Deletes a section.
    /// </summary>
    /// <param name="section">The section to delete.</param>
    void Delete(string section);

    /// <summary>
    /// Deletes a value.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="key">The key.</param>
    void Delete(string section, string key);
}
