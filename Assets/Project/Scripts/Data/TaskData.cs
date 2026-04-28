using UnityEngine;

[CreateAssetMenu(fileName = "NewTaskData", menuName = "ASAP/TaskData")]
public class TaskData : ScriptableObject
{
    [Header("Task Information")]
    public string taskName;
    public string taskDescription;
    public Sprite taskIcon;
    
    [Header("Task Requirements")]
    [Range(0, 100)]
    public int requiredTechnicalSkill = 0;
    
    [Range(0, 100)]
    public int requiredCreativeSkill = 0;

    [Range(0, 100)]
    public int minimumCommunicationSkill = 15;
    
    [Range(0, 100)]
    public int minimumManagementSkill = 10;
    
    [Range(0, 100)]
    public int minimumSanity = 10;


    [Header("Turn-Based Properties")]
    public int baseTurnDuration = 3; // turns to complete
    public int maxTurnsBeforeExpiry = 10; // turns before task expires
    public int difficultyLevel = 1;
    public TaskType taskType;

    [Header("Legacy Properties (Deprecated)")]
    [HideInInspector]
    public float baseCompletionTime = 5f; // in seconds - kept for compatibility

    [Header("Task Effects")]
    public int sanityChangeOnComplete = -5;
    public int experienceReward = 10;
    public int moneyReward = 50;

    public enum TaskType
    {
        Technical,
        Creative,
        Mixed,
        Administrative
    }

    public bool CanBeCompletedByEmployee(EmployeeData employee)
    {
        if (employee == null)
            UnityEngine.Debug.LogError("Employee is null");
        return employee.technicalSkill >= requiredTechnicalSkill &&
               employee.creativeSkill >= requiredCreativeSkill &&
               employee.communicationSkill >= minimumCommunicationSkill &&
               employee.managementSkill >= minimumManagementSkill &&
               employee.sanity >= minimumSanity;
    }

    public float GetCompletionTime(EmployeeData employee)
    {
        float skillModifier = 1f;
        
        // Calculate skill bonus
        if (taskType == TaskType.Technical || taskType == TaskType.Mixed)
        {
            skillModifier -= (employee.technicalSkill - requiredTechnicalSkill) * 0.01f;
        }
        
        if (taskType == TaskType.Creative || taskType == TaskType.Mixed)
        {
            skillModifier -= (employee.creativeSkill - requiredCreativeSkill) * 0.01f;
        }
        
        skillModifier = Mathf.Max(skillModifier, 0.3f); // Minimum 30% of base time
        
        return baseCompletionTime * skillModifier;
    }

    public float CalculateWorkSpeed(EmployeeData employee)
    {
        if (employee == null) return 1f;

        // Base speed
        float workSpeed = 1f;

        // Technical skill contribution
        float technicalBonus = 0f;
        if (taskType == TaskType.Technical || taskType == TaskType.Mixed)
        {
            int skillDifference = employee.technicalSkill - requiredTechnicalSkill;
            technicalBonus = skillDifference * 0.02f; // 2% speed per skill point above requirement
        }

        // Creative skill contribution
        float creativeBonus = 0f;
        if (taskType == TaskType.Creative || taskType == TaskType.Mixed)
        {
            int skillDifference = employee.creativeSkill - requiredCreativeSkill;
            creativeBonus = skillDifference * 0.02f; // 2% speed per skill point above requirement
        }

        // Communication skill contribution
        float communicationBonus = 0f;
        if (taskType == TaskType.Administrative || taskType == TaskType.Mixed)
        {
            int skillDifference = employee.communicationSkill - minimumCommunicationSkill;
            communicationBonus = skillDifference * 0.02f;
        }

        // Management skill contribution
        float managementBonus = 0f;
        if (taskType == TaskType.Administrative || taskType == TaskType.Mixed)
        {
            int skillDifference = employee.managementSkill - minimumManagementSkill;
            managementBonus = skillDifference * 0.02f;
        }

        workSpeed += technicalBonus + creativeBonus + communicationBonus + managementBonus;

        // Apply sanity-performance scaling
        float sanityMultiplier = GetSanityPerformanceMultiplier(employee.sanity);
        workSpeed *= sanityMultiplier;

        // Apply difficulty modifier
        float difficultyModifier = 1f / difficultyLevel;
        workSpeed *= difficultyModifier;

        // Minimum speed cap (0.1x minimum speed)
        workSpeed = Mathf.Max(workSpeed, 0.1f);

        return workSpeed;
    }

    public float GetSanityPerformanceMultiplier(int sanity)
    {
        if (sanity <= 0)
            return 0f; // Cannot work
        else if (sanity < 30)
            return 0.5f; // 50% speed reduction
        else if (sanity < 60)
            return 0.8f; // 20% speed reduction
        else if (sanity < 80)
            return 1.0f; // Normal speed
        else
            return 1.2f; // 20% speed bonus for high sanity
    }

    public float GetSkillMatchScore(EmployeeData employee)
    {
        if (employee == null) return 0f;

        float score = 0f;
        int totalRequirements = 0;

        // Technical skill matching
        if (taskType == TaskType.Technical || taskType == TaskType.Mixed)
        {
            int techDiff = employee.technicalSkill - requiredTechnicalSkill;
            score += Mathf.Max(0, techDiff) * 1.5f;
            totalRequirements++;
        }

        // Creative skill matching
        if (taskType == TaskType.Creative || taskType == TaskType.Mixed)
        {
            int creativeDiff = employee.creativeSkill - requiredCreativeSkill;
            score += Mathf.Max(0, creativeDiff) * 1.5f;
            totalRequirements++;
        }

        // Communication skill matching
        if (taskType == TaskType.Administrative || taskType == TaskType.Mixed)
        {
            int commDiff = employee.communicationSkill - minimumCommunicationSkill;
            score += Mathf.Max(0, commDiff) * 1.5f;
            totalRequirements++;
        }

        // Management skill matching
        if (taskType == TaskType.Administrative || taskType == TaskType.Mixed)
        {
            int mgmtDiff = employee.managementSkill - minimumManagementSkill;
            score += Mathf.Max(0, mgmtDiff) * 1.5f;
            totalRequirements++;
        }

        // Sanity bonus
        score += employee.sanity * 0.1f;

        // Normalize score if there are requirements
        if (totalRequirements > 0)
        {
            score /= totalRequirements;
        }

        return score;
    }

    public bool IsEmployeeOverqualified(EmployeeData employee)
    {
        if (employee == null) return false;

        bool overqualifiedTech = taskType == TaskType.Technical && 
                                employee.technicalSkill > requiredTechnicalSkill + 30;
        
        bool overqualifiedCreative = taskType == TaskType.Creative && 
                                    employee.creativeSkill > requiredCreativeSkill + 30;

        return overqualifiedTech || overqualifiedCreative;
    }

    public float GetTurnEfficiency(EmployeeData employee)
    {
        if (employee == null) return 1;

        int efficiency = 1; // Base efficiency

        // Skill bonuses
        if (taskType == TaskType.Technical || taskType == TaskType.Mixed)
        {
            efficiency += (employee.technicalSkill - requiredTechnicalSkill) / 20;
        }

        if (taskType == TaskType.Creative || taskType == TaskType.Mixed)
        {
            efficiency += (employee.creativeSkill - requiredCreativeSkill) / 20;
        }

        if (taskType == TaskType.Administrative || taskType == TaskType.Mixed)
        {
            efficiency += (employee.communicationSkill - minimumCommunicationSkill) / 20;
            efficiency += (employee.managementSkill - minimumManagementSkill) / 20;
        }

        // Sanity penalty
        if (employee.sanity < 30)
        {
            efficiency = Mathf.Max(1, efficiency - 1);
        }

        return Mathf.Max(1, efficiency);
    }

    // Template-only methods - no dynamic state management
    // All dynamic state is now handled by the Task component
}
