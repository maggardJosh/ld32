using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class C
{
    private static FCamObject camera;
    public static FCamObject getCameraInstance()
    {
        if (camera == null)
            camera = new FCamObject();
        return camera;
    }

    public const bool isDebug = false;
    public const float PIOVER180 = 3.1415f / 180f;

    public static readonly List<int> WALL_LIST = new List<int> { -1, 1,3,4,5,6,7,8,10,11,12,13,14,16,17,18,19,20,21,22,23,24,26,28,30,31,32,33,34,35,36,46,49,51};
    public static readonly List<int> HOOK_LIST = new List<int> { 37,38,39,40 };
    public static readonly List<int> ONE_WAY_LIST = new List<int> { 50 };
    public static readonly List<int> DEATH_LIST = new List<int> { 52 };

    public const string versionNumber = "v1.0";

    public static bool isTransitioning = false;

    public const string smallFontName = "smallFont";
    public const string smallDarkFontName = "smallFontDark";

    public static readonly KeyCode[] LEFT_KEY = new KeyCode[] { KeyCode.LeftArrow, KeyCode.A };
    public static readonly KeyCode[] RIGHT_KEY = new KeyCode[] { KeyCode.RightArrow, KeyCode.D };
    public static readonly KeyCode[] UP_KEY = new KeyCode[] { KeyCode.UpArrow, KeyCode.W };
    public static readonly KeyCode[] DOWN_KEY = new KeyCode[] { KeyCode.DownArrow, KeyCode.S };
    public static readonly KeyCode[] JUMP_KEY = new KeyCode[] { KeyCode.X, KeyCode.L, KeyCode.Space };
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

    public static bool getKeyUp(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKeyUp(key))
                return true;
        return false;
    }

    internal static bool getStartPressed()
    {
        return Input.GetButtonDown("Start");
    }
}
