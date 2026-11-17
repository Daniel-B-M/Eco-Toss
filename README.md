# Eco-Toss: Civic Memory

Educational Unity game about sorting waste: the player throws everyday waste into the correct bin (white, green or black) while the neighborhood reacts to each choice.

> **Current status: asset integration.** The `integration` branch brings together the team's 2D, 3D, audio and font assets.

## Tech
- **Unity 6000.3.16f1**, Universal Render Pipeline (URP 17.3)
- Input System 1.19, uGUI + TextMesh Pro
- Scenes: `MainMenu` (build start scene), `Gameplay`, `Gameplay 2` (test scene)

## Integrated assets

**3D** (`Art/Models`, low-poly)
- Bin model (`trash-can.fbx`) with white, green and black materials
- 15 waste models, 5 per bin:
  - **White (recyclable):** water bottle, soda can, beer bottle, cardboard box, crumpled paper
  - **Green (organic):** apple, banana, orange, eggs, hot dog
  - **Black (non-recyclable):** pizza box, coffee cup, egg carton, cigarettes, broken glass
- Environment: *SimplePoly City – Low Poly Assets* (third party, in `ThirdParty/`)

**2D** (`Art/Sprites`)
- Main title and background
- Character feedback sprites (idle, happy and sad states):
  - Don Jacinto (the waste picker)
  - The young couple
  - The community dog

**Audio** (`Audio/`)
- Music: `bg-music.mp3`
- SFX: success, fail, victory, lose, UI hover and UI select

**Font** (`UI/Fonts`)
- **Nunito** Regular and ExtraBold, with TMP SDF assets (supports accents and ñ)
- Source: Google Fonts, licensed under the SIL Open Font License 1.1

## How to open
1. Clone the repository and switch to the `integration` branch.
2. Add the project folder in Unity Hub and open it with Unity 6000.3.16f1.
3. Open `Assets/EcoToss/Scenes/MainMenu.unity` and press **Play**.

## Team
Daniel Bonilla Martínez · Jason Kim Rincon · Andrés Valbuena Almeida · Juliana Albarracin Casallas
UNAD — Programación para Videojuegos (213027), Group 08
