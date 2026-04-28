using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class TaskSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Components")]
    public Image slotImage;
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI taskDescriptionText;
    public Image taskIconImage;
    public Image employeePreviewImage;

    [Header("Slot Visuals")]
    public Color emptyColor = Color.gray;
    public Color occupiedColor = Color.green;
    public Color invalidColor = Color.red;
    public Color validDropColor = Color.yellow;

    [Header("Task Settings")]
    public TaskData taskData; // Template data
    public Task task; // Runtime component
    
    [Header("Turn-Based System")]
    public TextMeshProUGUI turnsRemainingText;
    public TextMeshProUGUI turnsUntilExpiryText;
    public Image expiryWarningImage;
    
    [Header("Legacy System (Deprecated)")]
    [HideInInspector]
    public float sanityDrainInterval = 2f; // seconds
    [HideInInspector]
    public int sanityDrainAmount = 1;
    [HideInInspector]
    public TaskProgress taskProgress;

    private DraggableCard currentEmployee;
    private bool isOccupied = false;
    private Coroutine sanityDrainCoroutine;
    private TaskSlotManager slotManager;
    private bool isCompleted = false;
    private bool isExpired = false;

    public TaskData TaskData => taskData;
    public Task Task => task;
    public DraggableCard CurrentEmployee => currentEmployee;
    public bool IsOccupied => isOccupied;

    private void Awake()
    {
        // Register with GameServiceLocator
        GameServiceLocator.RegisterTaskSlot(this);
        InitializeSlot();
    }

    private void Start()
    {
        if (taskData != null)
        {
            // Initialize Task component from template
            // Get existing Task component or add if not present
            if (task == null)
            {
                task = GetComponent<Task>();
            }
            
            if (task == null)
            {
                task = gameObject.AddComponent<Task>();
            }
            
            // Initialize the Task with the template data
            task.InitializeFromTemplate(taskData);
            task.parentSlot = this;
            
            UpdateSlotDisplay();
        }
    }

    private void InitializeSlot()
    {
        if (slotImage != null)
        {
            slotImage.color = emptyColor;
        }
        
        UpdateSlotDisplay();
    }

    public void InitializeSlot(TaskData data)
    {
        taskData = data;
        UpdateSlotDisplay();
    }

    public void UpdateSlotDisplay()
    {
        if (taskNameText != null)
            taskNameText.text = taskData != null ? taskData.taskName : "Empty Slot";
        
        if (taskDescriptionText != null)
        {
            if (taskData != null)
            {
                // Build description with skill requirements
                //string description = taskData.taskDescription;
                string requirements = GetSkillRequirementsText();
                
                if (!string.IsNullOrEmpty(requirements))
                {
                    //  description += $"\n\n{requirements}";
                }
                
                //taskDescriptionText.text = description;
                taskDescriptionText.text = requirements;
            }
            else
            {
                taskDescriptionText.text = "";
            }
        }
        
        //if (taskIconImage != null)
            //taskIconImage.gameObject.SetActive(taskData != null);
        
        if (taskIconImage != null && taskData != null && taskData.taskIcon != null)
        {
            taskIconImage.gameObject.SetActive(true);
            taskIconImage.sprite = taskData.taskIcon;
        }
        else if (taskIconImage != null)
        {
            taskIconImage.gameObject.SetActive(false);
        }
        
        UpdateTurnDisplay();
    }
    
    private string GetSkillRequirementsText()
    {
        if (taskData == null) return "";
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("Requirements: ");
        
        bool hasRequirements = false;
        
        if (taskData.requiredTechnicalSkill > 0)
        {
            sb.Append($"Tech {taskData.requiredTechnicalSkill}");
            hasRequirements = true;
        }
        
        if (taskData.requiredCreativeSkill > 0)
        {
            if (hasRequirements) sb.Append(", ");
            sb.Append($"Creative {taskData.requiredCreativeSkill}");
            hasRequirements = true;
        }
        
        if (taskData.minimumCommunicationSkill > 0)
        {
            if (hasRequirements) sb.Append(", ");
            sb.Append($"Comm {taskData.minimumCommunicationSkill}");
            hasRequirements = true;
        }
        
        if (taskData.minimumManagementSkill > 0)
        {
            if (hasRequirements) sb.Append(", ");
            sb.Append($"Mgmt {taskData.minimumManagementSkill}");
            hasRequirements = true;
        }
        
        if (taskData.minimumSanity > 0)
        {
            if (hasRequirements) sb.Append(", ");
            sb.Append($"Sanity {taskData.minimumSanity}");
            hasRequirements = true;
        }
        
        return hasRequirements ? sb.ToString() : "";
    }
    
    private void UpdateTurnDisplay()
    {
        if (task == null) return;
        
        if (turnsRemainingText != null)
        {
            if (task.IsCompleted)
                turnsRemainingText.text = "Completed";
            else
                turnsRemainingText.text = $"Turns: {task.TurnsRemaining}";
        }
        
        if (turnsUntilExpiryText != null)
        {
            if (task.IsExpired)
                turnsUntilExpiryText.text = "EXPIRED";
            else
                turnsUntilExpiryText.text = $"Expires: {task.TurnsUntilExpiry}";
        }
        
        if (expiryWarningImage != null)
        {
            if (task.TurnsUntilExpiry <= 2 && !task.IsCompleted)
                expiryWarningImage.gameObject.SetActive(true);
            else
                expiryWarningImage.gameObject.SetActive(false);
        }
    }

    public bool CanAssignEmployee(EmployeeData employee)
    {
        if (isOccupied || employee == null || taskData == null)
            return false;

        // Template-only check - runtime availability handled by Employee component
        return taskData.CanBeCompletedByEmployee(employee);
    }

    public bool CanAssignEmployee(Employee employee)
    {
        if (isOccupied || employee == null || taskData == null)
            return false;

        Debug.Log($"CanAssignEmployee check for {employee.Template?.employeeName}, isResting: {employee.isResting}");

        // Check if employee is already assigned to another task
        if (employee.currentTaskSlot != null && employee.currentTaskSlot != this)
        {
            // Check if the current task is in progress (has worked on it)
            if (employee.currentTaskSlot.Task != null && 
                employee.currentTaskSlot.Task.turnsRemaining < employee.currentTaskSlot.Task.Template.baseTurnDuration)
            {
                return false; // Cannot move from in-progress task
            }
        }

        UnityEngine.Debug.Log("CanBeCompletedByEmployee", employee);
        // Template check
        return taskData.CanBeCompletedByEmployee(employee.Template);
    }

    public void AssignEmployee(DraggableCard employeeCard)
    {
        if (employeeCard == null) return;

        // Check using Employee component if available
        EmployeeCard empCard = employeeCard as EmployeeCard;
        if (empCard != null && empCard.Employee != null)
        {
            if (!CanAssignEmployee(empCard.Employee))
                return;
        }
        else if (!CanAssignEmployee(employeeCard.EmployeeData))
        {
            return;
        }

        // Remove employee from previous task slot if any
        if (empCard != null && empCard.Employee != null && empCard.Employee.currentTaskSlot != null)
        {
            empCard.Employee.currentTaskSlot.RemoveEmployee();
        }

        // Remove employee from rest area if resting
        if (empCard != null && empCard.Employee != null && empCard.Employee.isResting)
        {
            Debug.Log($"Employee {empCard.EmployeeData?.employeeName} is resting, removing from rest area");
            
            // Find the rest area that contains this employee
            RestAreaSlot restArea = GameServiceLocator.FindRestAreaWithEmployee(empCard.EmployeeData);
            if (restArea != null)
            {
                Debug.Log($"Found rest area, removing employee");
                restArea.RemoveEmployee(empCard);
            }
            else
            {
                Debug.LogWarning($"Could not find rest area containing {empCard.EmployeeData?.employeeName}");
            }
        }

        currentEmployee = employeeCard;
        isOccupied = true;

        // Do NOT move the employee card - keep it in the employee container
        // This allows dragging the same card again to change tasks
        // employeeCard.transform.SetParent(transform);
        // employeeCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        employeeCard.SetCurrentSlot(this);

        // Update employee's current task slot reference
        if (empCard != null && empCard.Employee != null)
        {
            empCard.Employee.currentTaskSlot = this;
        }

        // Update slot visuals
        UpdateSlotVisuals(true);

        // Show employee preview
        if (employeePreviewImage != null && employeeCard.EmployeeData != null && employeeCard.EmployeeData.employeePortrait != null)
        {
            employeePreviewImage.sprite = employeeCard.EmployeeData.employeePortrait;
            employeePreviewImage.gameObject.SetActive(true);
        }

        // Assign employee to task component
        if (task != null && empCard != null)
        {
            task.AssignEmployee(empCard);

            // Notify manager
            if (slotManager != null)
            {
                slotManager.OnEmployeeAssigned(this, empCard.Employee);
            }
        }

        Debug.Log($"{employeeCard.EmployeeData?.employeeName ?? "Unknown"} assigned to {taskData.taskName}");
    }

    public void RemoveEmployee()
    {
        if (currentEmployee == null) return;

        // Clear employee's current task slot reference
        EmployeeCard empCard = currentEmployee as EmployeeCard;
        if (empCard != null && empCard.Employee != null)
        {
            empCard.Employee.currentTaskSlot = null;
        }

        // Reset employee state
        currentEmployee.SetCurrentSlot(null);

        // Clear slot
        currentEmployee = null;
        isOccupied = false;

        // Update visuals
        UpdateSlotVisuals(false);
        
        if (employeePreviewImage != null)
            employeePreviewImage.gameObject.SetActive(false);

        // Notify manager
        if (slotManager != null)
        {
            slotManager.OnEmployeeRemoved(this);
        }

        Debug.Log($"Employee removed from {taskData.taskName}");
    }

    private void UpdateSlotVisuals(bool occupied)
    {
        if (slotImage != null)
        {
            slotImage.color = occupied ? occupiedColor : emptyColor;
        }
    }

    private void StartSanityDrain()
    {
        if (sanityDrainCoroutine != null)
            StopCoroutine(sanityDrainCoroutine);

        sanityDrainCoroutine = StartCoroutine(SanityDrainRoutine());
    }

    private void StopSanityDrain()
    {
        if (sanityDrainCoroutine != null)
        {
            StopCoroutine(sanityDrainCoroutine);
            sanityDrainCoroutine = null;
        }
    }

    private IEnumerator SanityDrainRoutine()
    {
        while (isOccupied && currentEmployee != null)
        {
            yield return new WaitForSeconds(sanityDrainInterval);

            if (currentEmployee != null)
            {
                EmployeeCard empCard = currentEmployee as EmployeeCard;
                Employee employee = empCard?.Employee;
                
                if (employee != null)
                {
                    // Apply sanity drain
                    employee.UpdateSanity(-sanityDrainAmount);
                    currentEmployee.UpdateSanityDisplay();

                    // Check if employee sanity is too low
                    if (employee.CurrentSanity <= 0)
                    {
                        Debug.Log($"{employee.Template?.employeeName} has gone insane! Removing from task.");
                        RemoveEmployee();
                        break;
                    }

                    // Notify manager of sanity change
                    if (slotManager != null)
                    {
                        slotManager.OnEmployeeSanityChanged(this, employee.CurrentSanity);
                    }
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isOccupied) return;

        DraggableCard draggedCard = eventData.pointerDrag?.GetComponent<DraggableCard>();
        if (draggedCard == null) return;

        // Check using Employee component if available (EmployeeCard)
        EmployeeCard empCard = draggedCard as EmployeeCard;
        if (empCard != null && empCard.Employee != null)
        {
            if (CanAssignEmployee(empCard.Employee))
            {
                AssignEmployee(draggedCard);
            }
        }
        else if (CanAssignEmployee(draggedCard.EmployeeData))
        {
            AssignEmployee(draggedCard);
        }
    }

    public void HighlightAsValidDrop()
    {
        if (slotImage != null && !isOccupied)
        {
            slotImage.color = validDropColor;
        }
    }

    public void ResetHighlight()
    {
        UpdateSlotVisuals(isOccupied);
    }

    public void CompleteTask()
    {
        isCompleted = true;
        
        // Apply task effects to employee through Task component
        if (currentEmployee != null && task != null)
        {
            task.CompleteTask();
            currentEmployee.UpdateSanityDisplay();
        }

        // Remove employee
        if (currentEmployee != null)
        {
            currentEmployee.SetCurrentSlot(null);
            currentEmployee = null;
        }

        isOccupied = false;
        UpdateSlotVisuals(false);
        UpdateTurnDisplay();
        
        if (employeePreviewImage != null)
            employeePreviewImage.gameObject.SetActive(false);

        Debug.Log($"Task '{taskData.taskName}' completed!");
    }
    
    private void CheckUnfitRequirements(Employee employee)
    {
        if (employee == null || employee.Template == null || taskData == null) return;

        bool isUnfit = false;

        // Check each requirement using template data
        if (employee.Template.technicalSkill < taskData.requiredTechnicalSkill * 0.5f) // Less than 50% of required
        {
            isUnfit = true;
        }
        if (employee.Template.creativeSkill < taskData.requiredCreativeSkill * 0.5f)
        {
            isUnfit = true;
        }
        if (employee.Template.communicationSkill < taskData.minimumCommunicationSkill * 0.5f)
        {
            isUnfit = true;
        }
        if (employee.Template.managementSkill < taskData.minimumManagementSkill * 0.5f)
        {
            isUnfit = true;
        }

        if (isUnfit)
        {
            employee.statusManager?.AddStatus(StatusEffects.CreateOverworked());
            Debug.LogWarning($"{employee.Template.employeeName} is unfit for task {taskData.taskName} - applying Overworked status");
        }
    }

    public void ProcessTurnProgress()
    {
        if (task == null || !isOccupied || currentEmployee == null) return;
        
        EmployeeCard empCard = currentEmployee as EmployeeCard;
        if (empCard == null || empCard.Employee == null) return;
        
        Employee employee = empCard.Employee;

        // Process task turn
        task.ProcessTurn();
        
        // Apply turn effects to employee
        employee.UpdateSanity(-2); // Sanity cost per turn
        employee.UpdateCommunicationSkill(-1); // Communication skill cost per turn
        employee.UpdateManagementSkill(-1);    // Management skill cost per turn
        
        empCard.UpdateSanityDisplay();
        
        // Check for automatic sick status
        if (employee.CurrentSanity < 10 && !employee.HasStatus(StatusType.Sick))
        {
            employee.statusManager?.AddStatus(StatusEffects.CreateSick());
            Debug.LogWarning($"{employee.Template.employeeName} has burned out and is now Sick!");
            MoveToRestArea();
            return;
        }
        
        // Check if employee should go to rest area
        if (employee.CurrentSanity <= 0)
        {
            MoveToRestArea();
            return;
        }
        
        // Check if task is completed
        if (task.IsCompleted)
        {
            CompleteTask();
        }
        
        UpdateTurnDisplay();
    }
    
    public void ExpireTask()
    {
        isExpired = true;
        
        // Expire the task component
        if (task != null)
        {
            task.ExpireTask();
        }
        
        // Remove employee if assigned
        if (currentEmployee != null)
        {
            currentEmployee.SetCurrentSlot(null);
            currentEmployee = null;
        }
        
        isOccupied = false;
        UpdateSlotVisuals(false);
        UpdateTurnDisplay();
        
        if (employeePreviewImage != null)
            employeePreviewImage.gameObject.SetActive(false);
        
        // Mark task as expired visually
        if (slotImage != null)
        {
            slotImage.color = Color.red;
        }
        
        Debug.LogWarning($"Task '{taskData?.taskName}' has expired!");
    }

    public void OnTaskCompleted()
    {
        // Remove employee if assigned
        if (currentEmployee != null)
        {
            currentEmployee.SetCurrentSlot(null);
            currentEmployee = null;
        }
        
        isOccupied = false;
        isCompleted = true;
        
        // Destroy the task slot GameObject
        Destroy(gameObject);
        
        Debug.Log($"Task slot for '{taskData?.taskName}' destroyed after completion");
    }

    public void MoveToRestArea()
    {
        if (currentEmployee != null)
        {
            EmployeeCard empCard = currentEmployee as EmployeeCard;
            if (empCard != null)
            {
                empCard.GoToRestArea();
            }
        }
        
        RemoveEmployee();
    }

    public void UpdateTaskProgress()
    {
        if (taskProgress != null && currentEmployee != null)
        {
            taskProgress.UpdateWorkSpeed();
        }
    }

    private void OnDestroy()
    {
        // Legacy: Stop sanity drain (deprecated)
        // StopSanityDrain();
        // if (taskProgress != null)
        // {
        //     taskProgress.PauseWork();
        // }
        
        // Unregister from Service Locator
        GameServiceLocator.UnregisterTaskSlot(this);
    }
}
