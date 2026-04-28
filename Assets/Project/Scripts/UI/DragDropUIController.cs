using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragDropUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button resetButton;
    public Button autoAssignButton;
    public Button restoreSanityButton;
    public Slider sanityDrainSlider;
    public TextMeshProUGUI sanityDrainMultiplierText;

    [Header("Drag Clone")]
    public DragClone dragClonePrefab;
    public Canvas dragCanvas;

    [Header("Settings")]
    public float cardSpacing = 10f;
    public float slotSpacing = 15f;

    private DragClone sharedDragClone;

    private void Start()
    {
        InitializeDragClone();
        InitializeUI();
        SetupEventListeners();
    }

    private void InitializeDragClone()
    {
        if (dragClonePrefab == null)
        {
            Debug.LogError("Drag clone prefab not assigned in DragDropUIController!");
            return;
        }

        if (dragCanvas == null)
        {
            Debug.LogError("Drag canvas not assigned in DragDropUIController!");
            return;
        }

        // Create shared drag clone
        GameObject cloneObj = Instantiate(dragClonePrefab.gameObject, dragCanvas.transform);
        sharedDragClone = cloneObj.GetComponent<DragClone>();
        
        // Hide it initially
        sharedDragClone.Hide();

        // Remove any event handlers from the clone (it's just a visual)
        CanvasGroup cloneCanvasGroup = cloneObj.GetComponent<CanvasGroup>();
        if (cloneCanvasGroup == null)
        {
            cloneCanvasGroup = cloneObj.AddComponent<CanvasGroup>();
        }
        cloneCanvasGroup.blocksRaycasts = false;

        Debug.Log("Shared drag clone initialized");
    }

    private void InitializeUI()
    {
        // UI creation is now handled by GameServiceLocator
        GameServiceLocator.Instance?.CreateEmployeeCards();
        
        // Assign shared drag clone to all employee cards
        AssignDragCloneToCards();
        
        UpdateSanityDrainUI();
    }

    private void AssignDragCloneToCards()
    {
        if (sharedDragClone == null) return;

        var employeeCards = GameServiceLocator.EmployeeCards;
        foreach (var card in employeeCards)
        {
            if (card != null)
            {
                // Use reflection or a public setter to assign the drag clone
                // Since dragClone is private, we need to add a public setter or use a different approach
                // For now, let's add a public method to DraggableCard
                card.SetDragClone(sharedDragClone);
            }
        }
    }

    private void SetupEventListeners()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetAllAssignments);
        }

        if (autoAssignButton != null)
        {
            autoAssignButton.onClick.AddListener(AutoAssignEmployees);
        }

        if (restoreSanityButton != null)
        {
            restoreSanityButton.onClick.AddListener(RestoreAllSanity);
        }

        if (sanityDrainSlider != null)
        {
            sanityDrainSlider.onValueChanged.AddListener(OnSanityDrainSliderChanged);
        }
    }

    private void ResetAllAssignments()
    {
        Debug.Log("Resetting all employee assignments");

        var employeeCards = GameServiceLocator.EmployeeCards;
        foreach (var card in employeeCards)
        {
            if (card.CurrentSlot != null)
            {
                card.CurrentSlot.RemoveEmployee();
            }
            card.ReturnToOriginalPosition();
        }
    }

    private void AutoAssignEmployees()
    {
        Debug.Log("Auto-assigning employees to best matching tasks");

        if (TaskSlotManager.Instance == null)
        {
            Debug.LogWarning("TaskSlotManager not found!");
            return;
        }

        // First, remove all current assignments
        ResetAllAssignments();

        // Auto-assign each available employee
        var employeeCards = GameServiceLocator.EmployeeCards;
        foreach (var card in employeeCards)
        {
            if (card is EmployeeCard)
            {
                var employeeCard = card as EmployeeCard;
                if (employeeCard.Employee != null && employeeCard.Employee.isAvailable)
                {
                    TaskSlot bestSlot = TaskSlotManager.Instance.FindBestSlotForEmployee(employeeCard.Employee);
                
                    if (bestSlot != null)
                    {
                        bestSlot.AssignEmployee(card);
                    }
                }
            }
        }
    }

    private void RestoreAllSanity()
    {
        Debug.Log("Restoring sanity for all employees");

        if (TaskSlotManager.Instance != null)
        {
            TaskSlotManager.Instance.RestoreAllEmployeeSanity(25); // Restore 25 sanity points
        }
    }

    private void OnSanityDrainSliderChanged(float value)
    {
        float multiplier = value;
        
        if (TaskSlotManager.Instance != null)
        {
            TaskSlotManager.Instance.UpdateGlobalSanityDrainMultiplier(multiplier);
        }

        UpdateSanityDrainUI();
    }

    private void UpdateSanityDrainUI()
    {
        if (sanityDrainSlider != null && sanityDrainMultiplierText != null)
        {
            float value = sanityDrainSlider.value;
            sanityDrainMultiplierText.text = $"Sanity Drain: {value:F1}x";
        }
    }

    public void RefreshUI()
    {
        // Refresh all employee cards
        var employeeCards = GameServiceLocator.EmployeeCards;
        foreach (var card in employeeCards)
        {
            if (card != null)
            {
                card.UpdateSanityDisplay();
            }
        }

        // Refresh task slots
        var taskSlots = GameServiceLocator.TaskSlots;
        foreach (var slot in taskSlots)
        {
            if (slot != null)
            {
                slot.UpdateSlotDisplay();
            }
        }

        UpdateSanityDrainUI();
    }

    public void HighlightValidSlots(EmployeeData employee)
    {
        if (employee == null) return;

        foreach (var slot in GameServiceLocator.TaskSlots)
        {
            if (slot.CanAssignEmployee(employee))
            {
                slot.HighlightAsValidDrop();
            }
            else
            {
                slot.ResetHighlight();
            }
        }
    }

    public void ResetAllSlotHighlights()
    {
        foreach (var slot in GameServiceLocator.TaskSlots)
        {
            slot.ResetHighlight();
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
        }

        if (autoAssignButton != null)
        {
            autoAssignButton.onClick.RemoveAllListeners();
        }

        if (restoreSanityButton != null)
        {
            restoreSanityButton.onClick.RemoveAllListeners();
        }

        if (sanityDrainSlider != null)
        {
            sanityDrainSlider.onValueChanged.RemoveAllListeners();
        }
    }
}
