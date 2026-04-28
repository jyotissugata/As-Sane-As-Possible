using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnControlUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button nextTurnButton;
    public Button pauseButton;
    public Button speedButton;
    public TextMeshProUGUI currentTurnText;
    public TextMeshProUGUI currentDateText;
    public TextMeshProUGUI timeOfDayText;
    public TextMeshProUGUI nextTurnButtonText;

    [Header("Speed Settings")]
    public string[] speedModes = { "Normal", "Fast", "Instant" };
    public int currentSpeedIndex = 0;

    private bool isPaused = false;

    private void Start()
    {
        SetupButtons();
        UpdateUI();
    }

    private void SetupButtons()
    {
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(OnNextTurnClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }

        if (speedButton != null)
        {
            speedButton.onClick.AddListener(OnSpeedClicked);
        }
    }

    private void OnNextTurnClicked()
    {
        if (isPaused) return;

        if (CalendarManager.Instance != null)
        {
            CalendarManager.Instance.NextTurn();
            UpdateUI();
        }
        else
        {
            Debug.LogError("CalendarManager not found!");
        }
    }

    private void OnPauseClicked()
    {
        isPaused = !isPaused;
        
        if (pauseButton != null)
        {
            TextMeshProUGUI pauseText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (pauseText != null)
            {
                pauseText.text = isPaused ? "Resume" : "Pause";
            }
            
            ColorBlock colors = pauseButton.colors;
            colors.normalColor = isPaused ? Color.green : Color.red;
            pauseButton.colors = colors;
        }

        if (nextTurnButton != null)
        {
            nextTurnButton.interactable = !isPaused;
        }
    }

    private void OnSpeedClicked()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speedModes.Length;
        
        if (speedButton != null)
        {
            TextMeshProUGUI speedText = speedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (speedText != null)
            {
                speedText.text = $"Speed: {speedModes[currentSpeedIndex]}";
            }
        }

        // Apply speed settings based on mode
        ApplySpeedMode();
    }

    private void ApplySpeedMode()
    {
        switch (speedModes[currentSpeedIndex])
        {
            case "Normal":
                Time.timeScale = 1f;
                break;
            case "Fast":
                Time.timeScale = 2f;
                break;
            case "Instant":
                Time.timeScale = 5f;
                break;
        }
    }

    private void UpdateUI()
    {
        if (CalendarManager.Instance == null) return;

        if (currentTurnText != null)
        {
            currentTurnText.text = $"Turn: {CalendarManager.Instance.currentTurn}";
        }

        if (currentDateText != null)
        {
            currentDateText.text = CalendarManager.Instance.CurrentDate.ToString("dd MMMM yyyy");
        }

        if (timeOfDayText != null)
        {
            timeOfDayText.text = CalendarManager.Instance.GetTimeString();
        }

        if (nextTurnButtonText != null)
        {
            string nextTimeOfDay = GetNextTimeOfDay();
            nextTurnButtonText.text = $"Next Turn ({nextTimeOfDay})";
        }
    }

    private string GetNextTimeOfDay()
    {
        if (CalendarManager.Instance == null) return "Unknown";

        CalendarManager.TimeOfDay nextTime = CalendarManager.Instance.currentTimeOfDay;
        nextTime++;

        if ((int)nextTime >= 3) // Assuming 3 time periods
        {
            nextTime = CalendarManager.TimeOfDay.Morning;
        }

        return nextTime switch
        {
            CalendarManager.TimeOfDay.Morning => "Morning",
            CalendarManager.TimeOfDay.Afternoon => "Afternoon",
            CalendarManager.TimeOfDay.Evening => "Evening",
            _ => "Unknown"
        };
    }

    private void Update()
    {
        // Update UI periodically
        if (Time.frameCount % 30 == 0) // Every 30 frames
        {
            UpdateUI();
        }
    }

    public void EnableNextTurnButton(bool enable)
    {
        if (nextTurnButton != null)
        {
            nextTurnButton.interactable = enable && !isPaused;
        }
    }

    public void SetTurnCount(int turnCount)
    {
        if (currentTurnText != null)
        {
            currentTurnText.text = $"Turn: {turnCount}";
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.RemoveAllListeners();
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
        }

        if (speedButton != null)
        {
            speedButton.onClick.RemoveAllListeners();
        }
    }
}
