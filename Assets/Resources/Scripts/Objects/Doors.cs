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
    }

    public void Open()
    {
        currentState = State.OPEN;
        C.isTransitioning = true;
        C.getCameraInstance().shake(1.0f, 1.0f);
        Go.to(this, 1.0f, new TweenConfig().floatProp("y", 64, true).setEaseType(EaseType.QuadIn).onComplete((t) => { C.isTransitioning = false; }));
    }

    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        if (currentState == State.OPEN)
            return false;
        bool result = Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt(this.y / tileWidth) == y;
        if (!result)
        {
            if(!isVertical)
                return Mathf.FloorToInt(this.x / tileWidth) ==( x +1) && Mathf.FloorToInt(this.y / tileWidth) == y;
            else
                return Mathf.FloorToInt(this.x / tileWidth) == x && Mathf.FloorToInt(this.y / tileWidth) == (y+1);
        }
        return result;
    }
    public Boolean IsPlayerColliding(Player player)
    {
        float xDist = Mathf.Abs(player.x - this.x);
        float yDist = Mathf.Abs(player.y - this.y);
        switch (currentState)
        {
            case State.CLOSED:
                return (isVertical ? (xDist < player.tilemap.tileWidth && yDist < player.tilemap.tileWidth * 1.5f) : (yDist < player.tilemap.tileWidth * 1.5f && xDist < player.tilemap.tileWidth));
        }
        return false;
    }
}

