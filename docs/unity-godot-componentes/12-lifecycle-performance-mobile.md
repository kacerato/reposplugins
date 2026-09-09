# 12 — lifecycle, threads, memória, streaming e mobile

## Unity lifecycle

### Ordem prática

- `Awake`: criar/validar referências;
- `OnEnable`: registrar eventos;
- `Start`: iniciar depois de todos os Awakes;
- `Update`: lógica por frame;
- `FixedUpdate`: física/timestep fixo;
- `LateUpdate`: câmera e pós-processamento de gameplay;
- `OnDisable`: remover inscrições;
- `OnDestroy`: liberar recursos.

### Android

Tratar `OnApplicationPause`, `OnApplicationFocus`, perda/retorno de Surface, memory pressure, orientação, safe area, permissões e armazenamento.

## Godot lifecycle

- `_enter_tree`: Node entrou na árvore;
- `_ready`: filhos prontos;
- `_process(delta)`: por frame;
- `_physics_process(delta)`: timestep de física;
- `_input(event)`: input precoce;
- `_unhandled_input(event)`: gameplay após UI;
- `_exit_tree`: removendo;
- `_notification`: eventos de lifecycle/engine.

## Threads

### Unity

`System.Threading`, `Task`, C# Jobs, Burst e NativeArray têm papéis diferentes. UnityEngine objects normalmente só podem ser acessados na main thread. Geração de matrizes pode ir para worker; aplicação em Terrain/GameObject deve retornar à main thread.

### Godot

`Thread`, `WorkerThreadPool`, `Mutex`, `Semaphore`, `call_deferred` e servers permitem trabalho paralelo. Scene Tree e Nodes não devem ser manipulados livremente de worker thread; gere dados fora e aplique na main thread.

## MapMagic thread/application model

MapMagic v2 possui ThreadManager, workers, fila e rotina de apply. `Max Threads`, `Auto Max Threads`, `Apply Time per Frame`, `Instant Generate` e `Objects Num/Frame` equilibram velocidade contra frame spikes.

## Memória

### Principais consumidores

- TerrainData/heightmaps;
- splat/control maps;
- detail maps;
- texturas Read/Write;
- meshes duplicadas;
- GameObjects de objetos;
- NavMesh tiles;
- buffers de partículas;
- caches sem limite.

### Regras

- limitar resolução por distância;
- Draft para tiles longínquos;
- liberar tile fora do range;
- pooling de objetos;
- instancing para vegetação;
- evitar allocation por frame;
- reutilizar arrays/buffers;
- comprimir texturas;
- não manter Texture Read/Write quando não necessário;
- medir antes/depois.

## Streaming de mundo

```text
marker/câmera
   ↓
calcular tile coordinates
   ↓
priorizar Main/Draft
   ↓
gerar em worker
   ↓
aplicar em main thread
   ↓
ativar visual/collider/nav
   ↓
descarregar/poolar tiles distantes
```

## Performance de terreno

- resolução maior aumenta custo de mapa e apply;
- margins aumentam área processada;
- Erosion é iterativo e caro;
- Pathfinding cresce com resolução do grid;
- Forest/Scatter/Random crescem com densidade;
- Scale 2X/4X aumenta geometria aplicada;
- muitos GameObjects custam CPU/GC;
- sombras/GI/reflexos custam GPU;
- shaders complexos e overdraw impactam mobile.

## Android e MapMagic

Validar:

- ARM64;
- IL2CPP;
- Vulkan/OpenGL ES quando aplicável;
- DLL nativa e plugin import settings;
- símbolos `MM_NATIVE`;
- `UnityEditor` em assemblies runtime;
- Activity pause/resume;
- limite de memória;
- thermal throttling;
- orientação e safe area;
- build Development/Release;
- GPU Adreno/Mali reais.

## Tutorial de validação

### Unity

1. Gere um único tile.
2. Meça CPU/GPU/memória.
3. Ative Draft e quatro tiles.
4. Compare Main Range.
5. Aumente margins/resolução.
6. Teste Erosion/Forest separadamente.
7. Faça build Android e suspenda/retome o app.

### Godot

1. Gere ArrayMesh em worker sem tocar Scene Tree.
2. Use `call_deferred` para aplicar.
3. Crie chunks Main/Draft.
4. Instancie árvores via MultiMesh.
5. Atualize HeightMapShape3D.
6. Faça profiling no renderer Mobile.

## Sequência confirmada do scheduler v2.1.11

O `TerrainTile` declara uma tarefa de thread para gerar e uma coroutine para aplicar. Para Main, uma tarefa anterior ativa pode receber stop; para Draft, a tarefa é enfileirada com prioridade própria. A prioridade do tile é derivada da distância: o código declara `Priority => (int)(-distance*100)` e adiciona preferência ao Draft.

A prioridade aqui é interna do scheduler. Não é uma aba do Unity nem um componente externo.

O bundle declara também:

- `instantGenerate = true` por padrão;
- `saveIntermediate = true` por padrão;
- `tileResolution = 513`;
- `tileMargins = 16`;
- `draftResolution = 65`;
- `draftMargins = 2`;
- `draftsInEditor = true`;
- `draftsInPlaymode = true`;
- `mainRange = 1`;
- `hideFarTerrains = true`;
- `applyColliders = true`.

Esses são defaults confirmados do código v2.1.11, não valores universais para todo projeto. Alterá-los muda custo, memória, tempo e aparência.

## Fontes

- [Unity lifecycle](https://docs.unity3d.com/Manual/ExecutionOrder.html)
- [Unity Job System](https://docs.unity3d.com/Manual/job-system.html)
- [Godot Node lifecycle](https://docs.godotengine.org/en/stable/classes/class_node.html)
- [Godot threads](https://docs.godotengine.org/en/stable/tutorials/performance/using_multiple_threads.html)
- [MapMagic Settings wiki](https://gitlab.com/denispahunov/mapmagic/-/wikis/Main/Settings)
