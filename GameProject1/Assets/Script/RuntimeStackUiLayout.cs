using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class RuntimeStackUiLayout : MonoBehaviour
{
    private static RuntimeStackUiLayout _instance;
    private static TMP_FontAsset _cookieRunFont;

    private const string CookieRunFontResourcesPath = "Fonts/Regular SDF";
    private const string CookieRunFontAssetPath = "Assets/CookieRunFont_TTF/Regular SDF.asset";
    private const float LayoutScale = 1f;

    private Canvas _canvas;
    private bool _layoutApplied;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static void EnsureInstance()
    {
        if (_instance != null)
            return;

        GameObject host = new GameObject("RuntimeStackUiLayout");
        DontDestroyOnLoad(host);
        _instance = host.AddComponent<RuntimeStackUiLayout>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        RefreshSceneUi();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Update()
    {
        if (_canvas == null)
            RefreshSceneUi();

        HideCardSkillUi();
    }

    private void LateUpdate()
    {
        HideCardSkillUi();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _layoutApplied = false;
        RefreshSceneUi();
    }

    private void RefreshSceneUi()
    {
        _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null)
            return;

        RemoveRuntimeUi();
        RenameCraftTab();
        ApplyExistingUiLayout();
        HideCardSkillUi();
        ApplyCookieRunFontToScene();
    }

    private void ApplyExistingUiLayout()
    {
        if (_layoutApplied)
            return;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        if (canvasRect == null)
            return;

        ResizeCanvasScaler();
        LayoutTopHud();
        LayoutBottomButtons();

        _layoutApplied = true;
    }

    private void ResizeCanvasScaler()
    {
        CanvasScaler scaler = _canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            return;

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void LayoutTopHud()
    {
        SetRect("Gold", Scaled(new Vector2(-760f, -15f)), Scaled(new Vector2(96f, 34f)), TextAlignmentOptions.MidlineLeft, Scaled(20f));
        SetRect("FoodCount", Scaled(new Vector2(-630f, -15f)), Scaled(new Vector2(122f, 34f)), TextAlignmentOptions.MidlineLeft, Scaled(20f));
        SetRect("CardCount", Scaled(new Vector2(-500f, -15f)), Scaled(new Vector2(124f, 34f)), TextAlignmentOptions.MidlineLeft, Scaled(20f));
        SetSceneObjectActive("DayCount", false);

        Slider timer = FindObjectOfType<Slider>();
        if (timer != null)
        {
            RectTransform rect = timer.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.one;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.one;
                rect.localScale = Vector3.one;
                rect.sizeDelta = Scaled(new Vector2(440f, 42f));
                rect.anchoredPosition = Scaled(new Vector2(-20f, -12f));
                NormalizeSliderVisuals(rect);
            }

            TextMeshProUGUI timerText = timer.GetComponentInChildren<TextMeshProUGUI>(true);
            if (timerText != null)
            {
                timerText.fontSize = Scaled(18f);
                timerText.alignment = TextAlignmentOptions.Center;
                timerText.color = Color.white;
                ApplyCookieRunFont(timerText);
            }
        }
    }

    private void LayoutBottomButtons()
    {
        SetTopLeftButtonRect("Craftbtn", Scaled(new Vector2(56f, -35f)), Scaled(new Vector2(96f, 52f)), TextAlignmentOptions.Center, Scaled(18f));
        SetTopLeftButtonRect("Buy", Scaled(new Vector2(160f, -35f)), Scaled(new Vector2(96f, 52f)), TextAlignmentOptions.Center, Scaled(18f));
        SetTopLeftButtonRect("CardBuy", Scaled(new Vector2(160f, -35f)), Scaled(new Vector2(96f, 52f)), TextAlignmentOptions.Center, Scaled(18f));
        SetTopLeftButtonRect("SellBtn", Scaled(new Vector2(264f, -35f)), Scaled(new Vector2(96f, 52f)), TextAlignmentOptions.Center, Scaled(18f));
        SetTopLeftButtonRect("StoreUp", Scaled(new Vector2(368f, -35f)), Scaled(new Vector2(96f, 52f)), TextAlignmentOptions.Center, Scaled(18f));
    }

    private void SetTopLeftButtonRect(
        string objectName,
        Vector2 anchoredPosition,
        Vector2 size,
        TextAlignmentOptions alignment,
        float fontSize)
    {
        Transform transform = FindSceneTransform(objectName);
        if (transform == null)
            return;

        RectTransform rect = transform.GetComponent<RectTransform>();
        if (rect == null)
            return;

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localScale = Vector3.one;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        TextMeshProUGUI[] texts = transform.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].fontSize = fontSize;
            texts[i].alignment = alignment;
            ApplyCookieRunFont(texts[i]);
        }
    }

    private void SetButtonRect(
        string objectName,
        Vector2 anchoredPosition,
        Vector2 size,
        TextAlignmentOptions alignment,
        float fontSize)
    {
        Transform transform = FindSceneTransform(objectName);
        if (transform == null)
            return;

        RectTransform rect = transform.GetComponent<RectTransform>();
        if (rect == null)
            return;

        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.localScale = Vector3.one;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        TextMeshProUGUI[] texts = transform.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].fontSize = fontSize;
            texts[i].alignment = alignment;
            ApplyCookieRunFont(texts[i]);
        }
    }

    private void SetRect(
        string objectName,
        Vector2 anchoredPosition,
        Vector2 size,
        TextAlignmentOptions alignment,
        float fontSize)
    {
        Transform transform = FindSceneTransform(objectName);
        if (transform == null)
            return;

        RectTransform rect = transform.GetComponent<RectTransform>();
        if (rect == null)
            return;

        bool topRight = anchoredPosition.x < 0f;
        rect.anchorMin = topRight ? Vector2.one : new Vector2(0f, 1f);
        rect.anchorMax = topRight ? Vector2.one : new Vector2(0f, 1f);
        rect.pivot = topRight ? Vector2.one : new Vector2(0f, 1f);
        rect.localScale = Vector3.one;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        TextMeshProUGUI text = transform.GetComponentInChildren<TextMeshProUGUI>(true);
        if (text == null)
            return;

        text.fontSize = fontSize;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        ApplyCookieRunFont(text);
    }

    private static void NormalizeSliderVisuals(RectTransform sliderRect)
    {
        Transform background = sliderRect.Find("Background");
        if (background != null)
        {
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            StretchToParent(backgroundRect, Vector2.zero);
        }

        Transform fillArea = sliderRect.Find("Fill Area");
        if (fillArea != null)
        {
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            StretchToParent(fillAreaRect, Vector2.zero);

            Transform fill = fillArea.Find("Fill");
            if (fill != null)
            {
                RectTransform fillRect = fill.GetComponent<RectTransform>();
                StretchToParent(fillRect, Vector2.zero);
            }
        }

        Transform handleSlideArea = sliderRect.Find("Handle Slide Area");
        if (handleSlideArea != null)
            handleSlideArea.gameObject.SetActive(false);
    }

    private static void StretchToParent(RectTransform rect, Vector2 padding)
    {
        if (rect == null)
            return;

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = padding;
        rect.offsetMax = -padding;
        rect.localScale = Vector3.one;
    }

    private void RemoveRuntimeUi()
    {
        RemoveSceneObject("RuntimeStackTopHud");
        RemoveSceneObject("RuntimeRecipeList");
    }

    private void RemoveSceneObject(string objectName)
    {
        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform != null && transform.name == objectName)
                Destroy(transform.gameObject);
        }
    }

    private void SetSceneObjectActive(string objectName, bool active)
    {
        Transform transform = FindSceneTransform(objectName);
        if (transform != null)
            transform.gameObject.SetActive(active);
    }

    private void RenameCraftTab()
    {
        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform == null || transform.name != "Craftbtn")
                continue;

            SetChildText(transform.gameObject, "레시피");
        }
    }

    private void HideCardSkillUi()
    {
        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;

        if (gd != null)
            gd.Skill = false;

        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform != null && transform.name == "CardSkillUi")
                transform.gameObject.SetActive(false);
        }
    }

    private static void SetChildText(GameObject root, string value)
    {
        TextMeshProUGUI[] texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = value;
            ApplyCookieRunFont(texts[i]);
        }

        Text[] legacyTexts = root.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < legacyTexts.Length; i++)
            legacyTexts[i].text = value;
    }

    private void ApplyCookieRunFontToScene()
    {
        TMP_FontAsset fontAsset = LoadCookieRunFont();
        if (fontAsset == null)
            return;

        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform == null)
                continue;

            TextMeshProUGUI[] texts = transform.GetComponents<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
                texts[i].font = fontAsset;
        }
    }

    private static void ApplyCookieRunFont(TextMeshProUGUI text)
    {
        TMP_FontAsset fontAsset = LoadCookieRunFont();
        if (fontAsset != null)
            text.font = fontAsset;
    }

    private static TMP_FontAsset LoadCookieRunFont()
    {
        if (_cookieRunFont != null)
            return _cookieRunFont;

        _cookieRunFont = Resources.Load<TMP_FontAsset>(CookieRunFontResourcesPath);

#if UNITY_EDITOR
        if (_cookieRunFont == null)
            _cookieRunFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(CookieRunFontAssetPath);
#endif

        return _cookieRunFont;
    }

    private static Transform FindSceneTransform(string objectName)
    {
        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform != null && transform.name == objectName)
                return transform;
        }

        return null;
    }

    private static Vector2 Scaled(Vector2 value)
    {
        return value * LayoutScale;
    }

    private static float Scaled(float value)
    {
        return value * LayoutScale;
    }

    private static Transform[] GetSceneTransforms()
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        var sceneTransforms = new List<Transform>();

        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform transform = allTransforms[i];
            if (transform != null &&
                transform.gameObject.scene.IsValid() &&
                transform.gameObject.scene.isLoaded)
            {
                sceneTransforms.Add(transform);
            }
        }

        return sceneTransforms.ToArray();
    }
}
