# EmployeeData Design Guide for Demo

This guide provides specifications for creating diverse EmployeeData ScriptableObjects to make the demo gameplay more engaging and strategic.

## **Employee Archetypes**

Create 8-10 employees with different skill combinations to give players meaningful delegation choices.

### **1. The Specialist - High Technical Skill**
**Name:** Alex Chen
**Description:** Expert programmer who excels at technical tasks but struggles with communication.
**Skills:**
- Technical Skill: 90
- Creative Skill: 30
- Communication Skill: 40
- Management Skill: 35
- Sanity: 80
**Salary:** 150
**Best For:** Programming tasks, bug fixes, technical documentation

---

### **2. The Creative Visionary - High Creative Skill**
**Name:** Sarah Martinez
**Description:** Brilliant designer with artistic flair but weak in technical areas.
**Skills:**
- Technical Skill: 25
- Creative Skill: 95
- Communication Skill: 70
- Management Skill: 40
- Sanity: 75
**Salary:** 140
**Best For:** Design tasks, UI/UX work, creative projects

---

### **3. The People Person - High Communication Skill**
**Name:** Tommy
**Description:** Excellent communicator who bridges gaps between teams.
**Skills:**
- Technical Skill: 40
- Creative Skill: 50
- Communication Skill: 95
- Management Skill: 60
- Sanity: 85
**Salary:** 130
**Best For:** Client meetings, presentations, mixed tasks requiring communication

---

### **4. The Manager - High Management Skill**
**Name:** David Kim
**Description:** Natural leader who coordinates teams effectively.
**Skills:**
- Technical Skill: 45
- Creative Skill: 40
- Communication Skill: 75
- Management Skill: 90
- Sanity: 70
**Salary:** 160
**Best For:** Administrative tasks, project management, team coordination

---

### **5. The Balanced All-Rounder**
**Name:** Emma Wilson
**Description:** Solid performer across all areas with no major weaknesses.
**Skills:**
- Technical Skill: 65
- Creative Skill: 60
- Communication Skill: 70
- Management Skill: 65
- Sanity: 80
**Salary:** 120
**Best For:** Any task, flexible delegation option

---

### **6. The Burnout Risk - High Skills, Low Sanity**
**Name:** Michael Brown
**Description:** Talented but fragile employee who burns out quickly.
**Skills:**
- Technical Skill: 85
- Creative Skill: 80
- Communication Skill: 60
- Management Skill: 55
- Sanity: 40
**Salary:** 145
**Best For:** Critical tasks when time is short, but needs frequent rest

---

### **7. The Rookie - Low Skills, Low Salary**
**Name:** Jessica Lee
**Description:** New hire with potential but needs training.
**Skills:**
- Technical Skill: 35
- Creative Skill: 40
- Communication Skill: 50
- Management Skill: 30
- Sanity: 90
**Best For:** Simple tasks, learning opportunities, backup option
**Salary:** 80

---

### **8. The Veteran - High Skills, High Salary**
**Name:** Robert Davis
**Description:** Experienced professional with excellent all-around skills.
**Skills:**
- Technical Skill: 80
- Creative Skill: 75
- Communication Skill: 85
- Management Skill: 80
- Sanity: 75
**Salary:** 200
**Best For:** Complex tasks, critical projects, high-stakes work

---

### **9. The Creative-Tech Hybrid**
**Name:** Lisa Anderson
**Description:** Rare combination of technical and creative skills.
**Skills:**
- Technical Skill: 80
- Creative Skill: 85
- Communication Skill: 50
- Management Skill: 45
- Sanity: 70
**Salary:** 155
**Best For:** Technical design, frontend development, creative coding

---

### **10. The Support Specialist**
**Name:** James Taylor
**Description:** Excellent at supporting roles with strong communication and management.
**Skills:**
- Technical Skill: 30
- Creative Skill: 35
- Communication Skill: 80
- Management Skill: 75
- Sanity: 85
**Salary:** 110
**Best For:** Administrative tasks, team support, coordination

---

## **Skill Distribution Strategy**

**For Demo Balance:**
- **2 High Technical** (Alex Chen, Michael Brown)
- **2 High Creative** (Sarah Martinez, Lisa Anderson)
- **2 High Communication** (Jessica Lee's placeholder, James Taylor)
- **2 High Management** (David Kim, Robert Davis)
- **2 Balanced** (Emma Wilson, one flexible hire)

**Salary Range:** 80 - 200 (gives players budget management decisions)

**Sanity Range:** 40 - 90 (creates different rest management strategies)

## **Usage Tips**

1. **Start with 4 employees** for the demo to avoid overwhelming players
2. **Recommended starting team:** Alex Chen, Sarah Martinez, Emma Wilson, David Kim
3. **Unlock additional employees** as the game progresses or via shop system
4. **Use salary as a balancing factor** - higher skills should cost more
5. **Vary sanity levels** to create different rest management challenges

## **Creating EmployeeData in Unity Editor**

1. Right-click in Project window → Create → ASAP → EmployeeData
2. Name the asset (e.g., "AlexChen_EmployeeData")
3. Fill in the fields according to the specifications above
4. Assign a portrait sprite for visual identification
5. Save in a dedicated folder (e.g., Assets/Data/Employees)
