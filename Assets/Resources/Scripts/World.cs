using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    FTmxMap map;
    Player p;
    FTilemap tilemap;
    FContainer background;
    FContainer playerLayer;

    List<WorldTravelPoint> travelPoints = new List<WorldTravelPoint>();

    public World()
    {
        background = new FContainer();
        playerLayer = new FContainer();
        this.p = new Player();
        C.getCameraInstance().follow(p);
        Futile.stage.AddChild(C.getCameraInstance());
        map = new FTmxMap();
        playerLayer.AddChild(p);
        this.AddChild(background);
        this.AddChild(playerLayer);
        Futile.instance.SignalUpdate += Update;
    }
    public void LoadMap(string mapName)
    {
        background.RemoveAllChildren();

        travelPoints.Clear();
        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);
        MapLoader.LoadObjects(this, map.objects);
        tilemap = (FTilemap)this.map.getLayerNamed("tilemap");

        p.tilemap = this.tilemap;
        tilemap.clipNode = C.getCameraInstance();
        background.AddChild(tilemap);
        C.getCameraInstance().setWorldBounds(new Rect(0, -tilemap.height, tilemap.width, tilemap.height));

    }

    public void addTravelPoint(WorldTravelPoint travelPoint)
    {

        travelPoints.Add(travelPoint);
    }
    public string lastWarp = "";
    private void Update()
    {
        WorldTravelPoint activeTravelPoint = null;
        foreach (WorldTravelPoint travelPoint in travelPoints)
            if (!String.IsNullOrEmpty(travelPoint.mapToLoad) && travelPoint.CheckPlayerCollision(this.p))
            {
                if (travelPoint.name != lastWarp)
                {
                    activeTravelPoint = travelPoint;
                    break;
                }
            }
            else
                if (travelPoint.name == lastWarp)
                    lastWarp = "";

        if (activeTravelPoint != null)
        {
            LoadAndSpawn(activeTravelPoint.mapToLoad, activeTravelPoint.travelPointTarget);
        }

    }

    public void LoadAndSpawn(string mapName, string travelPoint)
    {
        LoadMap(mapName);
        foreach (WorldTravelPoint travelTo in travelPoints)
            if (travelTo.name == travelPoint)
            {
                lastWarp = travelTo.name;
                p.SetPosition(travelTo.rect.center);

                break;
            }
    }

}
