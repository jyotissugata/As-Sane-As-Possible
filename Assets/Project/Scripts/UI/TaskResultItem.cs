using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskResultItem : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI employeeNameText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI statusText;
    public Image statusImage;
    public TextMeshProUGUI statsText;

    [Header("Status Colors")]
    public Color completedColor = Color.green;
    public Color inProgressColor = Color.blue;
    public Color expiredColor = Color.red;
    public Color unassignedColor = Color.gray;

    public void Setup(ResultPanel.TaskResult result)
    {
        if (taskNameText != null)
            taskNameText.text = result.taskName;

        if (employeeNameText != null)
            employeeNameText.text = result.employeeName;

        if (progressText != null)
        {
            int progressPercentage = Mathf.RoundToInt((1f - (float)result.turnsRemaining / 3f) * 100f);
            progressText.text = $"Progress: {progressPercentage}%";
        }

        if (statsText != null && result.wasAssigned)
        {
            statsText.text = $"Sanity: {result.sanityChange} | Comm: {result.communicationChange} | Mgmt: {result.managementChange}";
        }
        else if (statsText != null)
        {
            statsText.text = "No work done";
        }

        SetupStatus(result);
    }

    private void SetupStatus(ResultPanel.TaskResult result)
    {
        string statusTextStr = "";
        Color statusColor = unassignedColor;

        if (result.isCompleted)
        {
            statusTextStr = "COMPLETED";
            statusColor = completedColor;
        }
        else if (result.isExpired)
        {
            statusTextStr = "EXPIRED";
            statusColor = expiredColor;
        }
        else if (result.wasAssigned)
        {
            statusTextStr = $"In Progress ({result.turnsRemaining} turns left)";
            statusColor = inProgressColor;
        }
        else
        {
            statusTextStr = $"Unassigned ({result.turnsUntilExpiry} turns to expiry)";
            statusColor = unassignedColor;
        }

        if (statusText != null)
            statusText.text = statusTextStr;

        if (statusImage != null)
            statusImage.color = statusColor;
    }
}
