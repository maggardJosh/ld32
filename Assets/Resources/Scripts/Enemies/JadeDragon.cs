using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class JadeDragon : FSprite
{
    float interval;
    float delay;
    float count;
    bool hasStarted = false;
    public JadeDragon(float x, float y, float delay, float interval) : base("dragon")
    {
        this.delay = delay;
        this.interval = interval;
        this.x = x;
        this.y = y;
        count = 0;
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        base.HandleRemovedFromStage();
    }
    public void Update()
    {
        if (C.isTransitioning)
            return;
        count += Time.deltaTime;
        if (!hasStarted)
        {
            if (count >= delay)
            {
                count -= delay;
                SpawnFireballs();
            }
        }
        else
        {
            if (count >= interval)
            {
                count -= interval;
                SpawnFireballs();
            }
        }
    }
    private void SpawnFireballs()
    {
        FSprite fireball = new FSprite ("flame_01");
        fireball.SetPosition(this.GetPosition());
        Go.to(fireball, 1.0f, new TweenConfig().floatProp("x", 100, true));
        Futile.stage.AddChild(fireball);
    }
}

