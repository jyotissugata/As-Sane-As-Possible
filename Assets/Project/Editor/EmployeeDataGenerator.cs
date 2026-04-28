using UnityEngine;
using UnityEditor;
using System.IO;

public class EmployeeDataGenerator
{
    [MenuItem("ASAP/Generate All EmployeeData")]
    public static void GenerateAllEmployeeData()
    {
        string folderPath = "Assets/Project/Data/EmployeeData";
        
        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // Generate all employees
        CreateAlexChen(folderPath);
        CreateSarahMartinez(folderPath);
        CreatePeoplePerson(folderPath);
        CreateDavidKim(folderPath);
        CreateEmmaWilson(folderPath);
        CreateMichaelBrown(folderPath);
        CreateJessicaLee(folderPath);
        CreateRobertDavis(folderPath);
        CreateLisaAnderson(folderPath);
        CreateJamesTaylor(folderPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("All EmployeeData assets generated successfully!");
    }

    private static void CreateAlexChen(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Alex Chen";
        data.employeeDescription = "Expert programmer who excels at technical tasks but struggles with communication.";
        data.technicalSkill = 90;
        data.creativeSkill = 30;
        data.communicationSkill = 40;
        data.managementSkill = 35;
        data.sanity = 80;
        data.salary = 150;
        
        string assetPath = $"{folderPath}/AlexChen_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateSarahMartinez(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Sarah Martinez";
        data.employeeDescription = "Brilliant designer with artistic flair but weak in technical areas.";
        data.technicalSkill = 25;
        data.creativeSkill = 95;
        data.communicationSkill = 70;
        data.managementSkill = 40;
        data.sanity = 75;
        data.salary = 140;
        
        string assetPath = $"{folderPath}/SarahMartinez_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreatePeoplePerson(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "The People Person";
        data.employeeDescription = "Excellent communicator who bridges gaps between teams.";
        data.technicalSkill = 40;
        data.creativeSkill = 50;
        data.communicationSkill = 95;
        data.managementSkill = 60;
        data.sanity = 85;
        data.salary = 130;
        
        string assetPath = $"{folderPath}/PeoplePerson_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateDavidKim(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "David Kim";
        data.employeeDescription = "Natural leader who coordinates teams effectively.";
        data.technicalSkill = 45;
        data.creativeSkill = 40;
        data.communicationSkill = 75;
        data.managementSkill = 90;
        data.sanity = 70;
        data.salary = 160;
        
        string assetPath = $"{folderPath}/DavidKim_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateEmmaWilson(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Emma Wilson";
        data.employeeDescription = "Solid performer across all areas with no major weaknesses.";
        data.technicalSkill = 65;
        data.creativeSkill = 60;
        data.communicationSkill = 70;
        data.managementSkill = 65;
        data.sanity = 80;
        data.salary = 120;
        
        string assetPath = $"{folderPath}/EmmaWilson_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateMichaelBrown(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Michael Brown";
        data.employeeDescription = "Talented but fragile employee who burns out quickly.";
        data.technicalSkill = 85;
        data.creativeSkill = 80;
        data.communicationSkill = 60;
        data.managementSkill = 55;
        data.sanity = 40;
        data.salary = 145;
        
        string assetPath = $"{folderPath}/MichaelBrown_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateJessicaLee(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Jessica Lee";
        data.employeeDescription = "New hire with potential but needs training.";
        data.technicalSkill = 35;
        data.creativeSkill = 40;
        data.communicationSkill = 50;
        data.managementSkill = 30;
        data.sanity = 90;
        data.salary = 80;
        
        string assetPath = $"{folderPath}/JessicaLee_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateRobertDavis(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Robert Davis";
        data.employeeDescription = "Experienced professional with excellent all-around skills.";
        data.technicalSkill = 80;
        data.creativeSkill = 75;
        data.communicationSkill = 85;
        data.managementSkill = 80;
        data.sanity = 75;
        data.salary = 200;
        
        string assetPath = $"{folderPath}/RobertDavis_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateLisaAnderson(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "Lisa Anderson";
        data.employeeDescription = "Rare combination of technical and creative skills.";
        data.technicalSkill = 80;
        data.creativeSkill = 85;
        data.communicationSkill = 50;
        data.managementSkill = 45;
        data.sanity = 70;
        data.salary = 155;
        
        string assetPath = $"{folderPath}/LisaAnderson_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    private static void CreateJamesTaylor(string folderPath)
    {
        EmployeeData data = ScriptableObject.CreateInstance<EmployeeData>();
        data.employeeName = "James Taylor";
        data.employeeDescription = "Excellent at supporting roles with strong communication and management.";
        data.technicalSkill = 30;
        data.creativeSkill = 35;
        data.communicationSkill = 80;
        data.managementSkill = 75;
        data.sanity = 85;
        data.salary = 110;
        
        string assetPath = $"{folderPath}/JamesTaylor_EmployeeData.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }
}
