# 00 — fundamentos de cena e composição

## Ideia central

Unity organiza comportamento em `GameObject + Component`. Godot organiza comportamento em `Node` dentro de uma `Scene Tree`. Essa diferença define como terreno, editor, objetos e scripts serão modelados.

## Unity: GameObject e Component

### GameObject

**O que faz:** é o contêiner de identidade, nome, layer, tag, estado ativo e componentes. Um GameObject vazio não renderiza, não colide e não tem lógica além do Transform.

**Onde fica:** `Hierarchy`; criar com `GameObject > Create Empty` ou pelos menus de objetos 2D/3D.

**Processo:**

1. Unity cria o objeto e seu `Transform` obrigatório.
2. Componentes são anexados.
3. O objeto entra na Scene Tree.
4. Unity chama lifecycle dos scripts anexados.
5. Componentes consultam Transform, recursos e outros componentes.

**Precisa de:** `Transform`, que não pode ser removido. Pode ter muitos componentes, mas não dois componentes do mesmo tipo quando o tipo não permite múltiplas instâncias.

**Exemplo:** um carro é um GameObject raiz com `Transform`, `MeshRenderer`, `Rigidbody`, `Collider`, script de veículo, áudio e filhos para rodas.

### Component

**O que faz:** base dos componentes anexáveis. Acesso comum: `GetComponent<T>()`, `TryGetComponent<T>()`, `GetComponentInChildren<T>()` e `GetComponentInParent<T>()`.

**Onde fica:** Inspector do GameObject selecionado.

**Processo:** o componente recebe referência ao GameObject e Transform; seus campos serializados aparecem no Inspector; métodos de lifecycle são chamados conforme o objeto e o componente são habilitados.

**Risco:** chamar `GetComponent` e assumir que existe causa referência nula. Use `RequireComponent` quando a dependência for estrutural e valide referências no `Awake`/`OnValidate`.

### Transform

**O que faz:** posição, rotação, escala, parent/child, matriz local/world e direção. É a fundação de terreno, câmera, objetos, gizmos e física.

**Onde fica:** primeiro componente de qualquer GameObject.

**Propriedades:** `position`, `localPosition`, `rotation`, `localRotation`, `localScale`, `parent`, `lossyScale`, `forward`, `right`, `up`.

**Processo:** local transform é combinado com o parent para produzir world transform. Ao reparentear, decidir entre manter ou converter posição mundial.

**MapMagic:** tile coordinates e offsets dependem de Transform e unidades de mundo. Não espalhar conversões X/Z em cada generator; centralizar em uma função de coordenadas.

### RectTransform

**O que faz:** extensão do Transform para UI. Usa anchors, pivot, offsets e size delta.

**Onde fica:** elementos dentro de `Canvas`.

**Precisa de:** Canvas/CanvasRenderer ou Controlador de UI.

**Erro comum:** misturar posição absoluta com anchors sem entender o parent; a UI quebra em resoluções diferentes.

## Unity: scripts e dados

### MonoBehaviour

**O que faz:** permite anexar script ao GameObject e participar do lifecycle.

**Lifecycle principal:** `Awake` prepara referências; `OnEnable` registra eventos; `Start` inicializa depois do enable; `Update` roda por frame; `FixedUpdate` roda no timestep de física; `LateUpdate` roda depois dos Updates; `OnDisable` desfaz inscrições; `OnDestroy` libera recursos.

**Onde fica:** arquivo C# dentro de `Assets`; arraste o script ao GameObject.

**Precisa de:** assembly compilável e classe herdando de `MonoBehaviour`. O nome do arquivo/classe deve respeitar as regras do Unity para aparecer como componente.

### ScriptableObject

**O que faz:** asset de dados compartilhado. Ideal para Graph, configuração, input map, material lógico, preset, biome, veículo e save schema.

**Onde fica:** `Project`/`Assets`; aparece por `CreateAssetMenu`.

**Processo:** criar asset, preencher dados, referenciar em componentes. O mesmo asset pode ser usado por várias instâncias; isso é compartilhamento, não cópia.

**MapMagic:** `Graph`, `Imported Map` e `Brush Preset` seguem esse padrão.

### Prefab

**O que faz:** salva um GameObject/hierarquia como asset reutilizável.

**Onde fica:** Project View; instâncias aparecem na Hierarchy.

**Processo:** criar prefab; instanciar; aplicar ou reverter overrides; destruir a instância quando o tile sai de alcance.

**MapMagic:** `Objects` e `Trees` recebem prefab; pooling reduz custo de destruir/recriar em streaming.

## Godot: Node e Scene Tree

### Node

**O que faz:** unidade básica de composição. Possui nome, parent, children, groups, signals, script e callbacks.

**Onde fica:** painel `Scene`.

**Processo:** criar node; anexar filhos; anexar script; adicionar à árvore; receber `_enter_tree`, `_ready`, `_process`, `_physics_process` e `_exit_tree`.

**Precisa de:** parent/SceneTree para processar e receber sinais. Um Node fora da árvore existe como objeto, mas não participa normalmente do ciclo da cena.

### Node2D

**O que faz:** Node com Transform2D para posição, rotação e escala 2D.

**Onde fica:** cenas 2D; raiz típica de um jogo 2D.

### Node3D

**O que faz:** Node com Transform3D para mundo 3D. É a base de `Camera3D`, `MeshInstance3D`, lights, corpos físicos e nodes de navegação.

**Onde fica:** cenas 3D; criar `Add Child Node > Node3D`.

### Control

**O que faz:** base da UI Godot; anchors, layout, foco, mouse/touch, minimum size e desenho.

**Onde fica:** cena com CanvasLayer ou diretamente sob raiz, conforme viewport.

### Resource

**O que faz:** asset/dado reutilizável. Pode ser salvo em `.tres`/`.res` e referenciado por vários Nodes.

**MapMagic-like:** graph, generator settings, biome, terrain chunk metadata e brush preset devem ser `Resource`, não milhares de Nodes na cena.

### PackedScene

**O que faz:** cena salva e instanciável, equivalente a prefab.

**Processo:** salvar cena; carregar com `load`/`preload`; `instantiate`; adicionar à árvore; remover quando sair do streaming.

## Comparação de dependências

| Necessidade | Unity | Godot |
|---|---|---|
| Transformação | `Transform` obrigatório | `Node2D`/`Node3D` |
| Script de cena | `MonoBehaviour` | Script anexado a `Node` |
| Asset de dados | `ScriptableObject` | `Resource` |
| Prefab | Prefab | `PackedScene` |
| Tag única | Tag | Groups, que permitem múltiplos grupos |
| Hierarquia | Hierarchy | Scene Tree |
| Inspector | Inspector | Inspector |
| Eventos | UnityEvent/C# events | Signals |

## Tutorial mínimo

### Unity

1. Crie `GameObject > Create Empty` chamado `World`.
2. Crie um script `WorldRoot : MonoBehaviour`.
3. Anexe-o a `World`.
4. Crie um `ScriptableObject` de configuração.
5. Referencie o asset no Inspector.
6. Em `Awake`, valide a referência; em `OnDestroy`, libere serviços.

### Godot

1. Crie uma cena com raiz `Node3D` chamada `World`.
2. Crie um script `world_root.gd`.
3. Anexe o script.
4. Crie um `Resource` de configuração.
5. Exponha-o com `@export`.
6. Valide em `_ready` e remova listeners em `_exit_tree`.

## Sequência confirmada no MapMagic v2.1.11

O código do bundle declara `MapMagicObject` como `MonoBehaviour`, `IMapMagic` e `ISerializationCallbackReceiver`. A sequência efetivamente declarada é:

1. `OnEnable` registra `EditorApplication.update` quando está no Editor.
2. Se o material de Terrain estiver vazio, chama `DefaultTerrainMaterial()`.
3. Para tiles que ainda não estão prontos, chama `StartGenerateNonReady()`.
4. Em `Update`, chama `tiles.Update(...)` e `Den.Tools.Tasks.CoroutineManager.Update()`.
5. Mudanças podem chamar `Refresh`, `Clear`, `ClearAll`, `StartGenerate`, `StopGenerate` ou `ResetTerrains`.
6. `StartGenerate` lança a geração dos tiles; se não existir Graph, o método lança erro explícito `MapMagic: Graph data is not assigned`.
7. `OnDisable` remove o callback de `EditorApplication.update`.

Isso é o fluxo confirmado pelo código; não significa que todo componente Unity listado neste catálogo seja criado pelo MapMagic. O bundle declara diretamente o `MapMagicObject`, `Graph`, Terrain/TerrainData e seus outputs. Animator, AudioSource, Rigidbody e outros pertencem aos prefabs/projeto quando utilizados, não ao núcleo do MapMagic.

## Fontes

- [Unity components](https://docs.unity3d.com/Manual/Components.html)
- [Godot Node](https://docs.godotengine.org/en/stable/classes/class_node.html)
- [Godot Node3D](https://docs.godotengine.org/en/stable/classes/class_node3d.html)
- [Godot Resource](https://docs.godotengine.org/en/stable/classes/class_resource.html)
