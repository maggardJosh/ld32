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
        addAnimation(new FAnimation("idle", new int[] { 1 }, 100, true));
        play("idle");
    }

    private FTilemap tilemap;

    private float xVel = 0;
    private float yVel = 0;
    private float airFriction = .9f;
    private float groundFriction = .7f;
    private float maxXVel = 10;
    private float minYVel = -15;
    private float maxYVel = 30;
    private bool grounded = false;
    private float jumpStrength = 20;
    private float speed = 200;
    private float gravity = -50;
    public void Update()
    {
        if (C.getKeyDown(C.UP_KEY) && grounded)
        {
            grounded = false;
            yVel = jumpStrength;
        }
        if (C.getKey(C.RIGHT_KEY))
            xVel += speed * Time.deltaTime;
        if (C.getKey(C.LEFT_KEY))
            xVel -= speed * Time.deltaTime;

        yVel += gravity * Time.deltaTime;
        if (yVel > 0)
        {
            yVel = Mathf.Min(maxYVel, yVel);
            TryMoveUp();
        }
        else if (yVel < 0)
        {
            yVel = Mathf.Max(minYVel, yVel);
            TryMoveDown();
        }
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

        xVel *= airFriction;

    }

    public void TryMoveRight()
    {
        float newX = this.x + xVel;
        float topY = this.y + this.height / 3;
        float bottomY = this.y - this.height / 3;
        if (tilemap.isPassable(newX + this.width/2, topY) &&
            tilemap.isPassable(newX + this.width/2, bottomY))
            this.x = newX;
        else
        {
            this.x = Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + this.width/2;
            xVel = 0;
        }
        
    }

    public void TryMoveLeft()
    {
        float newX = this.x + xVel;
        float topY = this.y + this.height / 3;
        float bottomY = this.y - this.height / 3;
        if (tilemap.isPassable(newX - this.width/2, topY) &&
            tilemap.isPassable(newX - this.width/2, bottomY))
            this.x = newX;
        else
        {
            this.x = Mathf.CeilToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth - this.width / 2;
            xVel = 0;
        }
    }

    public void TryMoveDown()
    {
        float newY = this.y + yVel;
        float leftX = this.x - this.width/3;
        float rightX = this.x + this.width / 3;
        if (tilemap.isPassable(leftX, newY - this.height/2) &&
            tilemap.isPassable(rightX, newY - this.height/2))
            this.y = newY;
        else
        {
            grounded = true;
            xVel *= groundFriction;
            yVel = 0;
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + this.height / 2;
        }
    }

    public void TryMoveUp()
    {
        float newY = this.y + yVel;
        float leftX = this.x - this.width / 3;
        float rightX = this.x + this.width / 3;
        if (tilemap.isPassable(leftX, newY + tilemap.tileHeight/2) &&
            tilemap.isPassable(rightX, newY + tilemap.tileHeight/2))
            this.y = newY;
        else
        {
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight/2;
        }
    }


}

