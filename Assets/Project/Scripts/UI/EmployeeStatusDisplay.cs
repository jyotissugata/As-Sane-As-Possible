using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EmployeeStatusDisplay : MonoBehaviour
{
    [Header("Status Display Components")]
    public Image statusIcon;
    public TextMeshProUGUI statusText;
    public Image statusBackground;
    public GameObject statusPanel;

    [Header("Status Icons")]
    public Sprite feelingGreatIcon;
    public Sprite overworkedIcon;
    public Sprite sickIcon;
    public Sprite restingIcon;
    public Sprite focusedIcon;
    public Sprite exhaustedIcon;
    public Sprite inspiredIcon;
    public Sprite stressedIcon;

    [Header("Display Settings")]
    public bool showMultipleStatuses = true;
    public float iconSize = 32f;
    public Color defaultColor = Color.white;

    private EmployeeCard employeeCard;
    private Employee employee;

    private void Awake()
    {
        employeeCard = GetComponent<EmployeeCard>();
        if (employeeCard != null)
        {
            employee = employeeCard.Employee;
        }
    }

    private void Start()
    {
        if (statusPanel != null)
        {
            statusPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (employee != null && employee.statusManager != null)
        {
            UpdateStatusDisplay();
        }
    }

    public void UpdateStatusDisplay()
    {
        if (employee == null || employee.statusManager == null) return;

        var activeStatuses = employee.statusManager.GetActiveStatuses();
        
        if (activeStatuses.Count == 0)
        {
            HideStatusDisplay();
            return;
        }

        if (statusPanel != null)
        {
            statusPanel.SetActive(true);
        }

        if (showMultipleStatuses && activeStatuses.Count > 1)
        {
            DisplayMultipleStatuses(activeStatuses);
        }
        else
        {
            DisplaySingleStatus(activeStatuses[0]);
        }
    }

    private void DisplaySingleStatus(EmployeeStatus status)
    {
        if (statusIcon != null)
        {
            statusIcon.sprite = GetStatusIcon(status.statusType);
            statusIcon.color = status.statusColor;
            statusIcon.gameObject.SetActive(true);
        }

        if (statusText != null)
        {
            statusText.text = status.displayName;
            statusText.color = status.statusColor;
            statusText.gameObject.SetActive(true);
        }

        if (statusBackground != null)
        {
            statusBackground.color = new Color(status.statusColor.r, status.statusColor.g, status.statusColor.b, 0.3f);
        }
    }

    private void DisplayMultipleStatuses(List<EmployeeStatus> statuses)
    {
        // Show the most important status (debuffs take priority)
        EmployeeStatus priorityStatus = GetPriorityStatus(statuses);
        DisplaySingleStatus(priorityStatus);

        // Update text to show count
        if (statusText != null && statuses.Count > 1)
        {
            statusText.text = $"{priorityStatus.displayName} (+{statuses.Count - 1})";
        }
    }

    private EmployeeStatus GetPriorityStatus(List<EmployeeStatus> statuses)
    {
        // Priority: Sick > Overworked > Stressed > Exhausted > Resting > Buffs
        var priorityOrder = new StatusType[]
        {
            StatusType.Sick,
            StatusType.Overworked,
            StatusType.Stressed,
            StatusType.Exhausted,
            StatusType.Resting,
            StatusType.FeelingGreat,
            StatusType.Focused,
            StatusType.Inspired
        };

        foreach (var statusType in priorityOrder)
        {
            var status = statuses.Find(s => s.statusType == statusType);
            if (status != null)
            {
                return status;
            }
        }

        return statuses[0]; // Fallback
    }

    private Sprite GetStatusIcon(StatusType statusType)
    {
        return statusType switch
        {
            StatusType.FeelingGreat => feelingGreatIcon,
            StatusType.Overworked => overworkedIcon,
            StatusType.Sick => sickIcon,
            StatusType.Resting => restingIcon,
            StatusType.Focused => focusedIcon,
            StatusType.Exhausted => exhaustedIcon,
            StatusType.Inspired => inspiredIcon,
            StatusType.Stressed => stressedIcon,
            _ => null
        };
    }

    private void HideStatusDisplay()
    {
        if (statusPanel != null)
        {
            statusPanel.SetActive(false);
        }

        if (statusIcon != null)
        {
            statusIcon.gameObject.SetActive(false);
        }

        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }
    }

    public void SetEmployee(Employee newEmployee)
    {
        employee = newEmployee;
    }

    public string GetDetailedStatusText()
    {
        if (employee == null || employee.statusManager == null) return "No status";

        var activeStatuses = employee.statusManager.GetActiveStatuses();
        if (activeStatuses.Count == 0) return "Normal";

        string details = "";
        foreach (var status in activeStatuses)
        {
            if (details.Length > 0) details += "\n";
            details += $"{status.displayName}: {status.description}";
        }

        return details;
    }

    public Color GetStatusColor()
    {
        if (employee == null || employee.statusManager == null) return defaultColor;

        return employee.statusManager.GetDominantStatusColor();
    }

    public bool HasPreventingWorkStatus()
    {
        if (employee == null || employee.statusManager == null) return false;

        return !employee.statusManager.CanWork();
    }

    public void OnStatusClicked()
    {
        // Could show detailed status popup
        Debug.Log($"Status details for {employee?.Template?.employeeName}: {GetDetailedStatusText()}");
    }
}
