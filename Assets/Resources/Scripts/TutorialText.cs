using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TutorialText : ShadowLabel
{
    int requiredPowerup = 1;
    Rect rect;
    public TutorialText(string text, int powerup, Rect rect)
        : base(text)
    {

        this.rect = rect;
        this.requiredPowerup = powerup;
        this.alpha = 0;
        this.y = -Futile.screen.halfHeight / 3f;
    }

    public void CheckInText(Player p)
    {
     
        //We have the powerup check if we want to display it
        if (HasPowerup(requiredPowerup) && rect.Contains(p.GetPosition()))
        {
            if (this.alpha < 1)
                this.alpha += 1.0f * Time.deltaTime;
        }
        else
        {
            if (this.alpha > 0)
                this.alpha -= 1.0f * Time.deltaTime;
        }
    }
    public static bool HasPowerup(int requiredPowerup)
    {
        switch ((Powerup.Name)requiredPowerup)
        {
            case Powerup.Name.CHARGE_JUMP:
                if (!Player.GetSaveStateInstance().chargeJump)
                    return false;
                break;
            case Powerup.Name.DOUBLE_JUMP:
                if (!Player.GetSaveStateInstance().doubleJump)
                    return false;
                break;
            case Powerup.Name.LEVER:
                if (!Player.GetSaveStateInstance().levers)
                    return false;
                break;
            case Powerup.Name.POLE_EXTEND:
                if (!Player.GetSaveStateInstance().poleExtend)
                    return false;
                break;
            case Powerup.Name.SLAM:
                if (!Player.GetSaveStateInstance().slam)
                    return false;
                break;
            case Powerup.Name.SUPER_AIR_ATTACK:
                if (!Player.GetSaveStateInstance().airAttackEndGame)
                    return false;
                break;
            case Powerup.Name.TAIL_HANG:
                if (!Player.GetSaveStateInstance().tailGrab)
                    return false;
                break;
            case Powerup.Name.THREE_HIT_COMBO:
                if (!Player.GetSaveStateInstance().thirdCombo)
                    return false;
                break;
        }
        return true;
    }
}
