using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class CeilButton : FSprite
{
    private enum State
    {
        ACTIVATED,
        INACTIVE
    }
    State currentState;

    public string name = "";
    public string doorTarget = "";
    public CeilButton(float x, float y, string name, string door)
        : base("button_floor_off")
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.doorTarget = door;
        this.rotation = 180;
        if (!Player.GetSaveStateInstance().activatedObjects.Contains(this.name))
            currentState = State.INACTIVE;
        else
        {
            currentState = State.ACTIVATED;
            this.SetElementByName("button_floor_on");
        }
    }
    public void Activate()
    {
        FSoundManager.PlaySound("WallButton");
        Player.GetSaveStateInstance().activatedObjects.Add(this.name);
        currentState = State.ACTIVATED;
        this.SetElementByName("button_floor_on");
    }
    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        return currentState == State.INACTIVE && ((Mathf.FloorToInt((this.x - tileWidth / 2) / tileWidth) == x) || (Mathf.FloorToInt((this.x + tileWidth / 2) / tileWidth) == x)) && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}

