using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Powerup : FSprite
{
    public enum Name
    {
        THREE_HIT_COMBO = 1,
        TAIL_HANG = 2,
        SLAM = 3,
        DOUBLE_JUMP = 4,
        POLE_EXTEND = 5,
        LEVER = 6,
        CHARGE_JUMP = 7,
        SUPER_AIR_ATTACK = 8
    }
    public Name powerupName;
    public Powerup(float x, float y, int powerup)
        : base("powerup_0" + powerup)
    {
        this.powerupName = (Name)powerup;
        this.x = x - 2.5f;
        this.y = y;
        Go.to(this, 3.0f, new TweenConfig().floatProp("x", 5, true).setEaseType(EaseType.QuadOut).setIterations(-1, LoopType.PingPong));
        Go.to(this, 3.7f, new TweenConfig().floatProp("y", 10, true).setEaseType(EaseType.QuadOut).setIterations(-1, LoopType.PingPong));
        Futile.instance.SignalUpdate += SpawnParticles;
    }
    FContainer parent;
    public override void HandleAddedToContainer(FContainer container)
    {
        parent = container;
        base.HandleAddedToContainer(container);
    }
    public override void HandleRemovedFromContainer()
    {
        parent = null;
        base.HandleRemovedFromContainer();
    }
    private void SpawnParticles()
    {
        foreach (Particle p in Particle.PowerupParticle.GetPowerupParticle(this.GetPosition()))
            if (parent != null)
                parent.AddChild(p);

        this.MoveToFront();
    }
    ShadowLabel description;
    public void ActivatePowerup(Player p)
    {
        FSoundManager.PlaySound("Powerup");
        Futile.instance.SignalUpdate -= SpawnParticles;
        C.isTransitioning = true;
        FCamObject cam = C.getCameraInstance();
        this.RemoveFromContainer();
        cam.AddChild(this);
        Go.killAllTweensWithTarget(this);
        this.x -= cam.x;
        this.y -= cam.y;
        Go.to(this, .5f, new TweenConfig().floatProp("x", 0).floatProp("y", 0).floatProp("scale", 4).setEaseType(EaseType.QuadOut));
        description = new ShadowLabel(GetDescription());
        description.alpha = 0;
        cam.AddChild(description);
        description.y = -Futile.screen.halfHeight / 2;
        Go.to(description, .5f, new TweenConfig().floatProp("alpha", 1).setDelay(.4f).setEaseType(EaseType.QuadOut).onComplete((t) => { Futile.instance.SignalUpdate += WaitForKey; }));

        this.p = p;
    }
    Player p;
    private void WaitForKey()
    {
        if (C.getKeyDown(C.ACTION_KEY) || C.getKeyDown(C.JUMP_KEY))
        {
            Futile.instance.SignalUpdate -= WaitForKey;
            Go.to(this, .5f, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadOut));
            Go.to(description, .5f, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadOut).onComplete((t) => { ApplyPowerup(this.p); C.isTransitioning = false; }));
        }
    }
    private void ApplyPowerup(Player p)
    {
        switch (powerupName)
        {
            case Name.THREE_HIT_COMBO: Player.GetSaveStateInstance().thirdCombo = true; break;
            case Name.TAIL_HANG: Player.GetSaveStateInstance().tailGrab = true; break;
            case Name.SLAM: Player.GetSaveStateInstance().slam = true; break;
            case Name.DOUBLE_JUMP: Player.GetSaveStateInstance().doubleJump = true; break;
            case Name.POLE_EXTEND: Player.GetSaveStateInstance().poleExtend = true; break;
            case Name.LEVER: Player.GetSaveStateInstance().levers = true; break;
            case Name.CHARGE_JUMP: Player.GetSaveStateInstance().chargeJump = true; break;
            case Name.SUPER_AIR_ATTACK: Player.GetSaveStateInstance().airAttackEndGame = true; break;
        }
    }
    public string GetDescription()
    {
        switch (powerupName)
        {
            case Name.CHARGE_JUMP: return "Charge Jump\nLaunch to the heavens and beyond!";
            default: return "Wat";
        }
    }

    public bool IsTileOccupied(int x, int y, float tileWidth)
    {
        return (Mathf.FloorToInt((this.x) / tileWidth) == x) && Mathf.FloorToInt(this.y / tileWidth) == y;
    }
}
