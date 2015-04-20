
using UnityEngine;
using System.Collections.Generic;
public class Particle : FSprite
{
    Vector2 vel;
    float rot;
    Vector2 accel;
    public bool isActive = false;
    int animRandom = 180;
    int animBaseSpeed = 100;
    float scaleVel = 0;
    float life = 1;
    float count = 0;
    private Particle(string elementName)
        : base(elementName)
    {
        this.isVisible = false;
    }

    public void activate(Vector2 pos, Vector2 vel, Vector2 accel, float rot, float life =1, float scaleVel = 0)
    {
        this.alpha = 1;
        this.isVisible = true;
        this.isActive = true;
        this.vel = vel;
        this.SetPosition(pos);
        this.rot = rot;
        this.accel = accel;
        this.scaleVel = scaleVel;
        this.life = 1;
        this.count = life;
        this.scale = .8f + RXRandom.Float()*.4f;
    }

    public override void HandleAddedToContainer(FContainer container)
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToContainer(container);
    }

    public override void HandleRemovedFromContainer()
    {
        Futile.instance.SignalUpdate -= Update;
        base.HandleRemovedFromContainer();
    }
    public void Update()
    {
        this.x += vel.x * Time.deltaTime;
        this.y += vel.y * Time.deltaTime;
        vel += accel * Time.deltaTime;

        this.rotation += rot * Time.deltaTime;
        this.alpha = count/life;
        this.scale += scaleVel * Time.deltaTime;
        this.count -= Time.deltaTime;
        if (count <= 0)
        {
            this.RemoveFromContainer();
            this.isActive = false;
        }

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
            float xDisp = 40;
            float yDisp = 10;
            List<CloudParticle> result = new List<CloudParticle>();
            for (int i = 0; i < 30; i++)
            {
                CloudParticle particle = CloudParticle.getParticle();
                particle.activate(pos + new Vector2(RXRandom.Float() * xDisp - xDisp / 2, RXRandom.Float() * yDisp - yDisp / 2), new Vector2(RXRandom.Float() * 40 - 20, RXRandom.Float() * -20), new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20), RXRandom.Float() * 10, 1f, RXRandom.Float() * -.5f);
                result.Add(particle);
            }
            return result;
        }

        public static List<CloudParticle> GetSlamParticles(Vector2 pos)
        {
            float xDisp = 30;
            float yDisp = 10;
            float xVelDisp = 150;
            
            List<CloudParticle> result = new List<CloudParticle>();
            for (int i = 0; i < 30; i++)
            {
                CloudParticle particle = CloudParticle.getParticle();
                particle.activate(pos + new Vector2(RXRandom.Float() * xDisp - xDisp / 2, RXRandom.Float() * yDisp - yDisp / 2), new Vector2(RXRandom.Float() * xVelDisp - xVelDisp/2f, RXRandom.Float() * -20), new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20), RXRandom.Float() * 10, 1f, RXRandom.Float() * -.5f);
                result.Add(particle);
            }
            return result;
        }

        public static List<CloudParticle> GetThirdAttackParticles(Vector2 pos, bool facingLeft)
        {
            float xDisp = 20;
            float yDisp = 5;
            float xVelDisp = 200;

            List<CloudParticle> result = new List<CloudParticle>();
            for (int i = 0; i < 30; i++)
            {
                CloudParticle particle = CloudParticle.getParticle();
                particle.activate(pos - Vector2.up * 16f + new Vector2(RXRandom.Float() * xDisp - xDisp / 2, RXRandom.Float() * yDisp - yDisp / 2), new Vector2((facingLeft ? -1 : 1 ) * RXRandom.Float() * xVelDisp/2f, RXRandom.Float() * 30 - 15), new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20), RXRandom.Float() * 10, 1f, RXRandom.Float() * -.5f);
                result.Add(particle);
            }
            return result;
        }
        
        private CloudParticle()
            : base("cloud_0" +( RXRandom.Int(3) + 1))
        {
        }
    }

    public class IceParticle : Particle
    {

        private static IceParticle[] particleList;
        const int MAX_PARTICLES = 100;
        public static IceParticle getParticle()
        {

            if (particleList == null)
                particleList = new IceParticle[MAX_PARTICLES];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    IceParticle p = new IceParticle();
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

        public static List<IceParticle> GetBreakableParticles(Vector2 pos)
        {
            float xDisp = 20;
            float yDisp = 50;
            List<IceParticle> result = new List<IceParticle>();
            for (int i = 0; i < 35; i++)
            {
                IceParticle particle = IceParticle.getParticle();
                particle.activate(pos + new Vector2(RXRandom.Float() * xDisp - xDisp / 2, RXRandom.Float() * yDisp - yDisp / 2), new Vector2(RXRandom.Float() * 200 - 100, RXRandom.Float() * 100), new Vector2(0, -200 + RXRandom.Float() * -220), RXRandom.Float() * 360, 1.5f, -.7f);

                result.Add(particle);
            }
            return result;
        }

        private IceParticle()
            : base("iceParticle_0" + (RXRandom.Int(3) + 1))
        {

            
        }
    }

    public override void HandleAddedToStage()
    {
        base.HandleAddedToStage();
    }

    public class PowerupParticle : Particle
    {

        private static PowerupParticle[] particleList;
        const int MAX_PARTICLES = 100;
        public static PowerupParticle getParticle()
        {

            if (particleList == null)
                particleList = new PowerupParticle[MAX_PARTICLES];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    PowerupParticle p = new PowerupParticle();
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

        public static List<PowerupParticle> GetPowerupParticle(Vector2 pos)
        {
            float xDisp = 10;
            float yDisp = 10;
            float xVelDisp = 10;
            
            List<PowerupParticle> result = new List<PowerupParticle>();
            for (int i = 0; i < 1; i++)
            {
                PowerupParticle particle = PowerupParticle.getParticle();
                particle.activate(pos + new Vector2(RXRandom.Float() * xDisp - xDisp / 2, RXRandom.Float() * yDisp - yDisp / 2), new Vector2(RXRandom.Float() * xVelDisp  - xVelDisp/2f, RXRandom.Float() * 30 - 15), new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20), RXRandom.Float() * 10, 2f, RXRandom.Float() * -.5f);
                result.Add(particle);
            }
            return result;
        }

        private PowerupParticle()
            : base("powerupParticle_0" + (RXRandom.Int(3) + 1))
        {


        }
    }
}