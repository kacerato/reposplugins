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

## Fontes

- [Unity EditorWindow](https://docs.unity3d.com/ScriptReference/EditorWindow.html)
- [Unity CustomEditor](https://docs.unity3d.com/ScriptReference/CustomEditor.html)
- [Unity Undo](https://docs.unity3d.com/ScriptReference/Undo.html)
- [Godot EditorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html)
- [Godot EditorInspectorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorinspectorplugin.html)
- [Godot EditorUndoRedoManager](https://docs.godotengine.org/en/stable/classes/class_editorundoredomanager.html)
