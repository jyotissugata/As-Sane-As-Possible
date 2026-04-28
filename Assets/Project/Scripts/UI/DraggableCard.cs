using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Card Components")]
    public Image cardImage;
    public TextMeshProUGUI employeeNameText;
    public TextMeshProUGUI technicalSkillText;
    public TextMeshProUGUI creativeSkillText;
    public TextMeshProUGUI communicationSkillText;
    public TextMeshProUGUI managementSkillText;
    public TextMeshProUGUI sanityText;
    public Image portraitImage;

    [Header("Drag Settings")]
    public Canvas canvas;
    public float dragScale = 1.1f;
    protected bool IsBeingDragged = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private EmployeeData employeeData;
    private TaskSlot currentSlot;
    protected DragClone dragClone; // Reference to reusable drag clone
    private RectTransform dragCloneRectTransform;
    private CanvasGroup dragCloneCanvasGroup;

    public EmployeeData EmployeeData => employeeData;
    public TaskSlot CurrentSlot => currentSlot;
    public DragClone DragClone => dragClone;

    public void SetDragClone(DragClone clone)
    {
        dragClone = clone;
    }

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // Register with Service Locator for efficient access
        GameServiceLocator.RegisterEmployeeCard(this);
    }

    protected virtual void Start()
    {

    }

    public void InitializeCard(EmployeeData data)
    {
        employeeData = data;

        if (data == null)
            UnityEngine.Debug.LogError("EmployeeData is null", this);
        
        if (employeeNameText != null)
            employeeNameText.text = data.employeeName;
        
        if (technicalSkillText != null)
            technicalSkillText.text = $"Tech: {data.technicalSkill}";
        
        if (creativeSkillText != null)
            creativeSkillText.text = $"Creative: {data.creativeSkill}";
        
        if (sanityText != null)
            sanityText.text = $"Sanity: {data.sanity}";
        
        if (portraitImage != null && data.employeePortrait != null)
            portraitImage.sprite = data.employeePortrait;
    }

    public virtual void UpdateSanityDisplay()
    {
        // Base implementation - should be overridden by EmployeeCard
        if (sanityText != null && employeeData != null)
        {
            sanityText.text = $"Sanity: {employeeData.sanity}";
            
            // Change color based on sanity level
            if (employeeData.sanity < 30)
                sanityText.color = Color.red;
            else if (employeeData.sanity < 60)
                sanityText.color = Color.yellow;
            else
                sanityText.color = Color.green;
        }
    }

    public virtual void UpdateCardDisplay()
    {

    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (employeeData == null) return;

        // Save original parent and position
        originalParent = transform.parent;
        originalPosition = rectTransform.position;

        IsBeingDragged = true;
        CreateDragClone(eventData);
        
        // Make original card semi-transparent
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (employeeData == null || dragCloneRectTransform == null) return;

        // Move the drag clone to follow mouse (use eventData.position for Input System compatibility)
        dragCloneRectTransform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        IsBeingDragged = false;
        if (employeeData == null) return;

        // Restore original card
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if dropped on a valid slot
        bool droppedOnSlot = false;
        if (eventData.pointerEnter != null)
        {
            TaskSlot slot = eventData.pointerEnter.GetComponent<TaskSlot>();
            if (slot != null && slot.CanAssignEmployee(employeeData))
            {
                slot.AssignEmployee(this);
                droppedOnSlot = true;
            }
        }

        // Destroy drag clone
        DestroyDragClone();

        // Always restore original parent - employee cards should stay in employee container
        transform.SetParent(originalParent);
        rectTransform.position = originalPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Remove from current slot if clicked while assigned
        if (currentSlot != null)
        {
            currentSlot.RemoveEmployee();
        }
    }

    public virtual void SetCurrentSlot(TaskSlot slot)
    {
        currentSlot = slot;
    }

    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            rectTransform.position = originalPosition;
        }
    }

    private void OnDestroy()
    {
        // Unregister from Service Locator
        GameServiceLocator.UnregisterEmployeeCard(this);
        
        // Clean up drag clone if it exists
        DestroyDragClone();
    }

    protected virtual void CreateDragClone(PointerEventData eventData)
    {
        if (dragClone == null)
        {
            Debug.LogWarning("Drag clone not assigned. Please set it up in DragDropUIController.");
            return;
        }

        // Initialize clone with current employee data
        dragClone.Initialize(employeeData);
        dragClone.UpdateDisplay();

        // Get components
        dragCloneRectTransform = dragClone.GetComponent<RectTransform>();
        dragCloneCanvasGroup = dragClone.GetComponent<CanvasGroup>();

        // Position clone at current mouse position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );
        dragCloneRectTransform.localPosition = localPoint;
        dragCloneRectTransform.localScale = Vector3.one * dragScale;

        // Set up clone to block raycasts so it can detect drop targets
        if (dragCloneCanvasGroup == null)
        {
            dragCloneCanvasGroup = dragClone.gameObject.AddComponent<CanvasGroup>();
        }
        dragCloneCanvasGroup.blocksRaycasts = true;
        dragCloneCanvasGroup.alpha = 0.9f;

        // Show the clone
        dragClone.Show();
    }

    private void DestroyDragClone()
    {
        if (dragClone != null)
        {
            // Hide the clone instead of destroying it
            dragClone.Hide();
            dragCloneRectTransform = null;
            dragCloneCanvasGroup = null;
        }
    }
}
