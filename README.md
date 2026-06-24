# BSP6 – Effects of Visual and Auditory Distractions in Virtual Reality

## Overview

This repository contains the Unity experiment and analysis pipelines developed for the BSP6 Bachelor Semester Project at the University of Luxembourg.

The project investigates how visual and auditory distractions influence physiological arousal, attention, and perceived workload during a VR-based card-matching memory task.

Data collected during the experiment include:

* Electrodermal Activity (EDA)
* Heart Rate (HR)
* Eye-Tracking (Pupil Diameter)
* Behavioral Performance Metrics
* NASA-TLX Workload Ratings

---

## Repository Structure

```text
.
├── Analysis/
│   └── Physiology/
│       ├── BSP6_Final_Analysis.ipynb
│       └── analysis_outputs/
│
├── Assets/
├── Docs/
├── Packages/
├── Physiology/
├── ProjectSettings/
└── README.md
```

---

## Running the Experiment

### Requirements

* Unity 2022 LTS
* SteamVR
* HTC Vive Headset with Eye Tracking
* Lab Streaming Layer (LSL)
* Lab Recorder
* EmotiBit Sensor

### Opening the Project

Clone the repository:

```bash
git clone https://github.com/BiancaGL2104/BSP6---VR.git
```

Open the project in Unity Hub and load the Unity project contained in this repository.

### Procedure

1. Start SteamVR.
2. Connect and verify the EmotiBit sensor.
3. Open Lab Recorder and select all available streams.
4. Open the project in Unity and load the experiment scene.
5. Perform eye-tracking calibration.
6. Start recording in Lab Recorder.
7. Press **Play** in Unity.

Controls:

* **Space** → Start experiment session
* **Enter** → Start next condition

At the end of the experiment, stop Lab Recorder and save the XDF recording.

---

## Analysis

The physiological analysis notebook is located in:

```text
Analysis/Physiology/
```

The notebook performs:

* Data cleaning
* Participant inclusion checks
* Eye-tracking validity assessment
* Local baseline correction
* Descriptive statistics
* Friedman tests
* Wilcoxon post-hoc comparisons
* Figure and table generation

---

## Authors

**Bianca Leoveanu**

* EmotiBit integration
* Physiological signal processing
* Eye-tracking analysis
* Statistical analysis

**Berin Venedik**

* Behavioral task implementation
* Distraction system development
* Behavioral analysis

---

**University of Luxembourg**
Bachelor Semester Project (BSP6)
2025–2026
