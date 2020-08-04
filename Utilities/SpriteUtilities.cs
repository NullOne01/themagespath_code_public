using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtilities
{
    public static float GetSpriteHeight(GameObject gameObject)
    {
        return gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y * gameObject.transform.localScale.y;
    }

    public static float GetSpriteWidth(GameObject gameObject, float scaleX)
    {
        return gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x * scaleX;
    }

    public static float GetSpriteWidth(GameObject gameObject)
    {
        return GetSpriteWidth(gameObject, gameObject.transform.localScale.x);
    }
}
