using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ResultPanel : MonoBehaviour
{
    public static ResultPanel Instance { get; private set; }

    [Header("Panel Components")]
    public GameObject resultPanel;
    public TextMeshProUGUI turnInfoText;
    public TextMeshProUGUI dateInfoText;
    public Transform taskResultsContainer;
    public Transform employeeResultsContainer;

    [Header("Prefabs")]
    public GameObject taskResultItemPrefab;
    public GameObject employeeResultItemPrefab;

    [Header("Buttons")]
    public Button continueButton;
    public Button detailedViewButton;

    private List<TaskResult> taskResults = new List<TaskResult>();
    private List<EmployeeResult> employeeResults = new List<EmployeeResult>();

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
        if (resultPanel != null)
            resultPanel.SetActive(false);

        SetupButtons();
        
        // Subscribe to game end event
        if (EconomyManager.Instance != null)
        {
            EconomyManager.OnGameEnded += OnGameEnded;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from game end event
        if (EconomyManager.Instance != null)
        {
            EconomyManager.OnGameEnded -= OnGameEnded;
        }
    }

    private void OnGameEnded(GameResult result)
    {
        Debug.Log($"ResultPanel received game end event: {result}");
        
        // Collect and display final results
        CollectResults();
        DisplayResults();
        
        // Update leadership title
        if (LeadershipManager.Instance != null)
        {
            LeadershipManager.Instance.UpdateLeadershipTitle();
        }
        
        // Update turn info with game result
        if (turnInfoText != null)
        {
            string resultText = result switch
            {
                GameResult.Victory => "VICTORY!",
                GameResult.Bankruptcy => "BANKRUPTCY!",
                GameResult.TimeOut => "TIME'S UP!",
                GameResult.Quit => "GAME QUIT",
                _ => "GAME OVER"
            };
            turnInfoText.text = resultText;
        }
        
        // Show the result panel
        if (resultPanel != null)
            resultPanel.SetActive(true);
    }

    private void SetupButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ReturnToMainMenu);
        }

        if (detailedViewButton != null)
        {
            detailedViewButton.onClick.AddListener(ShowDetailedView);
        }
    }

    public void ShowTurnResults()
    {
        CollectResults();
        DisplayResults();
        
        if (resultPanel != null)
            resultPanel.SetActive(true);
    }

    private void CollectResults()
    {
        taskResults.Clear();
        employeeResults.Clear();

        // Collect task results using Service Locator
        var taskSlots = GameServiceLocator.TaskSlots;
        foreach (var slot in taskSlots)
        {
            if (slot.Task != null)
            {
                TaskResult result = new TaskResult
                {
                    taskName = slot.Task.Template?.taskName ?? "Unknown",
                    employeeName = slot.Task.AssignedEmployee?.Template?.employeeName ?? "Unassigned",
                    progressBefore = slot.Task.ProgressPercentage,
                    turnsRemaining = slot.Task.TurnsRemaining,
                    turnsUntilExpiry = slot.Task.TurnsUntilExpiry,
                    isCompleted = slot.Task.IsCompleted,
                    isExpired = slot.Task.IsExpired,
                    wasAssigned = slot.IsOccupied
                };

                if (slot.IsOccupied && slot.CurrentEmployee != null)
                {
                    result.sanityChange = -2; // Base sanity cost per turn
                    result.communicationChange = -1; // Base communication cost per turn
                    result.managementChange = -1;    // Base management cost per turn
                }

                taskResults.Add(result);
            }
        }

        // Collect employee results
        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            if (employee == null) continue;

                EmployeeResult result = new EmployeeResult
                {
                    employeeName = employee.Template?.employeeName ?? "Unknown",
                    sanityBefore = employee.currentSanity,
                    communicationBefore = employee.currentCommunicationSkill,
                    managementBefore = employee.currentManagementSkill,
                    isWorking = !employee.isAvailable,
                    currentTask = employee.currentTaskName,
                    isResting = RestArea.Instance?.IsEmployeeResting(employee) ?? false
                };

                // Calculate changes (these would be applied during the turn)
                if (result.isWorking)
                {
                    result.sanityAfter = result.sanityBefore - 2;
                    result.communicationAfter = result.communicationBefore - 1;
                    result.managementAfter = result.managementBefore - 1;
                }
                else if (result.isResting)
                {
                    result.sanityAfter = Mathf.Min(100, result.sanityBefore + 5);
                    result.communicationAfter = Mathf.Min(100, result.communicationBefore + 3);
                    result.managementAfter = Mathf.Min(100, result.managementBefore + 2);
                }
                else
                {
                    // Resting but not in rest area
                    result.sanityAfter = Mathf.Min(100, result.sanityBefore + 2);
                    result.communicationAfter = Mathf.Min(100, result.communicationBefore + 2);
                    result.managementAfter = result.managementBefore; // No change
                }

                employeeResults.Add(result);
            
        }
    }

    private void DisplayResults()
    {
        // Update header info
        if (turnInfoText != null && CalendarManager.Instance != null)
        {
            turnInfoText.text = $"Turn {CalendarManager.Instance.currentTurn} - {CalendarManager.Instance.GetTimeString()}";
        }

        if (dateInfoText != null && CalendarManager.Instance != null)
        {
            dateInfoText.text = CalendarManager.Instance.CurrentDate.ToString("dd MMMM yyyy");
        }

        // Update leadership feedback
        if (LeadershipManager.Instance != null)
        {
            LeadershipManager.Instance.UpdateTurnFeedback();
        }

        // Clear previous results
        ClearResultsDisplay();

        // Display task results
        DisplayTaskResults();

        // Display employee results
        DisplayEmployeeResults();
    }

    private void ClearResultsDisplay()
    {
        if (taskResultsContainer != null)
        {
            foreach (Transform child in taskResultsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        if (employeeResultsContainer != null)
        {
            foreach (Transform child in employeeResultsContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void DisplayTaskResults()
    {
        if (taskResultsContainer == null || taskResultItemPrefab == null) return;

        foreach (var result in taskResults)
        {
            GameObject item = Instantiate(taskResultItemPrefab, taskResultsContainer);
            TaskResultItem resultItem = item.GetComponent<TaskResultItem>();
            
            if (resultItem != null)
            {
                resultItem.Setup(result);
            }
        }
    }

    private void DisplayEmployeeResults()
    {
        if (employeeResultsContainer == null || employeeResultItemPrefab == null) return;

        foreach (var result in employeeResults)
        {
            GameObject item = Instantiate(employeeResultItemPrefab, employeeResultsContainer);
            EmployeeResultItem resultItem = item.GetComponent<EmployeeResultItem>();
            
            if (resultItem != null)
            {
                resultItem.Setup(result);
            }
        }
    }

    public void HidePanel()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void ShowDetailedView()
    {
        // Could expand to show more detailed statistics
        Debug.Log("Detailed view requested");
    }

    [System.Serializable]
    public class TaskResult
    {
        public string taskName;
        public string employeeName;
        public float progressBefore;
        public int turnsRemaining;
        public int turnsUntilExpiry;
        public bool isCompleted;
        public bool isExpired;
        public bool wasAssigned;
        public int sanityChange;
        public int communicationChange;
        public int managementChange;
    }

    [System.Serializable]
    public class EmployeeResult
    {
        public string employeeName;
        public int sanityBefore;
        public int sanityAfter;
        public int communicationBefore;
        public int communicationAfter;
        public int managementBefore;
        public int managementAfter;
        public bool isWorking;
        public string currentTask;
        public bool isResting;
    }
}
