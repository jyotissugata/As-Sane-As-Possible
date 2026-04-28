using UnityEngine;

[CreateAssetMenu(fileName = "NewEmployeeData", menuName = "ASAP/EmployeeData")]
public class EmployeeData : ScriptableObject
{
    [Header("Employee Information")]
    public string employeeName;
    public Sprite employeePortrait;
    public string employeeDescription;

    [Header("Employee Stats")]
    [Range(0, 100)]
    public int technicalSkill = 50;
    
    [Range(0, 100)]
    public int creativeSkill = 50;

    [Range(0, 100)]
    public int communicationSkill = 70;
    
    [Range(0, 100)]
    public int managementSkill = 60;
    
    [Range(0, 100)]
    public int sanity = 75;

    [Header("Economy")]
    public int salary = 100;

    // Template-only data - no dynamic state
    // All dynamic state is now handled by the Employee component

    // Template-only methods - no dynamic state management
    // All dynamic state is now handled by the Employee component

    public bool CanPerformTask(TaskData task)
    {
        // Template-only check - runtime availability handled by Employee component
        return technicalSkill >= task.requiredTechnicalSkill && 
               creativeSkill >= task.requiredCreativeSkill &&
               sanity >= task.minimumSanity &&
               communicationSkill >= task.minimumCommunicationSkill &&
               managementSkill >= task.minimumManagementSkill;
    }

    // Template-only methods - no dynamic state management
    // All dynamic state is now handled by the Employee component
}
