using UnityEngine;
public static class RectTransformUtility {
    public static void SwapRectTransProperties(RectTransform objRectTransform, RectTransform rectTransform)
    {
        var temp = objRectTransform.anchorMin;
        objRectTransform.anchorMin = rectTransform.anchorMin;
        rectTransform.anchorMin = temp;

        temp = objRectTransform.anchorMax;
        objRectTransform.anchorMax = rectTransform.anchorMax;
        rectTransform.anchorMax = temp;

        temp = objRectTransform.anchoredPosition;
        objRectTransform.anchoredPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = temp;

        temp = rectTransform.sizeDelta;
        objRectTransform.sizeDelta = rectTransform.sizeDelta;
        rectTransform.sizeDelta = temp;
    }

    public static float GetLeft(this RectTransform rt)
    {
        return rt.offsetMin.x;
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetXPos(this RectTransform rt, float xPos)
    {
        rt.anchoredPosition = new Vector2(xPos, rt.anchoredPosition.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void SetHeight(this RectTransform rt, float height)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }

    public static float GetHeight(this RectTransform rt)
    {
        return rt.sizeDelta.y;
    }
}