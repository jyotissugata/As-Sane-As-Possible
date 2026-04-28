# ASAP (As Sane As Possible)
“A leader is only as good as their team’s mental health.”

ASAP is a turn-based corporate management simulation built for the Gosu Academy Game Developer challenge. In this game, players take on the role of a Lead Developer who must hit aggressive revenue targets while ensuring their team doesn't spiral into a total mental breakdown.

Core Gameplay Mechanic
The game operates on a Turn-Based System (Morning, Afternoon, Evening).

Delegate: Drag and drop Employee Cards into Task Slots.

Match Skills: Tasks require specific levels of Technical, Creative, Communication, and Management skills.

Manage Sanity: Every task has a "Sanity Cost." Assigning the wrong person or overworking them leads to Burnout.

Recover: Use the Rest Area to recover Sanity at the cost of immediate productivity.

Leadership Skill Connection
The mechanics are designed to simulate and develop real-world leadership competencies:

Strategic Delegation (The Right Fit): Players must analyze employee strengths. Putting a high-creative person on a technical-heavy task is inefficient and drains their Sanity faster. This teaches Talent Mapping.

Sustainability vs. Profit: High-profit tasks often have the highest Sanity costs. Players must decide when to "push" for a deadline and when to prioritize team health. This demonstrates Sustainable Productivity.

Resource Allocation: With limited turns and slots, players must decide if a "Rest Turn" for a top performer is more valuable than a low-quality "Work Turn." This builds Strategic Empathy.

AI Tools Collaboration Note
This project was developed using an "AI-First" workflow, leveraging state-of-the-art tools to achieve high-quality output within a 7-day sprint.

1. Gemini (Design Partner & Creative Director)
Role: Gemini acted as my Lead Designer and Leadership Consultant.

Impact: It helped translate abstract leadership concepts into concrete game mechanics (e.g., the Sanity system). It also provided the satirical narrative tone and the dynamic "Leadership Feedback" logic.

2. Windsurf (Technical Architect & Execution)
Role: Used as the primary agentic IDE.

Impact: Windsurf handled the rapid architecting of the C# ScriptableObject systems and the core Turn-Based engine. It allowed me to focus on high-level logic and balancing while the AI handled the boilerplate implementation of the Drag & Drop and UI Event systems.

3. Generative Assets
Role: Used for UI icons, employee portraits, and background music.

Impact: Allowed for a cohesive "Corporate Satire" aesthetic without diverting time from core mechanic development.

Evaluation Check
Functional: The core loop (Drag-to-Assign -> Next Turn -> Sanity/Money Calculation) is fully operational.

Clear: The "Leadership Feedback" system explicitly tells the player how their choices reflect their management style.

AI-Driven: Documentation of AI prompts and workflows is integrated into the development logs.

How to Play
Open Project: Import into Unity 2022.3+.

Run Scene: Open Scenes/MainGameplay.

Objective: Reach the target revenue within the time limit without letting your team reach 0 Sanity.

Next Turn: Click the "Next Turn" button to progress from Morning to Afternoon to Evening.

Author
Jyotis Sugata
