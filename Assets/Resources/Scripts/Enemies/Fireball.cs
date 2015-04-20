using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

	public class Fireball : FAnimatedSprite
	{
        private Vector2 vel = Vector2.zero;
        private World world;
        public Fireball(Vector2 vel, World world) : base ("flame")
        {
            this.vel = vel;
            this.world = world;
            addAnimation(new FAnimation("active", new int[] { 1, 2 }, 40, true));
            play("active");
            this.scale = 0.3f;
            Go.to(this, .2f, new TweenConfig().floatProp("scale", 1.0f).setEaseType(EaseType.QuadOut));
        }

        public override void Update()
        {
            if (C.isTransitioning)
                return;
            this.x += vel.x * Time.deltaTime;
            this.y += vel.y * Time.deltaTime;
            world.CheckFireball(this);
            base.Update();
        }
        public void InstaDie()
        {
            this.RemoveFromContainer();
        }
        public void Die()
        {
            FSoundManager.PlaySound("FireballDie");
            foreach (Particle p in Particle.CloudParticle.GetDoubleJumpParticles(this.GetPosition()))
                Futile.stage.AddChild(p);
            this.RemoveFromContainer();
        }
	}

