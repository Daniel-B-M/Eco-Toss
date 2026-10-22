# Eco Toss

Eco Toss es un proyecto de videojuego educativo desarrollado en Unity. Su propuesta es practicar la separación de residuos mediante una mecánica de lanzamiento hacia la caneca correspondiente.

**Estado:** configuración inicial del proyecto. Las mecánicas, reglas y controles se documentarán a medida que se implementen.

## Requisitos

- Unity Hub.
- **Unity Editor 6000.3.16f1**, versión registrada en `ProjectSettings/ProjectVersion.txt`.
- Git, si vas a clonar el repositorio.

El proyecto incluye Universal Render Pipeline (URP) y el paquete Input System. Unity resolverá las dependencias declaradas en `Packages/manifest.json` al abrirlo.

## Cómo abrir y ejecutar el proyecto

1. Clona este repositorio o descarga una copia en tu equipo.
2. Abre Unity Hub y agrega la carpeta raíz del proyecto: la que contiene `Assets`, `Packages` y `ProjectSettings`.
3. Abre el proyecto con Unity **6000.3.16f1**.
4. Espera a que Unity importe los recursos y resuelva los paquetes.
5. Abre `Assets/EcoToss/Scenes/MainMenu.unity` o `Assets/EcoToss/Scenes/Gameplay.unity`.
6. Presiona **Play** para revisar la escena en el editor.

MainMenu incluye una interfaz provisional editable en la jerarquía: título, Jugar, Cómo jugar y Salir. Jugar abre Gameplay; Cómo jugar muestra los controles y permite volver con el botón Volver o Escape. Salir detiene Play en el editor o cierra la aplicación compilada. MainMenu es la primera escena de la compilación.

La interfaz usa uGUI, un Canvas escalado a una referencia de 1280 × 900 y colores planos, sin recursos gráficos externos. Los textos, colores y tamaños se pueden ajustar desde el Inspector; `MainMenuController` conecta las acciones y crea el EventSystem con Input System al iniciar. El diseño base está pensado para pantallas horizontales.

## Controles

Controles del prototipo actual:

| Acción prevista | Control | Estado |
| --- | --- | --- |
| Apuntar el lanzamiento | Mover el mouse | Implementado |
| Lanzar el residuo | Mantener clic izquierdo para cargar; soltar para lanzar | Implementado; lanzamiento automático a los 5 segundos |
| Pausar o reanudar | Por definir | Pendiente |
| Interactuar con los menús | Clic o flechas y Enter; Escape cierra las instrucciones | Implementado |

## Estructura del proyecto

```text
Assets/EcoToss/
├── Scenes/           # MainMenu y Gameplay
├── Scripts/
│   ├── Core/         # Inicio y control de la partida
│   ├── Gameplay/     # Lanzamiento, residuos y canecas
│   ├── Input/        # Controles del jugador
│   ├── UI/           # Menús, puntaje y mensajes
│   └── Audio/        # Control de música y efectos
├── Prefabs/          # Residuos, canecas, entorno e interfaz
├── Art/              # Modelos, materiales, texturas, sprites y animaciones
├── Audio/            # Música y efectos de sonido
└── UI/               # Fuentes y recursos gráficos de interfaz
```

La documentación adicional se organizará en `Docs/` cuando se agreguen los documentos de planificación, diseño y notas técnicas.

## Convenciones de trabajo

- Guarda los recursos propios del juego en `Assets/EcoToss/`.
- Usa nombres claros y consistentes, como `WasteThrower.cs` o `BinPlastic.prefab`.
- Mueve y renombra recursos desde la ventana **Project** de Unity para conservar sus referencias.
- Incluye los archivos `.meta` al guardar cambios en Git.
- Mantén este README actualizado cuando cambien la versión de Unity, las instrucciones o los controles.

## Próximos pasos

- Añadir el regreso desde la partida al menú principal.
- Crear la mecánica básica de lanzamiento.
- Definir los tipos de residuos y las reglas de clasificación.
- Incorporar puntaje y mensajes de resultado.
- Definir e implementar los controles y documentarlos aquí.
