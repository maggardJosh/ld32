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
        if (!Player.GetSaveStateInstance().activatedObjects.Contains(this.name))
            currentState = State.INACTIVE;
        else
        {
            currentState = State.ACTIVATED;
            this.SetElementByName("button_slam_on");
            this.y -= 32;
        }

    }
    private const float SLAM_BUTTON_TIME = 1.5f;
    private const float SLAM_BUTTON_DELAY = 1.0f;
    public void Activate(Player p, Action onComplete)
    {
        Player.GetSaveStateInstance().activatedObjects.Add(this.name);
        currentState = State.ACTIVATED;
        p.y = this.y + 46;
        C.getCameraInstance().shake(2.0f, SLAM_BUTTON_DELAY + SLAM_BUTTON_TIME);
        Go.to(this, SLAM_BUTTON_TIME, new TweenConfig().floatProp("y", -32, true).setDelay(SLAM_BUTTON_DELAY).onStart((t) => { Go.to(p, SLAM_BUTTON_TIME, new TweenConfig().floatProp("y", -32, true)); }).onComplete((t2) => { onComplete.Invoke(); }));
        this.SetElementByName("button_slam_on");
    }
    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        return currentState == State.INACTIVE && ((Mathf.FloorToInt((this.x-tileWidth-16f) / tileWidth) <= x) && (Mathf.FloorToInt((this.x + tileWidth) / tileWidth) >= x)) && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}

