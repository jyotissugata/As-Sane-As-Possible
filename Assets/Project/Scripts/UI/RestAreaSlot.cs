using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RestAreaSlot : MonoBehaviour, IDropHandler
{
    [Header("Rest Area Components")]
    public Image slotImage;
    public TextMeshProUGUI restAreaTitleText;
    public TextMeshProUGUI capacityText;
    public Image[] employeePreviewImages; // Array of preview images for all resting employees
    public Transform restEmployeeContainer; // Container for resting employee cards with GridLayoutGroup

    [Header("Rest Area Settings")]
    public int maxCapacity = 4;
    public int sanityRecoveryPerTurn = 15;
    public int communicationRecoveryPerTurn = 3;
    public int managementRecoveryPerTurn = 2;

    [Header("Visual Settings")]
    public Color emptyColor = Color.blue;
    public Color occupiedColor = Color.cyan;
    public Color fullColor = Color.darkBlue;
    public Color validDropColor = Color.green;

    private EmployeeCard[] restingEmployees = new EmployeeCard[4];
    private bool[] slotOccupied = new bool[4];
    private int currentOccupancy = 0;

    public int CurrentOccupancy => currentOccupancy;
    public bool IsFull => currentOccupancy >= maxCapacity;
    public EmployeeCard[] RestingEmployees => restingEmployees;

    private void Awake()
    {
        InitializeRestArea();
        
        // Register with Service Locator for efficient access
        GameServiceLocator.RegisterRestArea(this);
    }

    private void Start()
    {
        UpdateRestAreaDisplay();
    }

    private void InitializeRestArea()
    {
        if (restAreaTitleText != null)
            restAreaTitleText.text = "Rest Area";
        
        if (slotImage != null)
            slotImage.color = emptyColor;
    }

    public bool CanAddEmployee(EmployeeCard employeeCard)
    {
        if (employeeCard == null || IsFull) return false;

        Employee employee = employeeCard.Employee;
        if (employee == null) return false;

        // Check if employee is working on a task that's in progress
        if (employee.currentTaskSlot != null)
        {
            if (employee.currentTaskSlot.Task != null && 
                employee.currentTaskSlot.Task.turnsRemaining < employee.currentTaskSlot.Task.Template.baseTurnDuration)
            {
                return false; // Cannot rest while working on in-progress task
            }
        }

        // Only allow employees who need rest
        return employee.CurrentSanity < 80 || employee.HasStatus(StatusType.Sick) || employee.HasStatus(StatusType.Stressed);
    }

    public bool AddEmployee(EmployeeCard employeeCard)
    {
        if (!CanAddEmployee(employeeCard)) return false;

        // Remove employee from current task slot if any
        Employee employee = employeeCard.Employee;
        if (employee != null && employee.currentTaskSlot != null)
        {
            employee.currentTaskSlot.RemoveEmployee();
        }

        // Find first empty slot
        for (int i = 0; i < maxCapacity; i++)
        {
            if (!slotOccupied[i])
            {
                restingEmployees[i] = employeeCard;
                slotOccupied[i] = true;
                currentOccupancy++;
                UnityEngine.Debug.Log($"Added employee to rest area {employee}", employee);
                // Update employee state
                if (employee != null)
                {
                    employee.GoToRestArea();
                }

                // Do NOT move the employee card - keep it in the employee container
                // This allows dragging the same card again to change tasks
                // employeeCard.transform.SetParent(restEmployeeContainer);
                // RectTransform cardRect = employeeCard.GetComponent<RectTransform>();
                // if (cardRect != null)
                // {
                //     cardRect.anchoredPosition = Vector2.zero;
                //     cardRect.localScale = Vector3.one;
                // }

                employeeCard.SetCurrentSlot(null); // Rest area is not a task slot

                UpdateRestAreaDisplay();
                Debug.Log($"{employee.Template?.employeeName} entered Rest Area");

                return true;
            }
        }

        return false;
    }

    public bool RemoveEmployee(EmployeeCard employeeCard)
    {
        Debug.Log($"RemoveEmployee called for {employeeCard.EmployeeData?.employeeName}");
        
        for (int i = 0; i < maxCapacity; i++)
        {
            if (restingEmployees[i] == employeeCard && slotOccupied[i])
            {
                Debug.Log($"Found employee at slot {i}, removing");
                restingEmployees[i] = null;
                slotOccupied[i] = false;
                currentOccupancy--;

                Employee employee = employeeCard.Employee;
                if (employee != null)
                {
                    employee.LeaveRestArea();
                    employee.currentTaskSlot = null; // Clear task slot reference when leaving rest
                }

                UpdateRestAreaDisplay();
                Debug.Log($"{employee.Template?.employeeName} left Rest Area");

                return true;
            }
        }

        Debug.LogWarning($"Employee {employeeCard.EmployeeData?.employeeName} not found in rest area");
        return false;
    }

    public void ProcessTurn()
    {
        for (int i = 0; i < maxCapacity; i++)
        {
            if (slotOccupied[i] && restingEmployees[i] != null)
            {
                Employee employee = restingEmployees[i].Employee;
                if (employee != null)
                {
                    // Apply rest recovery
                    employee.UpdateSanity(sanityRecoveryPerTurn);
                    employee.UpdateCommunicationSkill(communicationRecoveryPerTurn);
                    employee.UpdateManagementSkill(managementRecoveryPerTurn);

                    // Update employee card display
                    restingEmployees[i].UpdateSanityDisplay();

                    // Check if employee should leave rest area
                    if (employee.CurrentSanity >= 80 && !employee.HasStatus(StatusType.Sick))
                    {
                        Debug.Log($"{employee.Template?.employeeName} is fully rested and leaving Rest Area");
                        
                        // Return employee to original position
                        restingEmployees[i].ReturnToOriginalPosition();
                        restingEmployees[i] = null;
                        slotOccupied[i] = false;
                        currentOccupancy--;
                    }
                }
            }
        }

        UpdateRestAreaDisplay();
    }

    private void UpdateRestAreaDisplay()
    {
        if (capacityText != null)
        {
            capacityText.text = $"{currentOccupancy}/{maxCapacity}";
        }

        if (slotImage != null)
        {
            if (IsFull)
                slotImage.color = fullColor;
            else if (currentOccupancy > 0)
                slotImage.color = occupiedColor;
            else
                slotImage.color = emptyColor;
        }

        // Show employee previews for all resting employees
        if (employeePreviewImages != null && employeePreviewImages.Length > 0)
        {
            for (int i = 0; i < employeePreviewImages.Length; i++)
            {
                if (i < maxCapacity && slotOccupied[i] && restingEmployees[i] != null)
                {
                    EmployeeData employee = restingEmployees[i].EmployeeData;
                    if (employee != null && employee.employeePortrait != null)
                    {
                        employeePreviewImages[i].sprite = employee.employeePortrait;
                        employeePreviewImages[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    employeePreviewImages[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void HighlightAsValidDrop()
    {
        if (slotImage != null && !IsFull)
        {
            slotImage.color = validDropColor;
        }
    }

    public void ResetHighlight()
    {
        UpdateRestAreaDisplay();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsFull) return;

        EmployeeCard draggedCard = eventData.pointerDrag?.GetComponent<EmployeeCard>();
        if (draggedCard == null) return;

        // Check if employee is already in this rest area
        if (IsEmployeeInRestArea(draggedCard))
        {
            // Employee is already resting, do nothing
            Debug.Log($"{draggedCard.EmployeeData?.employeeName} is already in rest area");
            return;
        }

        if (CanAddEmployee(draggedCard))
        {
            // Remove from current task slot if assigned
            if (draggedCard.CurrentSlot != null)
            {
                draggedCard.CurrentSlot.RemoveEmployee();
            }

            AddEmployee(draggedCard);
        }
        else
        {
            Debug.Log($"Cannot add {draggedCard.EmployeeData?.employeeName} to rest area");
        }
    }

    private bool IsEmployeeInRestArea(EmployeeCard employeeCard)
    {
        for (int i = 0; i < maxCapacity; i++)
        {
            if (restingEmployees[i] == employeeCard && slotOccupied[i])
            {
                return true;
            }
        }
        return false;
    }

    public void ForceRemoveAllEmployees()
    {
        for (int i = maxCapacity - 1; i >= 0; i--)
        {
            if (slotOccupied[i] && restingEmployees[i] != null)
            {
                restingEmployees[i].ReturnToOriginalPosition();
                restingEmployees[i] = null;
                slotOccupied[i] = false;
            }
        }
        currentOccupancy = 0;
        UpdateRestAreaDisplay();
    }

    private void OnDestroy()
    {
        ForceRemoveAllEmployees();
        
        // Unregister from Service Locator
        GameServiceLocator.UnregisterRestArea(this);
    }
}
