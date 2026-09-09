# 11 — editor, Inspector, gizmos, plugins e undo/redo

## Unity Editor

### EditorWindow

Janela customizada. MapMagic usa uma janela de graph para desenhar nós, links, preview e toolbar.

### CustomEditor

Substitui ou amplia o Inspector de um componente.

### PropertyDrawer

Controla como um tipo/atributo é desenhado no Inspector.

### EditorGUI/EditorGUILayout

APIs de desenho de campos, botões, labels, foldouts e listas.

### MenuItem

Registra menus como `Assets/Create`, `Window` e `GameObject`.

### Handles/Gizmos

`Handles` permite manipular objetos com alças; `Gizmos` desenha debug/visualização. São editor/diagnóstico, não runtime de gameplay.

### SceneView

Viewport do editor; recebe seleção, mouse, handles e repaint.

### Undo

Registra alterações e permite Ctrl+Z/Ctrl+Y. Toda alteração de graph, posição, layer ou propriedade importante deve usar Undo.

### SerializedObject/SerializedProperty

Permitem Inspector consistente com multi-object editing, prefab overrides e Undo.

### AssetPostprocessor/ScriptedImporter

Customizam importação e criação de assets.

### PrefabUtility

Cria, instancia, aplica e reverte prefabs.

### EditorPlugin equivalente

Unity não tem uma classe única chamada EditorPlugin para tudo; usa assembly Editor, menus, windows, inspectors, drawers e callbacks.

## Godot Editor

### @tool

Permite que script rode no editor. Deve evitar operações destrutivas e distinguir editor/runtime.

### EditorPlugin

Base de plugin que registra docks, menus, inspectors, gizmos, importers e ferramentas.

### EditorInspectorPlugin

Adiciona ou substitui UI de propriedades no Inspector.

### EditorProperty

Editor customizado para um campo.

### EditorNode3DGizmoPlugin

Cria gizmos de viewport para nodes 3D.

### EditorImportPlugin

Importador customizado de arquivos.

### EditorExportPlugin

Altera processo de exportação.

### EditorUndoRedoManager

Registra comandos de edição e desfazer/refazer.

### EditorInterface

Controla docks, cenas, seleção e integração do plugin.

### GraphEdit/GraphNode

Base visual para graph editor. O programador deve implementar links válidos, tipos, conexão, seleção, duplicação, serialização e execução.

## Inspector de MapMagic-like

O Inspector não deve conhecer cada generator com vários `if`. Use metadata:

```text
Property metadata
   ↓
Property editor factory
   ↓
Inspector
   ↓
Undo + serialization + exposure + animation
```

Campos importantes: bool, int, float, string, enum, Vector2/3/4, Color, Curve, Resource, asset, lista, range, object reference e layer.

## Graph Editor

### Operações necessárias

- criar node;
- mover node;
- criar link;
- validar tipo de inlet/outlet;
- inserir node em link;
- duplicar;
- remover/desconectar;
- agrupar;
- importar/exportar subgraph;
- preview;
- expor valor;
- renomear layer;
- undo/redo;
- salvar e migrar versão.

### Onde ficam no MapMagic

No bundle: `Nodes/Editor`, `Popup`, `Preview`, `Core/Editor`, `Tools/GUI/Editor` e assemblies `MapMagic.Editor`/`Den.Tools.Editor`.

## Plugin/compatibilidade

Integrações externas devem ser isoladas por:

- assembly definition;
- define constraint;
- placeholder quando dependência ausente;
- warning claro;
- API de adapter;
- nenhum tipo externo vazando para o core.

## Tutorial de validação

### Unity

1. Crie EditorWindow.
2. Registre `Window/Test` com MenuItem.
3. Crie CustomEditor.
4. Altere propriedade com SerializedProperty.
5. Registre Undo.
6. Adicione Handle na SceneView.

### Godot

1. Crie plugin `EditorPlugin`.
2. Adicione dock Control.
3. Crie GraphEdit/GraphNode.
4. Registre link e seleção.
5. Use EditorUndoRedoManager.
6. Desative plugin e confirme que runtime não carrega código do editor.

## Sequência confirmada das ações do Graph Window

Pelos manuais e pelo editor v2.1.11, uma alteração de graph segue esta ordem observável:

1. usuário cria/seleciona um generator;
2. o menu filtra pelo contexto: espaço vazio, link, node/outlet;
3. generator recebe posição, tipo e inlets/outlets;
4. conexão é validada pelo tipo do inlet/outlet;
5. graph marca mudança;
6. Editor pode limpar/regerar conforme `instantGenerate`;
7. Graph/MapMagic prepara e gera tiles;
8. preview/progress/eventos atualizam a interface.

As ações declaradas de botão direito são Disable/Enable, Duplicate, Update legacy node, Unlink, Remove, Reset, Group/Ungroup, Export/Import graph, Update All legacy nodes e Value Expose/UnExpose.

## Dependências exatas do pacote

O Editor do bundle fica em assemblies editor-only. Os tipos de editor são utilizados para GraphWindow, Inspector, menus, preview e gizmos. O runtime não deve carregar a parte Editor. Integrações externas também usam define constraints/placeholders; não habilitar um adapter apenas porque seu arquivo existe no pacote.

## Inventário confirmado de interfaces do MapMagic v2.1.11

As telas abaixo foram verificadas nos arquivos `Assets/MapMagic/**/Editor` do bundle local. “Customizado” significa que o pacote desenha uma interface própria em `OnInspectorGUI`; não significa que todos os campos internos da classe sejam editáveis.

| Alvo selecionado | Interface | Campos, seções e ações confirmados |
|---|---|---|
| `MapMagicObject` | Inspector customizado (`MapMagicInspector`) e Scene View | Campo `Graph`, abrir graph, `Create Empty`, `Create Template`, `Select`, `Generate`, `Generate Changed`, foldouts `Tiles`, `Locks`, `Infinite Terrain (Playmode)`, `Tile Settings`, `Outputs Settings`, `Exposed Variables`, `Terrain Properties`, `Trees, Details and Grass Properties`, `Multithreading` e `About` |
| `Graph` | Inspector customizado (`GraphInspector`) e Graph Window | `Open Editor`, `Open in New Tab`, `Seed`, quantidade de `Nodes`, versão serializada, `Overridden Variables` e `Dependent Graphs`; há aviso de possível lentidão se o asset permanecer selecionado |
| `TerrainTile` | Inspector customizado (`TerrainTileInspector`) | `Coord`, `Remoteness`, `Priority`, estados `Draft`/`Main`, `New`, `Remove`, `Current Detail`, `Selected for Preview`, `Ready`, complexidade e progresso |
| `MapMagicBrush` | Inspector customizado (`BrushInspector`) e Scene View | `Draw`, `Preset Source`, preset, `Save`, `Save As...`, `Presets`, `Terrains`, `Settings`, `Hard Color`, `Falloff Color`, `Thickness` e links em `About` |
| `Preset` | Inspector customizado (`PresetInspector`) | `Graph`, `Radius`, `Hardness`, `Spacing`, valores sobrescritos, `Used Auto-Values` e `All Auto-Values` |
| `DirectMatricesHolder` | Inspector customizado | Lista de layers/matrizes, desenhada pelo editor de layers |
| `DirectTexturesHolder` | Inspector customizado | Lista de layers/texturas, ícone/preview de textura e nome de cada layer |
| `MatrixAsset` | Inspector customizado | Preview, origem `Raw`/`Texture`, `Load RAW`, textura, canal e `Reload` |
| `MatrixObject` | Inspector customizado e Scene View | Preview, origem `Raw`/`Texture`/`New`, resolução/offset quando aplicável, `Reload`, posição/tamanho/altura mundial, gizmo, centralização e filtro |
| `SplineObject` | Inspector customizado + Scene View | O Inspector próprio não desenha campos adicionais no método verificado; a edição de spline é desenhada/manipulada na Scene View |
| `Terrain` | Inspector Unity estendido somente com `MM_ExtendedTerrainInspector` | O adapter reutiliza o `TerrainInspector` interno do Unity e observa strokes da Scene View; sem esse define, essa extensão não é registrada como `CustomEditor` |

### Detalhe do `MapMagicObject` Inspector

Quando `Graph` está vazio, a interface mostra o aviso de graph não atribuído e os botões `Create Empty`, `Create Template` e `Select`; o código retorna antes de desenhar as demais seções. Quando há graph válido, a interface mostra os comandos de geração e os foldouts:

| Seção | Controles confirmados no código |
|---|---|
| `Tiles` | Ferramentas de pin/seleção/exportação desenhadas por `PinDraw`; parte das ações ocorre na Scene View |
| `Locks` | Interface de locks desenhada por `LockDraw` |
| `Infinite Terrain (Playmode)` | `Generate Infinite Terrain`, `Main Range`, `Drafts Range`, `Hide Out-of-Range Terrains`, geração ao redor da câmera e de objetos com tag |
| `Tile Settings` | `Size`, `Main Resolution`, `Main Margins`, `Draft Resolution`, `Draft Margins`, uso de Draft no Editor/Playmode |
| `Outputs Settings` | Height Output, tipo de aplicação Main/Draft, split por frame, Grass Output e `Objects Output`; CTS/MicroSplat/MegaSplat/VSPro aparecem somente quando seus defines estão presentes |
| `Exposed Variables` | Overrides dos valores expostos nos defaults do graph |
| `Terrain Properties` | Auto Connect, grouping, base map, `Terrain Settings`, material template, outline e cópia de layers/tags/componentes |
| `Trees, Details and Grass Properties` | Categorias correspondentes de `TerrainSettings` |
| `Multithreading` | `Use Multithreading`, `Auto Max Threads`, `Max Threads`, tempo de Apply por frame e `Instant Generate`; o código avisa que valores de multithreading são compartilhados entre MapMagic objects |

Quando um desses campos muda, o código não trata todos da mesma forma: configurações de tile podem parar a geração, redimensionar tiles, limpar dados e gerar novamente; configurações de Terrain podem chamar `ApplyTerrainSettings`; configurações de output podem limpar e regenerar. Isso é uma sequência do editor v2.1.11, não uma regra geral para qualquer Inspector Unity.

### O que é Inspector e o que é janela/viewport

- `MapMagicObject`, `Graph`, `TerrainTile`, `MapMagicBrush`, `Preset`, holders e matrizes são alvos selecionáveis que abrem conteúdo no Inspector.
- `GraphWindow` é `EditorWindow`: o graph, nodes, links, preview e toolbar aparecem em uma janela própria, não como uma lista padrão do Inspector.
- `PinDraw`, `LockDraw`, gizmos de matriz e edição de spline usam a `SceneView`; selecionar um objeto pode desenhar controles no viewport.
- `SettingsWindow`, `AboutWindow`, `DocScreensWindow`, `LogWindow` e `TimerWindow` são janelas do editor. Elas não viram componentes da cena.
- Os campos dos generators são desenhados no Graph Window por editores de nodes; não se deve afirmar que cada generator tenha um `CustomEditor` separado sem verificar seu editor específico.

### Sequência para usar as interfaces do MapMagic

1. Instalar o pacote em um projeto Unity compatível e aguardar a compilação dos assemblies.
2. Criar `GameObject > 3D Object > MapMagic` ou arrastar/selecionar um `MapMagicObject`.
3. Selecionar o objeto e conferir o Inspector customizado.
4. Criar, selecionar ou atribuir um `Graph`.
5. Clicar em `Open Editor`/abrir o Graph Window para configurar generators, inlets, outlets, layers e outputs.
6. Voltar ao Inspector do `MapMagicObject` para configurar tiles, outputs, Terrain Properties e multithreading.
7. Usar `Generate` ou `Generate Changed`.
8. Selecionar tiles, graph ou brush para abrir suas interfaces específicas e acompanhar estado/preview.

### Janelas e menus confirmados

| Menu/ação | Janela ou resultado |
|---|---|
| `Window/MapMagic/Editor` | Abre o Graph Window associado à seleção/MapMagic |
| Botões `Open Editor`/`Open in New Tab` | Abrem o graph no Graph Window, em aba normal ou nova |
| `Window/MapMagic/Settings` | Abre Settings com símbolos, compatibilidades e auto-reference de assemblies |
| `Window/MapMagic/About` | Abre About |
| `Window/MapMagic/DocScreens` | Abre DocScreens |
| `Window/Log` | Abre Log; oferece Record, Clear e Threaded view |
| `Window/Timers` | Abre Timers; oferece Record, Group e Clear |
| `Assets/To Matrix Preview` | Abre/gera preview de matriz para o asset elegível |

## Fontes

- [Unity EditorWindow](https://docs.unity3d.com/ScriptReference/EditorWindow.html)
- [Unity CustomEditor](https://docs.unity3d.com/ScriptReference/CustomEditor.html)
- [Unity Undo](https://docs.unity3d.com/ScriptReference/Undo.html)
- [Godot EditorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html)
- [Godot EditorInspectorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorinspectorplugin.html)
- [Godot EditorUndoRedoManager](https://docs.godotengine.org/en/stable/classes/class_editorundoredomanager.html)
