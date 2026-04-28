using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Adapter script for easy integration with AlpacaStudio Menu Template
/// Attach this to existing UI objects to add ASAP game functionality
/// </summary>
public class AlpacaStudioUIAdapter : MonoBehaviour
{
    [Header("Adapter Type")]
    public UIAdapterType adapterType;

    [Header("Economy Display")]
    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI targetDisplay;
    public Slider progressSlider;

    [Header("Game Info Display")]
    public TextMeshProUGUI dayDisplay;
    public TextMeshProUGUI turnDisplay;
    public TextMeshProUGUI timeDisplay;

    [Header("Shop Integration")]
    public Button shopButton;
    public Transform shopContainer;
    public GameObject shopItemPrefab;

    [Header("Task List Integration")]
    public Transform taskListContainer;
    public GameObject taskItemPrefab;

    [Header("Result Panel Integration")]
    public GameObject resultPanel;
    public Transform resultTaskContainer;
    public Transform resultEmployeeContainer;

    private void Start()
    {
        InitializeAdapter();
    }

    private void InitializeAdapter()
    {
        switch (adapterType)
        {
            case UIAdapterType.EconomyDisplay:
                SetupEconomyDisplay();
                break;
            case UIAdapterType.GameInfo:
                SetupGameInfoDisplay();
                break;
            case UIAdapterType.Shop:
                SetupShopIntegration();
                break;
            case UIAdapterType.TaskList:
                SetupTaskListIntegration();
                break;
            case UIAdapterType.ResultPanel:
                SetupResultPanelIntegration();
                break;
        }
    }

    private void SetupEconomyDisplay()
    {
        // Subscribe to economy events
        if (EconomyManager.Instance != null)
        {
            EconomyManager.OnMoneyChanged += UpdateEconomyDisplay;
            UpdateEconomyDisplay(EconomyManager.Instance.CurrentMoney);
        }
    }

    private void UpdateEconomyDisplay(int money)
    {
        if (moneyDisplay != null && EconomyManager.Instance != null)
        {
            moneyDisplay.text = $"${EconomyManager.Instance.CurrentMoney:N0}";
        }

        if (targetDisplay != null && EconomyManager.Instance != null)
        {
            targetDisplay.text = $"Target: ${EconomyManager.Instance.TargetMoney:N0}";
        }

        if (progressSlider != null && EconomyManager.Instance != null)
        {
            progressSlider.value = (float)EconomyManager.Instance.CurrentMoney / EconomyManager.Instance.TargetMoney;
        }
    }

    private void SetupGameInfoDisplay()
    {
        // Subscribe to calendar events
        if (CalendarManager.Instance != null)
        {
            CalendarManager.OnTurnChanged += UpdateGameInfoDisplay;
            UpdateGameInfoDisplay(CalendarManager.Instance.currentTurn, CalendarManager.Instance.currentTimeOfDay, CalendarManager.Instance.CurrentDate);
        }
    }

    private void UpdateGameInfoDisplay(int turn, CalendarManager.TimeOfDay timeOfDay, System.DateTime date)
    {
        if (dayDisplay != null && EconomyManager.Instance != null)
        {
            dayDisplay.text = $"Day {EconomyManager.Instance.CurrentDay}/{EconomyManager.Instance.maxGameDays}";
        }

        if (turnDisplay != null)
        {
            turnDisplay.text = $"Turn: {turn}";
        }

        if (timeDisplay != null)
        {
            timeDisplay.text = timeOfDay.ToString();
        }
    }

    private void SetupShopIntegration()
    {
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(ToggleShop);
        }

        // Initialize shop if ShopManager exists
        if (ShopManager.Instance != null && shopContainer != null && shopItemPrefab != null)
        {
            // Override shop manager's container with this one
            var shopManager = ShopManager.Instance;
            shopManager.shopItemContainer = shopContainer;
            shopManager.shopItemPrefab = shopItemPrefab;
            shopManager.RefreshShop();
        }
    }

    private void ToggleShop()
    {
        if (shopContainer != null)
        {
            bool isActive = shopContainer.gameObject.activeSelf;
            shopContainer.gameObject.SetActive(!isActive);
        }
    }

    private void SetupTaskListIntegration()
    {
        // This would integrate with TaskDiscoveryManager
        if (TaskDiscoveryManager.Instance != null && taskListContainer != null)
        {
            // Update task display when tasks change
            TaskDiscoveryManager.OnTaskDiscovered += UpdateTaskList;
            UpdateTaskList(null); // Initial update
        }
    }

    private void UpdateTaskList(TaskData task)
    {
        if (taskListContainer == null) return;

        // Clear existing items
        foreach (Transform child in taskListContainer)
        {
            Destroy(child.gameObject);
        }

        // Add current tasks
        if (TaskDiscoveryManager.Instance != null)
        {
            var activeTasks = TaskDiscoveryManager.Instance.GetActiveTasks();
            foreach (var taskData in activeTasks)
            {
                if (taskItemPrefab != null)
                {
                    GameObject taskObj = Instantiate(taskItemPrefab, taskListContainer);
                    TaskItemUI taskUI = taskObj.GetComponent<TaskItemUI>();
                    if (taskUI != null)
                    {
                        // Find the corresponding Task component for this TaskData
                        var taskSlots = GameServiceLocator.TaskSlots;
                        Task taskComponent = null;
                        foreach (var slot in taskSlots)
                        {
                            if (slot.TaskData == taskData && slot.Task != null)
                            {
                                taskComponent = slot.Task;
                                break;
                            }
                        }
                        taskUI.Setup(taskComponent);
                    }
                }
            }
        }
    }

    private void SetupResultPanelIntegration()
    {
        // Subscribe to result panel events
        if (ResultPanel.Instance != null)
        {
            // Override result panel containers
            if (resultTaskContainer != null)
            {
                ResultPanel.Instance.taskResultsContainer = resultTaskContainer;
            }
            if (resultEmployeeContainer != null)
            {
                ResultPanel.Instance.employeeResultsContainer = resultEmployeeContainer;
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (EconomyManager.Instance != null)
        {
            EconomyManager.OnMoneyChanged -= UpdateEconomyDisplay;
        }

        if (CalendarManager.Instance != null)
        {
            CalendarManager.OnTurnChanged -= UpdateGameInfoDisplay;
        }

        if (TaskDiscoveryManager.Instance != null)
        {
            TaskDiscoveryManager.OnTaskDiscovered -= UpdateTaskList;
        }
    }
}

public enum UIAdapterType
{
    EconomyDisplay,
    GameInfo,
    Shop,
    TaskList,
    ResultPanel
}
