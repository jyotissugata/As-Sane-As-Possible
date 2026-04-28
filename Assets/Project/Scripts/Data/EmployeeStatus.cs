using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class EmployeeStatus
{
    public StatusType statusType;
    public string displayName;
    public string description;
    public int duration; // -1 for permanent, 0 for expired
    public float effectivenessModifier = 1f;
    public bool preventsWork = false;
    public bool isBuff = true;
    public Color statusColor = Color.white;

    public bool IsActive => duration > 0 || duration == -1;
    public bool IsExpired => duration == 0;
}

public enum StatusType
{
    None,
    FeelingGreat,     // High Sanity buff
    Overworked,       // Unfit Requirement penalty
    Sick,            // Burnout status
    Resting,         // Rest Area status
    Focused,         // High Focus buff
    Exhausted,       // Low Stamina penalty
    Inspired,        // Creative buff
    Stressed         // Low Sanity penalty
}

public static class StatusEffects
{
    public static EmployeeStatus CreateFeelingGreat()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.FeelingGreat,
            displayName = "Feeling Great",
            description = "High sanity boosts performance by 20%",
            duration = -1, // Permanent while sanity > 80
            effectivenessModifier = 1.2f,
            isBuff = true,
            statusColor = Color.green
        };
    }

    public static EmployeeStatus CreateOverworked()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Overworked,
            displayName = "Overworked",
            description = "Unfit for task - 50% performance penalty",
            duration = -1, // Permanent while assigned to unfit task
            effectivenessModifier = 0.5f,
            isBuff = false,
            statusColor = Color.yellow
        };
    }

    public static EmployeeStatus CreateSick()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Sick,
            displayName = "Sick",
            description = "Too sick to work - must rest for 3 turns",
            duration = 3,
            effectivenessModifier = 0f,
            preventsWork = true,
            isBuff = false,
            statusColor = Color.red
        };
    }

    public static EmployeeStatus CreateResting()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Resting,
            displayName = "Resting",
            description = "Recovering in rest area",
            duration = -1, // Permanent while in rest area
            effectivenessModifier = 0f,
            preventsWork = true,
            isBuff = false,
            statusColor = Color.blue
        };
    }

    public static EmployeeStatus CreateFocused()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Focused,
            displayName = "Focused",
            description = "High focus improves efficiency by 15%",
            duration = -1, // Permanent while focus > 80
            effectivenessModifier = 1.15f,
            isBuff = true,
            statusColor = Color.cyan
        };
    }

    public static EmployeeStatus CreateExhausted()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Exhausted,
            displayName = "Exhausted",
            description = "Low stamina reduces performance by 30%",
            duration = -1, // Permanent while stamina < 20
            effectivenessModifier = 0.7f,
            isBuff = false,
            statusColor = Color.gray
        };
    }

    public static EmployeeStatus CreateInspired()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Inspired,
            displayName = "Inspired",
            description = "Creative inspiration boosts performance by 25%",
            duration = 2, // 2 turns
            effectivenessModifier = 1.25f,
            isBuff = true,
            statusColor = Color.magenta
        };
    }

    public static EmployeeStatus CreateStressed()
    {
        return new EmployeeStatus
        {
            statusType = StatusType.Stressed,
            displayName = "Stressed",
            description = "Low sanity reduces performance by 40%",
            duration = -1, // Permanent while sanity < 30
            effectivenessModifier = 0.6f,
            isBuff = false,
            statusColor = Color.red
        };
    }
}

public class EmployeeStatusManager
{
    private EmployeeData employee;
    private List<EmployeeStatus> activeStatuses = new List<EmployeeStatus>();

    public EmployeeStatusManager(EmployeeData employee)
    {
        this.employee = employee;
    }

    public void AddStatus(EmployeeStatus status)
    {
        if (status == null) return;

        // Remove existing status of same type (except for permanent statuses)
        RemoveStatus(status.statusType);

        activeStatuses.Add(status);
        Debug.Log($"{employee.employeeName} gained status: {status.displayName}");
    }

    public void RemoveStatus(StatusType statusType)
    {
        activeStatuses.RemoveAll(s => s.statusType == statusType);
    }

    public void UpdateStatuses()
    {
        // Update duration-based statuses
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            EmployeeStatus status = activeStatuses[i];
            
            if (status.duration > 0)
            {
                status.duration--;
                if (status.duration <= 0)
                {
                    Debug.Log($"{employee.employeeName} lost status: {status.displayName}");
                    activeStatuses.RemoveAt(i);
                }
            }
        }

        // Check for conditional statuses
        UpdateConditionalStatuses();
    }

    private void UpdateConditionalStatuses()
    {
        // High Sanity buff
        if (employee.sanity > 80 && !HasStatus(StatusType.FeelingGreat))
        {
            AddStatus(StatusEffects.CreateFeelingGreat());
        }
        else if (employee.sanity <= 80)
        {
            RemoveStatus(StatusType.FeelingGreat);
        }

        // Low Sanity penalty
        if (employee.sanity < 30 && !HasStatus(StatusType.Stressed))
        {
            AddStatus(StatusEffects.CreateStressed());
        }
        else if (employee.sanity >= 30)
        {
            RemoveStatus(StatusType.Stressed);
        }

        // High Focus buff
        if (employee.communicationSkill > 80 && !HasStatus(StatusType.Focused))
        {
            AddStatus(StatusEffects.CreateFocused());
        }
        else if (employee.communicationSkill <= 80)
        {
            RemoveStatus(StatusType.Focused);
        }

        // Burnout/Sick check
        if (employee.sanity < 10 && !HasStatus(StatusType.Sick))
        {
            AddStatus(StatusEffects.CreateSick());
        }
    }

    public bool HasStatus(StatusType statusType)
    {
        return activeStatuses.Exists(s => s.statusType == statusType);
    }

    public EmployeeStatus GetStatus(StatusType statusType)
    {
        return activeStatuses.Find(s => s.statusType == statusType);
    }

    public List<EmployeeStatus> GetActiveStatuses()
    {
        return new List<EmployeeStatus>(activeStatuses);
    }

    public float GetTotalEffectivenessModifier()
    {
        float modifier = 1f;

        foreach (var status in activeStatuses)
        {
            if (status.IsActive)
            {
                modifier *= status.effectivenessModifier;
            }
        }

        return modifier;
    }

    public bool CanWork()
    {
        foreach (var status in activeStatuses)
        {
            if (status.IsActive && status.preventsWork)
            {
                return false;
            }
        }
        return true;
    }

    public string GetStatusSummary()
    {
        if (activeStatuses.Count == 0)
            return "Normal";

        var activeBuffs = activeStatuses.FindAll(s => s.IsActive && s.isBuff);
        var activeDebuffs = activeStatuses.FindAll(s => s.IsActive && !s.isBuff);

        string summary = "";

        if (activeBuffs.Count > 0)
        {
            summary += activeBuffs[0].displayName;
            if (activeBuffs.Count > 1)
                summary += $" (+{activeBuffs.Count - 1})";
        }

        if (activeDebuffs.Count > 0)
        {
            if (summary.Length > 0)
                summary += " | ";
            summary += activeDebuffs[0].displayName;
            if (activeDebuffs.Count > 1)
                summary += $" (+{activeDebuffs.Count - 1})";
        }

        return summary;
    }

    public Color GetDominantStatusColor()
    {
        if (activeStatuses.Count == 0)
            return Color.white;

        // Priority: Red (debuffs) > Green/Yellow (buffs)
        foreach (var status in activeStatuses)
        {
            if (status.IsActive && !status.isBuff)
            {
                return status.statusColor;
            }
        }

        foreach (var status in activeStatuses)
        {
            if (status.IsActive && status.isBuff)
            {
                return status.statusColor;
            }
        }

        return Color.white;
    }
}
