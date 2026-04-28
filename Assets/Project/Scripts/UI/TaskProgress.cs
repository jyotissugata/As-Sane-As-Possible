using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TaskProgress : MonoBehaviour
{
    [Header("Progress UI Components")]
    public Image progressBarFill;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI timeRemainingText;
    public GameObject progressPanel;

    [Header("Progress Settings")]
    public float progressUpdateInterval = 0.1f; // Update every 0.1 seconds

    private TaskSlot taskSlot;
    private Employee assignedEmployee;
    private Task task;
    private float currentProgress = 0f;
    private float totalWorkRequired = 100f;
    private bool isWorking = false;
    private Coroutine progressCoroutine;
    private float workSpeed = 1f;

    public float CurrentProgress => currentProgress;
    public bool IsCompleted => currentProgress >= totalWorkRequired;
    public bool IsWorking => isWorking;

    private void Awake()
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    public void InitializeProgress(TaskSlot slot, Employee employee, Task taskComponent)
    {
        taskSlot = slot;
        assignedEmployee = employee;
        task = taskComponent;
        
        ResetProgress();
        CalculateWorkSpeed();
        StartWork();
    }

    private void CalculateWorkSpeed()
    {
        if (assignedEmployee == null || task == null || task.Template == null)
        {
            workSpeed = 1f;
            return;
        }

        // Base speed calculation
        workSpeed = 1f;

        // Skill-based speed bonus
        float technicalBonus = 0f;
        float creativeBonus = 0f;

        if (task.Template.taskType == TaskData.TaskType.Technical || task.Template.taskType == TaskData.TaskType.Mixed)
        {
            technicalBonus = (assignedEmployee.Template.technicalSkill - task.Template.requiredTechnicalSkill) * 0.02f; // 2% per skill point above requirement
        }

        if (task.Template.taskType == TaskData.TaskType.Creative || task.Template.taskType == TaskData.TaskType.Mixed)
        {
            creativeBonus = (assignedEmployee.Template.creativeSkill - task.Template.requiredCreativeSkill) * 0.02f; // 2% per skill point above requirement
        }

        workSpeed += technicalBonus + creativeBonus;

        // Sanity-Performance Scaling
        float sanityMultiplier = 1f;
        if (assignedEmployee.CurrentSanity <= 0)
        {
            sanityMultiplier = 0f; // Cannot work
        }
        else if (assignedEmployee.CurrentSanity < 30)
        {
            sanityMultiplier = 0.5f; // 50% speed reduction
        }
        else if (assignedEmployee.CurrentSanity < 60)
        {
            sanityMultiplier = 0.8f; // 20% speed reduction
        }

        workSpeed *= sanityMultiplier;

        // Minimum speed cap
        workSpeed = Mathf.Max(workSpeed, 0.1f);

        Debug.Log($"{assignedEmployee.Template?.employeeName} work speed: {workSpeed:F2}x (Technical: +{technicalBonus:F2}, Creative: +{creativeBonus:F2}, Sanity: x{sanityMultiplier:F2})");
    }

    public void StartWork()
    {
        if (isWorking || assignedEmployee == null || task == null) return;

        if (assignedEmployee.CurrentSanity <= 0)
        {
            Debug.LogWarning($"{assignedEmployee.Template?.employeeName} cannot work - sanity is 0!");
            MoveToRestArea();
            return;
        }

        isWorking = true;
        
        if (progressPanel != null)
            progressPanel.SetActive(true);

        if (progressCoroutine != null)
            StopCoroutine(progressCoroutine);

        progressCoroutine = StartCoroutine(WorkCoroutine());
    }

    public void PauseWork()
    {
        isWorking = false;
        
        if (progressCoroutine != null)
        {
            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }
    }

    public void ResumeWork()
    {
        if (assignedEmployee != null && assignedEmployee.CurrentSanity > 0)
        {
            CalculateWorkSpeed(); // Recalculate speed in case stats changed
            StartWork();
        }
    }

    private IEnumerator WorkCoroutine()
    {
        while (isWorking && currentProgress < totalWorkRequired)
        {
            // Check if employee can still work
            if (assignedEmployee == null || assignedEmployee.CurrentSanity <= 0)
            {
                Debug.Log($"{assignedEmployee?.Template?.employeeName ?? "Employee"} stopped working - sanity depleted");
                MoveToRestArea();
                yield break;
            }

            // Calculate work done this frame
            float workDone = workSpeed * progressUpdateInterval;
            currentProgress += workDone;

            // Update UI
            UpdateProgressUI();

            yield return new WaitForSeconds(progressUpdateInterval);
        }

        // Task completed
        if (currentProgress >= totalWorkRequired)
        {
            CompleteTask();
        }
    }

    private void UpdateProgressUI()
    {
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = currentProgress / totalWorkRequired;
        }

        if (progressText != null)
        {
            progressText.text = $"{Mathf.FloorToInt(currentProgress)}%";
        }

        if (timeRemainingText != null && workSpeed > 0)
        {
            float remainingWork = totalWorkRequired - currentProgress;
            float timeRemaining = remainingWork / workSpeed;
            timeRemainingText.text = $"Time: {timeRemaining:F1}s";
        }
    }

    private void CompleteTask()
    {
        PauseWork();
        currentProgress = totalWorkRequired;
        UpdateProgressUI();

        Debug.Log($"Task '{task?.Template?.taskName}' completed by {assignedEmployee?.Template?.employeeName}!");

        // Apply task completion effects through Task component
        if (task != null)
        {
            task.CompleteTask();
        }

        // Complete task through slot
        if (taskSlot != null)
        {
            taskSlot.CompleteTask();
        }

        // Hide progress panel after a delay
        StartCoroutine(HideProgressPanelDelayed());
    }

    private IEnumerator HideProgressPanelDelayed()
    {
        yield return new WaitForSeconds(2f);
        
        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    private void MoveToRestArea()
    {
        PauseWork();
        
        if (taskSlot != null)
        {
            taskSlot.MoveToRestArea();
        }
        
        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    public void ResetProgress()
    {
        PauseWork();
        currentProgress = 0f;
        UpdateProgressUI();
        
        if (progressPanel != null)
            progressPanel.SetActive(false);
    }

    public void UpdateWorkSpeed()
    {
        CalculateWorkSpeed();
    }

    private void OnDestroy()
    {
        PauseWork();
    }
}
