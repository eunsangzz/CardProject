using UnityEngine;
using UnityEngine.UI;

public sealed class CardWorkProgress : MonoBehaviour
{
    private Slider _slider;
    private float _remaining;
    private bool _running;

    public void Begin(float duration)
    {
        EnsureSlider();

        float safeDuration = Mathf.Max(0.01f, duration);
        _remaining = safeDuration;
        _slider.minValue = 0f;
        _slider.maxValue = safeDuration;
        _slider.value = _remaining;
        _slider.gameObject.SetActive(true);
        _running = true;
    }

    public void Finish()
    {
        _running = false;
        if (_slider != null)
            _slider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_running || _slider == null)
            return;

        _remaining = Mathf.Max(0f, _remaining - Time.deltaTime);
        _slider.value = _remaining;

        if (_remaining <= 0f)
            Finish();
    }

    private void EnsureSlider()
    {
        if (_slider != null)
            return;

        var canvasObject = new GameObject(
            "WorkProgressCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler));
        canvasObject.transform.SetParent(transform, false);
        canvasObject.transform.localPosition = new Vector3(0f, 0.72f, -0.1f);
        canvasObject.transform.localScale = Vector3.one * 0.01f;

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(90f, 12f);

        GameObject sliderObject = new GameObject(
            "WorkProgressSlider",
            typeof(RectTransform),
            typeof(Slider));
        sliderObject.transform.SetParent(canvasObject.transform, false);
        Stretch(sliderObject.GetComponent<RectTransform>());

        GameObject background = CreateImage(
            "Background",
            sliderObject.transform,
            new Color(0.12f, 0.12f, 0.12f, 0.9f));
        Stretch(background.GetComponent<RectTransform>());

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObject.transform, false);
        Stretch(fillArea.GetComponent<RectTransform>(), 2f);

        GameObject fill = CreateImage(
            "Fill",
            fillArea.transform,
            new Color(0.95f, 0.75f, 0.2f, 1f));
        Stretch(fill.GetComponent<RectTransform>());

        _slider = sliderObject.GetComponent<Slider>();
        _slider.fillRect = fill.GetComponent<RectTransform>();
        _slider.targetGraphic = fill.GetComponent<Image>();
        _slider.direction = Slider.Direction.LeftToRight;
        _slider.interactable = false;
    }

    private static GameObject CreateImage(
        string name,
        Transform parent,
        Color color)
    {
        var imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        imageObject.GetComponent<Image>().color = color;
        return imageObject;
    }

    private static void Stretch(RectTransform rect, float padding = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(padding, padding);
        rect.offsetMax = new Vector2(-padding, -padding);
    }
}
