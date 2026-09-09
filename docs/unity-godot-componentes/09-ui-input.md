# 09 — UI, Inspector visual, input e touchscreen

## Unity UI

### Canvas

Raiz de UI. Modos: Screen Space Overlay, Screen Space Camera e World Space.

### CanvasScaler

Escala UI por resolução, reference resolution, screen match e densidade.

### GraphicRaycaster

Converte pointer/touch em hits nos elementos gráficos do Canvas.

### CanvasGroup

Controla alpha, interactable e blocks raycasts de uma subárvore.

### Image e RawImage

`Image` desenha Sprite/color/fill/sliced. `RawImage` desenha Texture ou RenderTexture.

### Text/TextMeshPro

`Text` é legado. TextMeshPro fornece texto de maior qualidade, fontes, atlas, rich text e UI moderna.

### Controles

- `Button`: clique;
- `Toggle`: booleano;
- `Slider`: valor contínuo;
- `Scrollbar`: rolagem;
- `Dropdown`/`TMP_Dropdown`: seleção;
- `InputField`/`TMP_InputField`: texto;
- `Selectable`: base de foco/estado;
- `ScrollRect`: viewport rolável.

### Layout

`HorizontalLayoutGroup`, `VerticalLayoutGroup`, `GridLayoutGroup`, `ContentSizeFitter`, `LayoutElement`, `AspectRatioFitter`, `Mask` e `RectMask2D` distribuem, dimensionam e recortam filhos.

### EventSystem

Gerencia seleção, pointer, navegação e dispatch de eventos. `StandaloneInputModule` usa input clássico; `InputSystemUIInputModule` usa o pacote Input System.

### Raycasters

`PhysicsRaycaster` e `Physics2DRaycaster` conectam pointer/touch do EventSystem a colliders.

## Godot UI

### Control

Base de UI com anchors, offsets, mouse filter, foco e layout.

### Containers

`HBoxContainer`, `VBoxContainer`, `GridContainer`, `MarginContainer`, `CenterContainer`, `AspectRatioContainer`, `PanelContainer` e `ScrollContainer` posicionam filhos automaticamente.

### Visuais

`Panel`, `ColorRect`, `TextureRect`, `Label`, `RichTextLabel`, `Label3D` e `NinePatchRect` mostram conteúdo.

### Controles

`Button`, `TextureButton`, `CheckBox`, `CheckButton`, `OptionButton`, `MenuButton`, `LinkButton`, `LineEdit`, `TextEdit`, `SpinBox`, `HSlider`, `VSlider`, `ProgressBar`, `HScrollBar`, `VScrollBar`, `ItemList`, `Tree`, `TabBar`, `TabContainer`, `PopupMenu`, `Window`, `FileDialog` e `ColorPicker`.

### CanvasLayer

Mantém HUD/menu independente da transformação da câmera.

### SubViewport/SubViewportContainer

Renderiza uma câmera/scene em viewport separado e mostra o resultado em UI; ótimo para preview de Terrain/Graph.

### GraphEdit/GraphNode

Nodes nativos para editor visual de grafos. Exigem lógica própria de links, tipos, execução, seleção e undo.

## Input Unity

- `Input` clássico: teclado, mouse, joystick e touch;
- Input System: package moderno de actions/devices/bindings;
- `PlayerInput`: componente que conecta actions a callbacks;
- `Touch`: finger id, position, phase, pressure;
- `EventSystem`: pointer/UI;
- `Camera.ScreenPointToRay`: toque para ray 3D;
- `Physics.Raycast`/`Physics2D.Raycast`: seleção física;
- `Gizmos/Handles`: interação de editor, não gameplay.

## Input Godot

- `Input`: singleton para ações/estado;
- `InputMap`: cadastro de actions;
- `InputEventKey`, `MouseButton`, `MouseMotion`, `ScreenTouch`, `ScreenDrag`, `JoypadButton`, `JoypadMotion`;
- `_input`: evento precoce;
- `_unhandled_input`: gameplay após UI consumir;
- `_gui_input`: evento do Control;
- `RayCast3D`/`ShapeCast3D` e equivalentes 2D;
- `TouchScreenButton`;
- virtual joystick: normalmente cena/script próprio.

## Touch para editor mobile

O editor MapMagic clássico é orientado a mouse/keyboard. Para uma versão mobile é preciso adicionar:

- tap para selecionar;
- drag com um dedo para pan/arraste;
- pinch para zoom;
- long press para menu contextual;
- multi-touch para navegação da viewport;
- alvos de toque maiores que o ícone visual;
- teclado virtual para valores precisos;
- safe area/notch;
- drawers/bottom sheets para Inspector e Asset Browser.

## Processo de clique/toque em objeto

```text
Touch/mouse
  ↓
UI raycast
  ├── UI consumiu? encerra
  └── não consumiu
        ↓
screen point → ray
        ↓
physics/render picking
        ↓
seleção/interaction/event
```

## Dependências

- Unity UI precisa Canvas e EventSystem;
- novo Input System é opcional, mas deve ser consistente;
- Godot Control precisa de parent/viewport/layout;
- viewport 3D precisa Camera3D e ray conversion;
- editor graph precisa undo, seleção e validação de links;
- Android precisa tratar teclado virtual, orientação, safe area e lifecycle.

## Tutorial de validação

### Unity

1. Crie Canvas + CanvasScaler + GraphicRaycaster.
2. Adicione Button/Slider/InputField.
3. Verifique EventSystem.
4. Ative touch/mouse simulator se disponível.
5. Crie raycast de toque para Terrain.
6. Teste portrait/landscape e safe area.

### Godot

1. Crie CanvasLayer.
2. Adicione Panel/VBoxContainer/Button.
3. Use anchors e minimum size.
4. Configure InputMap.
5. Trate `_gui_input` e `_unhandled_input`.
6. Crie viewport preview com SubViewportContainer.

## Controles confirmados na documentação do MapMagic v2.1.11

O `Readme.txt` e a wiki do graph declaram estas interações: botão do meio para pan, roda do mouse para zoom, `Shift` + `=`/`-` como zoom alternativo, botão esquerdo para arrastar generators/criar links e botão direito para criar, remover, duplicar e preview.

No Brush, a documentação declara:

- `[` e `]` alteram Radius quando a Scene View está focada;
- `~`, `1`, `2`, `3` etc. selecionam slots de preset;
- `Ctrl-click` captura `CapturedPosition`;
- `Shift` é fornecido ao graph como valor automático para modo alternativo, como apagar;
- `Position`, `PrevPosition`, `CapturedPosition`, `TerrainHeight`, `Radius` e `Hardness` podem ser valores automáticos expostos.

Isso é input de ferramenta do Editor/Brush, não uma declaração de que MapMagic fornece um sistema de input de gameplay para o jogo exportado. O bundle não exige o novo Input System para o core.

## Fontes

- [Unity UI](https://docs.unity3d.com/Manual/UISystem.html)
- [Unity EventSystem](https://docs.unity3d.com/Manual/EventSystem.html)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)
- [Godot Control](https://docs.godotengine.org/en/stable/classes/class_control.html)
- [Godot InputEvent](https://docs.godotengine.org/en/stable/classes/class_inputevent.html)
- [Godot GraphEdit](https://docs.godotengine.org/en/stable/classes/class_graphedit.html)
