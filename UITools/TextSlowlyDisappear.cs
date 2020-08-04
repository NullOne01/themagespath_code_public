using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSlowlyDisappear : MonoBehaviour
{
    private const float SecondsToDisappear = 1f;
    private const float StepAlphaDecrease = 0.05f;

    private TextMeshProUGUI textMesh;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(DisappearCoroutine());
    }

    private IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(SecondsToDisappear);

        while (textMesh.color.a > 0) {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - StepAlphaDecrease);
            yield return null;
        }

        Destroy(gameObject);

        yield return null;
    }
}
