using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class WallButton : FSprite
{
    private enum State
    {
        ACTIVATED,
        INACTIVE
    }
    State currentState;
    bool faceLeft;

    public string name = "";
    public string doorTarget = "";
    public WallButton(float x, float y, string name, string door, bool faceLeft)
        : base("button_wall_off")
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.doorTarget = door;
        this.faceLeft = faceLeft;
        this.rotation = faceLeft ? 180 : 0;
        currentState = State.INACTIVE;
    }
    public void Activate()
    {
        currentState = State.ACTIVATED;
        this.SetElementByName("button_wall_on");
    }
    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        RXDebug.Log(x, y, this.x/tileWidth, this.y/tileWidth);
        RXDebug.Log(((Mathf.FloorToInt((this.x) / tileWidth) == x)) && Mathf.FloorToInt(this.y / tileWidth) == y);
        return currentState == State.INACTIVE && ((Mathf.FloorToInt((this.x) / tileWidth) == x)) && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}

