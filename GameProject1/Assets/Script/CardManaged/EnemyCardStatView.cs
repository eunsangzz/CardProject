using TMPro;
using UnityEngine;

public class EnemyCardStatView : MonoBehaviour
{
    [SerializeField] private EnemyCombatStats stats;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private TextMeshPro attackText;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<EnemyCombatStats>();

        if (healthText == null)
            healthText = CreateStatText("HealthText");

        if (attackText == null)
            attackText = CreateStatText("AttackText");

        LayoutStatTexts();
        Refresh();
    }

    private void LateUpdate()
    {
        LayoutStatTexts();
        Refresh();
    }

    private void Refresh()
    {
        if (stats == null)
            return;

        healthText.text = stats.CurrentHealth.ToString();
        attackText.text = stats.AttackPower.ToString();
    }

    private void LayoutStatTexts()
    {
        if (healthText == null || attackText == null)
            return;

        SpriteRenderer spriteRenderer = GetCardSpriteRenderer();
        if (spriteRenderer == null)
            return;

        Bounds cardBounds = spriteRenderer.bounds;
        float width = cardBounds.size.x;
        float height = cardBounds.size.y;
        float bottomY = cardBounds.min.y + height * 0.13f;
        float textZ = spriteRenderer.transform.position.z - 0.2f;

        healthText.transform.SetParent(transform, true);
        attackText.transform.SetParent(transform, true);
        KeepWorldTextScale(healthText.transform);
        KeepWorldTextScale(attackText.transform);
        healthText.transform.position =
            new Vector3(cardBounds.min.x + width * 0.20f, bottomY, textZ);
        attackText.transform.position =
            new Vector3(cardBounds.min.x + width * 0.80f, bottomY, textZ);

        healthText.fontSize = 2.4f;
        attackText.fontSize = 2.4f;
        healthText.rectTransform.sizeDelta = new Vector2(width * 0.24f, height * 0.22f);
        attackText.rectTransform.sizeDelta = new Vector2(width * 0.24f, height * 0.22f);

        ApplyReadableStyle(healthText);
        ApplyReadableStyle(attackText);

        SetSortingOverCard(healthText, spriteRenderer);
        SetSortingOverCard(attackText, spriteRenderer);
    }

    private void KeepWorldTextScale(Transform textTransform)
    {
        Vector3 scale = transform.lossyScale;
        textTransform.localScale = new Vector3(
            SafeInverse(scale.x),
            SafeInverse(scale.y),
            SafeInverse(scale.z));
    }

    private static float SafeInverse(float value)
    {
        return Mathf.Abs(value) > 0.0001f ? 1f / value : 1f;
    }

    private static void ApplyReadableStyle(TextMeshPro text)
    {
        text.color = Color.white;
        text.outlineColor = new Color32(35, 24, 16, 255);
        text.outlineWidth = 0.25f;
    }

    private SpriteRenderer GetCardSpriteRenderer()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer spriteRenderer = renderers[i];
            if (spriteRenderer == null || spriteRenderer.sprite == null)
                continue;

            if (spriteRenderer.gameObject.name == "Shadow")
                continue;

            return spriteRenderer;
        }

        return null;
    }

    private static void SetSortingOverCard(TextMeshPro text, SpriteRenderer cardRenderer)
    {
        Renderer textRenderer = text.GetComponent<Renderer>();
        if (textRenderer == null || cardRenderer == null)
            return;

        textRenderer.sortingLayerID = cardRenderer.sortingLayerID;
        textRenderer.sortingOrder = cardRenderer.sortingOrder + 200;
    }

    private TextMeshPro CreateStatText(string objectName)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshPro));
        textObject.transform.SetParent(transform, false);
        textObject.transform.localPosition = Vector3.zero;
        textObject.transform.localRotation = Quaternion.identity;
        textObject.transform.localScale = Vector3.one;

        TextMeshPro text = textObject.GetComponent<TextMeshPro>();
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        ApplyReadableStyle(text);
        text.rectTransform.sizeDelta = new Vector2(0.65f, 0.5f);
        text.enableWordWrapping = false;
        text.raycastTarget = false;

        Renderer textRenderer = text.GetComponent<Renderer>();
        if (textRenderer != null)
            textRenderer.sortingOrder = 80;

        return text;
    }
}
