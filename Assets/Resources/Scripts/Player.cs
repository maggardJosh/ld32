using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FAnimatedSprite
{
    private enum State
    {
        IDLE,
        RUN,
        JUMP,
        ATTACK_ONE,
        ATTACK_TWO,
        ATTACK_THREE

    }
    public Player(FTilemap tilemap)
        : base("player")
    {
        this.tilemap = tilemap;
        addAnimation(new FAnimation(State.IDLE.ToString(), new int[] { 1 }, 100, true));
        addAnimation(new FAnimation(State.RUN.ToString(), new int[] { 1, 2 }, 100, true));
        addAnimation(new FAnimation(State.JUMP.ToString(), new int[] { 1, 1, 1, 2 }, 100, true));
        play(State.IDLE.ToString());

    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalFixedUpdate += Update;
        base.HandleAddedToStage();
    }
    private State _currentState = State.IDLE;
    private State currentState
    {
        get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                stateCount = 0;
            }
        }
    }
    private bool isFacingLeft = false;
    private FTilemap tilemap;

    private float xVel = 0;
    private float yVel = 0;
    private float airFriction = .9f;
    private float groundFriction = .7f;
    private float maxXVel = 10;
    private float minYVel = -15;
    private float maxYVel = 30;
    private bool grounded = false;
    private float jumpStrength = 15;
    private float speed = 200;
    private float gravity = -50;
    private float stateCount = 0;
    public void Update()
    {
        switch (currentState)
        {
            case State.IDLE:
            case State.JUMP:
            case State.RUN:
                if (C.getKeyDown(C.UP_KEY) && grounded)
                {
                    grounded = false;
                    currentState = State.JUMP;
                    yVel = jumpStrength;
                }
                if (C.getKey(C.RIGHT_KEY))
                {
                    xVel += speed * Time.deltaTime;
                    isFacingLeft = false;
                }
                if (C.getKey(C.LEFT_KEY))
                {
                    xVel -= speed * Time.deltaTime;
                    isFacingLeft = true;
                }
                if (C.getKeyDown(C.ACTION_KEY) && grounded)
                {
                    StartAttackOne();
                }

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
                    yVel = Mathf.Max(minYVel, yVel);
                    TryMoveDown();
                }

                if (grounded && xVel != 0)
                {

                    if (currentState == State.IDLE)
                        currentState = State.RUN;
                }
                else
                {
                    if (currentState == State.RUN)
                        currentState = State.IDLE;
                }


                xVel *= airFriction;
                if (Mathf.Abs(xVel) < .1f)
                    xVel = 0;
                break;
            case State.ATTACK_ONE:
                xVel *= groundFriction;
                if (stateCount > ATTACK_ONE_TIME)
                    currentState = State.IDLE;

                break;
        }



        this.scaleX = isFacingLeft ? -1 : 1;

        this.play(currentState.ToString());
    }
    private const float ATTACK_ONE_TIME = 1.0f;
    private const float ATTACK_TWO_TIME = 1.0f;
    private const float ATTACK_THREE_TIME = 1.0f;
    private const float ATTACK_ONE_XVEL = 6;

    public void StartAttackOne()
    {
        currentState = State.ATTACK_ONE;
        xVel = ATTACK_ONE_XVEL * (isFacingLeft ? -1 : 1);

    }

    public void TryMoveRight()
    {
        float newX = this.x + xVel;
        float topY = this.y + this.height / 3;
        float bottomY = this.y - this.height / 3;
        if (tilemap.isPassable(newX + tilemap.tileWidth / 2, topY) &&
            tilemap.isPassable(newX + tilemap.tileWidth / 2, bottomY))
            this.x = newX;
        else
        {
            this.x = Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2;
            xVel = 0;
        }

    }

    public void TryMoveLeft()
    {
        float newX = this.x + xVel;
        float topY = this.y + this.height / 3;
        float bottomY = this.y - this.height / 3;
        if (tilemap.isPassable(newX - tilemap.tileWidth / 2, topY) &&
            tilemap.isPassable(newX - tilemap.tileWidth / 2, bottomY))
            this.x = newX;
        else
        {
            this.x = Mathf.CeilToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth - tilemap.tileWidth / 2;
            xVel = 0;
        }
    }

    public void TryMoveDown()
    {
        float newY = this.y + yVel;
        float leftX = this.x - this.width / 3;
        float rightX = this.x + this.width / 3;
        if (tilemap.isPassable(leftX, newY - tilemap.tileWidth / 2) &&
            tilemap.isPassable(rightX, newY - tilemap.tileWidth / 2))
            this.y = newY;
        else
        {
            grounded = true;
            if (currentState == State.JUMP)
                currentState = State.IDLE;
            xVel *= groundFriction;
            yVel = 0;
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileWidth / 2;
        }
    }

    public void TryMoveUp()
    {
        float newY = this.y + yVel;
        float leftX = this.x - this.width / 3;
        float rightX = this.x + this.width / 3;
        if (tilemap.isPassable(leftX, newY + tilemap.tileHeight / 2) &&
            tilemap.isPassable(rightX, newY + tilemap.tileHeight / 2))
            this.y = newY;
        else
        {
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight / 2;
        }
    }


}

