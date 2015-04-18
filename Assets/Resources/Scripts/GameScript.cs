using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{
    FTmxMap map;
    Player player;
    FContainer background;
    FContainer playerLayer;
    FTilemap tilemap;
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

        map = new FTmxMap();
        map.LoadTMX("Maps/mapOne");

        background = new FContainer();
        playerLayer = new FContainer();

        tilemap = (FTilemap)map.getLayerNamed("tilemap");
        player = new Player(tilemap);
        player.x = 100;
        player.y = -100;
        background.AddChild(tilemap);
        playerLayer.AddChild(player);
        FCamObject camera = C.getCameraInstance();
        camera.follow(player);
        camera.setWorldBounds(new Rect(0, -tilemap.height, tilemap.width, tilemap.height));
        tilemap.clipNode = camera;

        Futile.atlasManager.LoadFont(C.smallFontName, "debugFont_0", "Atlases/debugFont", 0, 0);

        FLabel versionLabel = new FLabel(C.smallFontName, C.versionNumber);
        versionLabel.y = -Futile.screen.height / 2 + versionLabel.textRect.height;
        Futile.stage.AddChild(background);
        Futile.stage.AddChild(playerLayer);
        Futile.stage.AddChild(camera);
        camera.AddChild(versionLabel);
        Go.to(versionLabel, 6.0f, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadIn));

    }

    // Update is called once per frame
    void Update()
    {

    }
}
