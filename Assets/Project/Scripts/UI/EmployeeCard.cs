using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// EmployeeCard UI component that inherits from DraggableCard
/// Represents the visual card for an employee with reference to EmployeeData template
/// </summary>
public class EmployeeCard : DraggableCard
{
    [Header("Employee Card Components")]
    public Employee employee; // Runtime employee component
    
    [Header("Status Display")]
    public EmployeeStatusDisplay statusDisplay;
    public Image frameImage;
    public Color normalFrameColor = Color.white;
    public Color selectedFrameColor = Color.yellow;
    public Color unavailableFrameColor = Color.gray;

    public Employee Employee => employee;

    protected override void Start()
    {
        base.Start();
        UpdateCardDisplay();
    }

    public override void UpdateCardDisplay()
    {
        if (EmployeeData == null) return;

        // Update basic card display using template
        if (employeeNameText != null)
            employeeNameText.text = EmployeeData.employeeName;

        if (technicalSkillText != null)
            technicalSkillText.text = $"Tech: {EmployeeData.technicalSkill}";

        if (creativeSkillText != null)
            creativeSkillText.text = $"Creative: {EmployeeData.creativeSkill}";

        if (communicationSkillText != null)
            communicationSkillText.text = $"Comm: {EmployeeData.communicationSkill}";

        if (managementSkillText != null)
            managementSkillText.text = $"Mgmt: {EmployeeData.managementSkill}";

        if (portraitImage != null && EmployeeData.employeePortrait != null)
            portraitImage.sprite = EmployeeData.employeePortrait;

        // Update sanity display using runtime employee data
        if (sanityText != null && employee != null)
            sanityText.text = $"Sanity: {employee.CurrentSanity}";

        // Update status display
        if (statusDisplay != null && employee != null)
        {
            statusDisplay.SetEmployee(employee);
        }

        if (employee != null)
        {
            employee.InitializeFromTemplate(EmployeeData);
        }

        // Update frame color based on availability
        UpdateFrameColor();
    }

    private void UpdateFrameColor()
    {
        if (frameImage == null) return;

        if (employee == null)
        {
            frameImage.color = unavailableFrameColor;
        }
        else if (!employee.CanWork)
        {
            frameImage.color = unavailableFrameColor;
        }
        else if (IsBeingDragged)
        {
            frameImage.color = selectedFrameColor;
        }
        else
        {
            frameImage.color = normalFrameColor;
        }
    }

    public void UpdateSanityDisplay()
    {
        if (sanityText != null && employee != null)
            sanityText.text = $"Sanity: {employee.CurrentSanity}";

        UpdateFrameColor();
    }

    public void UpdateStatusDisplay()
    {
        if (statusDisplay != null && employee != null)
        {
            statusDisplay.UpdateStatusDisplay();
        }
        
        UpdateFrameColor();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        UpdateFrameColor();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        UpdateFrameColor();
    }

    public override void SetCurrentSlot(TaskSlot slot)
    {
        base.SetCurrentSlot(slot);
        
        // Update employee runtime state
        if (employee != null)
        {
            if (slot != null && slot.TaskData != null)
            {
                employee.AssignToTask(slot.TaskData);
            }
            else
            {
                employee.RemoveFromTask();
            }
        }
    }

    public void ApplyTurnEffects()
    {
        if (employee != null)
        {
            employee.ProcessTurn();
            UpdateCardDisplay();
        }
    }

    public bool CanWork()
    {
        return employee != null && employee.CanWork;
    }

    public void GoToRestArea()
    {
        if (employee != null)
        {
            employee.GoToRestArea();
            UpdateCardDisplay();
        }
    }

    public void LeaveRestArea()
    {
        if (employee != null)
        {
            employee.LeaveRestArea();
            UpdateCardDisplay();
        }
    }

    private void Update()
    {
        // Update frame color continuously for status changes
        if (Time.frameCount % 30 == 0) // Every 30 frames
        {
            UpdateFrameColor();
        }
    }

    private void OnDestroy()
    {
        // Clean up runtime employee component
        if (employee != null)
        {
            employee.Cleanup();
        }
    }
}
