using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Door : FSprite
{
    private enum State
    {
        CLOSED,
        OPEN
    }
    private Boolean isVertical;
    private State currentState;

    public string name = "";
    public Door(float x, float y, string name, Boolean vert)
        : base("door")
    {
        isVertical = vert;
        if (!isVertical)
            this.rotation = 90f;
        this.x = x;
        this.y = y;
        this.name = name;
        if (Player.GetSaveStateInstance().activatedObjects.Contains(this.name))
        {
            currentState = State.OPEN;
            if (this.isVertical)
                this.y += 64;
            else
                this.x += 64;
        }
    }

    public void Open(Action onCompleteAction)
    {
        FSoundManager.PlaySound("DoorOpen");
        Player.GetSaveStateInstance().activatedObjects.Add(this.name);
        currentState = State.OPEN;
        C.getCameraInstance().shake(1.0f, 1.0f);
        if (this.isVertical)
            Go.to(this, 1.0f, new TweenConfig().floatProp("y", 64, true).setEaseType(EaseType.QuadIn).onComplete((t) => { onCompleteAction.Invoke(); }));
        else
            Go.to(this, 1.0f, new TweenConfig().floatProp("x", 64, true).setEaseType(EaseType.QuadIn).onComplete((t) => { onCompleteAction.Invoke(); }));
    }

    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        if (currentState == State.OPEN)
            return false;
        bool result = Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt(this.y / tileWidth) == y;
        if (!result)
        {
            if (!isVertical)
                return Mathf.FloorToInt(this.x / tileWidth) == (x + 1) && Mathf.FloorToInt(this.y / tileWidth) == y;
            else
                return Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt(this.y / tileWidth) == (y + 1);
        }
        return result;
    }
}

