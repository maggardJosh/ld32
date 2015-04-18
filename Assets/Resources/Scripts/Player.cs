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
        DOUBLE_JUMP,
        SLIDE,
        CROUCH,
        SUPERJUMP_CHARGE,
        SUPERJUMP_ABLE,
        ATTACK_ONE,
        ATTACK_TWO,
        ATTACK_THREE,
        ATTACK_THREE_EXTEND,
        POWERPOLE_EXTEND_DOWN_TRANS_IN,
        POWERPOLE_EXTEND_DOWN,
        POWERPOLE_EXTEND_DOWN_TRANS_OUT,
        TAIL_HANG

    }
    private FSprite extendPoleMiddle;
    private FSprite extendPoleEnd;
    public Player(FTilemap tilemap)
        : base("player")
    {
        extendPoleMiddle = new FSprite("pole_mid");
        extendPoleEnd = new FSprite("pole_end");

        this.tilemap = tilemap;
        addAnimation(new FAnimation(State.IDLE.ToString(), new int[] { 1 }, 100, true));
        addAnimation(new FAnimation(State.RUN.ToString(), new int[] { 2, 3, 4, 5 }, 100, true));
        addAnimation(new FAnimation(State.SLIDE.ToString(), new int[] { 6 }, 100, true));
        addAnimation(new FAnimation(State.JUMP.ToString(), new int[] { 1, 1, 1, 2 }, 100, true));
        addAnimation(new FAnimation(State.ATTACK_ONE.ToString(), new int[] { 7, 8, 9, 10}, 100, false));
        addAnimation(new FAnimation(State.ATTACK_TWO.ToString(), new int[] { 11, 12, 13}, 100, false));
        addAnimation(new FAnimation(State.ATTACK_THREE.ToString(), new int[] { 14,15 }, 100, false));
        addAnimation(new FAnimation(State.ATTACK_THREE_EXTEND.ToString(), new int[] { 16 }, 100, false));

        addAnimation(new FAnimation(State.SUPERJUMP_CHARGE.ToString(), new int[] { 6 }, 100, true));
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
                _currentState = value;
                stateCount = 0;
            }
        }
    }
    private bool isFacingLeft = false;
    private FTilemap tilemap;

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
    private bool grounded = false;
    private float jumpStrength = 15;
    private float superJumpStrength = 25;
    private float hangJumpStrength = 20;
    private float speed = 200;
    private float gravity = -50;
    private float stateCount = 0;
    private float poleExtendLength = 96;
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
                    Futile.stage.AddChild(extendPoleMiddle);
                    Futile.stage.AddChild(extendPoleEnd);
                    AttackExtendLength = 0;
                    currentState = State.ATTACK_THREE_EXTEND;
                    attackExtendTween = Go.to(this, ATTACK_THREE_EXTEND_TIME / 2, new TweenConfig().floatProp("AttackExtendLength", poleExtendLength).setIterations(2, LoopType.PingPong).setEaseType(EaseType.ExpoIn).onComplete((t) => { extendPoleEnd.RemoveFromContainer(); extendPoleMiddle.RemoveFromContainer(); currentState = State.IDLE; }));
                }
                break;
            case State.ATTACK_THREE_EXTEND:

                break;
            case State.SUPERJUMP_CHARGE:
                if (!C.getKey(C.DOWN_KEY))
                {
                    currentState = State.IDLE;
                    return;
                }
                if (!C.getKey(C.JUMP_KEY))
                {
                    currentState = State.CROUCH;
                    return;
                }
                if (C.getKey(C.DOWN_KEY) && C.getKey(C.JUMP_KEY))
                {
                    if (stateCount >= SUPERJUMP_CHARGE_TIME)
                        currentState = State.SUPERJUMP_ABLE;
                }
                break;
            case State.SUPERJUMP_ABLE:
                if (!C.getKey(C.DOWN_KEY))
                {
                    currentState = State.IDLE;
                    return;
                }
                if (!C.getKeyDown(C.JUMP_KEY))
                {
                    grounded = false;
                    currentState = State.JUMP;
                    yVel = superJumpStrength;
                }
                break;
            case State.TAIL_HANG:
                if (C.getKey(C.JUMP_KEY) && !lastJumpPress)
                {
                    currentState = State.JUMP;
                    yVel = hangJumpStrength;
                }
                break;
            case State.CROUCH:
                xVel *= groundFriction;
                if (C.getKey(C.ACTION_KEY))
                {
                    currentState = State.POWERPOLE_EXTEND_DOWN_TRANS_IN;
                }
                if (C.getKey(C.JUMP_KEY))
                {
                    currentState = State.SUPERJUMP_CHARGE;
                }
                if (!C.getKey(C.DOWN_KEY))
                {
                    currentState = State.IDLE;
                }
                break;
            case State.POWERPOLE_EXTEND_DOWN_TRANS_IN:
                if (C.getKey(C.ACTION_KEY))
                {
                    //just go to idle for now
                }
                currentState = State.IDLE;
                break;
            case State.POWERPOLE_EXTEND_DOWN:

                break;
            case State.POWERPOLE_EXTEND_DOWN_TRANS_OUT:

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
            yVel = Mathf.Max(minYVel, yVel);
            TryMoveDown();
        }

        stateCount += Time.deltaTime;

        this.scaleX = isFacingLeft ? -1 : 1;
        this.play(currentState.ToString());
        lastJumpPress = C.getKey(C.JUMP_KEY);
        lastDownPress = C.getKey(C.DOWN_KEY);
        lastActionPress = C.getKey(C.ACTION_KEY);
    }
    Tween attackExtendTween;
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

    private const float ATTACK_THREE_EXTEND_TIME = .5f;

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
        xVel = ATTACK_TWO_XVEL * (isFacingLeft ? -1 : 1);
        this.play(State.ATTACK_TWO.ToString(), true);
    }

    public void StartAttackThree()
    {
        currentState = State.ATTACK_THREE;
        xVel = ATTACK_THREE_XVEL * (isFacingLeft ? -1 : 1);
        this.play(State.ATTACK_THREE.ToString(), true);
    }

    private const float SUPERJUMP_CHARGE_TIME = 1.5f;

    public void TryMoveRight()
    {
        float newX = this.x + xVel;
        float topY = this.y + tilemap.tileHeight / 3;
        float bottomY = this.y - tilemap.tileHeight / 3;
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
        float topY = this.y + tilemap.tileHeight / 3;
        float bottomY = this.y - tilemap.tileHeight / 3;
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
        float leftX = this.x - tilemap.tileWidth / 3;
        float rightX = this.x + tilemap.tileWidth / 3;
        if (!tilemap.isPassable(leftX, newY - tilemap.tileWidth) ||
            !tilemap.isPassable(rightX, newY - tilemap.tileWidth))
        {
            grounded = true;
        }
        if (tilemap.isPassable(leftX, newY - tilemap.tileWidth / 2) &&
            tilemap.isPassable(rightX, newY - tilemap.tileWidth / 2))
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
    }
    private void CheckHookUp()
    {
        if (tilemap.isHook(this.x, this.y))
        {
            this.x = Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2;
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight / 2;
            currentState = State.TAIL_HANG;
            xVel = 0;
            yVel = 0;
        }
    }
    private void CheckHookDown()
    {
        if (tilemap.isHook(this.x, this.y))
        {
            this.x = Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2;
            this.y = Mathf.CeilToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight - tilemap.tileHeight / 2;
            currentState = State.TAIL_HANG;
            xVel = 0;
            yVel = 0;
        }
    }
    public void TryMoveUp()
    {
        float newY = this.y + yVel;
        float leftX = this.x - tilemap.tileWidth / 3;
        float rightX = this.x + tilemap.tileWidth / 3;
        if (tilemap.isPassable(leftX, newY + tilemap.tileHeight / 2) &&
            tilemap.isPassable(rightX, newY + tilemap.tileHeight / 2))
            this.y = newY;
        else
        {
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight / 2;
        }
        if (yVel > HOOK_MIN_Y_VEL)
            CheckHookUp();
    }
    private const float HOOK_MIN_Y_VEL = 5;


}

