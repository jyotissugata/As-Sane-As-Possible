# ResultPanel Setup Guide

This guide explains how to set up the ResultPanel in your Unity scene to display game end results and turn summaries.

## **1. Scene Hierarchy Structure**

Create the following hierarchy under your Canvas:

```
Canvas
└─ ResultPanel (GameObject - Panel)
   ├─ ResultPanel_Script (ResultPanel component)
   ├─ Panel_Background (Image - Dark semi-transparent background)
   ├─ Header_Panel (Vertical Layout Group)
   │  ├─ TurnInfo_Text (TextMeshPro - "Turn X - Morning")
   │  ├─ DateInfo_Text (TextMeshPro - "01 January 2026")
   │  └─ GameResult_Text (TextMeshPro - "VICTORY!" / "BANKRUPTCY!" etc.)
   ├─ TaskResults_Panel (ScrollRect)
   │  ├─ Viewport
   │  │  └─ TaskResults_Container (Vertical Layout Group)
   │  └─ Scrollbar
   ├─ EmployeeResults_Panel (ScrollRect)
   │  ├─ Viewport
   │  │  └─ EmployeeResults_Container (Vertical Layout Group)
   │  └─ Scrollbar
   └─ Buttons_Panel (Horizontal Layout Group)
      ├─ Continue_Button (Button)
      │  └─ Continue_Text (TextMeshPro - "Continue")
      └─ DetailedView_Button (Button)
         └─ DetailedView_Text (TextMeshPro - "Detailed View")
```

## **2. ResultPanel Component Settings**

Add the `ResultPanel` script to the `ResultPanel` GameObject:

```csharp
// In Inspector for ResultPanel:
Panel Components:
- Result Panel: [Drag ResultPanel GameObject]
- Turn Info Text: [Drag TurnInfo_Text]
- Date Info Text: [Drag DateInfo_Text]
- Task Results Container: [Drag TaskResults_Container Transform]
- Employee Results Container: [Drag EmployeeResults_Container Transform]

Prefabs:
- Task Result Item Prefab: [Drag TaskResultItem prefab]
- Employee Result Item Prefab: [Drag EmployeeResultItem prefab]

Buttons:
- Continue Button: [Drag Continue_Button]
- Detailed View Button: [Drag DetailedView_Button]
```

## **3. Required Prefabs**

### **TaskResultItem Prefab**
Create a prefab for displaying individual task results:

```
TaskResultItem (GameObject)
├─ TaskResultItem_Script (TaskResultItem component)
├─ Background (Image)
├─ TaskName_Text (TextMeshPro)
├─ EmployeeName_Text (TextMeshPro)
├─ Progress_Text (TextMeshPro)
├─ Status_Text (TextMeshPro - "Completed" / "In Progress" / "Expired")
└─ Stats_Panel (Horizontal Layout Group)
   ├─ SanityChange_Text (TextMeshPro - "-2")
   ├─ CommChange_Text (TextMeshPro - "-1")
   └─ MgmtChange_Text (TextMeshPro - "-1")
```

**TaskResultItem Component Settings:**
```csharp
// In Inspector for TaskResultItem:
- Task Name Text: [Drag TaskName_Text]
- Employee Name Text: [Drag EmployeeName_Text]
- Progress Text: [Drag Progress_Text]
- Status Text: [Drag Status_Text]
- Sanity Change Text: [Drag SanityChange_Text]
- Communication Change Text: [Drag CommChange_Text]
- Management Change Text: [Drag MgmtChange_Text]
```

### **EmployeeResultItem Prefab**
Create a prefab for displaying individual employee results:

```
EmployeeResultItem (GameObject)
├─ EmployeeResultItem_Script (EmployeeResultItem component)
├─ Background (Image)
├─ EmployeeName_Text (TextMeshPro)
├─ Status_Text (TextMeshPro - "Working" / "Resting" / "Available")
└─ Stats_Panel (Horizontal Layout Group)
   ├─ Sanity_Stats (Vertical Layout Group)
   │  ├─ SanityBefore_Text (TextMeshPro)
   │  └─ SanityAfter_Text (TextMeshPro)
   ├─ Comm_Stats (Vertical Layout Group)
   │  ├─ CommBefore_Text (TextMeshPro)
   │  └─ CommAfter_Text (TextMeshPro)
   └─ Mgmt_Stats (Vertical Layout Group)
      ├─ MgmtBefore_Text (TextMeshPro)
      └─ MgmtAfter_Text (TextMeshPro)
```

**EmployeeResultItem Component Settings:**
```csharp
// In Inspector for EmployeeResultItem:
- Employee Name Text: [Drag EmployeeName_Text]
- Status Text: [Drag Status_Text]
- Sanity Before Text: [Drag SanityBefore_Text]
- Sanity After Text: [Drag SanityAfter_Text]
- Communication Before Text: [Drag CommBefore_Text]
- Communication After Text: [Drag CommAfter_Text]
- Management Before Text: [Drag MgmtBefore_Text]
- Management After Text: [Drag MgmtAfter_Text]
```

## **4. Unity Editor Setup Steps**

### **Step 1: Create the ResultPanel GameObject**
1. Right-click in your Canvas → UI → Panel
2. Rename to "ResultPanel"
3. Set RectTransform to cover the entire screen (Anchor: Stretch, Offset: 0)
4. Set Panel color to a dark semi-transparent color (e.g., RGBA: 0, 0, 0, 0.8)
5. Add the `ResultPanel` script component

### **Step 2: Create UI Elements**
1. Create a child Panel for the header (Header_Panel)
   - Add Vertical Layout Group component
   - Add Content Size Fitter (Vertical Fit: Preferred Size)
2. Create TextMeshPro elements for turn info, date info, and game result
3. Create two ScrollRect panels for task and employee results
   - Each ScrollRect needs a Viewport with a Container
   - Add Vertical Layout Group to each Container
4. Create a Buttons_Panel with two buttons

### **Step 3: Configure the ResultPanel Script**
1. Drag the appropriate UI elements to the script fields:
   - Result Panel: The main ResultPanel GameObject
   - Turn Info Text: TurnInfo_Text
   - Date Info Text: DateInfo_Text
   - Task Results Container: TaskResults_Container Transform
   - Employee Results Container: EmployeeResults_Container Transform
2. Create and assign the result item prefabs (see Section 3)
3. Assign the button references

### **Step 4: Create Result Item Prefabs**
1. Create the TaskResultItem prefab following the hierarchy in Section 3
2. Add the `TaskResultItem` script and assign its text references
3. Save as a prefab in your Prefabs folder
4. Repeat for EmployeeResultItem prefab
5. Assign these prefabs to the ResultPanel script

### **Step 5: Test the Setup**
1. Ensure `EconomyManager.Instance` is available in the scene
2. Ensure `GameServiceLocator` is properly configured
3. Ensure `CalendarManager.Instance` is available
4. Run the game and trigger a game end condition:
   - Reach target money (Victory)
   - Go bankrupt (Bankruptcy)
   - Run out of days (TimeOut)
5. Verify the ResultPanel appears with correct information

## **5. Important Notes**

- **Singleton Pattern**: ResultPanel uses a singleton pattern. Only one instance should exist in the scene.
- **Event Subscription**: The ResultPanel automatically subscribes to `EconomyManager.OnGameEnded` in `Start()`.
- **Service Locator**: The panel uses `GameServiceLocator` to collect task and employee data.
- **Automatic Display**: The panel automatically shows when the game ends via the event system.
- **Manual Display**: You can also call `ResultPanel.Instance.ShowTurnResults()` to show turn summaries.

## **6. Troubleshooting**

**Problem**: ResultPanel doesn't appear when game ends
- **Solution**: Check that `EconomyManager.Instance` is not null and the event is being invoked

**Problem**: Results are empty
- **Solution**: Ensure `GameServiceLocator` has registered TaskSlots and Employees

**Problem**: Prefabs don't instantiate correctly
- **Solution**: Verify the prefab references are assigned in the ResultPanel inspector

**Problem**: Text doesn't update
- **Solution**: Check that all TextMeshPro references are correctly assigned in the result item prefabs
