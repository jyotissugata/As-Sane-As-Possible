using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeadershipManager : MonoBehaviour
{
    public static LeadershipManager Instance { get; private set; }

    [Header("Mission Brief System")]
    [SerializeField] private GameObject missionBriefPanel;
    [SerializeField] private TextMeshProUGUI missionTitleText;
    [SerializeField] private TextMeshProUGUI missionMessageText;
    [SerializeField] private Button missionCloseButton;
    
    [Space(10)]
    [SerializeField] private string introTitle = "Welcome, Lead Developer!";
    [TextArea(5, 10)]
    [SerializeField] private string introMessage = "Your goal is simple: hit the revenue target. But remember, a true leader doesn't just manage numbers—they manage people. Assigning the wrong person to a task will drain their Sanity faster. Keep your team's mental health in check while completing projects. Your mantra is ASAP: As Sane As Possible!";

    [Header("Dynamic Feedback System")]
    [SerializeField] private TextMeshProUGUI leadershipFeedbackText;
    
    [Space(10)]
    [TextArea(3, 5)]
    [SerializeField] private string positiveFeedback = "Excellent Delegation! You placed the right talent in the right role. Efficiency is at its peak.";
    
    [TextArea(3, 5)]
    [SerializeField] private string warningFeedback = "Leadership Warning: Your team is showing signs of burnout. A great leader knows when to push and when to protect their team's well-being.";
    
    [TextArea(3, 5)]
    [SerializeField] private string restFeedback = "Investing in People: Taking time to recover today ensures a stronger, more productive team tomorrow.";
    
    [TextArea(3, 5)]
    [SerializeField] private string defaultFeedback = "Keep up the good work";

    [Header("Post-Game Scoring System")]
    [SerializeField] private TextMeshProUGUI leadershipTitleText;
    
    [Space(10)]
    [TextArea(2, 3)]
    [SerializeField] private string highEfficiencyTitle = "The Tyrant: You hit the numbers, but at what cost? Your team is broken. Short-term gains, long-term disaster.";
    
    [TextArea(2, 3)]
    [SerializeField] private string highSanityTitle = "The People Pleaser: Your team is happy and rested, but the company is bankrupt. Leadership requires making tough calls for results.";
    
    [TextArea(2, 3)]
    [SerializeField] private string balancedTitle = "The Balanced Leader: Incredible work! You achieved the goals while maintaining a healthy team. This is the gold standard of sustainable leadership.";
    
    [TextArea(2, 3)]
    [SerializeField] private string pragmaticTitle = "You hit the targets with realistic discipline. You balanced pressure and output, keeping the machine running without a total team collapse.";
    
    [TextArea(2, 3)]
    [SerializeField] private string caringTitle = "You prioritized well-being over raw profit. While margins are tight, you’ve built a loyal team that is resilient and ready for anything.";
    
    [TextArea(2, 3)]
    [SerializeField] private string developingTitle = "You focused on long-term growth. By investing in recovery and support, you’ve built a team that will only get stronger over time.";

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
        SetupMissionBrief();
        ShowMissionBrief();
    }

    #region Mission Brief System

    private void SetupMissionBrief()
    {
        if (missionCloseButton != null)
        {
            missionCloseButton.onClick.AddListener(HideMissionBrief);
        }
    }

    public void ShowMissionBrief()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = introTitle;
        }

        if (missionMessageText != null)
        {
            missionMessageText.text = introMessage;
        }

        if (missionBriefPanel != null)
        {
            missionBriefPanel.SetActive(true);
        }
    }

    public void HideMissionBrief()
    {
        if (missionBriefPanel != null)
        {
            missionBriefPanel.SetActive(false);
        }
    }

    #endregion

    #region Dynamic Feedback System

    public void UpdateTurnFeedback()
    {
        if (leadershipFeedbackText == null) return;

        string feedback = CalculateTurnFeedback();
        leadershipFeedbackText.text = feedback;
    }

    private string CalculateTurnFeedback()
    {
        // Calculate team statistics
        var employees = GameServiceLocator.Employees;
        if (employees == null || employees.Count == 0) return defaultFeedback;

        int employeeCount = 0;
        int lowSanityCount = 0;
        int restingCount = 0;
        int goodAssignments = 0;
        int totalAssignments = 0;

        foreach (var employee in employees)
        {
            if (employee == null) continue;
            
            employeeCount++;

            if (employee.CurrentSanity < 30) lowSanityCount++;
            if (employee.isResting) restingCount++;
        }

        // Check task assignments
        var taskSlots = GameServiceLocator.TaskSlots;
        foreach (var slot in taskSlots)
        {
            if (slot.IsOccupied && slot.Task != null && slot.CurrentEmployee != null)
            {
                totalAssignments++;
                
                EmployeeCard empCard = slot.CurrentEmployee as EmployeeCard;
                if (empCard != null && empCard.Employee != null)
                {
                    bool isGoodMatch = IsGoodAssignment(slot.TaskData, empCard.Employee);
                    if (isGoodMatch) goodAssignments++;
                }
            }
        }

        // Generate feedback based on conditions (priority order)
        if (lowSanityCount > 0)
        {
            return warningFeedback;
        }
        else if (restingCount > 0)
        {
            return restFeedback;
        }
        else if (goodAssignments > totalAssignments / 2 && totalAssignments > 0)
        {
            return positiveFeedback;
        }
        else
        {
            return defaultFeedback;
        }
    }

    private bool IsGoodAssignment(TaskData taskData, Employee employee)
    {
        if (taskData == null || employee == null || employee.Template == null) return false;

        float skillMatch = 0f;
        int requiredSkills = 0;

        if (taskData.requiredTechnicalSkill > 0)
        {
            float match = (employee.Template.technicalSkill - taskData.requiredTechnicalSkill) / 50f;
            skillMatch += Mathf.Clamp01(match);
            requiredSkills++;
        }

        if (taskData.requiredCreativeSkill > 0)
        {
            float match = (employee.Template.creativeSkill - taskData.requiredCreativeSkill) / 50f;
            skillMatch += Mathf.Clamp01(match);
            requiredSkills++;
        }

        if (taskData.minimumCommunicationSkill > 0)
        {
            float match = (employee.CurrentCommunicationSkill - taskData.minimumCommunicationSkill) / 50f;
            skillMatch += Mathf.Clamp01(match);
            requiredSkills++;
        }

        if (taskData.minimumManagementSkill > 0)
        {
            float match = (employee.CurrentManagementSkill - taskData.minimumManagementSkill) / 50f;
            skillMatch += Mathf.Clamp01(match);
            requiredSkills++;
        }

        if (requiredSkills > 0)
        {
            skillMatch /= requiredSkills;
        }

        return skillMatch >= 0.3f;
    }

    #endregion

    #region Post-Game Scoring System

    public void UpdateLeadershipTitle()
    {
        if (leadershipTitleText == null) return;

        string title = CalculateLeadershipTitle();
        leadershipTitleText.text = title;
    }

    private string CalculateLeadershipTitle()
    {
        // Calculate team statistics
        var employees = GameServiceLocator.Employees;
        if (employees == null || employees.Count == 0) return developingTitle;

        float totalSanity = 0f;
        int employeeCount = 0;
        int zeroSanityCount = 0;

        foreach (var employee in employees)
        {
            if (employee == null) continue;
            
            totalSanity += employee.CurrentSanity;
            employeeCount++;

            if (employee.CurrentSanity == 0) zeroSanityCount++;
        }

        if (employeeCount == 0) return developingTitle;

        float averageSanity = totalSanity / employeeCount;
        float zeroSanityPercentage = (float)zeroSanityCount / employeeCount;

        // Get financial status
        int currentMoney = EconomyManager.Instance?.CurrentMoney ?? 0;
        int targetMoney = EconomyManager.Instance?.TargetMoney ?? 10000;
        bool financialSuccess = currentMoney >= targetMoney;

        // Determine leadership category
        if (financialSuccess && zeroSanityPercentage > 0.5f)
        {
            return highEfficiencyTitle;
        }
        else if (!financialSuccess && averageSanity >= 70)
        {
            return highSanityTitle;
        }
        else if (financialSuccess && averageSanity >= 50)
        {
            return balancedTitle;
        }
        else if (financialSuccess)
        {
            return pragmaticTitle;
        }
        else if (averageSanity >= 50)
        {
            return caringTitle;
        }
        else
        {
            return developingTitle;
        }
    }

    #endregion
}
