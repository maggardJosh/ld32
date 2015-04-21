﻿using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{
    World world;
    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);
        futileParams.AddResolutionLevel(480.0f, 1.0f, 1.0f, "");

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0, 0, 0);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/inGameAtlas");

        world = new World();
        world.LoadAndSpawn("room4-4", "mapOneSpawn");
        Futile.stage.AddChild(world);

        Futile.atlasManager.LoadFont(C.smallFontName, "debugFont_0", "Atlases/debugFont", 0, 0);
        ShadowLabel versionLabel = new ShadowLabel(C.versionNumber);
        versionLabel.y = -Futile.screen.height / 2 + versionLabel.label.textRect.height;
        C.getCameraInstance().AddChild(versionLabel);
        C.getCameraInstance().MoveToFront();
        Go.to(versionLabel, 6.0f, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadIn));

    }

    // Update is called once per frame
    void Update()
    {

    }
}
