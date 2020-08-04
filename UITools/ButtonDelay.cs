using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDelay : MonoBehaviour
{
    [SerializeField]
    private float delayToEnableMS = 500f;
    EventTrigger eventTrigger;

    void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null) {
            eventTrigger.enabled = false;
            StartCoroutine(OnDelay());
        }
    }

    IEnumerator OnDelay()
    {
        yield return new WaitForSeconds(delayToEnableMS / 1000f);
        eventTrigger.enabled = true;
        Destroy(this);
    }
}
