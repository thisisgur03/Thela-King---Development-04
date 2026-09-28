using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 100f;

    private Vector2 input;

    public Vector2 Input => input;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rect =
            transform as RectTransform;

        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        input = Vector2.ClampMagnitude(
            position / radius,
            1f
        );

        handle.anchoredPosition =
            input * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetJoystick();
    }

    public void ResetJoystick()
    {
        input = Vector2.zero;

        handle.anchoredPosition =
            Vector2.zero;
    }
}