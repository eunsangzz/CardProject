using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class MouseDrag : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private bool blockWhenPointerOverUI = true;
    [SerializeField] private float liftZOffset = 0f;
    [SerializeField] private bool useRaycastPlane = true;
    [SerializeField] private int dragSortingOrderBoost = 1000;

    [Header("Selection Feedback (Shadow)")]
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private Vector3 shadowLocalOffset =
        new Vector3(0f, -0.05f, 0f);
    [SerializeField] private bool shadowFollowWhileDragging = true;
    [SerializeField] private bool shadowHideOnRelease = true;

    [SerializeField] private CommandManager commandManager;

    private readonly List<Transform> _draggedTransforms = new List<Transform>();
    private readonly List<Vector3> _dragStartPositions = new List<Vector3>();
    private readonly List<RendererSortingState> _rendererSortingStates =
        new List<RendererSortingState>();
    private readonly List<CanvasSortingState> _canvasSortingStates =
        new List<CanvasSortingState>();

    private Camera _cam;
    private bool _dragging;
    private Vector3 _offsetWorld;
    private Vector3 _primaryDragStartPosition;
    private Plane _dragPlane;
    private float _originalZ;
    private GameObject _shadowInstance;

    private void Awake()
    {
        _cam = Camera.main;
        if (commandManager == null)
            commandManager = FindObjectOfType<CommandManager>();

        if (_cam == null)
            Debug.LogError("[MouseDrag] Main Camera was not found.");
    }

    private void OnMouseDown()
    {
        if (_cam == null ||
            CardWorkService.IsAnyLocked(CardStackService.GetCards(gameObject)))
            return;

        if (blockWhenPointerOverUI &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        _dragging = true;
        _originalZ = transform.position.z;
        BeginStackDrag();
        ResidentCombatView.Select(gameObject);
        RaiseDraggedCardsSortingOrder();

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
        if (!_dragging || _cam == null)
            return;

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
        MoveStack(target - _primaryDragStartPosition);

        if (shadowFollowWhileDragging)
            UpdateShadowPosition();
    }

    private void OnMouseUp()
    {
        if (!_dragging)
            return;

        _dragging = false;
        CardStackService.TryMergeWithMatchingCard(gameObject);
        RestoreDraggedCardsSortingOrder();
        RecordStackMove();

        if (shadowHideOnRelease)
            HideShadow();
        else
            UpdateShadowPosition();
    }

    private void BeginStackDrag()
    {
        _draggedTransforms.Clear();
        _dragStartPositions.Clear();

        List<GameObject> cards = CardStackService.GetCards(gameObject);
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null) continue;

            _draggedTransforms.Add(card.transform);
            _dragStartPositions.Add(card.transform.position);
        }

        _primaryDragStartPosition = transform.position;
    }

    private void MoveStack(Vector3 delta)
    {
        for (int i = 0; i < _draggedTransforms.Count; i++)
        {
            Transform cardTransform = _draggedTransforms[i];
            if (cardTransform != null)
                cardTransform.position = _dragStartPositions[i] + delta;
        }
    }

    private void RecordStackMove()
    {
        if (commandManager == null || _draggedTransforms.Count == 0)
            return;

        var endPositions = new List<Vector3>(_draggedTransforms.Count);
        bool moved = false;

        for (int i = 0; i < _draggedTransforms.Count; i++)
        {
            Vector3 endPosition = _draggedTransforms[i] != null
                ? _draggedTransforms[i].position
                : _dragStartPositions[i];

            endPositions.Add(endPosition);
            moved |= (endPosition - _dragStartPositions[i]).sqrMagnitude > 0.0001f;
        }

        if (moved)
        {
            commandManager.Do(new MoveCardCommand(
                _draggedTransforms,
                _dragStartPositions,
                endPositions));
        }
    }

    private void RaiseDraggedCardsSortingOrder()
    {
        RestoreDraggedCardsSortingOrder();

        for (int i = 0; i < _draggedTransforms.Count; i++)
        {
            Transform cardTransform = _draggedTransforms[i];
            if (cardTransform == null) continue;

            Renderer[] renderers = cardTransform.GetComponentsInChildren<Renderer>(true);
            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer renderer = renderers[r];
                _rendererSortingStates.Add(new RendererSortingState(
                    renderer,
                    renderer.sortingOrder));
                renderer.sortingOrder += dragSortingOrderBoost;
            }

            Canvas[] canvases = cardTransform.GetComponentsInChildren<Canvas>(true);
            for (int c = 0; c < canvases.Length; c++)
            {
                Canvas canvas = canvases[c];
                _canvasSortingStates.Add(new CanvasSortingState(
                    canvas,
                    canvas.overrideSorting,
                    canvas.sortingOrder));
                canvas.overrideSorting = true;
                canvas.sortingOrder += dragSortingOrderBoost;
            }
        }
    }

    private void RestoreDraggedCardsSortingOrder()
    {
        for (int i = 0; i < _rendererSortingStates.Count; i++)
        {
            RendererSortingState state = _rendererSortingStates[i];
            if (state.Renderer != null)
                state.Renderer.sortingOrder = state.SortingOrder;
        }

        for (int i = 0; i < _canvasSortingStates.Count; i++)
        {
            CanvasSortingState state = _canvasSortingStates[i];
            if (state.Canvas == null) continue;

            state.Canvas.overrideSorting = state.OverrideSorting;
            state.Canvas.sortingOrder = state.SortingOrder;
        }

        _rendererSortingStates.Clear();
        _canvasSortingStates.Clear();
    }

    private void ShowShadow()
    {
        if (shadowPrefab == null)
            return;

        if (_shadowInstance == null)
        {
            _shadowInstance = Instantiate(shadowPrefab);
            _shadowInstance.name = gameObject.name + "_Shadow";
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
        if (_shadowInstance != null)
            _shadowInstance.transform.position = transform.position + shadowLocalOffset;
    }

    private Vector3 RayToPlanePoint(Vector3 screenPos, Plane plane)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        return plane.Raycast(ray, out float enter)
            ? ray.GetPoint(enter)
            : transform.position;
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
        RestoreDraggedCardsSortingOrder();
        if (_shadowInstance != null)
            _shadowInstance.SetActive(false);
    }

    private readonly struct RendererSortingState
    {
        public readonly Renderer Renderer;
        public readonly int SortingOrder;

        public RendererSortingState(Renderer renderer, int sortingOrder)
        {
            Renderer = renderer;
            SortingOrder = sortingOrder;
        }
    }

    private readonly struct CanvasSortingState
    {
        public readonly Canvas Canvas;
        public readonly bool OverrideSorting;
        public readonly int SortingOrder;

        public CanvasSortingState(
            Canvas canvas,
            bool overrideSorting,
            int sortingOrder)
        {
            Canvas = canvas;
            OverrideSorting = overrideSorting;
            SortingOrder = sortingOrder;
        }
    }
}
