using UnityEngine;

public class ControllerInputHelper
{
    static readonly public bool isWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;

    static public bool IsYButtonDown()
    {
        return GetButtonDown("js0", "js3");
    }

    static public bool IsYButtonPressed()
    {
        return GetButton("js0", "js3");
    }

    static public bool IsYButtonUp()
    {
        return GetButtonUp("js0", "js3");
    }

    static public bool IsXButtonPressed()
    {
        return GetButton("js1", "js2");
    }

    static public bool IsXButtonDown()
    {
        return GetButtonDown("js1", "js2");
    }

    static public bool IsXButtonUp()
    {
        return GetButtonUp("js1", "js2");
    }

    static public bool IsBDown()
    {
        return GetButtonDown("js10", "js5");
    }

    static public bool IsBPressed()
    {
        return GetButton("js10", "js5");
    }

    static public bool IsBUp()
    {
        return GetButtonUp("js10", "js5");
    }

    static public bool IsADown()
    {
        return GetButtonDown("js8", "js10");
    }

    static public bool IsAPressed()
    {
        return GetButton("js8", "js10");
    }

    static public bool IsAUp()
    {
        return GetButtonUp("js8", "js10");
    }

    static public bool IsOKDown()
    {
        return GetButtonDown("js3", "js0");
    }

    static public bool IsOKPressed()
    {
        return GetButton("js3", "js0");
    }

    static public bool IsOKUp()
    {
        return GetButtonUp("js3", "js0");
    }

    static private bool GetButtonDown(string windowsName, string androidName)
    {
        if (isWindows)
        {
            return Input.GetButtonDown(windowsName);
        }
        return Input.GetButtonDown(androidName);
    }

    static private bool GetButton(string windowsName, string androidName)
    {
        if (isWindows)
        {
            return Input.GetButton(windowsName);
        }
        return Input.GetButton(androidName);
    }

    static private bool GetButtonUp(string windowsName, string androidName)
    {
        if (isWindows)
        {
            return Input.GetButtonUp(windowsName);
        }
        return Input.GetButtonUp(androidName);
    }
}
