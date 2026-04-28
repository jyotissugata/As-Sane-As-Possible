using UnityEngine;
using UnityEditor;
using System.IO;

public class TaskDataGenerator
{
    [MenuItem("ASAP/Generate All TaskData")]
    public static void GenerateAllTaskData()
    {
        string folderPath = "Assets/Project/Data/TaskData";
        
        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // Generate all tasks for Day 1
        CreateFixLoginBug(folderPath);
        CreateDesignLogo(folderPath);
        CreateClientMeeting(folderPath);

        // Generate all tasks for Day 2
        CreateDatabaseOptimization(folderPath);
        CreateUIRedesign(folderPath);
        CreateTeamCoordination(folderPath);

        // Generate all tasks for Day 3
        CreateMobileAppDevelopment(folderPath);
        CreateWebsiteRedesign(folderPath);
        CreateProjectPlanning(folderPath);

        // Generate all tasks for Day 4
        CreateServerMigration(folderPath);
        CreateBrandIdentity(folderPath);
        CreateStakeholderPresentation(folderPath);

        // Generate all tasks for Day 5
        CreateAPIIntegration(folderPath);
        CreateMarketingCampaign(folderPath);
        CreateBudgetReview(folderPath);

        // Generate all tasks for Day 6
        CreateEcommercePlatform(folderPath);
        CreateProductLaunch(folderPath);
        CreateTeamTraining(folderPath);

        // Generate all tasks for Day 7
        CreateSecurityAudit(folderPath);
        CreateUserResearch(folderPath);
        CreateResourceAllocation(folderPath);

        // Generate all tasks for Day 8
        CreatePerformanceOptimization(folderPath);
        CreateFeatureImplementation(folderPath);
        CreateClientNegotiation(folderPath);

        // Generate all tasks for Day 9
        CreateCriticalBugFix(folderPath);
        CreateMajorFeature(folderPath);
        CreateExecutiveReport(folderPath);

        // Generate all tasks for Day 10
        CreateSystemUpgrade(folderPath);
        CreateProductPolish(folderPath);
        CreateFinalReview(folderPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("All TaskData assets generated successfully!");
    }

    #region Day 1 Tasks

    private static void CreateFixLoginBug(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Fix Login Bug";
        data.taskDescription = "Fix the login authentication issue preventing users from accessing the system.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 40;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 0;
        data.minimumManagementSkill = 0;
        data.minimumSanity = 10;
        data.baseTurnDuration = 2;
        data.moneyReward = 300;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/FixLoginBug_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateDesignLogo(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Design Logo";
        data.taskDescription = "Create a new logo for the company rebranding initiative.";
        data.taskType = TaskData.TaskType.Creative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 50;
        data.minimumCommunicationSkill = 20;
        data.minimumManagementSkill = 0;
        data.minimumSanity = 10;
        data.baseTurnDuration = 2;
        data.moneyReward = 300;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/DesignLogo_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateClientMeeting(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Client Meeting";
        data.taskDescription = "Meet with key client to discuss project requirements and timeline.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 60;
        data.minimumManagementSkill = 40;
        data.minimumSanity = 20;
        data.baseTurnDuration = 1;
        data.moneyReward = 400;
        data.maxTurnsBeforeExpiry = 3;
        
        string assetPath = $"{folderPath}/ClientMeeting_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 2 Tasks

    private static void CreateDatabaseOptimization(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Database Optimization";
        data.taskDescription = "Optimize database queries to improve application performance.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 60;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 30;
        data.minimumManagementSkill = 0;
        data.minimumSanity = 15;
        data.baseTurnDuration = 3;
        data.moneyReward = 500;
        data.maxTurnsBeforeExpiry = 6;
        
        string assetPath = $"{folderPath}/DatabaseOptimization_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateUIRedesign(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "UI Redesign";
        data.taskDescription = "Redesign the user interface for better user experience.";
        data.taskType = TaskData.TaskType.Creative;
        data.requiredTechnicalSkill = 20;
        data.requiredCreativeSkill = 70;
        data.minimumCommunicationSkill = 40;
        data.minimumManagementSkill = 0;
        data.minimumSanity = 15;
        data.baseTurnDuration = 3;
        data.moneyReward = 450;
        data.maxTurnsBeforeExpiry = 6;
        
        string assetPath = $"{folderPath}/UIRedesign_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateTeamCoordination(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Team Coordination";
        data.taskDescription = "Coordinate between different teams to ensure project alignment.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 50;
        data.minimumManagementSkill = 60;
        data.minimumSanity = 20;
        data.baseTurnDuration = 2;
        data.moneyReward = 400;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/TeamCoordination_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 3 Tasks

    private static void CreateMobileAppDevelopment(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Mobile App Development";
        data.taskDescription = "Develop a mobile application for the new product line.";
        data.taskType = TaskData.TaskType.Mixed;
        data.requiredTechnicalSkill = 50;
        data.requiredCreativeSkill = 40;
        data.minimumCommunicationSkill = 30;
        data.minimumManagementSkill = 20;
        data.minimumSanity = 20;
        data.baseTurnDuration = 4;
        data.moneyReward = 600;
        data.maxTurnsBeforeExpiry = 8;
        
        string assetPath = $"{folderPath}/MobileAppDevelopment_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateWebsiteRedesign(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Website Redesign";
        data.taskDescription = "Complete redesign of the company website with modern aesthetics.";
        data.taskType = TaskData.TaskType.Mixed;
        data.requiredTechnicalSkill = 40;
        data.requiredCreativeSkill = 60;
        data.minimumCommunicationSkill = 40;
        data.minimumManagementSkill = 10;
        data.minimumSanity = 20;
        data.baseTurnDuration = 3;
        data.moneyReward = 550;
        data.maxTurnsBeforeExpiry = 7;
        
        string assetPath = $"{folderPath}/WebsiteRedesign_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateProjectPlanning(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Project Planning";
        data.taskDescription = "Plan the roadmap for the upcoming quarter's projects.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 70;
        data.minimumManagementSkill = 70;
        data.minimumSanity = 25;
        data.baseTurnDuration = 2;
        data.moneyReward = 500;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/ProjectPlanning_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 4 Tasks

    private static void CreateServerMigration(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Server Migration";
        data.taskDescription = "Migrate servers to new infrastructure with minimal downtime.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 70;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 40;
        data.minimumManagementSkill = 30;
        data.minimumSanity = 25;
        data.baseTurnDuration = 3;
        data.moneyReward = 700;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/ServerMigration_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateBrandIdentity(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Brand Identity";
        data.taskDescription = "Develop comprehensive brand identity guidelines.";
        data.taskType = TaskData.TaskType.Creative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 80;
        data.minimumCommunicationSkill = 50;
        data.minimumManagementSkill = 20;
        data.minimumSanity = 25;
        data.baseTurnDuration = 3;
        data.moneyReward = 650;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/BrandIdentity_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateStakeholderPresentation(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Stakeholder Presentation";
        data.taskDescription = "Present project progress to key stakeholders.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 80;
        data.minimumManagementSkill = 60;
        data.minimumSanity = 30;
        data.baseTurnDuration = 1;
        data.moneyReward = 600;
        data.maxTurnsBeforeExpiry = 2;
        
        string assetPath = $"{folderPath}/StakeholderPresentation_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 5 Tasks

    private static void CreateAPIIntegration(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "API Integration";
        data.taskDescription = "Integrate third-party APIs into the application.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 65;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 35;
        data.minimumManagementSkill = 0;
        data.minimumSanity = 30;
        data.baseTurnDuration = 3;
        data.moneyReward = 600;
        data.maxTurnsBeforeExpiry = 6;
        
        string assetPath = $"{folderPath}/APIIntegration_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateMarketingCampaign(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Marketing Campaign";
        data.taskDescription = "Design and launch a new marketing campaign.";
        data.taskType = TaskData.TaskType.Creative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 75;
        data.minimumCommunicationSkill = 60;
        data.minimumManagementSkill = 30;
        data.minimumSanity = 30;
        data.baseTurnDuration = 3;
        data.moneyReward = 580;
        data.maxTurnsBeforeExpiry = 6;
        
        string assetPath = $"{folderPath}/MarketingCampaign_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateBudgetReview(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Budget Review";
        data.taskDescription = "Review and optimize the department budget.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 60;
        data.minimumManagementSkill = 70;
        data.minimumSanity = 35;
        data.baseTurnDuration = 2;
        data.moneyReward = 550;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/BudgetReview_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 6 Tasks

    private static void CreateEcommercePlatform(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "E-commerce Platform";
        data.taskDescription = "Build a full e-commerce platform with payment integration.";
        data.taskType = TaskData.TaskType.Mixed;
        data.requiredTechnicalSkill = 70;
        data.requiredCreativeSkill = 50;
        data.minimumCommunicationSkill = 50;
        data.minimumManagementSkill = 40;
        data.minimumSanity = 35;
        data.baseTurnDuration = 5;
        data.moneyReward = 1000;
        data.maxTurnsBeforeExpiry = 10;
        
        string assetPath = $"{folderPath}/EcommercePlatform_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateProductLaunch(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Product Launch";
        data.taskDescription = "Coordinate the launch of a new product line.";
        data.taskType = TaskData.TaskType.Mixed;
        data.requiredTechnicalSkill = 40;
        data.requiredCreativeSkill = 70;
        data.minimumCommunicationSkill = 70;
        data.minimumManagementSkill = 60;
        data.minimumSanity = 35;
        data.baseTurnDuration = 4;
        data.moneyReward = 900;
        data.maxTurnsBeforeExpiry = 8;
        
        string assetPath = $"{folderPath}/ProductLaunch_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateTeamTraining(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Team Training";
        data.taskDescription = "Conduct training sessions for the development team.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 65;
        data.minimumManagementSkill = 65;
        data.minimumSanity = 40;
        data.baseTurnDuration = 2;
        data.moneyReward = 500;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/TeamTraining_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 7 Tasks

    private static void CreateSecurityAudit(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Security Audit";
        data.taskDescription = "Perform comprehensive security audit of the system.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 80;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 40;
        data.minimumManagementSkill = 30;
        data.minimumSanity = 40;
        data.baseTurnDuration = 3;
        data.moneyReward = 750;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/SecurityAudit_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateUserResearch(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "User Research";
        data.taskDescription = "Conduct user research to inform product decisions.";
        data.taskType = TaskData.TaskType.Creative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 65;
        data.minimumCommunicationSkill = 75;
        data.minimumManagementSkill = 30;
        data.minimumSanity = 40;
        data.baseTurnDuration = 3;
        data.moneyReward = 700;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/UserResearch_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateResourceAllocation(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Resource Allocation";
        data.taskDescription = "Allocate resources across multiple projects optimally.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 70;
        data.minimumManagementSkill = 80;
        data.minimumSanity = 45;
        data.baseTurnDuration = 2;
        data.moneyReward = 600;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/ResourceAllocation_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 8 Tasks

    private static void CreatePerformanceOptimization(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Performance Optimization";
        data.taskDescription = "Optimize application performance for better user experience.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 75;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 45;
        data.minimumManagementSkill = 20;
        data.minimumSanity = 45;
        data.baseTurnDuration = 3;
        data.moneyReward = 800;
        data.maxTurnsBeforeExpiry = 6;
        
        string assetPath = $"{folderPath}/PerformanceOptimization_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateFeatureImplementation(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Feature Implementation";
        data.taskDescription = "Implement new features based on user feedback.";
        data.taskType = TaskData.TaskType.Mixed;
        data.requiredTechnicalSkill = 60;
        data.requiredCreativeSkill = 55;
        data.minimumCommunicationSkill = 50;
        data.minimumManagementSkill = 35;
        data.minimumSanity = 45;
        data.baseTurnDuration = 4;
        data.moneyReward = 850;
        data.maxTurnsBeforeExpiry = 7;
        
        string assetPath = $"{folderPath}/FeatureImplementation_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateClientNegotiation(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Client Negotiation";
        data.taskDescription = "Negotiate contract terms with important client.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 85;
        data.minimumManagementSkill = 70;
        data.minimumSanity = 50;
        data.baseTurnDuration = 2;
        data.moneyReward = 700;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/ClientNegotiation_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 9 Tasks

    private static void CreateCriticalBugFix(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Critical Bug Fix";
        data.taskDescription = "Fix critical bug affecting production system immediately.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 85;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 50;
        data.minimumManagementSkill = 30;
        data.minimumSanity = 50;
        data.baseTurnDuration = 2;
        data.moneyReward = 900;
        data.maxTurnsBeforeExpiry = 3;
        
        string assetPath = $"{folderPath}/CriticalBugFix_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateMajorFeature(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Major Feature";
        data.taskDescription = "Develop a major new feature for the product.";
        data.taskType = TaskData.TaskType.Mixed;
        data.requiredTechnicalSkill = 70;
        data.requiredCreativeSkill = 65;
        data.minimumCommunicationSkill = 60;
        data.minimumManagementSkill = 45;
        data.minimumSanity = 50;
        data.baseTurnDuration = 4;
        data.moneyReward = 1000;
        data.maxTurnsBeforeExpiry = 6;
        
        string assetPath = $"{folderPath}/MajorFeature_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateExecutiveReport(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Executive Report";
        data.taskDescription = "Prepare and present quarterly report to executives.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 90;
        data.minimumManagementSkill = 80;
        data.minimumSanity = 55;
        data.baseTurnDuration = 2;
        data.moneyReward = 800;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/ExecutiveReport_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion

    #region Day 10 Tasks

    private static void CreateSystemUpgrade(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "System Upgrade";
        data.taskDescription = "Upgrade system to latest version with new capabilities.";
        data.taskType = TaskData.TaskType.Technical;
        data.requiredTechnicalSkill = 80;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 55;
        data.minimumManagementSkill = 35;
        data.minimumSanity = 55;
        data.baseTurnDuration = 3;
        data.moneyReward = 950;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/SystemUpgrade_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateProductPolish(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Product Polish";
        data.taskDescription = "Polish product UI/UX for final release.";
        data.taskType = TaskData.TaskType.Creative;
        data.requiredTechnicalSkill = 30;
        data.requiredCreativeSkill = 85;
        data.minimumCommunicationSkill = 65;
        data.minimumManagementSkill = 40;
        data.minimumSanity = 55;
        data.baseTurnDuration = 3;
        data.moneyReward = 900;
        data.maxTurnsBeforeExpiry = 5;
        
        string assetPath = $"{folderPath}/ProductPolish_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateFinalReview(string folderPath)
    {
        TaskData data = ScriptableObject.CreateInstance<TaskData>();
        data.taskName = "Final Review";
        data.taskDescription = "Conduct final review before product launch.";
        data.taskType = TaskData.TaskType.Administrative;
        data.requiredTechnicalSkill = 0;
        data.requiredCreativeSkill = 0;
        data.minimumCommunicationSkill = 80;
        data.minimumManagementSkill = 75;
        data.minimumSanity = 60;
        data.baseTurnDuration = 2;
        data.moneyReward = 850;
        data.maxTurnsBeforeExpiry = 4;
        
        string assetPath = $"{folderPath}/FinalReview_TaskData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    #endregion
}
