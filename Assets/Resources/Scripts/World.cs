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
    }
    public void LoadMap(string mapName)
    {
        this.map.LoadTMX(mapName);
        tilemap = (FTilemap)this.map.getLayerNamed("tilemap");
        p.tilemap = this.tilemap;
        tilemap.clipNode = C.getCameraInstance();
        background.AddChild(tilemap);
        C.getCameraInstance().setWorldBounds(new Rect(0, -tilemap.height, tilemap.width, tilemap.height));
        
    }

}
