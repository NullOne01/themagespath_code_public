using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustCloud : MonoBehaviour
{
    [SerializeField]
    private float defaultSpeed;
    [SerializeField]
    private float fasterSpeed;
    private float speed;

    void Start()
    {
        GetComponent<MeshRenderer>().sortingLayerName = "Clouds";
        GetComponent<MeshRenderer>().sortingOrder = 1;
        speed = defaultSpeed;
    }

    private void Update() {
        float prevValue = GetComponent<MeshRenderer>().material.GetFloat("_offset_x");
        //Меняем оффсет шейдера, чтобы создать анимацию движения
        GetComponent<MeshRenderer>().material.SetFloat("_offset_x", prevValue + speed);
    }

    public void SetColor(Color newColor) {
        //Меняем цвет облака через его шейдер
        GetComponent<MeshRenderer>().material.SetColor("_cloud_color", newColor);
    }

    public void OnMovementStart() {
        speed = fasterSpeed;
    }

    public void OnMovementEnd() {
        speed = defaultSpeed;
    }
}
