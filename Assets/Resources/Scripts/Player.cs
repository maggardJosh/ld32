using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FAnimatedSprite
{
    public class SaveState
    {
        public bool thirdCombo = true;
        public bool poleExtend = true;
        public bool tailGrab = true;
        public bool doubleJump = true;
        public bool levers = true;
        public bool chargeJump = false;
        public bool airJumpAttack = false;
        public bool slam = true;
        public bool airAttackEndGame = true;

        public List<string> activatedObjects = new List<string>();
        public SaveState()
        {

        }
        public SaveState(SaveState stateToCopy)
        {
            this.thirdCombo = stateToCopy.thirdCombo;
            this.poleExtend = stateToCopy.thirdCombo;
            this.tailGrab = stateToCopy.thirdCombo;
            this.doubleJump = stateToCopy.thirdCombo;
            this.levers = stateToCopy.thirdCombo;
            this.chargeJump = stateToCopy.thirdCombo;
            this.airJumpAttack = stateToCopy.thirdCombo;
            this.slam = stateToCopy.thirdCombo;
            this.airAttackEndGame = stateToCopy.thirdCombo;


            this.activatedObjects = new List<string>();
            this.activatedObjects.AddRange(stateToCopy.activatedObjects);
        }
    }
    private enum State
    {
        IDLE,
        RUN,
        JUMP,
        FALL,
        DOUBLE_JUMP,
        SLIDE,
        SUPERJUMP_CHARGE,
        SUPERJUMP_ABLE,
        SUPERJUMP,
        ATTACK_ONE,
        ATTACK_TWO,
        ATTACK_THREE,
        ATTACK_THREE_EXTEND,
        ATTACK_AIR,
        TAIL_HANG_TRANS_IN,
        TAIL_HANG_FALL,
        TAIL_HANG,
        SLAM_TRANS_IN,
        SLAM_MOVE,
        SLAM_LAND,
        TAKE_DAMAGE,
        INTERACT_LEVER,
        DYING

    }
    private FSprite extendPoleMiddle;
    private FSprite extendPoleEnd;

    private static SaveState saveState;
    private static SaveState lastRoomSaveState;
    public static void ResetSaveState()
    {
        saveState = new SaveState(GetLastRoomSaveStateInstance());
    }
    public static SaveState GetSaveStateInstance()
    {
        if (saveState == null)
            saveState = new SaveState();
        return saveState;
    }
    public static SaveState GetLastRoomSaveStateInstance()
    {
        if (lastRoomSaveState == null)
            lastRoomSaveState = new SaveState(GetSaveStateInstance());
        return lastRoomSaveState;
    }

    public static void SaveRoom()
    {
        lastRoomSaveState = new SaveState(GetSaveStateInstance());
    }
    public World world;
    private Lever interactLever;
    public FSprite indication;
    private float indYDiff = 0;
    private const float MAX_IND_Y_DIFF = 20;
    public float IndicationY { get { return indYDiff; } set { indYDiff = value; indication.SetPosition(this.GetPosition() + Vector2.up * (indYDiff + 30f)); indication.scale = .6f + (indYDiff / MAX_IND_Y_DIFF) * .3f; } }
    public Player()
        : base("player")
    {
        extendPoleMiddle = new FSprite("pole_mid");
        extendPoleEnd = new FSprite("pole_end");
        indication = new FSprite("indication");
        Go.to(this, 1.0f, new TweenConfig().floatProp("IndicationY", MAX_IND_Y_DIFF).setEaseType(EaseType.QuadIn).setIterations(-1, LoopType.PingPong));

        //Just landing 26
        addAnimation(new FAnimation(State.IDLE.ToString(), new int[] { 1, 2, 3, 4, 1 }, 100, false));
        addAnimation(new FAnimation(State.RUN.ToString(), new int[] { 5, 6, 7, 8 }, 100, true));
        addAnimation(new FAnimation(State.SLIDE.ToString(), new int[] { 9 }, 100, true));
        addAnimation(new FAnimation(State.TAKE_DAMAGE.ToString(), new int[] { 9, 10 }, 100, true));
        addAnimation(new FAnimation(State.JUMP.ToString(), new int[] { 21 }, 100, true));
        addAnimation(new FAnimation(State.DOUBLE_JUMP.ToString(), new int[] { 21 }, 100, true));
        addAnimation(new FAnimation(State.FALL.ToString(), new int[] { 22 }, 100, true));
        addAnimation(new FAnimation(State.SUPERJUMP_CHARGE.ToString(), new int[] { 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 23, 23, 23, 23, 24, 24, 24, 24, 23, 23, 23, 24, 24, 24, 23, 23, 24, 24, 23, 24, 23, 24 }, 40, false));
        addAnimation(new FAnimation(State.INTERACT_LEVER.ToString(), new int[] { 15, 16, 17 }, 300, false));
        addAnimation(new FAnimation(State.ATTACK_ONE.ToString(), new int[] { 11, 12, 13, 14 }, 100, false));
        addAnimation(new FAnimation(State.ATTACK_TWO.ToString(), new int[] { 15, 16, 17 }, 100, false));
        addAnimation(new FAnimation(State.ATTACK_THREE.ToString(), new int[] { 18, 19 }, 20, false));
        addAnimation(new FAnimation(State.ATTACK_THREE_EXTEND.ToString(), new int[] { 20 }, 100, false));
        addAnimation(new FAnimation(State.TAIL_HANG_TRANS_IN.ToString(), new int[] { 28, 29, 30, 31 }, 50, false));
        addAnimation(new FAnimation(State.TAIL_HANG.ToString(), new int[] { 27 }, 100, true));
        addAnimation(new FAnimation(State.TAIL_HANG_FALL.ToString(), new int[] { 28, 29, 30, 31 }, 50, true));
        addAnimation(new FAnimation(State.SLAM_TRANS_IN.ToString(), new int[] { 28, 29, 30, 31, 32, 33 }, 20, false));
        addAnimation(new FAnimation(State.SLAM_MOVE.ToString(), new int[] { 34 }, 100, false));
        addAnimation(new FAnimation(State.SLAM_LAND.ToString(), new int[] { 35, 36, 37, 38, 39 }, 50, false));
        addAnimation(new FAnimation(State.SUPERJUMP.ToString(), new int[] { 43, 44, 45, 46 }, 100, true));
        addAnimation(new FAnimation(State.ATTACK_AIR.ToString(), new int[] { 40, 40, 40, 40, 41, 41, 41, 41, 42 }, 10, false));

        addAnimation(new FAnimation(State.SUPERJUMP_ABLE.ToString(), new int[] { 25 }, 50, true));

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
                    case State.SUPERJUMP_CHARGE:
                    case State.INTERACT_LEVER:
                        this.play(value.ToString(), true);
                        break;
                    case State.ATTACK_AIR:
                        attackAirCount = ATTACK_AIR_COUNT_VALUE;
                        break;
                    case State.DYING:
                        C.getCameraInstance().shake(5.0f, .2f);
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
    private float speedMax = 180;
    private float acceleration = 0;
    private float speed = 0;
    private float accel = 15;
    private float accelDecay = .8f;
    private float superjumpAirSpeed = 5;
    private float gravity = -50;
    private float stateCount = 0;
    private float poleExtendLength = 96;
    private float attackAirCount = 0;
    private bool usedDoubleJump = true;

    private const float ATTACK_THREE_EXTEND_TIME = .3f;
    private const float ATTACK_AIR_YVEL = 8;
    private const float ATTACK_AIR_COUNT_VALUE = .5f;
    private const float DEATH_TIME = 1.0f;
    private Vector2 lastExtendPos = Vector2.zero;
    private float slamDist = 0;
    private float slamStart = 0;
    private float SLAM_BUTTON_DIST = 32 * 10;
    private float SLAM_PARTICLE_DIST = 32 * 7;
    public void Update()
    {
        if (C.isDebug)
            CheckPowerupKeys();
        if (C.isTransitioning)
            return;
        switch (currentState)
        {
            case State.IDLE:
            case State.JUMP:
            case State.DOUBLE_JUMP:
            case State.RUN:
            case State.SLIDE:
                if (C.getKey(C.ACTION_KEY) && !lastActionPress && interactLever != null)
                {
                    currentState = State.INTERACT_LEVER;
                    this.SetPosition(interactLever.GetPosition());
                    return;
                }
                if (currentState == State.IDLE)
                    if (this.IsStopped && RXRandom.Float() < .2f)
                        this.play(currentState.ToString(), true);
                bool isActivelyMoving = false;
                if (C.getKey(C.JUMP_KEY) && !lastJumpPress && currentState != State.DOUBLE_JUMP)
                {
                    if (currentState == State.JUMP && GetSaveStateInstance().doubleJump)
                    {
                        usedDoubleJump = true;
                        currentState = State.DOUBLE_JUMP;
                        foreach (Particle p in Particle.CloudParticle.GetDoubleJumpParticles(this.GetPosition()))
                            Futile.stage.AddChild(p);
                        yVel = jumpStrength;
                    }
                    if (grounded)
                    {
                        grounded = false;
                        currentState = State.JUMP;
                        yVel = jumpStrength;
                    }
                }
                if (C.getKey(C.DOWN_KEY) && (!C.getKey(C.LEFT_KEY) || !C.getKey(C.RIGHT_KEY)) && !lastDownPress && grounded && GetSaveStateInstance().chargeJump)
                {
                    currentState = State.SUPERJUMP_CHARGE;
                    return;
                }
                if (currentState == State.JUMP || currentState == State.DOUBLE_JUMP)
                {
                    if (GetSaveStateInstance().slam && C.getKey(C.DOWN_KEY) && !lastDownPress)
                    {
                        currentState = State.SLAM_TRANS_IN;
                        return;
                    }

                    if (C.getKey(C.ACTION_KEY) && !lastActionPress && attackAirCount <= 0)
                    {
                        StartAttackAir();
                        return;
                    }
                }
                if (C.getKey(C.RIGHT_KEY))
                {
                    isActivelyMoving = true;
                    //xVel += speed * Time.deltaTime;
                    speed += accel;
                    speed = Mathf.Min(speed, speedMax);
                    xVel += speed * Time.deltaTime;
                    isFacingLeft = false;
                }
                if (C.getKey(C.LEFT_KEY))
                {
                    isActivelyMoving = true;
                    //xVel -= speed * Time.deltaTime;
                    speed += accel;
                    speed = Mathf.Min(speed, speedMax);
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
                if (currentState == State.SLIDE || currentState == State.IDLE)
                {
                    speed *= accelDecay;
                    if (Mathf.Abs(speed) < .1f)
                        speed = 0;
                }
                if (grounded)
                    xVel *= groundFriction;
                else
                    xVel *= airFriction;
                if (Mathf.Abs(xVel) < .1f)
                    xVel = 0;
                break;
            case State.INTERACT_LEVER:
                if (this.IsStopped)
                {
                    world.ActivateLever(interactLever);
                    interactLever = null;
                    indication.RemoveFromContainer();
                    currentState = State.IDLE;
                }
                return;
            case State.ATTACK_ONE:
                xVel *= groundFriction;
                if (stateCount > ATTACK_ONE_TIME)
                    currentState = State.IDLE;
                if (stateCount > .15f && C.getKey(C.ACTION_KEY) && !lastActionPress)
                    StartAttackTwo();
                break;
            case State.ATTACK_TWO:
                xVel *= groundFriction;
                if (GetSaveStateInstance().thirdCombo)
                {
                    if (stateCount > ATTACK_TWO_TIME)
                        currentState = State.IDLE;
                    if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                        StartAttackThree();
                }
                else
                {
                    if (this.IsStopped)
                        currentState = State.IDLE;
                }
                break;
            case State.ATTACK_THREE:
                xVel *= groundFriction;
                if (this.IsStopped)
                {
                    xVel = 0;
                    this.container.AddChild(extendPoleMiddle);
                    this.container.AddChild(extendPoleEnd);
                    AttackExtendLength = 0;
                    currentState = State.ATTACK_THREE_EXTEND;
                    lastExtendPos = this.GetPosition();
                    attackExtendTween = Go.to(this, ATTACK_THREE_EXTEND_TIME / 8, new TweenConfig().floatProp("AttackExtendLength", GetSaveStateInstance().poleExtend ? poleExtendLength : 0).setEaseType(EaseType.QuartIn).onComplete(
                        (t) =>
                        {
                            world.tryBreakWall(lastExtendPos, extendPoleEnd.GetPosition());
                            int tileCollision = world.ExtendPolePassable(lastExtendPos, extendPoleEnd.GetPosition());
                            if (tileCollision != -1)
                            {

                                float tileCollisionPos = tileCollision * tilemap.tileWidth;
                                if (tileCollisionPos > this.x)
                                    AttackExtendLength = Mathf.Max(0, tileCollision * tilemap.tileWidth - this.x - 32f);
                                else
                                    AttackExtendLength = Mathf.Max(0, this.x - 64f - tileCollision * tilemap.tileWidth);

                            }
                            world.CheckWallButton(lastExtendPos, extendPoleEnd.GetPosition());
                            TweenExtendRetract();
                        }));
                }
                break;
            case State.ATTACK_THREE_EXTEND:
                world.tryBreakWall(lastExtendPos, extendPoleEnd.GetPosition());
                if (attackExtendTween.state == TweenState.Running)
                {
                    int tileCollision = world.ExtendPolePassable(lastExtendPos, extendPoleEnd.GetPosition());
                    if (tileCollision != -1)
                    {

                        attackExtendTween.pause();
                        //Go.removeTween(attackExtendTween);
                        float tileCollisionPos = tileCollision * tilemap.tileWidth;
                        if (tileCollisionPos > this.x)
                            AttackExtendLength = Mathf.Max(0, tileCollision * tilemap.tileWidth - this.x - 32f);
                        else
                            AttackExtendLength = Mathf.Max(0, this.x - 64f - tileCollision * tilemap.tileWidth);
                        world.CheckWallButton(lastExtendPos, extendPoleEnd.GetPosition());
                        TweenExtendRetract();
                    }
                }
                if (retractTween != null && retractTween.state == TweenState.Paused && !C.isTransitioning)
                    retractTween.play();
                lastExtendPos = extendPoleEnd.GetPosition();
                break;
            case State.ATTACK_AIR:
                yVel = 0;
                if (stateCount > ATTACK_AIR_TIME)
                {
                    if (!usedDoubleJump)
                        currentState = State.JUMP;
                    else
                        currentState = State.DOUBLE_JUMP;
                }
                //if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                //    StartAttackAir();
                break;
            case State.DYING:
                this.isVisible = false;
                stateCount += Time.deltaTime;
                if (stateCount > DEATH_TIME)
                {
                    Player.ResetSaveState();
                    world.Respawn();
                    currentState = State.IDLE;
                    this.isVisible = true;
                }
                return;
            case State.SUPERJUMP_ABLE:
                if (!C.getKey(C.DOWN_KEY) && lastDownPress)
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
                if (GetSaveStateInstance().slam && C.getKey(C.DOWN_KEY) && !lastDownPress && yVel <= 0)
                {
                    currentState = State.SLAM_TRANS_IN;
                    return;
                }
                if (yVel > 0)
                {
                    CeilButton button = world.getCeilButton(this.x, this.y);
                    if (button != null)
                        world.ActivateCeilButton(button);
                }
                if (grounded)
                    currentState = State.IDLE;
                break;
            case State.TAIL_HANG_TRANS_IN:
                return;
            case State.TAIL_HANG:
                xVel = 0;
                speed = speedMax * .5f;
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
                if (stateCount > .1f)
                    currentState = State.JUMP;
                if (this.IsStopped)
                    currentState = State.JUMP;
                break;
            case State.SUPERJUMP_CHARGE:
                xVel = 0;
                speed = 0;
                if (this.IsStopped)
                    currentState = State.SUPERJUMP_ABLE;
                if (!C.getKey(C.DOWN_KEY))
                {
                    currentState = State.IDLE;
                    return;
                }
                break;

            case State.SLAM_TRANS_IN:
                if (this.IsStopped)
                    currentState = State.SLAM_MOVE;
                slamDist = 0;
                slamStart = this.y;
                return;
            case State.SLAM_MOVE:
                yVel = slamSpeed * maxYVel;
                xVel = 0;
                speed = 0;
                slamDist = slamStart - this.y;
                if (slamDist > SLAM_PARTICLE_DIST)
                    foreach (Particle p in Particle.CloudParticle.GetMegaSlamParticles(this.GetPosition()))
                        Futile.stage.AddChild(p);
                if (grounded)
                {
                    float floorY = Mathf.FloorToInt((this.y) / tilemap.tileHeight) * tilemap.tileHeight;
                    if (world.isPassable(this.x, floorY - tilemap.tileHeight / 2f))
                        floorY -= tilemap.tileHeight;
                    //floorY -= tilemap.tileHeight f;
                    foreach (Particle p in Particle.CloudParticle.GetSlamParticles(new Vector2(this.x, floorY)))
                        Futile.stage.AddChild(p);
                    currentState = State.SLAM_LAND;
                    this.y = floorY;
                    FloorButton button = world.getFloorButton(this.x, floorY);
                    if (button != null)
                        world.ActivateFloorButton(button);
                    if (slamDist >= SLAM_BUTTON_DIST)
                    {

                        SlamButton slamButton = world.getSlamButton(this.x, floorY - tilemap.tileHeight / 2);
                        if (slamButton != null)
                            world.ActivateSlamButton(slamButton);
                    }
                    C.getCameraInstance().shake(SLAM_LAND_SHAKE, SLAM_LAND_SHAKE_TIME);
                }
                break;
            case State.SLAM_LAND:
                if (this.IsStopped)
                {
                    currentState = State.IDLE;
                }
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
        //RXDebug.Log(currentState);
        stateCount += Time.deltaTime;

        if (attackAirCount > 0)
            attackAirCount -= Time.deltaTime;

        this.scaleX = isFacingLeft ? -1 : 1;
        if (currentState == State.SUPERJUMP && yVel < 0)
            this.play(State.TAIL_HANG_FALL.ToString());
        else if ((currentState == State.JUMP || currentState == State.DOUBLE_JUMP) && yVel < 0)
            this.play(State.FALL.ToString());
        else
            this.play(currentState.ToString());

        CheckInteractableObjects();
        CheckPowerupCollision();
        world.CheckTutorialText();

        lastJumpPress = C.getKey(C.JUMP_KEY);
        lastDownPress = C.getKey(C.DOWN_KEY);
        lastActionPress = C.getKey(C.ACTION_KEY);
    }
    Tween retractTween;
    private void TweenExtendRetract()
    {
        foreach (Particle p in Particle.CloudParticle.GetThirdAttackParticles(this.GetPosition(), isFacingLeft))
            Futile.stage.AddChild(p);
        C.getCameraInstance().shake(1.0f, .2f);
        retractTween = Go.to(this, ATTACK_THREE_TIME / 3, new TweenConfig().floatProp("AttackExtendLength", 0).setEaseType(EaseType.QuadIn).onComplete((t2) =>
        {
            extendPoleEnd.RemoveFromContainer();
            extendPoleMiddle.RemoveFromContainer();
            currentState = State.IDLE;
        }));
        if (C.isTransitioning)
            retractTween.pause();
    }
    private void CheckPowerupCollision()
    {
        Powerup p = world.getPowerup(this.x, this.y);
        if (p != null)
        {
            this.SetPosition(p.GetPosition());
            this.play(State.IDLE.ToString(), true);
            yVel = -16; //Move to the pedestal
            TryMoveDown();
            world.ActivatePowerup(p);
        }
    }
    
    private void CheckPowerupKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            GetSaveStateInstance().thirdCombo = !GetSaveStateInstance().thirdCombo;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            GetSaveStateInstance().tailGrab = !GetSaveStateInstance().tailGrab;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            GetSaveStateInstance().slam = !GetSaveStateInstance().slam;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            GetSaveStateInstance().doubleJump = !GetSaveStateInstance().doubleJump;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            GetSaveStateInstance().poleExtend = !GetSaveStateInstance().poleExtend;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            GetSaveStateInstance().levers = !GetSaveStateInstance().levers;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            GetSaveStateInstance().chargeJump = !GetSaveStateInstance().chargeJump;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            GetSaveStateInstance().airAttackEndGame = !GetSaveStateInstance().airAttackEndGame;
    }
    private void CheckInteractableObjects()
    {
        if (GetSaveStateInstance().levers)
        {

            Lever interactableLever = world.GetInteractableLever();
            if (this.interactLever == null && interactableLever != null)
            {
                this.interactLever = interactableLever;
                Futile.stage.AddChild(indication);
            }
            else
            {
                if (this.interactLever != null && interactableLever == null)
                {
                    this.interactLever = null;
                    indication.RemoveFromContainer();
                }
            }
        }
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
        return currentState == State.ATTACK_ONE || currentState == State.ATTACK_TWO || currentState == State.ATTACK_THREE || currentState == State.ATTACK_AIR;
    }
    private const float ATTACK_ONE_TIME = .7f;
    private const float ATTACK_TWO_TIME = .7f;
    private const float ATTACK_THREE_TIME = .8f;
    private const float ATTACK_AIR_TIME = .1f;
    private const float ATTACK_ONE_XVEL = 20;
    private const float ATTACK_TWO_XVEL = 30;
    private const float ATTACK_THREE_XVEL = 10;
    private const float ATTACK_AIR_XVEL = 12;
    private const float ATTACK_AIR_XVEL_ENDGAME = 40;

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

    public void StartAttackAir()
    {
        yVel = ATTACK_AIR_YVEL;
        currentState = State.ATTACK_AIR;
        if (C.getKey(C.LEFT_KEY))
            isFacingLeft = true;
        else if (C.getKey(C.RIGHT_KEY))
            isFacingLeft = false;
        xVel = (GetSaveStateInstance().airAttackEndGame ? ATTACK_AIR_XVEL_ENDGAME : ATTACK_AIR_XVEL) * (isFacingLeft ? -1 : 1);
        this.play(currentState.ToString(), true);
    }

    private const float SUPERJUMP_CHARGE_TIME = 1.5f;

    public void TryMoveRight()
    {
        float newX = this.x + xVel;
        float topY = this.y + tilemap.tileHeight / 3;
        float bottomY = this.y - tilemap.tileHeight / 3;
        if (world.isPassable(newX + tilemap.tileWidth / 3, topY) &&
            world.isPassable(newX + tilemap.tileWidth / 3, bottomY))
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
        if (world.isPassable(newX - tilemap.tileWidth / 3, topY) &&
            world.isPassable(newX - tilemap.tileWidth / 3, bottomY))
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
        if ((!world.isPassable(leftX, newY - tilemap.tileWidth) ||
            !world.isPassable(rightX, newY - tilemap.tileWidth)) ||
            (world.isOneWay(leftX, newY - tilemap.tileWidth) ||
            world.isOneWay(rightX, newY - tilemap.tileWidth)))
        {

            if (currentState == State.TAIL_HANG_FALL)
                currentState = State.IDLE;
            grounded = true;
            usedDoubleJump = false;
        }
        else
        {
            switch (currentState)
            {
                case State.IDLE:
                case State.SLIDE:
                case State.RUN:
                case State.SUPERJUMP_CHARGE:
                    currentState = State.JUMP;
                    grounded = false;
                    break;
            }
        }
        if ((world.isPassable(leftX, newY - tilemap.tileWidth / 2) &&
            world.isPassable(rightX, newY - tilemap.tileWidth / 2)) &&
            !(world.isOneWay(leftX, newY - tilemap.tileWidth / 2) &&
            world.isOneWay(rightX, newY - tilemap.tileWidth / 2)))
        {
            this.y = newY;
            if (world.isDeath(this.x, this.y - tilemap.tileHeight / 2))
            {
                currentState = State.DYING;
                foreach (Particle particle in Particle.CloudParticle.GetDoubleJumpParticles(this.GetPosition()))
                    Futile.stage.AddChild(particle);
                return;
            }
        }
        else
        {

            grounded = true;
            usedDoubleJump = false;
            if (currentState == State.JUMP)
                currentState = State.IDLE;
            yVel = 0;
            this.y = Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileWidth / 2;
        }
        CheckHookDown();
        FloorButton button = world.getFloorButton(this.x, this.y);
        if (button != null)
            this.y = button.y + 6;
    }
    private void CheckHookUp()
    {
        if (GetSaveStateInstance().tailGrab && tilemap.isHook(this.x, this.y))
        {
            currentState = State.TAIL_HANG_TRANS_IN;
            Go.to(this, TRANS_HOOK_TIME, new TweenConfig().floatProp("x", Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2).floatProp("y", Mathf.FloorToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight + tilemap.tileHeight / 2).setEaseType(EaseType.Linear).onComplete((t) => { currentState = State.TAIL_HANG; }));
            xVel = 0;
            yVel = 0;
        }
    }
    private const float TRANS_HOOK_TIME = .2f;
    private void CheckHookDown()
    {
        if (!GetSaveStateInstance().tailGrab || C.getKey(C.DOWN_KEY) || currentState == State.TAIL_HANG_FALL)
            return;
        if (tilemap.isHook(this.x, this.y))
        {
            currentState = State.TAIL_HANG_TRANS_IN;
            Go.to(this, TRANS_HOOK_TIME, new TweenConfig().floatProp("x", Mathf.FloorToInt(this.x / tilemap.tileWidth) * tilemap.tileWidth + tilemap.tileWidth / 2).floatProp("y", Mathf.CeilToInt(this.y / tilemap.tileHeight) * tilemap.tileHeight - tilemap.tileHeight / 2).setEaseType(EaseType.Linear).onComplete((t) => { currentState = State.TAIL_HANG; }));
            xVel = 0;
            yVel = 0;
        }
    }

    public void TryMoveUp()
    {
        float newY = this.y + yVel;
        float leftX = this.x - tilemap.tileWidth / 4;
        float rightX = this.x + tilemap.tileWidth / 4;
        if (world.isPassable(leftX, newY + tilemap.tileHeight / 2) &&
            world.isPassable(rightX, newY + tilemap.tileHeight / 2))
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

