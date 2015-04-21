using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Ending : FContainer
{
    private FSprite background;
    private ShadowLabel credits;

    public Ending()
    {
        
        background = new FSprite("MainMenuBackground");
        this.AddChild(background);

        credits = new ShadowLabel("Thanks for Playing!\n\nJosh Maggard - Programmer/SFX/Design\n\nDan Konves - Programmer/Level Design\n\nDenver Poteet - Art/Design");
        this.AddChild(credits);
        credits.alpha = 0;
        Go.to(credits, 1.0f, new TweenConfig().floatProp("alpha", 1).setEaseType(EaseType.QuadOut));
    }

}