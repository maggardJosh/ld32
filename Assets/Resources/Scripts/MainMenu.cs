using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public class MainMenu : FContainer
	{
        private FSprite background;
        private FAnimatedSprite wukong;
        private FSprite pressButton;
        private FSprite logo;
        private Action action;
        public MainMenu(Action startGameAction)
        {
            this.action = startGameAction;
            background = new FSprite("MainMenuBackground");
            this.AddChild(background);
            
            logo = new FSprite("MainMenuLogo");
            logo.y = Futile.screen.halfHeight + logo.height;
            this.AddChild(logo);

            wukong = new FAnimatedSprite("player");
            wukong.addAnimation(new FAnimation("idle", new int[] { 1, 2, 3, 4, 1, 1, 1, 1, 2, 3, 4, 1, 2, 3, 4, 1, 1, 1, 1,1 , 1, 1 }, 100, true));
            wukong.addAnimation(new FAnimation("run", new int[] { 5, 6, 7, 8 }, 100, true));
            wukong.x = -Futile.screen.halfWidth - wukong.width;
            wukong.y = -Futile.screen.halfHeight + 48f;
            this.AddChild(wukong);


            pressButton = new FSprite("PressButton");
            pressButton.alpha = 0;
            this.AddChild(pressButton);

            Go.to(logo, 2.0f, new TweenConfig().floatProp("y", Futile.screen.halfHeight / 2f).setDelay(4.0f).setEaseType(EaseType.BounceOut));
            wukong.play("run");
            Go.to(wukong, 1.5f, new TweenConfig().floatProp("x", 0).onComplete((t) => { wukong.play("idle"); }));

            Go.to(pressButton, 1.0f, new TweenConfig().floatProp("alpha", 1).setDelay(6f).setEaseType(EaseType.QuadIn));

            Futile.instance.SignalUpdate += Update;   
        }

        private void Update()
        {
            if (C.getKeyDown(C.ACTION_KEY))
            {
                Futile.instance.SignalUpdate -= Update;
                this.RemoveFromContainer();
                action.Invoke();
            }
        }
	}