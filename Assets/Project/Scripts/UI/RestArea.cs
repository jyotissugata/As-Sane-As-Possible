using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class RestArea : MonoBehaviour
{
    [Header("Rest Area Settings")]
    public float sanityRestoreRate = 2f; // Sanity points per second
    public int maxSanityRestore = 100;
    public float restInterval = 1f; // Update every 1 second

    [Header("UI Components")]
    public Transform restAreaContainer;
    public GameObject employeeRestCardPrefab;
    public TextMeshProUGUI restAreaTitleText;
    public TextMeshProUGUI restingEmployeesCountText;

    [Header("Visual Settings")]
    public Color restingCardColor = Color.blue;
    public float cardSpacing = 10f;

    private List<RectTransform> restingEmployeeCards = new List<RectTransform>();
    private Dictionary<Employee, GameObject> restingEmployees = new Dictionary<Employee, GameObject>();
    private Coroutine restCoroutine;
    private bool isRestActive = false;

    public static RestArea Instance { get; private set; }

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
        UpdateUI();
    }

    public void AddEmployeeToRest(Employee employee, DraggableCard employeeCard = null)
    {
        if (employee == null) return;

        Debug.Log($"Adding {employee.Template?.employeeName} to rest area (Sanity: {employee.CurrentSanity})");

        // Remove from current task if assigned
        if (employeeCard != null && employeeCard.CurrentSlot != null)
        {
            employeeCard.CurrentSlot.RemoveEmployee();
        }

        // Set employee as unavailable for work
        employee.isAvailable = false;
        employee.currentTaskName = "Resting";

        // Create rest card for employee
        GameObject restCard = CreateRestCard(employee);
        restingEmployees[employee] = restCard;

        // Start rest coroutine if not already running
        if (!isRestActive)
        {
            StartRestProcess();
        }

        UpdateUI();
    }

    private GameObject CreateRestCard(Employee employee)
    {
        if (employeeRestCardPrefab == null)
        {
            Debug.LogError("Employee rest card prefab not assigned!");
            return null;
        }

        GameObject restCard = Instantiate(employeeRestCardPrefab, restAreaContainer);
        RectTransform rectTransform = restCard.GetComponent<RectTransform>();

        // Position the card
        float yOffset = restingEmployeeCards.Count * (rectTransform.rect.height + cardSpacing);
        rectTransform.anchoredPosition = new Vector2(0, -yOffset);

        // Set up card components
        SetupRestCard(restCard, employee);

        restingEmployeeCards.Add(rectTransform);

        return restCard;
    }

    private void SetupRestCard(GameObject restCard, Employee employee)
    {
        // Find text components
        TextMeshProUGUI nameText = restCard.GetComponentInChildren<TextMeshProUGUI>();
        Image backgroundImage = restCard.GetComponent<Image>();

        if (nameText != null)
        {
            nameText.text = $"{employee.Template?.employeeName}\nSanity: {employee.CurrentSanity}/100";
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = restingCardColor;
        }

        // Add click handler to remove from rest
        Button button = restCard.GetComponent<Button>();
        if (button == null)
        {
            button = restCard.AddComponent<Button>();
        }

        button.onClick.AddListener(() => RemoveEmployeeFromRest(employee));
    }

    public void RemoveEmployeeFromRest(Employee employee)
    {
        if (employee == null || !restingEmployees.ContainsKey(employee)) return;

        Debug.Log($"Removing {employee.Template?.employeeName} from rest area");

        // Mark employee as available again
        employee.isAvailable = true;
        employee.currentTaskName = "";

        // Destroy rest card
        if (restingEmployees[employee] != null)
        {
            RectTransform rectTransform = restingEmployees[employee].GetComponent<RectTransform>();
            restingEmployeeCards.Remove(rectTransform);
            Destroy(restingEmployees[employee]);
        }

        restingEmployees.Remove(employee);

        // Reorganize remaining cards
        ReorganizeRestCards();

        // Stop rest process if no employees are resting
        if (restingEmployees.Count == 0)
        {
            StopRestProcess();
        }

        UpdateUI();
    }

    private void ReorganizeRestCards()
    {
        for (int i = 0; i < restingEmployeeCards.Count; i++)
        {
            float yOffset = i * (restingEmployeeCards[i].rect.height + cardSpacing);
            restingEmployeeCards[i].anchoredPosition = new Vector2(0, -yOffset);
        }
    }

    private void StartRestProcess()
    {
        if (isRestActive) return;

        isRestActive = true;
        restCoroutine = StartCoroutine(RestCoroutine());
        Debug.Log("Rest process started");
    }

    private void StopRestProcess()
    {
        if (!isRestActive) return;

        isRestActive = false;
        
        if (restCoroutine != null)
        {
            StopCoroutine(restCoroutine);
            restCoroutine = null;
        }

        Debug.Log("Rest process stopped");
    }

    private IEnumerator RestCoroutine()
    {
        while (isRestActive && restingEmployees.Count > 0)
        {
            // Restore sanity for all resting employees
            List<Employee> employeesToCheck = new List<Employee>(restingEmployees.Keys);

            foreach (var employee in employeesToCheck)
            {
                if (employee == null) continue;

                // Restore sanity
                int sanityRestore = Mathf.RoundToInt(sanityRestoreRate * restInterval);
                employee.UpdateSanity(sanityRestore);

                // Update rest card display
                UpdateRestCardDisplay(employee);

                // Check if employee is fully restored
                if (employee.CurrentSanity >= maxSanityRestore)
                {
                    Debug.Log($"{employee.Template?.employeeName} is fully rested!");
                    // Auto-remove from rest area
                    RemoveEmployeeFromRest(employee);
                }
            }

            yield return new WaitForSeconds(restInterval);
        }
    }

    private void UpdateRestCardDisplay(Employee employee)
    {
        if (!restingEmployees.ContainsKey(employee)) return;

        GameObject restCard = restingEmployees[employee];
        if (restCard == null) return;

        TextMeshProUGUI nameText = restCard.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = $"{employee.Template?.employeeName}\nSanity: {employee.CurrentSanity}/100";
        }

        // Update color based on sanity level
        Image backgroundImage = restCard.GetComponent<Image>();
        if (backgroundImage != null)
        {
            if (employee.CurrentSanity < 30)
                backgroundImage.color = Color.red;
            else if (employee.CurrentSanity < 60)
                backgroundImage.color = Color.yellow;
            else
                backgroundImage.color = Color.green;
        }
    }

    private void UpdateUI()
    {
        if (restAreaTitleText != null)
        {
            restAreaTitleText.text = "Rest Area";
        }

        if (restingEmployeesCountText != null)
        {
            restingEmployeesCountText.text = $"Resting: {restingEmployees.Count}";
        }
    }

    public bool IsEmployeeResting(Employee employee)
    {
        return restingEmployees.ContainsKey(employee);
    }

    public int GetRestingEmployeeCount()
    {
        return restingEmployees.Count;
    }

    private void OnDestroy()
    {
        StopRestProcess();
    }
}
