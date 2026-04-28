using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Economy Settings")]
    public int startingMoney = 1000;
    public int targetMoney = 10000;
    public int maxGameDays = 30;
    public int dailyOperationalCost = 100;

    [Header("UI References (Optional - for AlpacaStudio integration)")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI targetMoneyText;
    public TextMeshProUGUI dayText;
    public Slider moneyProgressSlider;

    [Header("Shop Settings")]
    public int recruitCost = 200;
    public int restAreaUpgradeCost = 500;
    public int petBuffCost = 300;

    private int currentMoney;
    private int currentDay = 1;
    private bool gameEnded = false;

    public int CurrentMoney => currentMoney;
    public int TargetMoney => targetMoney;
    public int CurrentDay => currentDay;
    public bool GameEnded => gameEnded;

    public static event Action<int> OnMoneyChanged;
    public static event Action<int> OnDayChanged;
    public static event Action<GameResult> OnGameEnded;

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
        InitializeEconomy();
        
        // Subscribe to calendar events
        if (CalendarManager.Instance != null)
        {
            CalendarManager.OnDayChanged += OnNewDay;
        }
    }

    private void InitializeEconomy()
    {
        currentMoney = startingMoney;
        currentDay = 1;
        gameEnded = false;
        UpdateUI();
    }

    public bool AddMoney(int amount)
    {
        if (gameEnded) return false;

        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        UpdateUI();
        
        Debug.Log($"Money added: +{amount}. Current: {currentMoney}");
        
        // Check victory condition
        CheckVictoryCondition();
        
        return true;
    }

    public bool SpendMoney(int amount)
    {
        if (gameEnded) return false;
        
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            UpdateUI();
            
            Debug.Log($"Money spent: -{amount}. Current: {currentMoney}");
            
            // Check loss condition
            CheckLossCondition();
            
            return true;
        }
        
        Debug.LogWarning($"Insufficient funds! Need {amount}, have {currentMoney}");
        return false;
    }

    public bool CanAfford(int amount)
    {
        return currentMoney >= amount && !gameEnded;
    }

    private void OnNewDay(int turn)
    {
        if (!gameEnded)
        {
            Debug.Log($"OnNewDay: currentDay={currentDay}, maxGameDays={maxGameDays}");
            
            ProcessDailyCosts();
            currentDay++;
            OnDayChanged?.Invoke(currentDay);
            
            // Check time limit
            if (currentDay > maxGameDays)
            {
                Debug.Log($"Time limit reached: currentDay={currentDay}, maxGameDays={maxGameDays}. Ending game.");
                EndGame(GameResult.TimeOut);
            }
        }
    }

    private void ProcessDailyCosts()
    {
        int totalCost = dailyOperationalCost;
        
        // Add employee salary costs using GameServiceLocator
        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            if (employee != null)
            {
                totalCost += CalculateEmployeeSalary(employee.Template);
            }
        }
        
        // Check if player can afford the costs
        if (currentMoney < totalCost)
        {
            Debug.LogWarning($"Insufficient funds for daily costs! Need {totalCost}, have {currentMoney}");
            // Deduct the money anyway to trigger loss condition
            currentMoney -= totalCost;
            OnMoneyChanged?.Invoke(currentMoney);
            UpdateUI();
            CheckLossCondition();
        }
        else
        {
            SpendMoney(totalCost);
        }
        
        Debug.Log($"Daily operational costs: -{totalCost} (Day {currentDay})");
    }

    private int CalculateEmployeeSalary(EmployeeData employee)
    {
        // Use the salary defined in EmployeeData
        return employee.salary;
    }

    private void CheckVictoryCondition()
    {
        Debug.Log($"CheckVictoryCondition: currentMoney={currentMoney}, targetMoney={targetMoney}, gameEnded={gameEnded}");
        if (currentMoney >= targetMoney && !gameEnded)
        {
            Debug.Log("Victory condition met! Ending game.");
            EndGame(GameResult.Victory);
        }
    }

    private void CheckLossCondition()
    {
        Debug.Log($"CheckLossCondition: currentMoney={currentMoney}, gameEnded={gameEnded}");
        if (currentMoney < 0 && !gameEnded)
        {
            Debug.Log("Loss condition met! Ending game.");
            EndGame(GameResult.Bankruptcy);
        }
    }

    private void EndGame(GameResult result)
    {
        if (gameEnded) return;
        
        gameEnded = true;
        OnGameEnded?.Invoke(result);
        
        Debug.Log($"Game Ended: {result}");
        
        // Show game end screen
        if (ResultPanel.Instance != null)
        {
            // ResultPanel.Instance.ShowGameEndResult(result);
        }
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"${currentMoney:N0}";
        }

        if (targetMoneyText != null)
        {
            targetMoneyText.text = $"Target: ${targetMoney:N0}";
        }

        if (dayText != null)
        {
            dayText.text = $"Day {currentDay}/{maxGameDays}";
        }

        if (moneyProgressSlider != null)
        {
            moneyProgressSlider.value = (float)currentMoney / targetMoney;
        }
    }

    // Shop System Methods
    public bool PurchaseRecruit()
    {
        if (SpendMoney(recruitCost))
        {
            // Trigger recruit system
            Debug.Log("Recruit purchased!");
            return true;
        }
        return false;
    }

    public bool PurchaseRestAreaUpgrade()
    {
        if (SpendMoney(restAreaUpgradeCost))
        {
            // Trigger rest area upgrade
            Debug.Log("Rest Area upgrade purchased!");
            return true;
        }
        return false;
    }

    public bool PurchasePetBuff()
    {
        if (SpendMoney(petBuffCost))
        {
            // Trigger pet buff system
            Debug.Log("Pet buff purchased!");
            return true;
        }
        return false;
    }

    // Save/Load System
    public EconomyData SaveEconomyData()
    {
        return new EconomyData
        {
            currentMoney = currentMoney,
            targetMoney = targetMoney,
            currentDay = currentDay,
            maxGameDays = maxGameDays,
            dailyOperationalCost = dailyOperationalCost
        };
    }

    public void LoadEconomyData(EconomyData data)
    {
        currentMoney = data.currentMoney;
        targetMoney = data.targetMoney;
        currentDay = data.currentDay;
        maxGameDays = data.maxGameDays;
        dailyOperationalCost = data.dailyOperationalCost;
        
        UpdateUI();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (CalendarManager.Instance != null)
        {
            CalendarManager.OnDayChanged -= OnNewDay;
        }
    }
}

[System.Serializable]
public class EconomyData
{
    public int currentMoney;
    public int targetMoney;
    public int currentDay;
    public int maxGameDays;
    public int dailyOperationalCost;
}

public enum GameResult
{
    Victory,
    Bankruptcy,
    TimeOut,
    Quit
}
