using UnityEngine;

/// <summary>
/// Runtime Task component that holds dynamic state
/// TaskData ScriptableObject holds only template data
/// </summary>
public class Task : MonoBehaviour
{
    [Header("Runtime State")]
    public int turnsRemaining;
    public int turnsUntilExpiry;
    public bool isCompleted = false;
    public bool isExpired = false;
    public float currentProgress = 0f;
    public Employee assignedEmployee;
    
    [Header("Task References")]
    public TaskData template;
    public TaskSlot parentSlot;
    
    public int TurnsRemaining => turnsRemaining;
    public int TurnsUntilExpiry => turnsUntilExpiry;
    public bool IsCompleted => isCompleted;
    public bool IsExpired => isExpired;
    public float ProgressPercentage => currentProgress;
    public TaskData Template => template;
    public Employee AssignedEmployee => assignedEmployee;
    
    private void Awake()
    {
        // Register with GameServiceLocator
        GameServiceLocator.RegisterTask(this);
    }
    
    private void OnDestroy()
    {
        // Unregister from GameServiceLocator
        GameServiceLocator.UnregisterTask(this);
        Cleanup();
    }
    
    public void InitializeFromTemplate(TaskData taskTemplate)
    {
        template = taskTemplate;
        
        // Initialize runtime state from template
        turnsRemaining = template.baseTurnDuration;
        turnsUntilExpiry = template.maxTurnsBeforeExpiry;
        isCompleted = false;
        isExpired = false;
        currentProgress = 0f;
        assignedEmployee = null;
    }
    
    public void ProcessTurn()
    {
        if (isCompleted || isExpired) return;
        
        // Decrease turns remaining
        turnsRemaining--;
        
        // Decrease expiry timer
        turnsUntilExpiry--;
        
        // Update progress
        UpdateProgress();
        
        // Check completion
        if (turnsRemaining <= 0)
        {
            CompleteTask();
        }
        
        // Check expiry
        if (turnsUntilExpiry <= 0)
        {
            ExpireTask();
        }
    }
    
    private void UpdateProgress()
    {
        if (template == null) return;
        
        // Calculate progress based on turns completed
        int totalTurns = template.baseTurnDuration;
        int completedTurns = totalTurns - turnsRemaining;
        currentProgress = (float)completedTurns / totalTurns;
        
        // Apply employee efficiency modifier
        if (assignedEmployee != null )
        {
            float efficiency = assignedEmployee.statusManager?.GetTotalEffectivenessModifier() ?? 1f;
            currentProgress *= efficiency;
        }
        
        currentProgress = Mathf.Clamp01(currentProgress);
    }
    
    public void AssignEmployee(EmployeeCard employeeCard)
    {
        assignedEmployee = employeeCard?.Employee;
        
        // Check for unfit requirements
        if (assignedEmployee != null && template != null)
        {
            CheckUnfitRequirements(assignedEmployee);
        }
    }
    
    public void RemoveEmployee()
    {
        assignedEmployee = null;
    }
    
    private void CheckUnfitRequirements(Employee employee)
    {
        if (template == null || employee.Template == null) return;
        
        bool isUnfit = false;
        
        // Check each requirement (less than 50% of required)
        if (employee.Template.technicalSkill < template.requiredTechnicalSkill * 0.5f)
        {
            isUnfit = true;
        }
        if (employee.Template.creativeSkill < template.requiredCreativeSkill * 0.5f)
        {
            isUnfit = true;
        }
        if (employee.CurrentCommunicationSkill < template.minimumCommunicationSkill * 0.5f)
        {
            isUnfit = true;
        }
        if (employee.CurrentManagementSkill < template.minimumManagementSkill * 0.5f)
        {
            isUnfit = true;
        }
        
        if (isUnfit)
        {
            employee.statusManager?.AddStatus(StatusEffects.CreateOverworked());
            Debug.LogWarning($"{employee.Template.employeeName} is unfit for task {template.taskName}");
        }
    }
    
    public void CompleteTask()
    {
        if (isCompleted) return;
        
        Debug.Log($"CompleteTask called for '{template?.taskName}'");
        
        isCompleted = true;
        currentProgress = 1f;
        
        // Calculate performance-based reward
        float performanceMultiplier = CalculatePerformanceMultiplier();
        
        // Apply task effects to assigned employee
        if (assignedEmployee != null && assignedEmployee != null)
        {
            ApplyTaskEffects(assignedEmployee);
        }
        
        // Add money reward based on performance
        if (EconomyManager.Instance != null && template != null)
        {
            int finalReward = Mathf.RoundToInt(template.moneyReward * performanceMultiplier);
            EconomyManager.Instance.AddMoney(finalReward);
            Debug.Log($"Task '{template?.taskName}' completed! Reward: ${finalReward} (x{performanceMultiplier:F2} performance bonus)");
        }
        else
        {
            Debug.LogError($"EconomyManager.Instance is null or template is null! Instance: {EconomyManager.Instance != null}, Template: {template != null}");
        }
        
        // Notify parent slot to destroy itself
        if (parentSlot != null)
        {
            parentSlot.OnTaskCompleted();
        }
        
        Debug.Log($"Task '{template?.taskName}' completed!");
    }
    
    private float CalculatePerformanceMultiplier()
    {
        if (assignedEmployee == null || template == null) return 1f;
        
        float multiplier = 1f;
        
        // Skill match bonus
        float skillMatch = 0f;
        skillMatch += Mathf.Clamp01((assignedEmployee.Template.technicalSkill - template.requiredTechnicalSkill) / 50f) * 0.2f;
        skillMatch += Mathf.Clamp01((assignedEmployee.Template.creativeSkill - template.requiredCreativeSkill) / 50f) * 0.2f;
        skillMatch += Mathf.Clamp01((assignedEmployee.CurrentCommunicationSkill - template.minimumCommunicationSkill) / 50f) * 0.2f;
        skillMatch += Mathf.Clamp01((assignedEmployee.CurrentManagementSkill - template.minimumManagementSkill) / 50f) * 0.2f;
        multiplier += skillMatch;
        
        // Sanity bonus
        if (assignedEmployee.CurrentSanity >= 80)
        {
            multiplier += 0.3f; // 30% bonus for high sanity
        }
        else if (assignedEmployee.CurrentSanity >= 60)
        {
            multiplier += 0.1f; // 10% bonus for good sanity
        }
        else if (assignedEmployee.CurrentSanity < 30)
        {
            multiplier -= 0.2f; // 20% penalty for low sanity
        }
        
        // Status penalty
        if (assignedEmployee.HasStatus(StatusType.Sick) || assignedEmployee.HasStatus(StatusType.Stressed))
        {
            multiplier -= 0.15f;
        }
        
        return Mathf.Max(0.5f, multiplier); // Minimum 50% of base reward
    }
    
    public void ExpireTask()
    {
        if (isExpired) return;
        
        isExpired = true;
        currentProgress = 0f;
        
        // Remove assigned employee
        if (assignedEmployee != null)
        {
            assignedEmployee.RemoveFromTask();
            assignedEmployee = null;
        }
        
        Debug.LogWarning($"Task '{template?.taskName}' expired!");
    }
    
    private void ApplyTaskEffects(Employee employee)
    {
        if (template == null) return;
        
        // Apply template effects
        employee.UpdateSanity(template.sanityChangeOnComplete);
        employee.UpdateCommunicationSkill(-2);
        employee.UpdateManagementSkill(-1);
        
        Debug.Log($"Task effects applied to {employee.Template.employeeName}");
    }
    
    public bool ShouldExpire()
    {
        return turnsUntilExpiry <= 0 && !isExpired;
    }
    
    public void Cleanup()
    {
        assignedEmployee = null;
        template = null;
        parentSlot = null;
    }
}
