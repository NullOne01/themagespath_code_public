using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParts : MonoBehaviour {
    public Transform PlayerHand;

    public Transform GetHand()
    {
        return PlayerHand;
    }

    public void TurnIntoBlur()
    {
        foreach (Transform child in transform) {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.material = Camera.main.GetComponent<ShopMenu>().blurMaterial; //Блюр по Гауссу
        }
    }
}
