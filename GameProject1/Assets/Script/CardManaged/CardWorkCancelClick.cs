using UnityEngine;

public sealed class CardWorkCancelClick : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
            CardWorkService.TryCancelWork(gameObject);
    }
}
