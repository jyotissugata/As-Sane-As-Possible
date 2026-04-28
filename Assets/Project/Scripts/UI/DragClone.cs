using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple visual clone for drag operations
/// Only displays employee data, no logic or drag handling
/// </summary>
public class DragClone : MonoBehaviour
{
    [Header("UI Components")]
    public Image cardImage;
    public TextMeshProUGUI employeeNameText;
    public TextMeshProUGUI technicalSkillText;
    public TextMeshProUGUI creativeSkillText;
    public TextMeshProUGUI communicationSkillText;
    public TextMeshProUGUI managementSkillText;
    public TextMeshProUGUI sanityText;
    public Image portraitImage;

    private EmployeeData employeeData;

    public void Initialize(EmployeeData data)
    {
        employeeData = data;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (employeeData == null) return;

        if (employeeNameText != null)
            employeeNameText.text = employeeData.employeeName;

        if (technicalSkillText != null)
            technicalSkillText.text = $"Tech: {employeeData.technicalSkill}";

        if (creativeSkillText != null)
            creativeSkillText.text = $"Creative: {employeeData.creativeSkill}";

        if (communicationSkillText != null)
            communicationSkillText.text = $"Comm: {employeeData.communicationSkill}";

        if (managementSkillText != null)
            managementSkillText.text = $"Mgmt: {employeeData.managementSkill}";

        if (sanityText != null)
            sanityText.text = $"Sanity: {employeeData.sanity}";

        if (portraitImage != null && employeeData.employeePortrait != null)
            portraitImage.sprite = employeeData.employeePortrait;

        //if (cardImage != null && employeeData.cardColor != default)
        //    cardImage.color = employeeData.cardColor;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
