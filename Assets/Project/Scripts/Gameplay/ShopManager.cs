using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Shop Items")]
    public List<ShopItem> shopItems = new List<ShopItem>();

    [Header("UI References (Optional - for AlpacaStudio integration)")]
    public Transform shopItemContainer;
    public GameObject shopItemPrefab;
    public TextMeshProUGUI playerMoneyText;

    private List<ShopItemUI> shopItemUIs = new List<ShopItemUI>();

    public static event Action<ShopItem> OnItemPurchased;
    public static event Action<EmployeeData> OnEmployeeRecruited;

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
        InitializeShop();
        
        // Subscribe to economy events
        if (EconomyManager.Instance != null)
        {
            EconomyManager.OnMoneyChanged += UpdateMoneyDisplay;
        }
    }

    private void InitializeShop()
    {
        // Create default shop items if none exist
        if (shopItems.Count == 0)
        {
            CreateDefaultShopItems();
        }

        // Create UI for shop items
        CreateShopItemUIs();
    }

    private void CreateDefaultShopItems()
    {
        // Recruit items
        shopItems.Add(new ShopItem
        {
            itemName = "Junior Developer",
            itemDescription = "Basic technical skills, low salary",
            itemType = ShopItemType.Recruit,
            cost = 200,
            employeeData = CreateJuniorDeveloper()
        });

        shopItems.Add(new ShopItem
        {
            itemName = "Senior Designer",
            itemDescription = "High creative skills, moderate salary",
            itemType = ShopItemType.Recruit,
            cost = 350,
            employeeData = CreateSeniorDesigner()
        });

        // Upgrade items
        shopItems.Add(new ShopItem
        {
            itemName = "Rest Area Upgrade",
            itemDescription = "Increase rest area capacity by 2",
            itemType = ShopItemType.RestAreaUpgrade,
            cost = 500,
            upgradeAmount = 2
        });

        shopItems.Add(new ShopItem
        {
            itemName = "Pet Buff",
            itemDescription = "+10% sanity recovery for all employees",
            itemType = ShopItemType.PetBuff,
            cost = 300,
            buffDuration = 5 // 5 turns
        });

        // Special items
        shopItems.Add(new ShopItem
        {
            itemName = "Energy Drinks",
            itemDescription = "Instant +20 sanity to all employees",
            itemType = ShopItemType.Consumable,
            cost = 150,
            effectValue = 20
        });
    }

    private EmployeeData CreateJuniorDeveloper()
    {
        EmployeeData employee = ScriptableObject.CreateInstance<EmployeeData>();
        employee.employeeName = "Junior Developer";
        employee.technicalSkill = 60;
        employee.creativeSkill = 30;
        employee.communicationSkill = 40;
        employee.managementSkill = 20;
        employee.sanity = 70;
        return employee;
    }

    private EmployeeData CreateSeniorDesigner()
    {
        EmployeeData employee = ScriptableObject.CreateInstance<EmployeeData>();
        employee.employeeName = "Senior Designer";
        employee.technicalSkill = 40;
        employee.creativeSkill = 75;
        employee.communicationSkill = 60;
        employee.managementSkill = 50;
        employee.sanity = 65;
        return employee;
    }

    private void CreateShopItemUIs()
    {
        if (shopItemContainer == null || shopItemPrefab == null) return;

        // Clear existing UI
        foreach (Transform child in shopItemContainer)
        {
            Destroy(child.gameObject);
        }
        shopItemUIs.Clear();

        // Create UI for each shop item
        foreach (var shopItem in shopItems)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopItemContainer);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Setup(shopItem);
                shopItemUIs.Add(itemUI);
            }
        }
    }

    public bool PurchaseItem(ShopItem item)
    {
        if (item == null) return false;

        if (EconomyManager.Instance != null && EconomyManager.Instance.SpendMoney(item.cost))
        {
            ProcessPurchase(item);
            OnItemPurchased?.Invoke(item);
            return true;
        }

        Debug.LogWarning($"Failed to purchase {item.itemName} - insufficient funds");
        return false;
    }

    private void ProcessPurchase(ShopItem item)
    {
        switch (item.itemType)
        {
            case ShopItemType.Recruit:
                ProcessRecruitPurchase(item);
                break;

            case ShopItemType.RestAreaUpgrade:
                ProcessRestAreaUpgrade(item);
                break;

            case ShopItemType.PetBuff:
                ProcessPetBuff(item);
                break;

            case ShopItemType.Consumable:
                ProcessConsumable(item);
                break;
        }
    }

    private void ProcessRecruitPurchase(ShopItem item)
    {
        if (item.employeeData != null)
        {
            // Create Employee component from template - it will auto-register with GameServiceLocator
            GameObject employeeObj = new GameObject($"Employee_{item.employeeData.employeeName}");
            Employee employeeComponent = employeeObj.AddComponent<Employee>();
            employeeComponent.InitializeFromTemplate(item.employeeData);

            Debug.Log($"Recruited new employee: {item.employeeData.employeeName}");
            OnEmployeeRecruited?.Invoke(item.employeeData);
        }
    }

    private void ProcessRestAreaUpgrade(ShopItem item)
    {
        // Use efficient Service Locator instead of FindObjectsOfType
        var restAreas = GameServiceLocator.RestAreaSlots;
        foreach (var restArea in restAreas)
        {
            restArea.maxCapacity += item.upgradeAmount;
        }
        Debug.Log($"Rest Area upgraded! Capacity +{item.upgradeAmount}");
    }

    private void ProcessPetBuff(ShopItem item)
    {
        // Use GameServiceLocator to get all employees
        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            if (employee != null)
            {
                // Create a custom buff for pet effect
                EmployeeStatus petBuff = StatusEffects.CreateInspired();
                petBuff.duration = item.buffDuration;
                
                employee.statusManager?.AddStatus(petBuff);
            }
        }
        Debug.Log($"Pet buff applied to all employees for {item.buffDuration} turns");
    }

    private void ProcessConsumable(ShopItem item)
    {
        // Apply instant effects using GameServiceLocator
        var employees = GameServiceLocator.Employees;
        foreach (var employee in employees)
        {
            if (employee != null)
            {
                employee.UpdateSanity(item.effectValue);
                Debug.Log($"{employee.Template?.employeeName} received {item.effectValue} sanity from {item.itemName}");
            }
        }
        Debug.Log($"Consumable {item.itemName} used");
    }

    private void UpdateMoneyDisplay(int newAmount)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"${newAmount:N0}";
        }

        // Update shop item availability
        UpdateShopItemAvailability();
    }

    private void UpdateShopItemAvailability()
    {
        foreach (var itemUI in shopItemUIs)
        {
            if (itemUI != null)
            {
                itemUI.UpdateAvailability();
            }
        }
    }

    public void RefreshShop()
    {
        CreateShopItemUIs();
        UpdateMoneyDisplay(EconomyManager.Instance?.CurrentMoney ?? 0);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (EconomyManager.Instance != null)
        {
            EconomyManager.OnMoneyChanged -= UpdateMoneyDisplay;
        }
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemName;
    [TextArea(2, 4)]
    public string itemDescription;
    public ShopItemType itemType;
    public int cost;
    public Sprite itemIcon;

    [Header("Item-Specific Data")]
    public EmployeeData employeeData; // For recruits
    public int upgradeAmount; // For upgrades
    public int buffDuration; // For buffs
    public int effectValue; // For consumables
}

public enum ShopItemType
{
    Recruit,
    RestAreaUpgrade,
    PetBuff,
    Consumable
}
