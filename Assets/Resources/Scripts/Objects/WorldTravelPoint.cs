using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldTravelPoint
{
    public Rect rect;
    public string mapToLoad;
    public string travelPointTarget;
    public WorldTravelPoint(Rect rect, string mapToLoad, string travelPointTarget)
    {
        this.rect = rect;
        this.mapToLoad = mapToLoad;
        this.travelPointTarget = travelPointTarget;
    }

    public bool CheckPlayerCollision(Player p)
    {
        return rect.Contains(p.GetPosition());
    }
}

