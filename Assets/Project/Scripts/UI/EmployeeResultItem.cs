using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmployeeResultItem : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI employeeNameText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI sanityText;
    public TextMeshProUGUI communicationText;
    public TextMeshProUGUI managementText;
    public Image statusImage;

    [Header("Status Colors")]
    public Color workingColor = Color.blue;
    public Color restingColor = Color.green;
    public Color availableColor = Color.gray;
    public Color criticalColor = Color.red;

    public void Setup(ResultPanel.EmployeeResult result)
    {
        if (employeeNameText != null)
            employeeNameText.text = result.employeeName;

        SetupStatus(result);
        SetupStats(result);
    }

    private void SetupStatus(ResultPanel.EmployeeResult result)
    {
        string statusTextStr = "";
        Color statusColor = availableColor;

        if (result.isWorking)
        {
            statusTextStr = $"Working on: {result.currentTask}";
            statusColor = workingColor;
        }
        else if (result.isResting)
        {
            statusTextStr = "Resting";
            statusColor = restingColor;
        }
        else
        {
            statusTextStr = "Available";
            statusColor = availableColor;
        }

        if (statusText != null)
            statusText.text = statusTextStr;

        if (statusImage != null)
            statusImage.color = statusColor;
    }

    private void SetupStats(ResultPanel.EmployeeResult result)
    {
        // Sanity
        if (sanityText != null)
        {
            int sanityChange = result.sanityAfter - result.sanityBefore;
            string sanityChangeStr = sanityChange >= 0 ? $"+{sanityChange}" : sanityChange.ToString();
            sanityText.text = $"Sanity: {result.sanityAfter} ({sanityChangeStr})";
            
            // Color based on final sanity
            if (result.sanityAfter < 30)
                sanityText.color = criticalColor;
            else if (sanityChange < 0)
                sanityText.color = Color.red;
            else if (sanityChange > 0)
                sanityText.color = Color.green;
            else
                sanityText.color = Color.white;
        }

        // Communication Skill
        if (communicationText != null)
        {
            int communicationChange = result.communicationAfter - result.communicationBefore;
            string communicationChangeStr = communicationChange >= 0 ? $"+{communicationChange}" : communicationChange.ToString();
            communicationText.text = $"Comm: {result.communicationAfter} ({communicationChangeStr})";
            
            // Color based on change
            if (result.communicationAfter < 20)
                communicationText.color = criticalColor;
            else if (communicationChange < 0)
                communicationText.color = Color.red;
            else if (communicationChange > 0)
                communicationText.color = Color.green;
            else
                communicationText.color = Color.white;
        }

        // Management Skill
        if (managementText != null)
        {
            int managementChange = result.managementAfter - result.managementBefore;
            string managementChangeStr = managementChange >= 0 ? $"+{managementChange}" : managementChange.ToString();
            managementText.text = $"Mgmt: {result.managementAfter} ({managementChangeStr})";
            
            // Color based on change
            if (result.managementAfter < 20)
                managementText.color = criticalColor;
            else if (managementChange < 0)
                managementText.color = Color.red;
            else if (managementChange > 0)
                managementText.color = Color.green;
            else
                managementText.color = Color.white;
        }
    }
}
