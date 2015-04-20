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
        if (!Player.GetSaveStateInstance().activatedObjects.Contains(this.name))
            currentState = State.INACTIVE;
        else
        {
            currentState = State.ACTIVATED;
            this.SetElementByName("button_wall_on");
        }
    }
    public void Activate()
    {
        Player.GetSaveStateInstance().activatedObjects.Add(this.name);
        currentState = State.ACTIVATED;
        this.SetElementByName("button_wall_on");
    }
    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        return currentState == State.INACTIVE && ((Mathf.FloorToInt((this.x) / tileWidth) == x)) && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}

