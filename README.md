# Time Urgency Study

Time Urgency Study is a Unity VR research prototype for exploring how different representations of time pressure affect task performance and perceived stress. Participants complete short puzzle tasks under one shared urgency condition while the application records objective performance data and self-reported ratings.

The project was developed for CAP 6119. Its guiding research question is:

> How do different representations of time pressure in VR affect task performance and perceived stress?

## Study conditions

The prototype supports three time-pressure conditions. The objective time limit for a task remains the same; only the way urgency is communicated changes.

| Condition | How urgency is presented |
| --- | --- |
| Visual countdown | An in-world timer displays the remaining time. |
| Audio countdown | Spoken warnings announce minute thresholds, 30 seconds remaining, and the final 10 seconds. |
| NPC urging | An animated non-player character delivers increasingly urgent voice prompts during the task. |

A condition can be selected at the beginning of a session or randomized through the `StudySessionManager` configuration. The selected condition is applied to both tasks for that session.

## Implemented tasks

### 1. Tower of Hanoi

Move the complete tower from the first peg to the third peg. Only one disk may be moved at a time, and a larger disk cannot be placed on a smaller disk.

Recorded measures:

- Task success or timeout
- Completion time
- Number of moves
- Illegal moves
- Invalid drops

The task manager's default time limit is 60 seconds and can be changed in the Unity Inspector.

### 2. Liquid Sort

Sort the colored liquid segments until every non-empty tube contains a single color. A pour is valid only when the destination has room and is either empty or topped by the same color.

Recorded measures:

- Task success or timeout
- Completion time
- Number of pours
- Illegal pour attempts

The task manager's default time limit is 120 seconds and can be changed in the Unity Inspector.

## Session flow

1. Select a time-pressure condition. The manager can also be configured to choose one at random.
2. Complete the pre-task rating panel for stress, calmness, and mood.
3. Read the in-world instructions and start either available task.
4. Complete the puzzle before its time limit expires.
5. Review the success or timeout message.
6. Complete the post-task ratings for stress, perceived pressure, and difficulty.
7. Repeat for the other implemented task.

The session manager prevents a new task from starting while another task, an instruction panel, or a rating panel is active.

## Data collection

Results are appended to two CSV files in Unity's `Application.persistentDataPath`. The exact paths are printed in the Unity Console when a session begins.

### `study_results.csv`

Stores the participant and session identifiers, task, urgency condition, success status, move and error counts, completion time, and time limit.

### `rating_results.csv`

Stores pre-task and post-task ratings for stress, calmness, mood, perceived pressure, and difficulty, along with the task, urgency condition, and timestamp.

Session identifiers are generated from the current date and time. The current prototype uses `AUTO` as the participant ID, so researchers should update `DataLogger.GenerateSessionIdentifiers()` if manually assigned or anonymized participant identifiers are required.

## Built with

- Unity `6000.3.8f1`
- Universal Render Pipeline `17.3.0`
- XR Interaction Toolkit `3.3.1`
- OpenXR Plugin `1.16.1`
- Input System `1.18.0`
- XR Hands `1.7.3`

OpenXR loaders are configured for both Standalone and Android build targets.

## Getting started

### Prerequisites

- Unity Hub
- Unity Editor `6000.3.8f1` (using the matching editor version is recommended)
- An OpenXR-compatible headset and controller setup, or a compatible Unity XR simulation setup
- The platform build module required for your intended target, if creating a standalone or Android build

### Run the project

1. Clone the repository:

   ```bash
   git clone https://github.com/rubaismail/Time-Urgency-Study.git
   ```

2. Add the cloned folder as a project in Unity Hub.
3. Open the project with Unity `6000.3.8f1` and allow the Package Manager to restore dependencies.
4. Open `Assets/Scenes/SampleScene.unity` if it is not already open. This is the scene included in Build Settings.
5. Connect and configure an OpenXR-compatible headset, then enter Play mode.
6. In VR, use the configured XR interactors to select UI options, start a task, and manipulate the puzzle objects.

For a device build, switch to the desired supported target, confirm the appropriate OpenXR features in **Project Settings > XR Plug-in Management**, and build `Assets/Scenes/SampleScene.unity`.

## Project structure

```text
Assets/
├── Scenes/SampleScene.unity       Main study scene
├── Scripts/
│   ├── StudySessionManager.cs     Session state and shared urgency condition
│   ├── TimePressureController.cs  Visual, audio, and NPC urgency behavior
│   ├── DataLogger.cs              CSV output
│   ├── RatingPanel/               Pre-task and post-task questionnaires
│   ├── Station1/                  Tower of Hanoi logic
│   └── Station2/                  Liquid Sort logic
├── Audio/                         Countdown, feedback, and NPC voice clips
└── XR/                            OpenXR and XR simulation settings
```

## Research context

The prototype builds on research showing that immersive VR can influence stress, attention, and time perception. It treats urgency presentation as a design variable rather than assuming that all timed tasks communicate pressure in the same way.

- Mullen, G., & Davidenko, N. (2021). *Time Compression in Virtual Reality*. Timing & Time Perception, 9, 1-16. https://doi.org/10.1163/22134468-bja10034
- Syrigou, K., Stoforou, M., & Kourtesis, P. (2025). *Time Perception in Virtual Reality: Effects of Emotional Valence and Stress Level*. https://doi.org/10.21203/rs.3.rs-6641763/v1

## Project status

This repository is an academic research prototype rather than a production-ready application. The two implemented tasks, urgency conditions, rating workflow, and CSV logging are present; the proposed Sequence Ordering task is outside the final implementation scope.
