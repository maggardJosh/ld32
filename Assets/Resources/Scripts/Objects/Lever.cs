using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Lever : FSprite
{
    private enum State
    {
        ACTIVATED,
        INACTIVE
    }
     State currentState;

    public string name = "";
    public string doorTarget = "";
    public Lever(float x, float y, string name, string door)
        : base("lever_on")
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.doorTarget = door;
        if (!Player.GetSaveStateInstance().activatedObjects.Contains(this.name))
            currentState = State.INACTIVE;
        else
        {
            currentState = State.ACTIVATED;
            this.SetElementByName("lever_off");
        }

    }
    public void Activate()
    {
        Player.GetSaveStateInstance().activatedObjects.Add(this.name);
        currentState = State.ACTIVATED;
        this.SetElementByName("lever_off");
    }
    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        return currentState == State.INACTIVE && Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}

