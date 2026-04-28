# LeadershipManager Setup Guide

This guide explains how to set up the LeadershipManager in your Unity scene to display leadership messages, feedback, and titles.

## **1. Scene Hierarchy Structure**

Create the following hierarchy under your Canvas:

```
Canvas
├─ LeadershipManager (GameObject - Empty)
│  └─ LeadershipManager_Script (LeadershipManager component)
│
├─ MissionBrief_Panel (GameObject - Panel)
│  ├─ Panel_Background (Image - Dark semi-transparent background)
│  ├─ Title_Panel (Vertical Layout Group)
│  │  └─ MissionTitle_Text (TextMeshPro - "Selamat Datang, Lead!")
│  ├─ Message_Panel (Vertical Layout Group)
│  │  └─ MissionMessage_Text (TextMeshPro - Intro message)
│  └─ Button_Panel (Horizontal Layout Group)
│     └─ MissionClose_Button (Button)
│        └─ Close_Text (TextMeshPro - "Mulai")
│
└─ ResultPanel (Existing)
   ├─ LeadershipFeedback_Text (TextMeshPro - Dynamic feedback)
   └─ LeadershipTitle_Text (TextMeshPro - Leadership title)
```

## **2. LeadershipManager Component Settings**

Add the `LeadershipManager` script to the `LeadershipManager` GameObject:

```csharp
// In Inspector for LeadershipManager:

Mission Brief System:
- Mission Brief Panel: [Drag MissionBrief_Panel GameObject]
- Mission Title Text: [Drag MissionTitle_Text]
- Mission Message Text: [Drag MissionMessage_Text]
- Mission Close Button: [Drag MissionClose_Button]

Mission Message Settings:
- Intro Title: "Selamat Datang, Lead!" (editable)
- Intro Message: [Full intro message text] (editable via TextArea)

Dynamic Feedback System:
- Leadership Feedback Text: [Drag LeadershipFeedback_Text in ResultPanel]

Feedback Messages (all editable via TextArea):
- Positive Feedback: "Kepemimpinan yang Bagus! Anda menempatkan orang yang tepat di tempat yang tepat."
- Warning Feedback: "Peringatan Kepemimpinan: Tim Anda mulai burnout. Pemimpin yang hebat tahu kapan harus memberikan waktu istirahat."
- Rest Feedback: "Investasi pada manusia: Memulihkan tim akan meningkatkan produktivitas jangka panjang."
- Default Feedback: "Terus kembangkan kemampuan delegasi Anda untuk hasil yang lebih optimal."

Post-Game Scoring System:
- Leadership Title Text: [Drag LeadershipTitle_Text in ResultPanel]

Leadership Titles (all editable via TextArea):
- High Efficiency Title: "The Tyrant - Target uang tercapai, tapi >50% tim Sanity-nya 0"
- High Sanity Title: "The People Pleaser - Sanity tim penuh, tapi target uang tidak tercapai/bangkrut"
- Balanced Title: "The Balanced Leader (ASAP) - Target uang tercapai dan rata-rata Sanity tim di atas 50%"
- Pragmatic Title: "The Pragmatic Leader - Target tercapai dengan Sanity tim rendah"
- Caring Title: "The Caring Leader - Tim sehat tapi bisnis perlu perbaikan"
- Developing Title: "The Developing Leader - Terus belajar menyeimbangkan produktivitas dan kesejahteraan tim"
```

## **3. Integration with ResultPanel**

Update `ResultPanel.cs` to call LeadershipManager functions:

### **Step 1: Add LeadershipManager call in DisplayResults()**

```csharp
private void DisplayResults()
{
    // Update header info
    if (turnInfoText != null && CalendarManager.Instance != null)
    {
        turnInfoText.text = $"Turn {CalendarManager.Instance.currentTurn} - {CalendarManager.Instance.GetTimeString()}";
    }

    if (dateInfoText != null && CalendarManager.Instance != null)
    {
        dateInfoText.text = CalendarManager.Instance.CurrentDate.ToString("dd MMMM yyyy");
    }

    // Update leadership feedback
    if (LeadershipManager.Instance != null)
    {
        LeadershipManager.Instance.UpdateTurnFeedback();
    }

    // Clear previous results
    ClearResultsDisplay();

    // Display task results
    DisplayTaskResults();

    // Display employee results
    DisplayEmployeeResults();
}
```

### **Step 2: Add LeadershipManager call in OnGameEnded()**

```csharp
private void OnGameEnded(GameResult result)
{
    Debug.Log($"ResultPanel received game end event: {result}");
    
    // Collect and display final results
    CollectResults();
    DisplayResults();
    
    // Update leadership title
    if (LeadershipManager.Instance != null)
    {
        LeadershipManager.Instance.UpdateLeadershipTitle();
    }
    
    // Update turn info with game result
    if (turnInfoText != null)
    {
        string resultText = result switch
        {
            GameResult.Victory => "VICTORY!",
            GameResult.Bankruptcy => "BANKRUPTCY!",
            GameResult.TimeOut => "TIME'S UP!",
            GameResult.Quit => "GAME QUIT",
            _ => "GAME OVER"
        };
        turnInfoText.text = resultText;
    }
    
    // Show the result panel
    if (resultPanel != null)
        resultPanel.SetActive(true);
}
```

## **4. Unity Editor Setup Steps**

### **Step 1: Create LeadershipManager GameObject**
1. Right-click in your Hierarchy → Create Empty
2. Rename to "LeadershipManager"
3. Add the `LeadershipManager` script component

### **Step 2: Create Mission Brief UI**
1. Right-click in Canvas → UI → Panel
2. Rename to "MissionBrief_Panel"
3. Set RectTransform to cover the entire screen (Anchor: Stretch, Offset: 0)
4. Set Panel color to a dark semi-transparent color (e.g., RGBA: 0, 0, 0, 0.9)
5. Create child UI elements:
   - Title Panel with MissionTitle_Text
   - Message Panel with MissionMessage_Text
   - Button Panel with MissionClose_Button

### **Step 3: Add Leadership Feedback Text to ResultPanel**
1. Open your existing ResultPanel hierarchy
2. Add a new TextMeshProUGUI element for leadership feedback
3. Name it "LeadershipFeedback_Text"
4. Position it appropriately in the ResultPanel layout

### **Step 4: Add Leadership Title Text to ResultPanel**
1. In ResultPanel hierarchy, add a new TextMeshProUGUI element for leadership title
2. Name it "LeadershipTitle_Text"
3. Position it below the turn info text

### **Step 5: Configure LeadershipManager Script**
1. Drag the MissionBrief_Panel to the Mission Brief Panel field
2. Drag MissionTitle_Text to the Mission Title Text field
3. Drag MissionMessage_Text to the Mission Message Text field
4. Drag MissionClose_Button to the Mission Close Button field
5. Drag LeadershipFeedback_Text to the Leadership Feedback Text field
6. Drag LeadershipTitle_Text to the Leadership Title Text field

### **Step 6: Customize Messages**
1. Edit the Intro Title and Intro Message in the Inspector
2. Customize all feedback messages (Positive, Warning, Rest, Default)
3. Customize all leadership titles (High Efficiency, High Sanity, Balanced, etc.)
4. All text fields use [TextArea] for easy multi-line editing

### **Step 7: Integrate with ResultPanel**
1. Open `ResultPanel.cs`
2. Add the code changes from Section 3
3. Save the script

## **5. Important Notes**

- **Singleton Pattern**: LeadershipManager uses a singleton pattern. Only one instance should exist in the scene.
- **Automatic Display**: The Mission Brief automatically shows when the game starts via `Start()`.
- **Manual Calls**: Dynamic feedback and leadership titles are called from ResultPanel.
- **Editable Text**: All message strings use [TextArea] attribute for easy editing in Unity Inspector.
- **Priority System**: Feedback messages have priority (Warning > Rest > Positive > Default).

## **6. How It Works**

### **Mission Brief**
- Shows automatically when game starts
- Displays customizable title and message
- Closes when player clicks the close button

### **Dynamic Feedback**
- Called every time Result Panel appears (turn change)
- Checks team conditions:
  - Low sanity (< 30) → Warning feedback
  - Employees in rest area → Rest feedback
  - Good skill matching → Positive feedback
  - Default → Default feedback

### **Post-Game Leadership Title**
- Called when game ends (Win/Loss)
- Calculates based on:
  - Financial success (reached target money)
  - Team sanity (average and zero sanity percentage)
- Returns appropriate leadership category

## **7. Troubleshooting**

**Problem**: Mission Brief doesn't appear
- **Solution**: Check that LeadershipManager GameObject is in the scene and script is attached

**Problem**: Feedback text doesn't update
- **Solution**: Ensure ResultPanel.cs calls `LeadershipManager.Instance.UpdateTurnFeedback()`

**Problem**: Leadership title doesn't show
- **Solution**: Ensure ResultPanel.cs calls `LeadershipManager.Instance.UpdateLeadershipTitle()` in OnGameEnded

**Problem**: Text references are null
- **Solution**: Verify all TextMeshProUGUI references are assigned in the LeadershipManager inspector

**Problem**: Messages don't change
- **Solution**: Edit the message strings in the Inspector (they use [TextArea] for easy editing)
