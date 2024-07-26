using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Config
{
    public readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

    public Config(string filePath)
    {
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=');
            if (parts.Length == 2)
            {
                _settings[parts[0].Trim()] = parts[1].Trim();
            }
        }
    }

    public string Get(string key)
    {
        return _settings.ContainsKey(key) ? _settings[key] : null;
    }

    public bool GetBool(string key)
    {
        if (_settings.ContainsKey(key) && bool.TryParse(_settings[key], out var result))
        {
            return result;
        }
        return false;
    }

    public int GetInt(string key)
    {
        if (_settings.ContainsKey(key) && int.TryParse(_settings[key], out var result))
        {
            return result;
        }
        return 0;
    }

    public (string, string) GetFirstTwoValues()
    {
        if (_settings.Count >= 2)
        {
            var firstTwo = _settings.Take(2).ToList();
            return (firstTwo[0].Value, firstTwo[1].Value);
        }
        return (null, null);
    }
}
