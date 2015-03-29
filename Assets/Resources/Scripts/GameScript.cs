using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{

    FSprite white;
    FSprite[] whiteShadows = new FSprite[NUM_SHADOWS];
    const int NUM_SHADOWS = 5;
    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);
        futileParams.AddResolutionLevel(480.0f, 1.0f, 1.0f, "");

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0, 0, 0);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/inGameAtlas");

        white = new FSprite("white");
        white.color = new Color(.6f,.6f, 1.0f);
        for (int i = 0; i < NUM_SHADOWS; i++)
        {
            whiteShadows[i] = new FSprite("white");
            whiteShadows[i].color = new Color(.3f,.3f,.7f);
            whiteShadows[i].alpha = .3f - (i / (float)NUM_SHADOWS)*.3f;
            if(i != NUM_SHADOWS-1)
            Futile.stage.AddChild(whiteShadows[i]);
        }
        Futile.stage.AddChild(white);

    }

    float speed = 400;
    // Update is called once per frame
    void Update()
    {
        if (C.getKey(C.UP_KEY))
            white.y += Time.deltaTime * speed;

        else if (C.getKey(C.DOWN_KEY))
            white.y -= Time.deltaTime * speed;

        if (C.getKey(C.RIGHT_KEY))
            white.x += Time.deltaTime * speed;
        else if (C.getKey(C.LEFT_KEY))
            white.x -= Time.deltaTime * speed;

        for (int i = 0; i < NUM_SHADOWS; i++)
        {
            whiteShadows[i].x += (white.x - whiteShadows[i].x) * ((NUM_SHADOWS-i)/(float)NUM_SHADOWS)*.4f;
            whiteShadows[i].y += (white.y - whiteShadows[i].y) * ((NUM_SHADOWS - i) / (float)NUM_SHADOWS) * .4f;
        }
    }
}
