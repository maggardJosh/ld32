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
    World world;
    public JadeDragon(float x, float y, float delay, float interval, World world) : base("dragon")
    {
        this.delay = delay;
        this.interval = interval;
        this.x = x;
        this.y = y;
        this.world = world;
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
        float angle = 0;
        float speed = 200;
        for (int i = 0; i < 8; i++)
        {
            Vector2 vel = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            Vector2 futurePos =  this.GetPosition() + vel * 32;
            if(!world.isPassable(futurePos.x, futurePos.y))
            {
            angle += Mathf.PI/4f;
                continue;
            }
            vel *= speed;
            Fireball f = new Fireball(vel, this.world);
            f.rotation = 180+angle *(180f/Mathf.PI);
            angle += Mathf.PI/4f;
            f.SetPosition(this.GetPosition());
            world.addFireball(f);
            
        }
    }
}

