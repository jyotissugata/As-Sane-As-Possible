using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI costText;
    public Button purchaseButton;
    public Image availabilityOverlay;

    [Header("Visual Settings")]
    public Color availableColor = Color.white;
    public Color unavailableColor = Color.gray;
    public Color affordableColor = Color.green;
    public Color expensiveColor = Color.red;

    private ShopItem shopItem;

    public void Setup(ShopItem item)
    {
        shopItem = item;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (shopItem == null) return;

        if (itemNameText != null)
            itemNameText.text = shopItem.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = shopItem.itemDescription;

        if (costText != null)
            costText.text = $"${shopItem.cost:N0}";

        if (itemIcon != null && shopItem.itemIcon != null)
            itemIcon.sprite = shopItem.itemIcon;

        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        if (shopItem == null) return;

        bool canAfford = EconomyManager.Instance?.CanAfford(shopItem.cost) ?? false;
        bool isAvailable = IsItemAvailable();

        if (purchaseButton != null)
        {
            purchaseButton.interactable = canAfford && isAvailable;
        }

        if (costText != null)
        {
            costText.color = canAfford ? affordableColor : expensiveColor;
        }

        if (availabilityOverlay != null)
        {
            availabilityOverlay.gameObject.SetActive(!isAvailable);
        }
    }

    private bool IsItemAvailable()
    {
        // Check specific item availability rules
        switch (shopItem.itemType)
        {
            case ShopItemType.Recruit:
                // Limit number of employees
                return true;//GameServiceLocator.Instance.Employees.Count < 10;
            
            case ShopItemType.RestAreaUpgrade:
                // Use efficient Service Locator instead of FindObjectsOfType
                var restAreas = GameServiceLocator.RestAreaSlots;
                return restAreas.Count > 0 && restAreas[0].maxCapacity < 8;
            
            default:
                return true;
        }
    }

    public void OnPurchaseClicked()
    {
        if (shopItem != null && ShopManager.Instance != null)
        {
            if (ShopManager.Instance.PurchaseItem(shopItem))
            {
                // Purchase successful - could add visual feedback
                Debug.Log($"Successfully purchased {shopItem.itemName}");
                UpdateAvailability();
            }
        }
    }

    private void Start()
    {
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }

    private void OnDestroy()
    {
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
        }
    }
}
