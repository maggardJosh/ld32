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
    FTilemap foregroundTilemap;
    FContainer background;
    FContainer playerLayer;
    FContainer foreground;

    List<WorldTravelPoint> travelPoints = new List<WorldTravelPoint>();
    List<Door> doors = new List<Door>();
    List<Lever> levers = new List<Lever>();
    List<BreakableWall> breakableWalls = new List<BreakableWall>();
    List<FloorButton> floorButtons = new List<FloorButton>();
    List<CeilButton> ceilButtons = new List<CeilButton>();
    List<SlamButton> slamButtons = new List<SlamButton>();
    List<Powerup> powerups = new List<Powerup>();
    List<WallButton> wallButtons = new List<WallButton>();
    List<TutorialText> tutorialTexts = new List<TutorialText>();
    List<Fireball> fireballs = new List<Fireball>();
    string lastMap = "";
    string lastTravelPoint = "";
    public World()
    {
        background = new FContainer();
        playerLayer = new FContainer();
        foreground = new FContainer();
        this.p = new Player();
        this.p.world = this;
        C.getCameraInstance().follow(p);
        Futile.stage.AddChild(C.getCameraInstance());
        map = new FTmxMap();
        playerLayer.AddChild(p);
        this.AddChild(background);
        this.AddChild(playerLayer);
        this.AddChild(foreground);
        Futile.instance.SignalUpdate += Update;
    }
    public void LoadMap(string mapName)
    {
        if (mapName.ToUpper() == "ENDING")
        {
            if (C.isTransitioning)
                return;
            this.RemoveFromContainer();
            C.isTransitioning = true;
            Ending newEnding = new Ending();
           C.getCameraInstance().AddChild(new Ending());
            return;
        }
        background.RemoveAllChildren();
        foreground.RemoveAllChildren();

        travelPoints.Clear();
        doors.Clear();
        levers.Clear();
        breakableWalls.Clear();
        floorButtons.Clear();
        ceilButtons.Clear();
        wallButtons.Clear();
        slamButtons.Clear();
        foreach (Fireball f in fireballs)
            f.InstaDie();
        fireballs.Clear();
        powerups.Clear();
        foreach (TutorialText text in tutorialTexts)
            text.RemoveFromContainer();
        tutorialTexts.Clear();
        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);
        tilemap = (FTilemap)this.map.getLayerNamed("tilemap");
        foregroundTilemap = (FTilemap)this.map.getLayerNamed("foreground");

        p.tilemap = this.tilemap;
        tilemap.clipNode = C.getCameraInstance();
        foregroundTilemap.clipNode = C.getCameraInstance();
        background.AddChild(tilemap);
        foreground.AddChild(foregroundTilemap);
        MapLoader.LoadObjects(this, map.objects);
        C.getCameraInstance().setWorldBounds(new Rect(0, -tilemap.height, tilemap.width, tilemap.height));
        Futile.stage.AddChild(C.getCameraInstance());

    }
    public void Respawn()
    {
        LoadAndSpawn(lastMap, lastTravelPoint);
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
    public void addBreakableWall(BreakableWall wall)
    {
        if (Player.GetSaveStateInstance().activatedObjects.Contains(wall.name))
            return;
        breakableWalls.Add(wall);
        background.AddChild(wall);
    }
    public void addTutorialText(TutorialText text)
    {
        tutorialTexts.Add(text);
        C.getCameraInstance().AddChild(text);
    }

    public void addJadeDragon(JadeDragon jadeDragon)
    {

        background.AddChild(jadeDragon);
    }

    public void addDoor(Door door)
    {
        doors.Add(door);
        background.AddChild(door);
    }
    public void addFloorButton(FloorButton button)
    {
        floorButtons.Add(button);
        background.AddChild(button);
    }
    public void addSlamButton(SlamButton button)
    {
        slamButtons.Add(button);
        background.AddChild(button);
    }
    public void addCeilButton(CeilButton button)
    {
        ceilButtons.Add(button);
        background.AddChild(button);
    }
    public void addWallButton(WallButton button)
    {
        wallButtons.Add(button);
        background.AddChild(button);
    }
    public void addPowerup(Powerup powerup)
    {
        powerups.Add(powerup);
        background.AddChild(powerup);
    }
    public string lastWarp = "";
    private void Update()
    {
        CheckTravelPoints();

    }
    public void addFireball(Fireball f)
    {
        fireballs.Add(f);
        background.AddChild(f);
    }
    public void CheckFireball(Fireball f)
    {
        if (!isPassable(f.x, f.y))
            f.Die();
        if (!p.IsDying())
        {

            float xDiff = Mathf.Abs(f.x - p.x);
            float yDiff = Mathf.Abs(f.y - p.y);
            if (xDiff < 16 && yDiff < 16)
            {
                p.Kill();
            }
        }
    }
    public void tryBreakWall(Vector2 lastPos, Vector2 position)
    {
        int lastTileX = Mathf.FloorToInt(lastPos.x / tilemap.tileWidth);
        int tileX = Mathf.FloorToInt(position.x / tilemap.tileWidth);
        int tileY = Mathf.FloorToInt(position.y / tilemap.tileHeight);
        if (lastTileX < tileX)
        {
            int temp = tileX;
            tileX = lastTileX;
            lastTileX = temp;
        }

        for (; tileX <= lastTileX; tileX++)
            foreach (BreakableWall wall in breakableWalls)
                if (wall.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                    wall.Open();
    }
    public int ExtendPolePassable(Vector2 lastPos, Vector2 position)
    {
        int lastTileX = Mathf.FloorToInt(lastPos.x / tilemap.tileWidth);
        int tileX = Mathf.FloorToInt(position.x / tilemap.tileWidth);
        int tileY = Mathf.FloorToInt(position.y / tilemap.tileHeight) + 1;
        int step;
        if (lastTileX < tileX)
            step = 1;
        else
            step = -1;

        for (lastTileX = Mathf.FloorToInt(lastPos.x / tilemap.tileWidth); step > 0 ? lastTileX <= tileX : lastTileX >= tileX; lastTileX += step)
        {

            if (foregroundTilemap.getFrameNum(lastTileX, -tileY) != 8 && !isPassable(lastTileX * tilemap.tileWidth, tileY * tilemap.tileWidth, false))
            {
                return lastTileX;
            }
        }
        return -1;
    }

    public bool CheckWallButton(Vector2 lastPos, Vector2 position)
    {
        int lastTileX = Mathf.FloorToInt(lastPos.x / tilemap.tileWidth);
        int tileX = Mathf.FloorToInt(position.x / tilemap.tileWidth);
        int tileY = Mathf.FloorToInt(position.y / tilemap.tileHeight);
        int step;
        if (lastTileX < tileX)
            step = 1;
        else
            step = -1;

        foreach (WallButton button in wallButtons)
            for (lastTileX = Mathf.FloorToInt(lastPos.x / tilemap.tileWidth); step > 0 ? lastTileX <= tileX : lastTileX >= tileX; lastTileX += step)
            {
                if (button.IsTileOccupied(lastTileX, tileY, tilemap.tileWidth))
                {
                    ActivateWallButton(button);
                    return true;
                }
            }
        return false;
    }
    public bool isPassable(float x, float y, bool checkBreakableWalls = true)
    {
        bool result = foregroundTilemap.isPassable(x, y);
        int tileX = Mathf.FloorToInt(x / foregroundTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / foregroundTilemap.tileHeight);
        if (!result)
            return result;

        foreach (Door door in doors)
            if (door.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                return false;
        if (checkBreakableWalls)
            foreach (BreakableWall wall in breakableWalls)
                if (wall.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                    return false;
        foreach (SlamButton button in slamButtons)
            if (button.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                return false;

        return result;
    }
    public bool isDeath(float x, float y)
    {
        return foregroundTilemap.isDeath(x, y);
    }
    public bool isOneWay(float x, float y)
    {
        return foregroundTilemap.isOneWay(x, y);
    }
    public FloorButton getFloorButton(float x, float y)
    {
        int tileX = Mathf.FloorToInt(x / foregroundTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / foregroundTilemap.tileHeight);
        foreach (FloorButton button in floorButtons)
            if (button.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                return button;
        return null;
    }

    public SlamButton getSlamButton(float x, float y)
    {
        int tileX = Mathf.FloorToInt(x / foregroundTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / foregroundTilemap.tileHeight);
        foreach (SlamButton button in slamButtons)
            if (button.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                return button;
        return null;
    }
    public CeilButton getCeilButton(float x, float y)
    {
        int tileX = Mathf.FloorToInt(x / foregroundTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / foregroundTilemap.tileHeight);
        foreach (CeilButton button in ceilButtons)
            if (button.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                return button;
        return null;
    }

    public Powerup getPowerup(float x, float y)
    {
        int tileX = Mathf.FloorToInt(x / foregroundTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / foregroundTilemap.tileHeight);
        foreach (Powerup powerup in powerups)
            if (powerup.IsTileOccupied(tileX, tileY, tilemap.tileWidth))
                return powerup;
        return null;
    }
    public void ActivatePowerup(Powerup p)
    {
        powerups.Remove(p);
        p.ActivatePowerup(this.p);
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
    public void CheckTutorialText()
    {
        foreach (TutorialText text in tutorialTexts)
            text.CheckInText(p);
    }
    public void ActivateLever(Lever lever)
    {
        lever.Activate();
        OpenDoorWithCam(lever.doorTarget);
    }
    public void ActivateWallButton(WallButton button)
    {
        button.Activate();
        OpenDoorWithCam(button.doorTarget);
    }
    public void ActivateFloorButton(FloorButton button)
    {
        button.Activate();
        OpenDoorWithCam(button.doorTarget);
    }
    public void ActivateSlamButton(SlamButton button)
    {
        C.isTransitioning = true;
        button.Activate(p, () => { OpenDoorWithCam(button.doorTarget); });
    }
    public void ActivateCeilButton(CeilButton button)
    {
        button.Activate();
        OpenDoorWithCam(button.doorTarget);
    }
    private void OpenDoorWithCam(string doorName)
    {
        foreach (Door d in doors)
            if (d.name == doorName)
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
            if (lever.IsTileOccupied(Mathf.FloorToInt(p.x / tilemap.tileWidth), Mathf.FloorToInt(p.y / tilemap.tileHeight), tilemap.tileWidth))
                return lever;
        }
        return null;
    }
    public void LoadAndSpawn(string mapName, string travelPoint)
    {
        Player.SaveRoom();
        this.lastMap = mapName;
        this.lastTravelPoint = travelPoint;
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
