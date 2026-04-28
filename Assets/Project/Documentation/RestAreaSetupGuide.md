# RestArea and RestAreaSlot Setup Guide

## 🎯 Architecture Overview

The new architecture separates **template data** (ScriptableObjects) from **runtime state** (MonoBehaviour components):

### **Template Data (ScriptableObjects):**
- **EmployeeData**: Static employee info (name, base skills, portrait)
- **TaskData**: Static task info (name, requirements, rewards, duration)

### **Runtime Components (MonoBehaviours):**
- **EmployeeCard**: Inherits from DraggableCard, represents UI card
- **Employee**: Runtime employee state (sanity, status, availability)
- **Task**: Runtime task state (progress, assigned employee, expiry)
- **RestAreaSlot**: Manages multiple resting employees

## 🔧 RestArea Setup in Scene

### **1. Create RestArea GameObject**
```
RestArea (Empty GameObject)
├─ RestAreaSlot (Add RestAreaSlot script)
│   ├─ RestArea_Image (Image - Background)
│   ├─ RestAreaTitle_Text (TextMeshPro - "Rest Area")
│   ├─ Capacity_Text (TextMeshPro - "0/4")
│   ├─ EmployeePreview_Image (Image - Shows first employee)
│   └─ DropZone (Image - Transparent drop area)
│       ├─ BoxCollider2D (For drop detection)
│       └─ RestAreaSlot (Script component)
```

### **2. RestAreaSlot Component Settings**
```csharp
// In Inspector for RestAreaSlot:
Rest Area Settings:
- Max Capacity: 4
- Sanity Recovery Per Turn: 15
- Communication Recovery Per Turn: 3
- Management Recovery Per Turn: 2

Visual Settings:
- Empty Color: Blue
- Occupied Color: Cyan
- Full Color: Dark Blue
- Valid Drop Color: Green

UI Components:
- Slot Image: [Drag RestArea_Image]
- Rest Area Title Text: [Drag RestAreaTitle_Text]
- Capacity Text: [Drag Capacity_Text]
- Employee Preview Image: [Drag EmployeePreview_Image]
```

### **3. Drop Zone Setup**
```
DropZone (Image)
├─ RectTransform (Size: 200x150, anchored to center)
├─ Canvas Group (Blocks Raycasts: false, Interactable: true)
├─ BoxCollider2D (Size: 200x150, Is Trigger: true)
└─ RestAreaSlot (Script - same as parent)
```

## 🎮 EmployeeCard Setup

### **1. Create EmployeeCard Prefab**
```
EmployeeCard_Prefab (Add EmployeeCard script)
├─ Card_Image (Image - Card background)
├─ Portrait_Image (Image - Employee portrait)
├─ Frame_Image (Image - Colored frame)
├─ EmployeeName_Text (TextMeshPro - Employee name)
├─ TechnicalSkill_Text (TextMeshPro - "Tech: XX")
├─ CreativeSkill_Text (TextMeshPro - "Creative: XX")
├─ Sanity_Text (TextMeshPro - "Sanity: XX")
└─ StatusDisplay_Panel
    ├─ Status_Icon (Image - Status icon)
    ├─ Status_Text (TextMeshPro - Status name)
    └─ Status_Background (Image - Status color)
```

### **2. EmployeeCard Component Settings**
```csharp
// In Inspector for EmployeeCard:
Employee Card Components:
- Employee Template: [Drag EmployeeData ScriptableObject]
- Status Display: [Drag StatusDisplay_Panel or add EmployeeStatusDisplay]
- Frame Image: [Drag Frame_Image]

Drag Settings:
- Canvas: [Drag main Canvas]
- Drag Scale: 1.1

Card Components:
- Card Image: [Drag Card_Image]
- Employee Name Text: [Drag EmployeeName_Text]
- Technical Skill Text: [Drag TechnicalSkill_Text]
- Creative Skill Text: [Drag CreativeSkill_Text]
- Sanity Text: [Drag Sanity_Text]
- Portrait Image: [Drag Portrait_Image]
```

### **3. Employee Component (Auto-Added)**
The EmployeeCard automatically adds an Employee component at runtime:
- No manual setup needed
- Initializes from EmployeeData template
- Manages runtime state (sanity, status, etc.)

## 🔄 TaskSlot Integration

### **1. TaskSlot Updates**
TaskSlot now works with Task components instead of TaskData directly:

```csharp
// Old way (deprecated):
taskData.turnsRemaining = 3;
taskData.IsCompleted();

// New way:
Task taskComponent = gameObject.AddComponent<Task>();
taskComponent.InitializeFromTemplate(taskData);
taskComponent.ProcessTurn();
```

### **2. TaskSlot Component Settings**
```csharp
// In Inspector for TaskSlot:
Task Settings:
- Task Data: [Drag TaskData ScriptableObject template]
- Task Component: [Auto-created at runtime]

Turn-Based System:
- Turns Remaining Text: [Drag TurnsRemaining_Text]
- Turns Until Expiry Text: [Drag TurnsUntilExpiry_Text]
- Expiry Warning Image: [Drag ExpiryWarning_Image]
```

## 🎯 Workflow Integration

### **1. Employee Creation Flow**
```
1. Create EmployeeData ScriptableObject (template)
2. Create EmployeeCard prefab with EmployeeCard script
3. Assign EmployeeData to EmployeeCard.EmployeeTemplate
4. EmployeeCard automatically creates Employee component
5. Employee component manages runtime state
```

### **2. Task Creation Flow**
```
1. Create TaskData ScriptableObject (template)
2. TaskDiscoveryManager creates Task component from template
3. Task component manages runtime state (progress, expiry)
4. TaskSlot displays Task component data
```

### **3. Rest Area Flow**
```
1. Drag EmployeeCard to RestAreaSlot
2. RestAreaSlot validates employee needs rest
3. Employee component goes to rest state
4. RestAreaSlot processes turn recovery
5. Employee leaves when fully recovered
```

## 🚀 Testing Checklist

### **RestArea Testing:**
- [ ] Can drag EmployeeCard to RestAreaSlot
- [ ] RestAreaSlot validates employee needs rest
- [ ] Employee status changes to "Resting"
- [ ] Turn processing applies recovery
- [ ] Employee leaves when sanity ≥ 80
- [ ] Capacity limits work (max 4 employees)
- [ ] Visual feedback shows occupied state

### **EmployeeCard Testing:**
- [ ] EmployeeCard inherits from DraggableCard
- [ ] Employee component auto-created from template
- [ ] Status display shows current status
- [ ] Frame color changes based on availability
- [ ] Sanity updates correctly during turns
- [ ] Can work/cannot work based on status

### **TaskSlot Testing:**
- [ ] Task component created from TaskData template
- [ ] Turn progression works correctly
- [ ] Task expiry system functions
- [ ] Employee assignment updates Task component
- [ ] Task completion triggers rewards

## 🎨 Visual Feedback

### **RestArea Visual States:**
- **Empty**: Blue background, "0/4" capacity
- **Occupied**: Cyan background, shows current count
- **Full**: Dark blue background, "4/4" capacity
- **Valid Drop**: Green highlight during drag

### **EmployeeCard Visual States:**
- **Normal**: White frame, all stats visible
- **Selected**: Yellow frame during drag
- **Unavailable**: Gray frame (sick, overworked)
- **Status Icons**: Color-coded status indicators

## 📁 File Organization

```
Assets/Project/Scripts/
├─ UI/
│  ├─ DraggableCard.cs (Base class)
│  ├─ EmployeeCard.cs (Inherits from DraggableCard)
│  ├─ RestAreaSlot.cs (Multi-employee rest area)
│  └─ TaskSlot.cs (Updated for Task components)
├─ Gameplay/
│  ├─ Employee.cs (Runtime employee state)
│  ├─ Task.cs (Runtime task state)
│  └─ [Other managers...]
└─ Data/
   ├─ EmployeeData.cs (Template only)
   └─ TaskData.cs (Template only)
```

This architecture provides clean separation between data and behavior, making the system more maintainable and extensible for future features like PetCard inheritance!
