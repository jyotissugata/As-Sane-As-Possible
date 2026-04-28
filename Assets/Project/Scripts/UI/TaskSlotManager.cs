using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TaskSlotManager : MonoBehaviour
{
    [Header("Manager Settings")]
    public float globalSanityDrainMultiplier = 1f;
    public int criticalSanityThreshold = 20;
    public bool enableAutoRemoveOnCriticalSanity = true;

    [Header("UI References")]
    public TextMeshProUGUI totalEmployeesText;
    public TextMeshProUGUI averageSanityText;
    public TextMeshProUGUI criticalEmployeesText;

    private List<TaskSlot> allSlots = new List<TaskSlot>();
    private List<Employee> allEmployees = new List<Employee>();

    public static TaskSlotManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeSlots();
        InitializeEmployees();
        UpdateUI();
    }

    private void InitializeSlots()
    {
        TaskSlot[] slots = FindObjectsOfType<TaskSlot>();
        allSlots = slots.ToList();
        
        Debug.Log($"Found {allSlots.Count} task slots");
    }

    private void InitializeEmployees()
    {
        // Use GameServiceLocator to get employees
        allEmployees = GameServiceLocator.Employees.ToList();
    }

    public void OnEmployeeAssigned(TaskSlot slot, Employee employee)
    {
        Debug.Log($"Manager: {employee.Template?.employeeName} assigned to {slot.TaskData.taskName}");
        UpdateUI();
        CheckCriticalSanity();
    }

    public void OnEmployeeRemoved(TaskSlot slot)
    {
        Debug.Log($"Manager: Employee removed from {slot.TaskData.taskName}");
        UpdateUI();
    }

    public void OnEmployeeSanityChanged(TaskSlot slot, int newSanity)
    {
        UpdateUI();
        
        if (newSanity <= criticalSanityThreshold)
        {
            HandleCriticalSanity(slot, newSanity);
        }
    }

    private void HandleCriticalSanity(TaskSlot slot, int sanity)
    {
        EmployeeCard empCard = slot.CurrentEmployee as EmployeeCard;
        Employee employee = empCard?.Employee;
        if (employee == null) return;

        Debug.LogWarning($"CRITICAL: {employee.Template?.employeeName} sanity is critically low at {sanity}!");

        if (enableAutoRemoveOnCriticalSanity)
        {
            Debug.Log($"Auto-removing {employee.Template?.employeeName} from task due to critical sanity");
            slot.RemoveEmployee();
        }
    }

    private void CheckCriticalSanity()
    {
        foreach (var slot in allSlots)
        {
            if (slot.CurrentEmployee != null)
            {
                EmployeeCard empCard = slot.CurrentEmployee as EmployeeCard;
                Employee employee = empCard?.Employee;
                if (employee != null && employee.CurrentSanity <= criticalSanityThreshold)
                {
                    HandleCriticalSanity(slot, employee.CurrentSanity);
                }
            }
        }
    }

    public float GetAverageSanity()
    {
        if (allEmployees.Count == 0) return 0f;

        float totalSanity = 0f;
        int employeeCount = 0;

        foreach (var employee in allEmployees)
        {
            if (employee != null)
            {
                totalSanity += employee.CurrentSanity;
                employeeCount++;
            }
        }

        return employeeCount > 0 ? totalSanity / employeeCount : 0f;
    }

    public int GetCriticalEmployeeCount()
    {
        return allEmployees.Count(e => e != null && e.CurrentSanity <= criticalSanityThreshold);
    }

    public int GetOccupiedSlotCount()
    {
        return allSlots.Count(s => s.IsOccupied);
    }

    public int GetAvailableEmployeeCount()
    {
        return allEmployees.Count(e => e != null && e.isAvailable);
    }

    public void UpdateGlobalSanityDrainMultiplier(float multiplier)
    {
        globalSanityDrainMultiplier = multiplier;
        
        // Update all slots with new multiplier
        foreach (var slot in allSlots)
        {
            // You could add a method in TaskSlot to update drain amount
            // slot.UpdateSanityDrainAmount(sanityDrainAmount * multiplier);
        }
    }

    public void ForceRemoveAllEmployees()
    {
        foreach (var slot in allSlots)
        {
            if (slot.IsOccupied)
            {
                slot.RemoveEmployee();
            }
        }
    }

    public void RestoreAllEmployeeSanity(int amount)
    {
        foreach (var employee in allEmployees)
        {
            if (employee != null)
            {
                employee.UpdateSanity(amount);
            }
        }

        // Update all employee cards
        var employeeCards = GameServiceLocator.EmployeeCards;
        foreach (var card in employeeCards)
        {
            if (card is EmployeeCard empCard)
            {
                empCard.UpdateSanityDisplay();
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (totalEmployeesText != null)
        {
            totalEmployeesText.text = $"Total Employees: {allEmployees.Count}";
        }

        if (averageSanityText != null)
        {
            float avgSanity = GetAverageSanity();
            averageSanityText.text = $"Average Sanity: {avgSanity:F1}";
            
            // Change color based on average sanity
            if (avgSanity < 30)
                averageSanityText.color = Color.red;
            else if (avgSanity < 60)
                averageSanityText.color = Color.yellow;
            else
                averageSanityText.color = Color.green;
        }

        if (criticalEmployeesText != null)
        {
            int criticalCount = GetCriticalEmployeeCount();
            criticalEmployeesText.text = $"Critical: {criticalCount}";
            
            // Change color if there are critical employees
            criticalEmployeesText.color = criticalCount > 0 ? Color.red : Color.white;
        }
    }

    public TaskSlot FindBestSlotForEmployee(Employee employee)
    {
        if (employee == null) return null;

        TaskSlot bestSlot = null;
        float bestScore = -1f;

        foreach (var slot in allSlots)
        {
            if (!slot.IsOccupied && slot.TaskData != null && employee.Template != null)
            {
                // Calculate a score based on how well the employee matches the task
                float score = CalculateEmployeeTaskMatch(employee.Template, slot.TaskData);
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestSlot = slot;
                }
            }
        }

        return bestSlot;
    }

    private float CalculateEmployeeTaskMatch(EmployeeData employee, TaskData task)
    {
        if (task == null || employee == null) return 0f;

        float score = 0f;
        
        // Technical skill matching
        if (task.taskType == TaskData.TaskType.Technical || task.taskType == TaskData.TaskType.Mixed)
        {
            score += (employee.technicalSkill - task.requiredTechnicalSkill) * 0.5f;
        }
        
        // Creative skill matching
        if (task.taskType == TaskData.TaskType.Creative || task.taskType == TaskData.TaskType.Mixed)
        {
            score += (employee.creativeSkill - task.requiredCreativeSkill) * 0.5f;
        }
        
        // Sanity bonus
        score += employee.sanity * 0.1f;
        
        return score;
    }

    private void Update()
    {
        // Update UI periodically (you might want to optimize this)
        if (Time.frameCount % 60 == 0) // Every 60 frames
        {
            UpdateUI();
        }
    }
}
