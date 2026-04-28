using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ScheduledTaskPopulator
{
    [MenuItem("ASAP/Create Empty Task Slots")]
    public static void CreateEmptyTaskSlots()
    {
        // Find TaskSlotContainer in the scene
        GameObject taskSlotContainer = GameObject.Find("TaskSlotContainer");
        if (taskSlotContainer == null)
        {
            Debug.LogError("TaskSlotContainer not found in the scene! Please create a GameObject named 'TaskSlotContainer' first.");
            return;
        }

        // Find TaskSlot prefab
        string[] prefabGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Project/Prefabs" });
        GameObject taskSlotPrefab = null;
        
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && prefab.GetComponent<TaskSlot>() != null)
            {
                taskSlotPrefab = prefab;
                break;
            }
        }

        if (taskSlotPrefab == null)
        {
            Debug.LogError("TaskSlot prefab not found! Please create a prefab with TaskSlot component.");
            return;
        }

        // Clear existing slots
        foreach (Transform child in taskSlotContainer.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // Create 8 empty task slots
        for (int i = 0; i < 8; i++)
        {
            GameObject slotObj = (GameObject)PrefabUtility.InstantiatePrefab(taskSlotPrefab, taskSlotContainer.transform);
            slotObj.name = $"TaskSlot_{i + 1}";
        }

        Debug.Log("Created 8 empty TaskSlot objects in TaskSlotContainer");
    }

    [MenuItem("ASAP/Populate Scheduled Tasks")]
    public static void PopulateScheduledTasks()
    {
        // Find TaskDiscoveryManager in the scene
        TaskDiscoveryManager taskDiscoveryManager = GameObject.FindObjectOfType<TaskDiscoveryManager>();
        if (taskDiscoveryManager == null)
        {
            Debug.LogError("TaskDiscoveryManager not found in the scene!");
            return;
        }

        // Check if there are available task slots
        var taskSlots = GameServiceLocator.TaskSlots;
        if (taskSlots == null || taskSlots.Count == 0)
        {
            Debug.LogError("No TaskSlot objects found in the scene! Please create empty TaskSlot objects first, or use 'ASAP/Create Empty Task Slots' menu item.");
            return;
        }

        // Clear existing scheduled tasks
        taskDiscoveryManager.scheduledTasks.Clear();

        // Load all TaskData assets
        string taskDataPath = "Assets/Project/Data/TaskData";
        string[] guids = AssetDatabase.FindAssets("t:TaskData", new[] { taskDataPath });
        
        if (guids.Length == 0)
        {
            Debug.LogError($"No TaskData assets found in {taskDataPath}. Please generate TaskData assets first!");
            return;
        }

        // Create a dictionary to map task names to TaskData
        var taskDataDict = new System.Collections.Generic.Dictionary<string, TaskData>();
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TaskData taskData = AssetDatabase.LoadAssetAtPath<TaskData>(assetPath);
            if (taskData != null)
            {
                taskDataDict[taskData.taskName] = taskData;
            }
        }

        // Populate scheduled tasks based on the design guide
        // Day 1 (Turns 1-3)
        AddScheduledTask(taskDiscoveryManager, "Fix Login Bug", 1, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Design Logo", 1, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Client Meeting", 2, taskDataDict);

        // Day 2 (Turns 4-6)
        AddScheduledTask(taskDiscoveryManager, "Database Optimization", 4, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "UI Redesign", 4, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Team Coordination", 5, taskDataDict);

        // Day 3 (Turns 7-9)
        AddScheduledTask(taskDiscoveryManager, "Mobile App Development", 7, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Website Redesign", 7, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Project Planning", 8, taskDataDict);

        // Day 4 (Turns 10-12)
        AddScheduledTask(taskDiscoveryManager, "Server Migration", 10, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Brand Identity", 10, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Stakeholder Presentation", 11, taskDataDict);

        // Day 5 (Turns 13-15)
        AddScheduledTask(taskDiscoveryManager, "API Integration", 13, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Marketing Campaign", 13, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Budget Review", 14, taskDataDict);

        // Day 6 (Turns 16-18)
        AddScheduledTask(taskDiscoveryManager, "E-commerce Platform", 16, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Product Launch", 16, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Team Training", 17, taskDataDict);

        // Day 7 (Turns 19-21)
        AddScheduledTask(taskDiscoveryManager, "Security Audit", 19, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "User Research", 19, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Resource Allocation", 20, taskDataDict);

        // Day 8 (Turns 22-24)
        AddScheduledTask(taskDiscoveryManager, "Performance Optimization", 22, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Feature Implementation", 22, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Client Negotiation", 23, taskDataDict);

        // Day 9 (Turns 25-27)
        AddScheduledTask(taskDiscoveryManager, "Critical Bug Fix", 25, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Major Feature", 25, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Executive Report", 26, taskDataDict);

        // Day 10 (Turns 28-30)
        AddScheduledTask(taskDiscoveryManager, "System Upgrade", 28, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Product Polish", 28, taskDataDict);
        AddScheduledTask(taskDiscoveryManager, "Final Review", 29, taskDataDict);

        // Mark scene as dirty to save changes
        EditorUtility.SetDirty(taskDiscoveryManager);
        
        Debug.Log($"Successfully populated {taskDiscoveryManager.scheduledTasks.Count} scheduled tasks!");
        Debug.Log($"TaskDiscoveryManager will spawn tasks at turns: {string.Join(", ", taskDiscoveryManager.scheduledTasks.Select(st => st.triggerTurn))}");
    }

    private static void AddScheduledTask(TaskDiscoveryManager manager, string taskName, int triggerTurn, System.Collections.Generic.Dictionary<string, TaskData> taskDataDict)
    {
        if (!taskDataDict.ContainsKey(taskName))
        {
            Debug.LogWarning($"TaskData not found: {taskName}. Skipping.");
            return;
        }

        ScheduledTask scheduledTask = new ScheduledTask
        {
            taskName = taskName,
            taskData = taskDataDict[taskName],
            triggerTurn = triggerTurn,
            hasBeenTriggered = false
        };

        manager.scheduledTasks.Add(scheduledTask);
        Debug.Log($"Added scheduled task: {taskName} at turn {triggerTurn}");
    }
}
