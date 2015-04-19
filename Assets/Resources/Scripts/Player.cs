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
        FALL,
        DOUBLE_JUMP,
        SLIDE,
        CROUCH,
        SUPERJUMP_CHARGE,
        SUPERJUMP_ABLE,
        SUPERJUMP,
        ATTACK_ONE,
        ATTACK_TWO,
        ATTACK_THREE,
        ATTACK_THREE_EXTEND,
        TAIL_HANG_TRANS_IN,
        TAIL_HANG_FALL,
        TAIL_HANG,
        SLAM_TRANS_IN,
        SLAM_MOVE,
        SLAM_LAND

    }
    private FSprite extendPoleMiddle;
    private FSprite extendPoleEnd;
    
    public Player()
        : base("player")
    {
        extendPoleMiddle = new FSprite("pole_mid");
        extendPoleEnd = new FSprite("pole_end");

        addAnimation(new FAnimation(State.IDLE.ToString(), new int[] { 1 }, 100, true));
        addAnimation(new FAnimation(State.RUN.ToString(), new int[] { 2, 3, 4, 5 }, 100, true));
        addAnimation(new FAnimation(State.SLIDE.ToString(), new int[] { 6 }, 100, true));
        addAnimation(new FAnimation(State.JUMP.ToString(), new int[] { 17 }, 100, true));
        addAnimation(new FAnimation(State.FALL.ToString(), new int[] { 18 }, 100, true));
        addAnimation(new FAnimation(State.CROUCH.ToString(), new int[] { 19 }, 100, true));
        addAnimation(new FAnimation(State.ATTACK_ONE.ToString(), new int[] { 7, 8, 9, 10 }, 100, false));
        addAnimation(new FAnimation(State.ATTACK_TWO.ToString(), new int[] { 11, 12, 13 }, 100, false));
        addAnimation(new FAnimation(State.ATTACK_THREE.ToString(), new int[] { 14, 15 }, 50, false));
        addAnimation(new FAnimation(State.ATTACK_THREE_EXTEND.ToString(), new int[] { 16 }, 100, false));
        addAnimation(new FAnimation(State.TAIL_HANG_TRANS_IN.ToString(), new int[] { 21 }, 100, false));
        addAnimation(new FAnimation(State.TAIL_HANG.ToString(), new int[] { 21 }, 100, true));
        addAnimation(new FAnimation(State.TAIL_HANG_FALL.ToString(), new int[] { 2, 3, 4 }, 100, true));
        addAnimation(new FAnimation(State.SLAM_TRANS_IN.ToString(), new int[] { 11, 12, 13 }, 100, false));
        addAnimation(new FAnimation(State.SLAM_MOVE.ToString(), new int[] { 13 }, 100, false));
        addAnimation(new FAnimation(State.SLAM_LAND.ToString(), new int[] { 20 }, 100, false));

        addAnimation(new FAnimation(State.SUPERJUMP_CHARGE.ToString(), new int[] {  19 }, 100, true));
        addAnimation(new FAnimation(State.SUPERJUMP_ABLE.ToString(), new int[] { 1, 6 }, 50, true));

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
                switch (value)
                {
                    case State.SLAM_TRANS_IN:
                    case State.SLAM_LAND:
                    case State.TAIL_HANG:
                        this.play(value.ToString(), true);
                        break;
                }
                _currentState = value;
                stateCount = 0;
            }
        }
    }
    private bool isFacingLeft = false;
    public FTilemap tilemap;

    private bool lastJumpPress = false;
    private bool lastActionPress = false;
    private bool lastDownPress = false;

    private float xVel = 0;
    private float yVel = 0;
    private float airFriction = .7f;
    private float groundFriction = .7f;
    private float maxXVel = 10;
    private float minYVel = -15;
    private float maxYVel = 30;
    private float slamSpeed = -1.5f;
    private bool grounded = false;
    private float jumpStrength = 13;
    private float superJumpStrength = 30;
    private float hangJumpStrength = 12;
    private float speed = 180;
    private float superjumpAirSpeed = 5;
    private float gravity = -50;
    private float stateCount = 0;
    private float poleExtendLength = 96;

    private const float ATTACK_THREE_EXTEND_TIME = .3f;
    public void Update()
    {

        switch (currentState)
        {
            case State.IDLE:
            case State.JUMP:
            case State.DOUBLE_JUMP:
            case State.RUN:
            case State.SLIDE:
                bool isActivelyMoving = false;
                if (C.getKey(C.JUMP_KEY) && !lastJumpPress && currentState != State.DOUBLE_JUMP)
                {
                    if (currentState == State.JUMP)
                    {
                        currentState = State.DOUBLE_JUMP;
                    }
                    if (grounded)
                    {
                        grounded = false;
                        currentState = State.JUMP;
                    }
                    yVel = jumpStrength;
                }
                if (C.getKey(C.DOWN_KEY) && (!C.getKey(C.LEFT_KEY) || !C.getKey(C.RIGHT_KEY)) && !lastDownPress && grounded)
                {
                    currentState = State.CROUCH;
                    return;
                }
                if (currentState == State.JUMP || currentState == State.DOUBLE_JUMP)
                {
                    if (C.getKey(C.DOWN_KEY) && !lastDownPress)
                    {
                        currentState = State.SLAM_TRANS_IN;
                        return;
                    }
                }
                if (C.getKey(C.RIGHT_KEY))
                {
                    isActivelyMoving = true;
                    xVel += speed * Time.deltaTime;
                    isFacingLeft = false;
                }
                if (C.getKey(C.LEFT_KEY))
                {
                    isActivelyMoving = true;
                    xVel -= speed * Time.deltaTime;
                    isFacingLeft = true;
                }
                if (C.getKey(C.ACTION_KEY) && !lastActionPress && grounded)
                {
                    StartAttackOne();
                    return;
                }

                if (grounded)
                    if (xVel != 0)
                    {
                        if (isActivelyMoving)
                            currentState = State.RUN;
                        else
                            currentState = State.SLIDE;
                    }
                    else
                    {
                        currentState = State.IDLE;

                    }
                if (grounded)
                    xVel *= groundFriction;
                else
                    xVel *= airFriction;
                if (Mathf.Abs(xVel) < .1f)
                    xVel = 0;
                break;
            case State.ATTACK_ONE:
                xVel *= groundFriction;
                if (stateCount > ATTACK_ONE_TIME)
                    currentState = State.IDLE;
                if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                    StartAttackTwo();
                break;
            case State.ATTACK_TWO:
                xVel *= groundFriction;
                if (stateCount > ATTACK_TWO_TIME)
                    currentState = State.IDLE;
                if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                    StartAttackThree();
                break;
            case State.ATTACK_THREE:
                xVel *= groundFriction;
                if (this.IsStopped)
                {
                    xVel = 0;
                    C.getCameraInstance().shake(.8f, .1f);
                    Futile.stage.AddChild(extendPoleMiddle);
                    Futile.stage.AddChild(extendPoleEnd);
                    AttackExtendLength = 0;
                    currentState = State.ATTACK_THREE_EXTEND;

                    attackExtendTween = Go.to(this, ATTACK_THREE_EXTEND_TIME / 8, new TweenConfig().floatProp("AttackExtendLength", poleExtendLength).setEaseType(EaseType.QuartIn).onComplete(
                        (t) =>
                        {

                            C.getCameraInstance().shake(.5f, .1f);
                            Go.to(this, ATTACK_THREE_TIME / 2, new TweenConfig().floatProp("AttackExtendLength", 0).setEaseType(EaseType.QuadIn).onComplete((t2) =>
                            {
                                extendPoleEnd.RemoveFromContainer();
                                extendPoleMiddle.RemoveFromContainer();
                                currentState = State.IDLE;
                            }));
                        }));
                }
                break;
            case State.ATTACK_THREE_EXTEND:

                break;
            case State.SUPERJUMP_CHARGE:
                if (!C.getKey(C.JUMP_KEY))
                {
                    currentState = State.IDLE;
                    return;
                }
                if (C.getKey(C.JUMP_KEY))
                {
                    if (stateCount >= SUPERJUMP_CHARGE_TIME)
                        currentState = State.SUPERJUMP_ABLE;
                }
                break;
            case State.SUPERJUMP_ABLE:
                if (!C.getKey(C.JUMP_KEY) && lastJumpPress)
                {
                    grounded = false;
                    currentState = State.SUPERJUMP;
                    yVel = superJumpStrength;
                }
                break;
            case State.SUPERJUMP:
                if (C.getKey(C.RIGHT_KEY))
                {
                    isActivelyMoving = true;
                    xVel += superjumpAirSpeed * Time.deltaTime;
                    isFacingLeft = false;
                }
                if (C.getKey(C.LEFT_KEY))
                {
                    isActivelyMoving = true;
                    xVel -= superjumpAirSpeed * Time.deltaTime;
                    isFacingLeft = true;
                }
                if (C.getKey(C.DOWN_KEY) && !lastDownPress && yVel <= 0)
                {
                    currentState = State.SLAM_TRANS_IN;
                    return;
                }
                if (grounded)
                    currentState = State.IDLE;
                break;
            case State.TAIL_HANG_TRANS_IN:
                return;
            case State.TAIL_HANG:
                xVel = 0;
                if (C.getKey(C.JUMP_KEY) && !lastJumpPress)
                {
                    currentState = State.JUMP;
                    yVel = hangJumpStrength;
                }
                if (C.getKey(C.DOWN_KEY))
                    currentState = State.TAIL_HANG_FALL;
                if (C.getKey(C.LEFT_KEY))
                    isFacingLeft = true;
                else if (C.getKey(C.RIGHT_KEY))
                    isFacingLeft = false;
                scaleX = isFacingLeft ? -1 : 1;
                lastJumpPress = C.getKey(C.JUMP_KEY);
                return;
            case State.TAIL_HANG_FALL:
                if (stateCount > TAIL_HANG_FALL_TIME)
                    currentState = State.JUMP;
                break;
            case State.CROUCH:
                xVel = 0;
               
                if (C.getKey(C.JUMP_KEY))
                {
                    currentState = State.SUPERJUMP_CHARGE;
                }
                if (!C.getKey(C.DOWN_KEY))
                {
                    currentState = State.IDLE;
                }
                break;
         
            case State.SLAM_TRANS_IN:
                if (this.IsStopped)
                    currentState = State.SLAM_MOVE;
                return;
            case State.SLAM_MOVE:
                yVel = slamSpeed * maxYVel;
                xVel = 0;
                if (grounded)
                {
                    currentState = State.SLAM_LAND;
                    C.getCameraInstance().shake(SLAM_LAND_SHAKE, SLAM_LAND_SHAKE_TIME);
                }
                break;
            case State.SLAM_LAND:
                if (this.IsStopped)
                    currentState = State.IDLE;
                break;
        }

        yVel += gravity * Time.deltaTime;
        if (xVel > 0)
        {
            if (!isAttacking())
                xVel = Mathf.Min(maxXVel, xVel);
            TryMoveRight();
        }
        else if (xVel < 0)
        {
            if (!isAttacking())
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
            if (currentState != State.SLAM_MOVE)
                yVel = Mathf.Max(minYVel, yVel);
            TryMoveDown();
        }
        RXDebug.Log(currentState);
        stateCount += Time.deltaTime;
        
        this.scaleX = isFacingLeft ? -1 : 1;
        this.play(currentState.ToString());
        lastJumpPress = C.getKey(C.JUMP_KEY);
        lastDownPress = C.getKey(C.DOWN_KEY);
        lastActionPress = C.getKey(C.ACTION_KEY);
    }
    private const float SLAM_LAND_SHAKE = 2.0f;
    private const float SLAM_LAND_SHAKE_TIME = .2f;
    Tween attackExtendTween;
    private const float TAIL_HANG_FALL_TIME = .1f;
    public float AttackExtendLength
    {
        get { return extendPoleMiddle.width; }
        set
        {
            extendPoleEnd.scaleX = isFacingLeft ? -1 : 1;

            extendPoleMiddle.width = value;
            extendPoleMiddle.SetPosition(this.GetPosition() + (Vector2.right * (this.width / 2 + (extendPoleMiddle.width / 2) * (isFacingLeft ? -1 : 1))));

            extendPoleEnd.SetPosition(extendPoleMiddle.GetPosition() + (Vector2.right * ((extendPoleMiddle.width / 2) * (isFacingLeft ? -1 : 1) + extendPoleEnd.width / 2)));
        }
    }


    private bool isAttacking()
    {
        return currentState == State.ATTACK_ONE || currentState == State.ATTACK_TWO || currentState == State.ATTACK_THREE;
    }
    private const float ATTACK_ONE_TIME = .7f;
    private const float ATTACK_TWO_TIME = .7f;
    private const float ATTACK_THREE_TIME = .8f;
    private const float ATTACK_ONE_XVEL = 20;
    private const float ATTACK_TWO_XVEL = 30;
    private const float ATTACK_THREE_XVEL = 10;

    public void StartAttackOne()
    {
        currentState = State.ATTACK_ONE;
        xVel = ATTACK_ONE_XVEL * (isFacingLeft ? -1 : 1);
        this.play(State.ATTACK_ONE.ToString(), true);
        lastActionPress = C.getKey(C.ACTION_KEY);
    }

    public void StartAttackTwo()
    {
        currentState = State.ATTACK_TWO;
        if (C.getKey(C.LEFT_KEY))
            isFacingLeft = true;
        else if (C.getKey(C.RIGHT_KEY))
            isFacingLeft = false;
        xVel = ATTACK_TWO_XVEL * (isFacingLeft ? -1 : 1);
        this.play(State.ATTACK_TWO.ToString(), true);
    }

    public void StartAttackThree()
    {
        currentState = State.ATTACK_THREE;
        if (C.getKey(C.LEFT_KEY))
            isFacingLeft = true;
        else if (C.getKey(C.RIGHT_KEY))
            isFacingLeft = false;
        xVel = ATTACK_THREE_XVEL * (isFacingLeft ? -1 : 1);
        this.play(State.ATTACK_THREE.ToString(), true);
    }

    private const float SUPERJUMP_CHARGE_TIME = 1.5f;

    public void TryMoveRight()
    {
        float newX = this.x + xVel;
        float topY = this.y + tilemap.tileHeight / 3;
        float bottomY = this.y - tilemap.tileHeight / 3;
        if (tilemap.isPassable(newX + tilemap.tileWidth / 3, topY) &&
            tilemap.isPassable(newX + tilemap.tileWidth / 3, bottomY))
            this.x = newX;
        else
        {
            this.x = Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth * 2 / 3f;
            xVel = 0;
        }

    }

    public void TryMoveLeft()
    {
        float newX = this.x + xVel;
        float topY = this.y + tilemap.tileHeight / 3;
        float bottomY = this.y - tilemap.tileHeight / 3;
        if (tilemap.isPassable(newX - tilemap.tileWidth / 3, topY) &&
            tilemap.isPassable(newX - tilemap.tileWidth / 3, bottomY))
            this.x = newX;
        else
        {
            this.x = Mathf.CeilToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth - tilemap.tileWidth * 2 / 3f;
            xVel = 0;
        }
    }

    public void TryMoveDown()
    {
        float newY = this.y + yVel;
        float leftX = this.x - tilemap.tileWidth / 4;
        float rightX = this.x + tilemap.tileWidth / 4;
        if ((!tilemap.isPassable(leftX, newY - tilemap.tileWidth) ||
            !tilemap.isPassable(rightX, newY - tilemap.tileWidth)) ||
            (tilemap.isOneWay(leftX, newY - tilemap.tileWidth) ||
            tilemap.isOneWay(rightX, newY - tilemap.tileWidth)))
        {
            if (currentState == State.TAIL_HANG_FALL)
                currentState = State.IDLE;
            grounded = true;
        }
        else
        {
            switch (currentState)
            {
                case State.IDLE:
                case State.SLIDE:
                case State.RUN:
                case State.CROUCH:
                    currentState = State.JUMP;
                    grounded = false;
                    break;
            }
        }
        if ((tilemap.isPassable(leftX, newY - tilemap.tileWidth / 2) &&
            tilemap.isPassable(rightX, newY - tilemap.tileWidth / 2)) &&
            !(tilemap.isOneWay(leftX, newY - tilemap.tileWidth / 2) &&
            tilemap.isOneWay(rightX, newY - tilemap.tileWidth / 2)))
            this.y = newY;
        else
        {
            grounded = true;
            if (currentState == State.JUMP)
                currentState = State.IDLE;
            yVel = 0;
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileWidth / 2;
        }
        CheckHookDown();
        CheckOneWayDown();
    }
    private void CheckHookUp()
    {
        if (tilemap.isHook(this.x, this.y))
        {
            currentState = State.TAIL_HANG_TRANS_IN;
            Go.to(this, TRANS_HOOK_TIME, new TweenConfig().floatProp("x", Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2).floatProp("y", Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight / 2).setEaseType(EaseType.Linear).onComplete((t) => { currentState = State.TAIL_HANG; }));
            xVel = 0;
            yVel = 0;
        }
    }
    private const float TRANS_HOOK_TIME = .05f;
    private void CheckHookDown()
    {
        if (C.getKey(C.DOWN_KEY) || currentState == State.TAIL_HANG_FALL)
            return;
        if (tilemap.isHook(this.x, this.y))
        {
            currentState = State.TAIL_HANG_TRANS_IN;
            Go.to(this, TRANS_HOOK_TIME, new TweenConfig().floatProp("x", Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2).floatProp("y", Mathf.CeilToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight - tilemap.tileHeight / 2).setEaseType(EaseType.Linear).onComplete((t) => { currentState = State.TAIL_HANG; }));
            xVel = 0;
            yVel = 0;
        }
    }
    private void CheckOneWayDown()
    {
        
    }
    public void TryMoveUp()
    {
        float newY = this.y + yVel;
        float leftX = this.x - tilemap.tileWidth / 4;
        float rightX = this.x + tilemap.tileWidth / 4;
        if (tilemap.isPassable(leftX, newY + tilemap.tileHeight / 2) &&
            tilemap.isPassable(rightX, newY + tilemap.tileHeight / 2))
            this.y = newY;
        else
        {
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight / 2;
        }
        if (yVel < HOOK_MIN_Y_VEL)
            CheckHookUp();
    }
    private const float HOOK_MIN_Y_VEL = 5;


}

