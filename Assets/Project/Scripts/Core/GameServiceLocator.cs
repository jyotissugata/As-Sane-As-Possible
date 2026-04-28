using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Efficient service locator pattern to avoid FindObjectsOfType calls
/// Components register themselves and can be accessed efficiently
/// </summary>
public class GameServiceLocator : MonoBehaviour
{
    public static GameServiceLocator Instance { get; private set; }

    [Header("Service References")]
    [SerializeField] private RestAreaSlot[] restAreaSlots;
    [SerializeField] private TaskSlot[] taskSlots;
    [SerializeField] private DraggableCard[] employeeCards;

    [Header("Prefab References")]
    [SerializeField] private GameObject employeeCardPrefab;
    [SerializeField] private GameObject taskSlotPrefab;
    [SerializeField] private Transform employeeCardContainer;
    [SerializeField] private Transform taskSlotContainer;
    [SerializeField] private Canvas mainCanvas;

    // Static registries for efficient access
    private static readonly List<RestAreaSlot> registeredRestAreas = new List<RestAreaSlot>();
    private static readonly List<TaskSlot> registeredTaskSlots = new List<TaskSlot>();
    private static readonly List<DraggableCard> registeredEmployeeCards = new List<DraggableCard>();
    private static readonly List<Employee> registeredEmployees = new List<Employee>();
    private static readonly List<Task> registeredTasks = new List<Task>();

    public static IReadOnlyList<RestAreaSlot> RestAreaSlots => registeredRestAreas;
    public static IReadOnlyList<TaskSlot> TaskSlots => registeredTaskSlots;
    public static IReadOnlyList<DraggableCard> EmployeeCards => registeredEmployeeCards;
    public static IReadOnlyList<Employee> Employees => registeredEmployees;
    public static IReadOnlyList<Task> Tasks => registeredTasks;

    public static event Action<RestAreaSlot> OnRestAreaRegistered;
    public static event Action<TaskSlot> OnTaskSlotRegistered;
    public static event Action<DraggableCard> OnEmployeeCardRegistered;
    public static event Action<Employee> OnEmployeeRegistered;
    public static event Action<Task> OnTaskRegistered;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeServices()
    {
        // Register manually assigned services
        RegisterManualServices();

        // Find and register any existing services
        FindAndRegisterExistingServices();
    }

    private void RegisterManualServices()
    {
        if (restAreaSlots != null)
        {
            foreach (var slot in restAreaSlots)
            {
                if (slot != null)
                {
                    RegisterRestArea(slot);
                }
            }
        }

        if (taskSlots != null)
        {
            foreach (var slot in taskSlots)
            {
                if (slot != null)
                {
                    RegisterTaskSlot(slot);
                }
            }
        }

        if (employeeCards != null)
        {
            foreach (var card in employeeCards)
            {
                if (card != null)
                {
                    RegisterEmployeeCard(card);
                }
            }
        }
    }

    private void FindAndRegisterExistingServices()
    {
        // Only use FindObjectsOfType once during initialization
        var existingRestAreas = FindObjectsOfType<RestAreaSlot>();
        foreach (var slot in existingRestAreas)
        {
            if (!registeredRestAreas.Contains(slot))
            {
                RegisterRestArea(slot);
            }
        }

        var existingTaskSlots = FindObjectsOfType<TaskSlot>();
        foreach (var slot in existingTaskSlots)
        {
            if (!registeredTaskSlots.Contains(slot))
            {
                RegisterTaskSlot(slot);
            }
        }

        var existingEmployeeCards = FindObjectsOfType<DraggableCard>();
        foreach (var card in existingEmployeeCards)
        {
            if (!registeredEmployeeCards.Contains(card))
            {
                RegisterEmployeeCard(card);
            }
        }

        var existingEmployees = FindObjectsOfType<Employee>();
        foreach (var employee in existingEmployees)
        {
            if (!registeredEmployees.Contains(employee))
            {
                RegisterEmployee(employee);
            }
        }

        var existingTasks = FindObjectsOfType<Task>();
        foreach (var task in existingTasks)
        {
            if (!registeredTasks.Contains(task))
            {
                RegisterTask(task);
            }
        }
    }

    // Static registration methods - components call these in Awake/Start
    public static void RegisterRestArea(RestAreaSlot restArea)
    {
        if (restArea != null && !registeredRestAreas.Contains(restArea))
        {
            registeredRestAreas.Add(restArea);
            OnRestAreaRegistered?.Invoke(restArea);
        }
    }

    public static void RegisterTaskSlot(TaskSlot taskSlot)
    {
        if (taskSlot != null && !registeredTaskSlots.Contains(taskSlot))
        {
            registeredTaskSlots.Add(taskSlot);
            OnTaskSlotRegistered?.Invoke(taskSlot);
        }
    }

    public static void RegisterEmployeeCard(DraggableCard employeeCard)
    {
        if (employeeCard != null && !registeredEmployeeCards.Contains(employeeCard))
        {
            registeredEmployeeCards.Add(employeeCard);
            OnEmployeeCardRegistered?.Invoke(employeeCard);
        }
    }

    public static void RegisterEmployee(Employee employee)
    {
        if (employee != null && !registeredEmployees.Contains(employee))
        {
            registeredEmployees.Add(employee);
            OnEmployeeRegistered?.Invoke(employee);
        }
    }

    public static void RegisterTask(Task task)
    {
        if (task != null && !registeredTasks.Contains(task))
        {
            registeredTasks.Add(task);
            OnTaskRegistered?.Invoke(task);
        }
    }

    // Static unregistration methods - components call these in OnDestroy
    public static void UnregisterRestArea(RestAreaSlot restArea)
    {
        if (restArea != null && registeredRestAreas.Contains(restArea))
        {
            registeredRestAreas.Remove(restArea);
        }
    }

    public static void UnregisterTaskSlot(TaskSlot taskSlot)
    {
        if (taskSlot != null && registeredTaskSlots.Contains(taskSlot))
        {
            registeredTaskSlots.Remove(taskSlot);
        }
    }

    public static void UnregisterEmployeeCard(DraggableCard employeeCard)
    {
        if (employeeCard != null && registeredEmployeeCards.Contains(employeeCard))
        {
            registeredEmployeeCards.Remove(employeeCard);
        }
    }

    public static void UnregisterEmployee(Employee employee)
    {
        if (employee != null && registeredEmployees.Contains(employee))
        {
            registeredEmployees.Remove(employee);
        }
    }

    public static void UnregisterTask(Task task)
    {
        if (task != null && registeredTasks.Contains(task))
        {
            registeredTasks.Remove(task);
        }
    }

    // Utility methods for efficient access
    public static RestAreaSlot GetRestArea(int index = 0)
    {
        return index < registeredRestAreas.Count ? registeredRestAreas[index] : null;
    }

    public static TaskSlot GetTaskSlot(int index)
    {
        return index < registeredTaskSlots.Count ? registeredTaskSlots[index] : null;
    }

    public static DraggableCard GetEmployeeCard(int index)
    {
        return index < registeredEmployeeCards.Count ? registeredEmployeeCards[index] : null;
    }

    // Find methods that use the registry instead of FindObjectsOfType
    public static RestAreaSlot FindRestAreaWithEmployee(EmployeeData employee)
    {
        foreach (var restArea in registeredRestAreas)
        {
            if (restArea.RestingEmployees != null)
            {
                foreach (var restingEmployee in restArea.RestingEmployees)
                {
                    if (restingEmployee?.EmployeeData == employee)
                    {
                        return restArea;
                    }
                }
            }
        }
        return null;
    }

    public static TaskSlot FindTaskSlotWithEmployee(EmployeeData employee)
    {
        foreach (var taskSlot in registeredTaskSlots)
        {
            if (taskSlot.CurrentEmployee?.EmployeeData == employee)
            {
                return taskSlot;
            }
        }
        return null;
    }

    public static TaskSlot FindAvailableTaskSlot()
    {
        foreach (var taskSlot in registeredTaskSlots)
        {
            if (taskSlot.TaskData == null)
            {
                return taskSlot;
            }
        }
        return null;
    }

    private void OnDestroy()
    {
        // Clear all registries
        registeredRestAreas.Clear();
        registeredTaskSlots.Clear();
        registeredEmployeeCards.Clear();
        registeredEmployees.Clear();
        registeredTasks.Clear();
    }

    // UI Creation Methods
    public void CreateEmployeeCards()
    {
        if (GameManager.Instance == null || GameManager.Instance.availableEmployeeDatas == null)
        {
            Debug.LogWarning("GameManager or available employee datas not found!");
            return;
        }

        if (employeeCardPrefab == null || employeeCardContainer == null || mainCanvas == null)
        {
            Debug.LogWarning("Employee card prefab, container, or canvas not assigned!");
            return;
        }

        // Clear existing cards
        foreach (Transform child in employeeCardContainer)
        {
            Destroy(child.gameObject);
        }

        // Create cards for each employee data
        foreach (var employeeData in GameManager.Instance.availableEmployeeDatas)
        {
            if (employeeData == null) continue;

            GameObject cardObj = Instantiate(employeeCardPrefab, employeeCardContainer);
            DraggableCard card = cardObj.GetComponent<DraggableCard>();
            
            if (card != null)
            {
                card.canvas = mainCanvas;
                card.InitializeCard(employeeData);
            }
        }

        Debug.Log($"Created employee cards from {GameManager.Instance.availableEmployeeDatas.Length} employee datas");
    }
}
