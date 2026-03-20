using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public RectTransform textTarget;
    public Vector2 pressedOffset = new Vector2(0, -8);

    private Vector2 originalPosition;

    void Start()
    {
        if (textTarget != null)
        {
            originalPosition = textTarget.anchoredPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (textTarget != null)
        {
            textTarget.anchoredPosition = originalPosition + pressedOffset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetTextPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetTextPosition();
    }

    private void ResetTextPosition()
    {
        if (textTarget != null)
        {
            textTarget.anchoredPosition = originalPosition;
        }
    }
}