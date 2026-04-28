# ASAP Game Scene Setup Guide

## 🎯 Required Scene Objects

### 1. **Core Managers** (Create Empty GameObjects)
```
GameManager (Add GameManager script)
├─ EconomyManager (Add EconomyManager script)
├─ CalendarManager (Add CalendarManager script)
├─ TaskDiscoveryManager (Add TaskDiscoveryManager script)
├─ ShopManager (Add ShopManager script)
└─ GameServiceLocator (Add GameServiceLocator script)
```

### 2. **UI Canvas** (Create Canvas)
```
Canvas
├─ GameUI_Panel
│   ├─ Economy_Display
│   │   ├─ Money_Text (TextMeshPro - TMPro)
│   │   ├─ Target_Text (TextMeshPro - TMPro)
│   │   └─ Progress_Slider (Slider)
│   │
│   ├─ GameInfo_Display
│   │   ├─ Day_Text (TextMeshPro - TMPro)
│   │   ├─ Turn_Text (TextMeshPro - TMPro)
│   │   └─ Time_Text (TextMeshPro - TMPro)
│   │
│   ├─ TurnControl_Panel
│   │   ├─ NextTurn_Button (Button)
│   │   ├─ Pause_Button (Button)
│   │   └─ Speed_Button (Button)
│   │
│   ├─ Shop_Panel
│   │   ├─ Shop_Toggle_Button (Button)
│   │   ├─ Shop_Container (Vertical Layout Group)
│   │   └─ ShopItem_Prefab (Create as Prefab)
│   │       ├─ Item_Image (Image)
│   │       ├─ Name_Text (TextMeshPro - TMPro)
│   │       ├─ Desc_Text (TextMeshPro - TMPro)
│   │       ├─ Cost_Text (TextMeshPro - TMPro)
│   │       └─ Purchase_Button (Button)
│   │
│   └─ Result_Panel
│       ├─ TaskResults_Container
│       └─ EmployeeResults_Container
│
└─ TaskArea_Panel
    ├─ TaskSlot_Container (Grid Layout Group)
    └─ TaskSlot_Prefab (Create as Prefab)
        ├─ Slot_Image (Image)
        ├─ TaskName_Text (TextMeshPro - TMPro)
        ├─ TaskDesc_Text (TextMeshPro - TMPro)
        ├─ TaskIcon_Image (Image)
        ├─ EmployeePreview_Image (Image)
        ├─ TurnsRemaining_Text (TextMeshPro - TMPro)
        ├─ TurnsUntilExpiry_Text (TextMeshPro - TMPro)
        └─ ExpiryWarning_Image (Image)
```

### 3. **Rest Area** (Create Empty GameObject)
```
RestArea (Add RestAreaSlot script)
├─ RestArea_Image (Image)
├─ RestAreaTitle_Text (TextMeshPro - TMPro)
├─ Capacity_Text (TextMeshPro - TMPro)
├─ EmployeePreview_Image (Image)
└─ DropZone (Image - Set Alpha to 0)
```

### 4. **Employee Cards** (Create as Prefab)
```
EmployeeCard_Prefab (Add DraggableCard script)
├─ Card_Image (Image)
├─ Portrait_Image (Image)
├─ EmployeeName_Text (TextMeshPro - TMPro)
├─ TechnicalSkill_Text (TextMeshPro - TMPro)
├─ CreativeSkill_Text (TextMeshPro - TMPro)
├─ Sanity_Text (TextMeshPro - TMPro)
└─ StatusDisplay_Panel
    ├─ Status_Icon (Image)
    ├─ Status_Text (TextMeshPro - TMPro)
    └─ Status_Background (Image)
```

## 🔧 Script Component Setup

### **GameManager**
- Drag EmployeeData assets to `availableEmployees` array
- Drag TaskData assets to `availableTasks` array

### **EconomyManager**
- Set `startingMoney = 1000`
- Set `targetMoney = 10000`
- Set `maxGameDays = 30`
- Set `dailyOperationalCost = 100`
- Assign UI references to TextMeshPro and Slider components

### **CalendarManager**
- Set `currentDate` to January 1, 2026
- Assign UI references for turn control

### **TaskDiscoveryManager**
- Assign `taskSlotContainer` (the TaskSlot_Container)
- Assign `taskSlotPrefab` (the TaskSlot_Prefab)
- Create TaskTemplate assets and assign to `taskTemplates`

### **ShopManager**
- Assign `shopContainer` (the Shop_Container)
- Assign `shopItemPrefab` (the ShopItem_Prefab)
- Assign `playerMoneyText` (Money_Text)

### **RestAreaSlot**
- Set `maxCapacity = 4`
- Set `sanityRecoveryPerTurn = 15`
- Assign UI components (images, texts)

### **TaskSlot**
- Assign UI components (images, texts, etc.)
- Set colors for different states

### **DraggableCard**
- Assign `canvas` reference
- Set `dragScale = 1.1`
- Assign UI components

## 🎮 AlpacaStudio Template Integration

### **For Existing UI Panels:**
1. Add `AlpacaStudioUIAdapter` component to existing panels
2. Set `adapterType` appropriately:
   - EconomyDisplay → Economy panels
   - GameInfo → Game info panels  
   - Shop → Shop panels
   - TaskList → Task list panels
   - ResultPanel → Result panels
3. Assign the UI references in the adapter component

### **Example Integration:**
```
ExistingMoneyPanel (Add AlpacaStudioUIAdapter)
├─ Adapter Type: EconomyDisplay
├─ Money Display: [Drag your Money_Text here]
├─ Target Display: [Drag your Target_Text here]
└─ Progress Slider: [Drag your Progress_Slider here]
```

## 📁 Asset Creation

### **ScriptableObject Assets**
1. Right-click in Project window → Create → ASAP → EmployeeData
2. Right-click in Project window → Create → ASAP → TaskData  
3. Right-click in Project window → Create → ASAP → TaskTemplate

### **Prefabs**
1. Create GameObjects with required components
2. Drag from Hierarchy to Project window to create prefab
3. Delete original from hierarchy (use prefab instances)

## 🎯 Testing Checklist

- [ ] All managers are in scene and have scripts assigned
- [ ] UI components are properly linked to scripts
- [ ] Prefabs are created and assigned to managers
- [ ] EmployeeData and TaskData assets are created
- [ ] GameServiceLocator has manual references assigned
- [ ] Canvas has proper event system (EventSystem object)
- [ ] Graphic Raycaster is on Canvas
- [ ] Test drag and drop functionality
- [ ] Test turn progression
- [ ] Test economy system

## 🚀 Quick Start

1. Create new scene
2. Add all manager objects
3. Create UI structure
4. Create prefabs
5. Assign all references
6. Create ScriptableObject assets
7. Test basic functionality

The game should now be fully functional with all systems integrated!
