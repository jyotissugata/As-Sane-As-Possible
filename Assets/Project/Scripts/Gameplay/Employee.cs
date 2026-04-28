using UnityEngine;

/// <summary>
/// Runtime Employee component that holds dynamic state
/// EmployeeData ScriptableObject holds only template data
/// </summary>
public class Employee : MonoBehaviour
{
    [Header("Runtime State")]
    public int currentSanity;
    public int currentCommunicationSkill;
    public int currentManagementSkill;
    public bool isAvailable = true;
    public string currentTaskName = "";
    public bool isResting = false;
    public TaskSlot currentTaskSlot; // Track current task slot assignment
    
    [Header("Status System")]
    public EmployeeStatusManager statusManager;
    
    private EmployeeData template;
    
    public int CurrentSanity => currentSanity;
    public int CurrentCommunicationSkill => currentCommunicationSkill;
    public int CurrentManagementSkill => currentManagementSkill;
    public bool CanWork => isAvailable && statusManager?.CanWork() == true;
    public EmployeeData Template => template;
    
    private void Awake()
    {
        // Register with GameServiceLocator
        GameServiceLocator.RegisterEmployee(this);
    }
    
    private void OnDestroy()
    {
        // Unregister from GameServiceLocator
        GameServiceLocator.UnregisterEmployee(this);
        Cleanup();
    }
    
    public bool HasStatus(StatusType statusType)
    {
        return statusManager?.HasStatus(statusType) ?? false;
    }
    
    public void InitializeFromTemplate(EmployeeData employeeTemplate)
    {
        template = employeeTemplate;
        
        // Initialize runtime state from template
        currentSanity = template.sanity;
        currentCommunicationSkill = template.communicationSkill;
        currentManagementSkill = template.managementSkill;
        isAvailable = true;
        currentTaskName = "";
        isResting = false;
        
        // Initialize status manager
        if (statusManager == null)
        {
            statusManager = new EmployeeStatusManager(template);
        }
    }
    
    public void UpdateSanity(int amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, 0, 100);
    }
    
    public void UpdateCommunicationSkill(int amount)
    {
        currentCommunicationSkill = Mathf.Clamp(currentCommunicationSkill + amount, 0, 100);
    }
    
    public void UpdateManagementSkill(int amount)
    {
        currentManagementSkill = Mathf.Clamp(currentManagementSkill + amount, 0, 100);
    }
    
    public void ProcessTurn()
    {
        // Update statuses
        statusManager?.UpdateStatuses();
        
        // Apply status effects
        float modifier = statusManager?.GetTotalEffectivenessModifier() ?? 1f;
        
        // Natural recovery when not working
        if (isAvailable && !isResting)
        {
            UpdateCommunicationSkill(2);
            UpdateManagementSkill(Random.Range(-1, 2));
            UpdateSanity(2);
        }
    }
    
    public void AssignToTask(TaskData task)
    {
        UnityEngine.Debug.Log($"Employee AssignToTask {task?.taskName}", this);
        if (task == null) return;
        
        isAvailable = false;
        currentTaskName = task.taskName;
    }
    
    public void RemoveFromTask()
    {
        UnityEngine.Debug.Log($"Employee RemoveFromTask", this);
        isAvailable = true;
        currentTaskName = "";
    }
    
    public void GoToRestArea()
    {
        UnityEngine.Debug.Log($"Employee GoToRestArea", this);
        isAvailable = false;
        currentTaskName = "Resting";
        isResting = true;
        
        // Add resting status
        statusManager?.AddStatus(StatusEffects.CreateResting());
    }
    
    public void LeaveRestArea()
    {
        UnityEngine.Debug.Log($"Employee LeaveRestArea", this);
        isAvailable = true;
        currentTaskName = "";
        isResting = false;
        
        // Remove resting status
        statusManager?.RemoveStatus(StatusType.Resting);
    }
    
    public bool CanPerformTask(TaskData task)
    {
        if (!CanWork) return false;
        
        return template.technicalSkill >= task.requiredTechnicalSkill &&
               template.creativeSkill >= task.requiredCreativeSkill &&
               currentSanity >= task.minimumSanity &&
               currentCommunicationSkill >= task.minimumCommunicationSkill &&
               currentManagementSkill >= task.minimumManagementSkill;
    }
    
    public void Cleanup()
    {
        statusManager = null;
        template = null;
    }
}
