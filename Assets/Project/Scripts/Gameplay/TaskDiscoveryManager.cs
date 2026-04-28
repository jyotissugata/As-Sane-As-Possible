using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class TaskDiscoveryManager : MonoBehaviour
{
    public static TaskDiscoveryManager Instance { get; private set; }

    [Header("Task Discovery Settings")]
    public float randomTaskChance = 0.4f; // 40% chance per turn
    public int maxActiveTasks = 8;
    public int minTasksRequired = 3;

    [Header("Scheduled Tasks")]
    public List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();

    [Header("Task Templates")]
    public List<TaskData> taskTemplates = new List<TaskData>();

    [Header("Task Slot Prefab")]
    public GameObject taskSlotPrefab;
    public Transform taskSlotContainer;

    private List<TaskData> activeTasks = new List<TaskData>();
    private int currentTurn = 1;
    private DateTime currentDate;

    public static event Action<TaskData> OnTaskDiscovered;
    public static event Action<TaskData> OnTaskExpired;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //InitializeDefaultTasks();
        
        // Subscribe to calendar events
        if (CalendarManager.Instance != null)
        {
            CalendarManager.OnTurnChanged += OnTurnChanged;
        }

        ProcessScheduledTasks();
    }

    private void InitializeDefaultTasks()
    {
        // Create some initial tasks if templates are available
        if (taskTemplates.Count > 0 && GameManager.Instance?.availableTaskDatas != null)
        {
            for (int i = 0; i < minTasksRequired && i < taskTemplates.Count; i++)
            {
                CreateTaskFromTemplate(taskTemplates[i]);
            }
        }
    }

    private void OnTurnChanged(int turn, CalendarManager.TimeOfDay timeOfDay, DateTime date)
    {
        currentTurn = turn;
        currentDate = date;

        // Process random task discovery
        ProcessRandomTaskDiscovery();

        // Process scheduled tasks
        ProcessScheduledTasks();

        // Check for task expiry
        CheckTaskExpiry();
    }

    private void ProcessRandomTaskDiscovery()
    {
        if (UnityEngine.Random.value <= randomTaskChance)
        {
            // Try to discover a new task
            if (activeTasks.Count < maxActiveTasks && taskTemplates.Count > 0)
            {
                TaskData template = taskTemplates[UnityEngine.Random.Range(0, taskTemplates.Count)];
                CreateTaskFromTemplate(template);
            }
        }
    }

    private void ProcessScheduledTasks()
    {
        Debug.Log($"ProcessScheduledTasks: currentTurn={currentTurn}, scheduledTasks.Count={scheduledTasks.Count}");
        
        foreach (var scheduledTask in scheduledTasks)
        {
            if (!scheduledTask.hasBeenTriggered && ShouldTriggerScheduledTask(scheduledTask))
            {
                Debug.Log($"Triggering scheduled task: {scheduledTask.taskData?.taskName} at turn {currentTurn}");
                CreateTaskFromTemplate(scheduledTask.taskData);
                scheduledTask.hasBeenTriggered = true;
                Debug.Log($"Scheduled task triggered: {scheduledTask.taskData?.taskName}");
            }
        }
    }

    private bool ShouldTriggerScheduledTask(ScheduledTask scheduledTask)
    {
        // Only use TurnNumber trigger
        return currentTurn >= scheduledTask.triggerTurn;
    }

    private void CreateTaskFromTemplate(TaskData template)
    {
        if (template == null) return;

        // Find available task slot from GameServiceLocator
        TaskSlot availableSlot = GameServiceLocator.FindAvailableTaskSlot();
        if (availableSlot == null)
        {
            Debug.LogWarning("No available task slots for new task, creating a new one");
            availableSlot = CreateNewTaskSlot();
            if (availableSlot == null)
            {
                Debug.LogError("Failed to create new task slot");
                return;
            }
        }

        // Create a new TaskData instance from the template (to allow variations)
        TaskData newTask = ScriptableObject.CreateInstance<TaskData>();
        newTask.taskName = template.taskName;
        newTask.taskDescription = template.taskDescription;
        newTask.taskIcon = template.taskIcon;
        newTask.taskType = template.taskType;
        newTask.difficultyLevel = template.difficultyLevel;

        // Set requirements with some variation
        newTask.requiredTechnicalSkill = template.requiredTechnicalSkill + UnityEngine.Random.Range(-5, 6);
        newTask.requiredCreativeSkill = template.requiredCreativeSkill + UnityEngine.Random.Range(-5, 6);
        newTask.minimumSanity = template.minimumSanity;
        newTask.minimumCommunicationSkill = template.minimumCommunicationSkill;
        newTask.minimumManagementSkill = template.minimumManagementSkill;

        // Set turn-based properties with variation
        newTask.baseTurnDuration = template.baseTurnDuration + UnityEngine.Random.Range(-1, 2);
        newTask.maxTurnsBeforeExpiry = template.maxTurnsBeforeExpiry;
        newTask.sanityChangeOnComplete = template.sanityChangeOnComplete;
        newTask.experienceReward = template.experienceReward;
        newTask.moneyReward = template.moneyReward + UnityEngine.Random.Range(-10, 21); // ±10 variation

        // Assign template to slot (Task component will be created automatically)
        availableSlot.taskData = newTask;
        availableSlot.UpdateSlotDisplay();

        // Add to active tasks (track templates)
        activeTasks.Add(newTask);

        // Create Task component - it will auto-register with GameServiceLocator
        GameObject taskObj = new GameObject($"Task_{newTask.taskName}");
        Task taskComponent = taskObj.AddComponent<Task>();
        taskComponent.InitializeFromTemplate(newTask);

        Debug.Log($"New task discovered: {newTask.taskName}");
        OnTaskDiscovered?.Invoke(newTask);
    }

    private TaskSlot FindAvailableTaskSlot()
    {
        // Use GameServiceLocator to find available slot
        return GameServiceLocator.FindAvailableTaskSlot();
    }

    private TaskSlot CreateNewTaskSlot()
    {
        if (taskSlotPrefab == null)
        {
            Debug.LogError("TaskSlot prefab not assigned in TaskDiscoveryManager!");
            return null;
        }

        if (taskSlotContainer == null)
        {
            Debug.LogError("TaskSlotContainer not assigned in TaskDiscoveryManager!");
            return null;
        }

        GameObject slotObj = Instantiate(taskSlotPrefab, taskSlotContainer);
        TaskSlot slot = slotObj.GetComponent<TaskSlot>();
        
        if (slot == null)
        {
            Debug.LogError("TaskSlot prefab does not have TaskSlot component!");
            Destroy(slotObj);
            return null;
        }

        Debug.Log($"Created new TaskSlot: {slotObj.name}");
        return slot;
    }

    private void CheckTaskExpiry()
    {
        // Check task slots for expiry
        var taskSlots = GameServiceLocator.TaskSlots;
        foreach (var slot in taskSlots)
        {
            if (slot.Task != null && slot.Task.ShouldExpire())
            {
                slot.ExpireTask();
            }
        }
    }

    private void ExpireTask(Task task)
    {
        // Find and clear the slot using GameServiceLocator
        var taskSlots = GameServiceLocator.TaskSlots;
        TaskSlot taskSlot = taskSlots.FirstOrDefault(slot => slot.Task == task);
        if (taskSlot != null)
        {
            taskSlot.ExpireTask();
        }

        // Task will be destroyed and auto-unregister from GameServiceLocator
        Debug.LogWarning($"Task expired: {task.Template?.taskName}");
        OnTaskExpired?.Invoke(task.Template);
    }

    public void ForceDiscoverTask(TaskData template)
    {
        CreateTaskFromTemplate(template);
    }

    public List<TaskData> GetActiveTasks()
    {
        return new List<TaskData>(activeTasks);
    }

    public int GetActiveTaskCount()
    {
        return activeTasks.Count;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (CalendarManager.Instance != null)
        {
            CalendarManager.OnTurnChanged -= OnTurnChanged;
        }
    }
}

[Serializable]
public class ScheduledTask
{
    public string taskName;
    public TaskData taskData;
    public bool hasBeenTriggered = false;

    [Header("Turn Number Trigger")]
    public int triggerTurn = 5;
}
