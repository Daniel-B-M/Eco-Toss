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

Las escenas están creadas como base inicial. El flujo entre el menú y la partida y la interacción jugable están pendientes de implementación.

## Controles

Los controles todavía no están definidos ni implementados. Esta tabla se actualizará con las asignaciones reales del juego.

| Acción prevista | Control | Estado |
| --- | --- | --- |
| Apuntar el lanzamiento | Por definir | Pendiente |
| Lanzar el residuo | Por definir | Pendiente |
| Pausar o reanudar | Por definir | Pendiente |
| Interactuar con los menús | Por definir | Pendiente |

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

- Implementar el flujo entre el menú principal y la partida.
- Crear la mecánica básica de lanzamiento.
- Definir los tipos de residuos y las reglas de clasificación.
- Incorporar puntaje y mensajes de resultado.
- Definir e implementar los controles y documentarlos aquí.
