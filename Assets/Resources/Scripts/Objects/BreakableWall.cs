using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class BreakableWall : FSprite
{

    public string name = "";
    public BreakableWall(float x, float y, string name)
        : base("breakableWall")
    {
        this.x = x;
        this.y = y;
        this.name = name;
    }

    public void Open()
    {
        C.getCameraInstance().shake(1.0f, .5f);
        foreach(Particle p in Particle.IceParticle.GetBreakableParticles(this.GetPosition()))
            Futile.stage.AddChild(p);
        this.isVisible = false;
    }

    public bool IsTileOccupied(int x, int y, float tileWidth)
    {  
        return this.isVisible && (Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt((this.y - tileWidth/2) / tileWidth) == y || Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt((this.y + tileWidth/2) / tileWidth) == y);
    }
}

