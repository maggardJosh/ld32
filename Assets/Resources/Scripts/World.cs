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
    List<Door> doors = new List<Door>();
    List<Lever> levers = new List<Lever>();

    public World()
    {
        background = new FContainer();
        playerLayer = new FContainer();
        this.p = new Player();
        this.p.world = this;
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
        doors.Clear();
        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);
        tilemap = (FTilemap)this.map.getLayerNamed("tilemap");

        p.tilemap = this.tilemap;
        tilemap.clipNode = C.getCameraInstance();
        background.AddChild(tilemap);
        MapLoader.LoadObjects(this, map.objects);
        C.getCameraInstance().setWorldBounds(new Rect(0, -tilemap.height, tilemap.width, tilemap.height));
        Futile.stage.AddChild(C.getCameraInstance());

    }

    public void addTravelPoint(WorldTravelPoint travelPoint)
    {

        travelPoints.Add(travelPoint);
    }
    public void addLever(Lever lever)
    {
        levers.Add(lever);
        background.AddChild(lever);
    }

    public void addDoor(Door door)
    {
        doors.Add(door);
        background.AddChild(door);
    }
    public string lastWarp = "";
    private void Update()
    {
        CheckTravelPoints();

    }
    public bool isPassable(float x, float y)
    {
        bool result = tilemap.isPassable(x, y);
        int tileX = Mathf.FloorToInt(x / tilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / tilemap.tileHeight);
        if (result)
            foreach (Door door in doors)
                if (door.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                    return false;

        return result;
    }
    private void CheckTravelPoints()
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
    public void ActivateLever(Lever lever)
    {
        lever.Activate();
        foreach(Door d in doors)
            if (d.name == lever.doorTarget)
            {
                FNode node = new FNode();
                node.SetPosition(p.GetPosition());
                C.getCameraInstance().follow(node);
                C.isTransitioning = true;
                Go.to(node, .7f, new TweenConfig().floatProp("x", d.x).floatProp("y", d.y).setEaseType(EaseType.QuadOut).onComplete((t) => 
                {
                    d.Open(() => { Go.to(node, .7f, new TweenConfig().floatProp("x", p.x).floatProp("y", p.y).setEaseType(EaseType.QuadOut).onComplete((t2) => { C.getCameraInstance().follow(p); C.isTransitioning = false; })); });
                }));
                break;
            }
    }
    public Lever GetInteractableLever()
    {
        foreach (Lever lever in levers)
        {
            if(lever.IsTileOccupied(Mathf.FloorToInt(p.x/tilemap.tileWidth), Mathf.FloorToInt(p.y/tilemap.tileHeight),tilemap.tileWidth))
                return lever;
        }
        return null;
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
