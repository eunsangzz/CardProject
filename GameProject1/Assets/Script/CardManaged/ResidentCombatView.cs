using TMPro;
using UnityEngine;

public class ResidentCombatView : MonoBehaviour
{
    [SerializeField] private Vector3 localPosition = new Vector3(0f, -0.74f, -0.05f);
    [SerializeField] private int sortingOrder = 120;

    private static ResidentCombatView _selectedView;

    private ResidentCombatStats _stats;
    private TextMeshPro _label;

    public void Bind(ResidentCombatStats stats)
    {
        _stats = stats;
        EnsureLabel();
        Refresh();
        SetVisible(false);
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void EnsureLabel()
    {
        if (_label != null)
            return;

        Transform existing = transform.Find("ResidentCombatText");
        GameObject labelObject = existing != null
            ? existing.gameObject
            : new GameObject("ResidentCombatText");

        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = localPosition;

        _label = labelObject.GetComponent<TextMeshPro>();
        if (_label == null)
            _label = labelObject.AddComponent<TextMeshPro>();

        _label.fontSize = 2.2f;
        _label.alignment = TextAlignmentOptions.Center;
        _label.color = Color.white;
        _label.rectTransform.sizeDelta = new Vector2(3.2f, 0.85f);

        Renderer renderer = _label.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sortingOrder = sortingOrder;
    }

    private void Refresh()
    {
        if (_stats == null || _label == null)
            return;

        _label.text = _stats.CurrentHealth + " " + _stats.AttackPower;
    }

    private void SetVisible(bool visible)
    {
        EnsureLabel();
        _label.gameObject.SetActive(visible);
    }

    public static void Select(GameObject card)
    {
        if (_selectedView != null)
            _selectedView.SetVisible(false);

        _selectedView = card != null
            ? card.GetComponent<ResidentCombatView>()
            : null;

        if (_selectedView != null)
            _selectedView.SetVisible(true);
    }
}
