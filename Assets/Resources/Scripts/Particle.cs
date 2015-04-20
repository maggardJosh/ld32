
using UnityEngine;
using System.Collections.Generic;
public class Particle : FAnimatedSprite
{
    Vector2 vel;
    float rot;
    Vector2 accel;
    public bool isActive = false;
    int animRandom = 180;
    int animBaseSpeed = 100;
    float scaleVel = 0;
    private Particle(string elementName)
        : base(elementName)
    {
        this.isVisible = false;
    }

    public void activate(Vector2 pos, Vector2 vel, Vector2 accel, float rot, float scaleVel = 0)
    {
        this.alpha = 1;
        this.isVisible = true;
        this.isActive = true;
        this.vel = vel;
        this.SetPosition(pos);
        this.rot = rot;
        this.accel = accel;
        this.play("active", true);
        this.scaleVel = scaleVel;
        this.scale = .8f + RXRandom.Float()*.4f;
        this.currentAnim.delay = animBaseSpeed + (int)(RXRandom.Float() * animRandom);
    }

    public override void Update()
    {
        this.x += vel.x * Time.deltaTime;
        this.y += vel.y * Time.deltaTime;
        vel += accel * Time.deltaTime;

        this.rotation += rot * Time.deltaTime;
        this.alpha -= Time.deltaTime / (((float)this.currentAnim.delay * this.currentAnim.frames.Length) / 1000f);
        this.scale += scaleVel * Time.deltaTime;
        if (this.IsStopped)
        {
            this.RemoveFromContainer();
            this.isActive = false;
        }

        base.Update();
    }
    public class CloudParticle : Particle
    {

        private static CloudParticle[] particleList;
        const int MAX_PARTICLES = 100;
        public static CloudParticle getParticle()
        {

            if (particleList == null)
                particleList = new CloudParticle[MAX_PARTICLES];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    CloudParticle p = new CloudParticle();
                    particleList[x] = p;
                    return p;
                }
                else if (!particleList[x].isActive)
                {
                    return particleList[x];
                }
            }
            return particleList[RXRandom.Int(MAX_PARTICLES)];
        }

        public static List<CloudParticle> GetDoubleJumpParticles(Vector2 pos)
        {
            List<CloudParticle> result = new List<CloudParticle>();
            for (int i = 0; i < 15; i++)
            {
                CloudParticle particle = CloudParticle.getParticle();
                particle.activate(pos, new Vector2(RXRandom.Float() * 50 -25, RXRandom.Float() * -20), new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20), RXRandom.Float() * 5, RXRandom.Float()*-.3f);
                result.Add(particle);
            }
            return result;
        }
        public static List<CloudParticle> GetBreakableParticles(Vector2 pos)
        {
            float xDisp = 20;
            float yDisp = 50;
            List<CloudParticle> result = new List<CloudParticle>();
            for (int i = 0; i < 35; i++)
            {
                CloudParticle particle = CloudParticle.getParticle();
                particle.activate(pos + new Vector2(RXRandom.Float()*xDisp - xDisp/2, RXRandom.Float()*yDisp - yDisp/2), new Vector2(RXRandom.Float() * 200 - 100, RXRandom.Float() * 100), new Vector2(0, -200 + RXRandom.Float() * -220), RXRandom.Float() * 5);

                result.Add(particle);
            }
            return result;
        }
        
        private CloudParticle()
            : base("cloud")
        {

            this.animRandom = 180;
            this.animBaseSpeed = 200;
            this.addAnimation(new FAnimation("active", new int[] { 1, 2, 3, 4 }, animBaseSpeed + (int)(RXRandom.Float() * animRandom), false));
        }
    }
}