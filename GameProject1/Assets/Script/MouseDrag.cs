using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class MouseDrag : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private bool blockWhenPointerOverUI = true;
    [SerializeField] private float liftZOffset = 0f;
    [SerializeField] private bool useRaycastPlane = true;

    [Header("Selection Feedback (Shadow)")]
    [SerializeField] private GameObject shadowPrefab;      // 원형 스프라이트/쿼드 프리팹
    [SerializeField] private Vector3 shadowLocalOffset = new Vector3(0f, -0.05f, 0f);
    [SerializeField] private bool shadowFollowWhileDragging = true;
    [SerializeField] private bool shadowHideOnRelease = true;

    private Camera _cam;
    private bool _dragging;

    private Vector3 _offsetWorld;
    private Plane _dragPlane;
    private float _originalZ;

    // Shadow instance
    private GameObject _shadowInstance;

    private void Awake()
    {
        _cam = Camera.main;
        if (_cam == null)
            Debug.LogError("[MouseDrag] Main Camera가 없습니다. 카메라에 MainCamera 태그를 달아주세요.");
    }

    private void OnMouseDown()
    {
        if (_cam == null) return;

        if (blockWhenPointerOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        _dragging = true;
        _originalZ = transform.position.z;

        // 드래그 평면 설정
        if (useRaycastPlane)
        {
            _dragPlane = new Plane(-_cam.transform.forward, transform.position);
            Vector3 hitPoint = RayToPlanePoint(Input.mousePosition, _dragPlane);
            _offsetWorld = transform.position - hitPoint;
        }
        else
        {
            Vector3 mouseWorld = ScreenToWorldAtZ(Input.mousePosition, _originalZ);
            _offsetWorld = transform.position - mouseWorld;
        }

        ShowShadow();
    }

    private void OnMouseDrag()
    {
        if (!_dragging || _cam == null) return;

        Vector3 target;
        if (useRaycastPlane)
        {
            Vector3 hitPoint = RayToPlanePoint(Input.mousePosition, _dragPlane);
            target = hitPoint + _offsetWorld;
        }
        else
        {
            Vector3 mouseWorld = ScreenToWorldAtZ(Input.mousePosition, _originalZ);
            target = mouseWorld + _offsetWorld;
        }

        target.z = _originalZ - liftZOffset;
        transform.position = target;

        if (shadowFollowWhileDragging)
            UpdateShadowPosition();
    }

    private void OnMouseUp()
    {
        _dragging = false;

        if (shadowHideOnRelease)
            HideShadow();
        else
            UpdateShadowPosition(); // 놓은 위치로 갱신
    }

    // ---------------- Shadow ----------------

    private void ShowShadow()
    {
        if (shadowPrefab == null) return;

        if (_shadowInstance == null)
        {
            // 처음 한 번만 생성
            _shadowInstance = Instantiate(shadowPrefab);
            _shadowInstance.name = $"{gameObject.name}_Shadow";
        }

        _shadowInstance.SetActive(true);
        UpdateShadowPosition();
    }

    private void HideShadow()
    {
        if (_shadowInstance != null)
            _shadowInstance.SetActive(false);
    }

    private void UpdateShadowPosition()
    {
        if (_shadowInstance == null) return;

        // 카드 아래로 위치시키기(월드 오브젝트 기준)
        _shadowInstance.transform.position = transform.position + shadowLocalOffset;

        // 회전/스케일을 카드와 맞추고 싶으면(원하면 켜도 됨)
        // _shadowInstance.transform.rotation = transform.rotation;
        // _shadowInstance.transform.localScale = transform.localScale;

        // 그림자를 카드보다 "뒤"로 보내고 싶으면 Z를 조절
        // (2D에서 sorting order를 쓰는 경우엔 SpriteRenderer sortingOrder로 처리 권장)
    }

    // ---------------- Utils ----------------

    private Vector3 RayToPlanePoint(Vector3 screenPos, Plane plane)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return transform.position;
    }

    private Vector3 ScreenToWorldAtZ(Vector3 screenPos, float worldZ)
    {
        float distance = Mathf.Abs(worldZ - _cam.transform.position.z);
        screenPos.z = distance;
        return _cam.ScreenToWorldPoint(screenPos);
    }

    private void OnDisable()
    {
        _dragging = false;

        if (_shadowInstance != null)
            _shadowInstance.SetActive(false);
    }
}
