## Demo Video
[Demo Video](/Demo.mp4)

## How to Run the Project

1. Open the project using **Unity 2021.3.xxx LTS**.
2. Open the `GameScene` and press **Play**.
3. The current song is configured in:  
   `Assets/Project/Levels/TestSong`

> 💡 To switch or add new songs, create a new `TrackProfile` and update the `Conductor` script to reference it.

---

## Core Gameplay Features

- **Rhythmic Tile Spawning**  
  Tiles are spawned based on a `TileMapping` configuration that defines note timing, lane count, and total notes.

- **Scrolling & Synchronization**  
  Tiles scroll downward via a `Scroller` script. Scroll speed is based on the track’s BPM and is corrected every beat for synchronization.

- **Player Input & Scoring**  
  - Tap tiles to register hits.
  - Scoring logic:
    - **Perfect**: If tap occurs within the first 25% of tile length from the line.
    - **Good/Normal**: Within acceptable margin beyond Perfect.
    - **Miss/Early**: Too early or late.

- **Game Over Condition**  
  Occurs when a tile reaches the bottom without being tapped.

---

## Technical Implementation

### Rhythm System (Conductor)
- Controls playback and beat detection through `TrackInstance`.
- Emits beat signals via C# delegates to synchronize game events.

### Tile Management
- Tiles are spawned according to `TileMapping` configs.
- Object pooling is used for tile reuse and performance.

### Scrolling Logic
- `Scroller` script moves tiles downward each frame.
- On each beat signal, scroll position is corrected to maintain sync.

### Input Detection
- On tap, the game checks tile proximity to the hit line.
- Hit result is determined based on distance at tap time.

---

## Bonus Features Implemented

- **Combo System**  
  Tracks consecutive hits and increases score multipliers.

- **Visual Feedback**  
  - Flashing/popping effects on tap using tweening.
  - UI animations for combo and score changes.

- **Audio Feedback**  
  - Distinct sound effects for Perfect and Normal hits.

- **Dynamic Background**  
  - Beat-synced UV tiling scroll creates a dynamic visual background.

---

## Performance Consideration

- **Object Pooling**: Avoids frequent instantiation/destruction.
- **Canvas Splitting**: UI is separated into static/dynamic canvases to reduce redraw cost.

--- 

## Assets & Attribution

All code, visual elements, and feedback systems were created by me.  
**No third-party assets or plugins were used.**

---
