using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

public static class UnityScriptingDefineSymbols
{
    public static bool IsDefined(string symbol)
    {
        var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
        var symbols = GetSymbols(platform);
        return symbols.Contains(symbol);
    }
    
    public static void Add(string symbol, BuildTargetGroup platform)
    {
        var symbols = GetSymbols(platform);
        symbols.Add(symbol);
        SetSymbols(symbols, platform);
    }
    
    public static void Remove(string symbol, BuildTargetGroup platform)
    {
        var symbols = GetSymbols(platform);
        symbols.Remove(symbol);
        SetSymbols(symbols, platform);
    }
    
    private static HashSet<string> GetSymbols(BuildTargetGroup platform)
    {
        var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(platform);
        return new HashSet<string>(PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';'));
    }
    
    private static void SetSymbols(HashSet<string> symbols, BuildTargetGroup platform)
    {
        var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(platform);
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", symbols));
    }

    public static void Toggle(string symbol, BuildTargetGroup platform)
    {
        var symbols = GetSymbols(platform);
        
        if (symbols.Contains(symbol))
        {
            symbols.Remove(symbol);
        }
        else
        {
            symbols.Add(symbol);
        }
        
        SetSymbols(symbols, platform);
    }
}