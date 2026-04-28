using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager Instance { get; private set; }

    [Header("Calendar Settings")]
    public int startYear = 2026;
    public int startMonth = 1;
    public int startDay = 1;
    
    [Header("Time System")]
    public int currentTurn = 1;
    public TimeOfDay currentTimeOfDay = TimeOfDay.Morning;
    
    [Header("UI References")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI timeOfDayText;
    public Button nextTurnButton;

    [Header("Turn Settings")]
    public int maxTurnsPerDay = 3; // Morning, Afternoon, Evening
    
    public enum TimeOfDay
    {
        Morning,
        Afternoon,
        Evening
    }

    private DateTime currentDate;
    public DateTime CurrentDate => currentDate;
    private List<TaskData> activeTasks = new List<TaskData>();

    public static event Action<int, TimeOfDay, DateTime> OnTurnChanged;
    public static event Action<int> OnDayChanged;

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
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(NextTurn);
        }
    }

    private void Start()
    {
        InitializeCalendar();
        UpdateUI();
    }

    private void InitializeCalendar()
    {
        currentDate = new DateTime(startYear, startMonth, startDay);
        currentTurn = 1;
        currentTimeOfDay = TimeOfDay.Morning;
    }

    public void NextTurn()
    {
        // Progress time of day
        currentTimeOfDay++;
        
        if ((int)currentTimeOfDay >= maxTurnsPerDay)
        {
            // Move to next day
            currentTimeOfDay = TimeOfDay.Morning;
            currentDate = currentDate.AddDays(1);
            OnDayChanged?.Invoke(currentTurn);
        }

        currentTurn++;
        
        Debug.Log($"Turn {currentTurn}: {GetTimeString()} - {currentDate:dd MMM yyyy}");
        
        // Process turn effects
        ProcessTurnEffects();
        
        // Update UI
        UpdateUI();
        
        // Trigger events
        OnTurnChanged?.Invoke(currentTurn, currentTimeOfDay, currentDate);
        
        // Check for task expiry
        CheckTaskExpiry();
    }

    private void ProcessTurnEffects()
    {
        // Apply turn-based effects to employees
        ApplyEmployeeTurnEffects();
        
        // Process rest area recovery
        ProcessRestAreaTurn();
        
        // Process task progress
        ProcessTaskProgress();
        
        // ResultPanel only shows on game end via EconomyManager.OnGameEnded event
        // Do not show turn results during normal gameplay
    }

    private void ApplyEmployeeTurnEffects()
    {
        // Use GameServiceLocator to get all employees
        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            if (employee == null) continue;

            employee.ProcessTurn();
        }
    }

    private void ProcessRestAreaTurn()
    {
        // Use efficient Service Locator instead of FindObjectsOfType
        var restAreaSlots = GameServiceLocator.RestAreaSlots;
        
        foreach (var restArea in restAreaSlots)
        {
            restArea.ProcessTurn();
        }
    }

    private void ProcessTaskProgress()
    {
        // Use efficient Service Locator instead of FindObjectsOfType
        var taskSlots = GameServiceLocator.TaskSlots;
        
        foreach (var slot in taskSlots)
        {
            if (slot.IsOccupied && slot.CurrentEmployee != null)
            {
                slot.ProcessTurnProgress();
            }
        }
    }

    private void CheckTaskExpiry()
    {
        var taskSlots = GameServiceLocator.TaskSlots;
        
        foreach (var slot in taskSlots)
        {
            if (slot.Task != null && slot.Task.ShouldExpire())
            {
                Debug.LogWarning($"Task '{slot.Task.Template?.taskName}' has expired!");
                slot.ExpireTask();
            }
        }
    }

    private void UpdateUI()
    {
        if (dateText != null)
            dateText.text = currentDate.ToString("dd MMMM yyyy");
        
        if (turnText != null)
            turnText.text = $"Turn: {currentTurn}";
        
        if (timeOfDayText != null)
            timeOfDayText.text = GetTimeString();
    }

    public string GetTimeString()
    {
        return currentTimeOfDay switch
        {
            TimeOfDay.Morning => "Morning",
            TimeOfDay.Afternoon => "Afternoon", 
            TimeOfDay.Evening => "Evening",
            _ => "Unknown"
        };
    }

    public string GetFullTimeString()
    {
        return $"Turn {currentTurn} - {GetTimeString()} - {currentDate:dd MMM yyyy}";
    }

    public int GetCurrentDay()
    {
        return currentDate.Day;
    }

    public int GetCurrentMonth()
    {
        return currentDate.Month;
    }

    public int GetCurrentYear()
    {
        return currentDate.Year;
    }

    public void AddActiveTask(TaskData task)
    {
        if (task != null && !activeTasks.Contains(task))
        {
            activeTasks.Add(task);
        }
    }

    public void RemoveActiveTask(TaskData task)
    {
        if (task != null && activeTasks.Contains(task))
        {
            activeTasks.Remove(task);
        }
    }

    public List<TaskData> GetActiveTasks()
    {
        return new List<TaskData>(activeTasks);
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        OnTurnChanged = null;
        OnDayChanged = null;
    }
}
