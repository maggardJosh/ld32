using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FAnimatedSprite
{
    public Player(FTilemap tilemap)
        : base("player")
    {
        this.tilemap = tilemap;
        addAnimation(new FAnimation("idle", new int[] { 1, 2 }, 100, true));
        play("idle");
    }

    private FTilemap tilemap;

    private float xVel = 0;
    private float yVel = 0;
    private float groundFriction = .9f;
    private float maxXVel = 10;
    private float maxYVel = 10;

    private float speed = 300;
    private float gravity = -100;
    public void Update()
    {
        if (C.getKey(C.UP_KEY))
            yVel += speed * Time.deltaTime;
        if (C.getKey(C.DOWN_KEY))
            yVel -= speed * Time.deltaTime;
        if (C.getKey(C.RIGHT_KEY))
            xVel += speed * Time.deltaTime;
        if (C.getKey(C.LEFT_KEY))
            xVel -= speed * Time.deltaTime;

        yVel += gravity * Time.deltaTime;
        if (xVel > 0)
        {
            xVel = Mathf.Min(maxXVel, xVel);
            TryMoveRight();
        }
        else if (xVel < 0)
        {
            xVel = Mathf.Max(-maxXVel, xVel);
            TryMoveLeft();
        }
        if (yVel > 0)
        {
            yVel = Mathf.Min(maxYVel, yVel);
            TryMoveUp();
        }
        else if (yVel < 0)
        {
            yVel = Mathf.Max(-maxYVel, yVel);
            TryMoveDown();
        }
        xVel *= groundFriction;
        yVel *= groundFriction;

        TryMove();

    }

    public void TryMoveRight()
    {
        float newX = this.x + xVel;
        if (tilemap.isPassable(newX, this.y))
            this.x = newX;
        else
            xVel = 0;
        
    }

    public void TryMoveLeft()
    {
        float newX = this.x + xVel;
        if (tilemap.isPassable(newX, this.y))
            this.x = newX;
        else
        {
            xVel = 0;
        }
    }

    public void TryMoveDown()
    {
        float newY = this.y + yVel;
        if (tilemap.isPassable(this.x, newY))
            this.y = newY;
        else
        {
            yVel = 0;
        }
    }

    public void TryMoveUp()
    {
        float newY = this.y + yVel;
        if (tilemap.isPassable(this.x, newY))
            this.y = newY;
        else
            yVel = 0;
    }

    public void TryMove()
    {
        
    }


}

