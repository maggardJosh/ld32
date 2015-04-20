using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class SlamButton : FSprite
{
    private enum State
    {
        ACTIVATED,
        INACTIVE
    }
     State currentState;

    public string name = "";
    public string doorTarget = "";
    public SlamButton(float x, float y, string name, string door)
        : base("button_slam_off")
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.doorTarget = door;
        currentState = State.INACTIVE;
    }
    public void Activate()
    {
        currentState = State.ACTIVATED;
        this.SetElementByName("button_slam_on");
    }
    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        return currentState == State.INACTIVE && ((Mathf.FloorToInt((this.x-tileWidth-16f) / tileWidth) <= x) && (Mathf.FloorToInt((this.x + tileWidth) / tileWidth) >= x)) && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}

