using TMPro;
using UnityEngine;

public class ResidentCombatView : MonoBehaviour
{
    private static ResidentCombatView _selectedView;

    private TextMeshPro _label;

    public void Bind(ResidentCombatStats stats)
    {
        SetVisible(false);
    }

    private void LateUpdate()
    {
    }

    private void SetVisible(bool visible)
    {
        if (_label == null)
        {
            Transform existing = transform.Find("ResidentCombatText");
            if (existing != null)
                _label = existing.GetComponent<TextMeshPro>();
        }

        if (_label != null)
            _label.gameObject.SetActive(false);
    }

    public static void Select(GameObject card)
    {
        if (_selectedView != null)
            _selectedView.SetVisible(false);

        _selectedView = card != null
            ? card.GetComponent<ResidentCombatView>()
            : null;

        if (_selectedView != null)
            _selectedView.SetVisible(false);
    }
}
