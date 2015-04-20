using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShadowLabel : FContainer
{
    public FLabel label;
    private FLabel shadow;
    public ShadowLabel(string text)
    {
        label = new FLabel(C.smallFontName, text);
        shadow = new FLabel(C.smallFontName, text);
        shadow.color = Color.black;
        this.AddChild(shadow);
        this.AddChild(label);
        shadow.x = 1;
        shadow.y = -1;
    }
}

