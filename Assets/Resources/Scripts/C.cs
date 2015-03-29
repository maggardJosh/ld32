using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class C
{
    public const string versionNumber = "v1.1";

    public static bool isTransitioning = false;

    public const string smallFontName = "smallFont";
    public const string smallDarkFontName = "smallFontDark";

    public static readonly KeyCode[] LEFT_KEY = new KeyCode[] { KeyCode.LeftArrow, KeyCode.A };
    public static readonly KeyCode[] RIGHT_KEY = new KeyCode[] { KeyCode.RightArrow, KeyCode.D };
    public static readonly KeyCode[] UP_KEY = new KeyCode[] { KeyCode.UpArrow, KeyCode.W };
    public static readonly KeyCode[] DOWN_KEY = new KeyCode[] { KeyCode.DownArrow, KeyCode.S };
    public static readonly KeyCode[] JUMP_KEY = new KeyCode[] { KeyCode.X, KeyCode.L };
    public static readonly KeyCode[] ACTION_KEY = new KeyCode[] { KeyCode.Z, KeyCode.K };

    private static float lastVerticalValue = 0;
    public static bool getUpPress()
    {
        bool upPressed = lastVerticalValue >= 0 && Input.GetAxis("Vertical") < 0;
        lastVerticalValue = Input.GetAxis("Vertical");
        return upPressed || Input.GetButtonDown("Action");
    }
    public static bool getJumpPress() { return Input.GetButtonDown("Jump"); }

    public static bool getKey(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKey(key))
                return true;
        return false;
    }

    public static bool getKeyDown(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKeyDown(key))
                return true;
        return false;
    }

    internal static bool getStartPressed()
    {
        return Input.GetButtonDown("Start");
    }
}
