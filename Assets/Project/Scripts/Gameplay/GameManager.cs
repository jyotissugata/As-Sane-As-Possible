using UnityEngine;

/// <summary>
/// Manages gameplay data and logic: employees, tasks, game results
/// Does not manage scenes or UI - that's handled by AppManager
/// Employee and Task components are now managed by GameServiceLocator
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Static Data")]
    public EmployeeData[] availableEmployeeDatas;
    public TaskData[] availableTaskDatas;

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
        // Gameplay data is initialized by AppManager when entering gameplay state
    }

    public void InitializeGameplayData()
    {
        // Reset employee states using GameServiceLocator
        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            employee.isAvailable = true;
            employee.currentTaskName = "";
            employee.currentSanity = Mathf.Clamp(employee.currentSanity, 50, 100); // Reset sanity to reasonable range
        }

        Debug.Log("Gameplay initialized with " + 
                 (employees.Count) + " employees and " + 
                 (GameServiceLocator.Tasks.Count) + " tasks");
    }

    public void CalculateGameResults()
    {
        // Calculate final game results
        int totalCompletedTasks = 0;
        float averageSanity = 0f;

        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            averageSanity += employee.currentSanity;
        }
        if (employees.Count > 0)
        {
            averageSanity /= employees.Count;
        }

        Debug.Log($"Game Results - Completed Tasks: {totalCompletedTasks}, Average Sanity: {averageSanity:F1}");
    }
}
