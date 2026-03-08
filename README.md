# FPS Aim Trainer

A machine learning–assisted FPS aim trainer designed to help players improve their aim through **adaptive training scenarios and performance analysis**. The project combines real-time gameplay mechanics with machine learning techniques to analyze player behavior and adjust training difficulty dynamically.

The goal of this project is to create a training environment that helps FPS players improve **accuracy, reaction time, and tracking ability** through data-driven feedback.

---

# Features

- **Real-Time Aim Training**
  - Target spawning system
  - Click-based shooting mechanics
  - Reaction time tracking

- **Machine Learning Adaptation**
  - Analyzes player accuracy and reaction time
  - Dynamically adjusts difficulty
  - Generates personalized training patterns

- **Performance Analytics**
  - Accuracy tracking
  - Reaction time measurement
  - Hit/miss statistics

- **Adaptive Difficulty**
  - Target speed adjustments
  - Spawn frequency changes
  - Pattern variation

- **Training Modes**
  - Flick aim training
  - Target switching
  - Tracking targets

---

# How It Works

The system collects gameplay data during training sessions and feeds it into a machine learning pipeline that evaluates player performance.

### Training Loop

1. Player starts a training session
2. Targets spawn in different patterns
3. Player attempts to hit targets
4. Game records:
   - Reaction time
   - Hit accuracy
   - Target tracking behavior
5. Machine learning model evaluates performance
6. Difficulty parameters adjust for the next round

This creates a **feedback loop** that continually adapts training to the player's skill level.

---

# Project Architecture
Player Input
↓
Game Engine (Unity)
↓
Gameplay Data Collection
↓
Data Processing
↓
ML Model
↓
Difficulty Adjustment
↓
Next Training Scenario

---

# Tech Stack

**Game Engine**

- Unity (3D)

**Programming**

- C#

**Machine Learning**

- Bayesian Optimizations

**Data Processing**

- CSV / structured gameplay logs

---

# Training Metrics

The aim trainer evaluates players based on several key metrics:

| Metric | Description |
|------|------|
| Accuracy | Percentage of targets hit |
| Reaction Time | Time between target spawn and shot |
| Tracking Score | Ability to follow moving targets |
| Hit Consistency | Variance in hit performance |

These metrics are used to adapt training difficulty and generate personalized sessions.

---

# Installation

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/ml-fps-aim-trainer.git
cd ml-fps-aim-trainer
```

### 2. Open the project

Open the project in Unity Hub.

### 3. Run the game

Press Play inside the Unity editor to start training.

# Future Improvements
	•	AI-generated aim drills
	•	Player skill ranking system
	•	Heatmap visualization of aim accuracy
	•	Reinforcement learning for drill generation
	•	Multiplayer aim competitions
	•	Web-based leaderboard

# Project Goals
This project explores how machine learning can improve player training tools in competitive gaming. Instead of static aim drills, the trainer adapts to the player’s strengths and weaknesses to create more effective practice sessions.

# Acknowledgments
	•	Inspired by popular aim trainers like Kovaak’s and Aim Lab
	•	Built as an experimental project exploring ML applications in gaming

### Special Thanks to Dr. April Grow of Cal Poly CS for advising and helping make this project come to reality!
  


