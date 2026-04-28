using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image taskIcon;
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI taskDescriptionText;
    public TextMeshProUGUI requirementsText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI turnsText;
    public Image statusImage;

    [Header("Status Colors")]
    public Color availableColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color expiredColor = Color.red;

    private Task task;

    public void Setup(Task taskComponent)
    {
        task = taskComponent;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (task == null || task.Template == null) return;

        if (taskNameText != null)
            taskNameText.text = task.Template.taskName;

        if (taskDescriptionText != null)
            taskDescriptionText.text = task.Template.taskDescription;

        if (taskIcon != null && task.Template.taskIcon != null)
            taskIcon.sprite = task.Template.taskIcon;

        if (requirementsText != null)
        {
            requirementsText.text = $"Tech: {task.Template.requiredTechnicalSkill} | " +
                                  $"Creative: {task.Template.requiredCreativeSkill} | " +
                                  $"Comm: {task.Template.minimumCommunicationSkill} | " +
                                  $"Mgmt: {task.Template.minimumManagementSkill}";
        }

        if (rewardText != null)
        {
            rewardText.text = $"${task.Template.moneyReward} | {task.Template.experienceReward} XP";
        }

        if (turnsText != null)
        {
            turnsText.text = $"Turns: {task.TurnsRemaining} | Expires: {task.TurnsUntilExpiry}";
        }

        UpdateStatusColor();
    }

    private void UpdateStatusColor()
    {
        if (statusImage == null) return;

        if (task.IsExpired)
        {
            statusImage.color = expiredColor;
        }
        else if (task.TurnsUntilExpiry <= 2)
        {
            statusImage.color = warningColor;
        }
        else
        {
            statusImage.color = availableColor;
        }
    }

    public void Refresh()
    {
        UpdateDisplay();
    }
}
