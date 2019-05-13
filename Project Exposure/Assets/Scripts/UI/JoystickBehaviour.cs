using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class JoystickBehaviour : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image _background;
    private Image _button;
    private Vector3 _inputVector;
    private bool _isPressed;
    private float _timeIdle;

    void Start()
    {
        _background = GetComponent<Image>();
        _button = transform.GetChild(0).GetComponent<Image>();
        _isPressed = false;
    }

    private void Update()
    {
        if (!_isPressed)
            _timeIdle += Time.deltaTime;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _timeIdle = 0;
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_background.rectTransform,
                                                                    eventData.position,
                                                                    eventData.pressEventCamera,
                                                                    out position))
        {
            position.x = position.x / _background.rectTransform.sizeDelta.x;
            position.y = position.y / _background.rectTransform.sizeDelta.y;

            _inputVector = new Vector3(position.x * 2 + 1, position.y * 2 - 1, 0);
            _inputVector = (_inputVector.magnitude > 1) ? _inputVector.normalized : _inputVector;

            _button.rectTransform.anchoredPosition = new Vector3(_inputVector.x * (_background.rectTransform.sizeDelta.x * 0.4f),
                                                                 _inputVector.y * (_background.rectTransform.sizeDelta.y * 0.4f),
                                                                 0);
        }

        _isPressed = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputVector = Vector3.zero;
        _button.rectTransform.anchoredPosition = _inputVector;
        _isPressed = false;
    }

    public float Horizontal()
    {
        if (_inputVector.x < 0.15f && _inputVector.x > -0.15f)
            return 0;
        else
            return _inputVector.x;
    }

    public float Vertical()
    {
        if (_inputVector.y < 0.15f && _inputVector.y > -0.15f)
            return 0;
        else
            return _inputVector.y;
    }

    public float GetTimeIdle()
    {
        return _timeIdle;
    }

    public bool IsPressed()
    {
        return _isPressed;
    }
}