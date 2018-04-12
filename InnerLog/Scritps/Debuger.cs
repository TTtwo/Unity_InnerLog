using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Debuger
{

    static public bool EnableLog = true;
    static public bool EnableLogOnScreen = true;

    static public Callback Code
    {
        set
        {
            if (EnableLog)
            {
                if (value != null)
                {
                    value();
                }
            }
        }
    }

    static Debuger()
    {
        if (EnableLogOnScreen)
        {
            GameObject _log = new GameObject("Inner Log");
            _log.AddComponent<LogConsoleWindow>();
        }
    }

    static public void Log(object message)
    {
        Log(message, null);
    }

    static public void Log(object message, LogColor _color)
    {
        string _c = LogColorToHex(_color);
        string _m = "<color=#" + _c + ">" + message + "</color>";
        Log(_m, null);
    }
    static public void Log(object message, Object context, LogColor _color)
    {
        string _c = LogColorToHex(_color);
        string _m = "<color=#" + _c + ">" + message + "</color>";
        Log(_m, context);
    }
    static public void Log(object message, Object context)
    {
        if (EnableLog)
        {
            Debug.Log(message, context);
        }
    }
    static public void LogError(object message)
    {
        LogError(message, null);
    }
    static public void LogError(object message, Object context)
    {
        if (EnableLog)
        {
            Debug.LogError(message, context);
        }
    }
    static public void LogWarning(object message)
    {
        LogWarning(message, null);
    }
    static public void LogWarning(object message, Object context)
    {
        if (EnableLog)
        {
            Debug.LogWarning(message, context);
        }
    }

    private static string LogColorToHex(LogColor _color)
    {
        switch (_color.ToString())
        {
            case "Red" :
                return "ff0000ff";
            case "Green":
                return "008000ff";
            case "Blue":
                return "0000ffff";
            case "Aqua":
                return "00ffffff";
            case "Black":
                return "000000ff";
            case "Brown":
                return "a52a2aff";
            case "Cyan":
                return "00ffffff";
            case "Darkblue":
                return "0000a0ff";
            case "Fuchsia":
                return "ff00ffff";
            case "Grey":
                return "808080ff";
            case "Lightblue":
                return "add8e6ff";
            case "Lime":
                return "00ff00ff";
            case "Magenta":
                return "ff00ffff";
            case "Maroon":
                return "800000ff";
            case "Navy":
                return "000080ff";
            case "Olive":
                return "808000ff";
            case "Orange":
                return "ffa500ff";
            case "Purple":
                return "800080ff";
            case "Silver":
                return "c0c0c0ff";
            case "Teal":
                return "008080ff";
            case "Yellow":
                return "ffff00ff";

            default:
            return null;
        }
    }

}

public enum LogColor
{
    Red,Green,Blue,Aqua, Black,Brown,
    Cyan, Darkblue,Fuchsia,Grey,Lightblue, 
    Lime,Magenta,Maroon, Navy, Olive,Orange,
    Purple, Silver, Teal, Yellow 
}