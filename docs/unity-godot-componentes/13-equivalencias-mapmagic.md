# 13 — arquitetura MapMagic para Godot ou engine própria

## Objetivo

Este documento mostra como mapear o contrato do MapMagic 2 para Godot sem tentar transformar cada node do graph em um Node da Scene Tree.

> Importante: este arquivo contém duas camadas. As partes que dizem “MapMagic declara” são baseadas no bundle/wiki; as partes que dizem “implementação sugerida” são proposta de arquitetura para Godot, não funcionalidades existentes do MapMagic.

## Camadas

```text
Core
 ├── IDs, Result, Variant, serialization, events
 ├── math, matrix, spline, hash, jobs
Graph runtime
 ├── Generator, inlet, outlet, link, layer
 ├── prepare, generate, apply, clear
World streaming
 ├── tile coordinates, Main/Draft, margins, locks
Godot adapter
 ├── MeshInstance3D, materials, physics, navigation, audio
Editor
 ├── GraphEdit, Inspector, gizmos, undo, asset browser
```

## Componentes customizados sugeridos

| Classe | Responsabilidade |
|---|---|
| `MapMagicWorld : Node3D` | Coordena graph, markers, tiles e lifecycle |
| `TerrainGraph : Resource` | Serializa generators/links/layers/defaults |
| `Generator : Resource` | Algoritmo isolado, sem depender da cena |
| `MatrixWorld` | Array/Imagem espacial com coordenadas mundiais |
| `TerrainChunk : Node3D` | Estado de um tile e seus filhos |
| `TerrainMeshView : MeshInstance3D` | Visual do chunk |
| `TerrainCollision : StaticBody3D` | Collider do chunk |
| `TerrainMaterial` | Shader/splat/biome material |
| `ObjectSpawner` | PackedScene/MultiMesh e pooling |
| `SplineResource` | Pontos, segmentos, metadata e portais |
| `BiomeResource` | Graph/mask/configuração do biome |
| `TerrainBrush : Node3D` | Stroke e edição de height/textures/objects |
| `TileCache` | Estado/cache/version/hash/dependencies |
| `GenerationScheduler` | Prioridade, workers, cancelamento e orçamento |
| `GraphEditorPlugin` | GraphEdit, Inspector, menus e UndoRedo |

## Processo de um tile

1. Marker pede coordenada.
2. Scheduler classifica Main/Draft.
3. Graph prepara dependências.
4. Worker gera matrices/splines/objects.
5. Resultado é validado por versão/hash.
6. Main thread cria/atualiza mesh.
7. Collider é atualizado.
8. Material recebe splat/biome maps.
9. Objects/MultiMesh são aplicados.
10. NavigationRegion é baked/updated quando necessário.
11. Chunk vira Ready.
12. Tile distante é Unloading e devolvido ao pool/cache.

## Equivalências de outputs

| MapMagic | Implementação Godot |
|---|---|
| Height Output | ArrayMesh + vertices ou Resource de height |
| Textures Output | control map + ShaderMaterial |
| Grass Output | MultiMeshInstance3D/GPUParticles3D/shader |
| Objects Output | PackedScene instances/pool |
| Trees Output | MultiMeshInstance3D ou PackedScene |
| Spline Output | ArrayMesh/ImmediateMesh/custom spline renderer |
| Direct Matrices | Resource/buffer signal |
| Direct Textures | ImageTexture/texture buffer |
| Draft | menor ArrayMesh/resolução/qualidade |
| Locks | máscara persistente por coordenada/versão |

## Contratos de generator

Todo generator deve informar:

- ID estável;
- tipo de entrada/saída;
- propriedades refletidas;
- dependências;
- custo estimado;
- possibilidade de rodar em worker;
- dados necessários de margin;
- output level Main/Draft/Both;
- serialização/versionamento;
- clear/invalidate;
- preview;
- erro contextual.

## Prioridade

Não criar aba especial de “Prioridades”. Use scheduler com:

- distância da câmera;
- Main antes de Draft perto do player;
- tile visível antes de invisível;
- dependência de graph;
- custo estimado;
- cancelamento de trabalho obsoleto;
- limite de CPU/GPU/memória;
- Apply budget por frame.

## Tutorial vertical slice

### Fase 1 — terreno

1. Graph Resource com Constant e Noise.
2. MapMagicWorld.
3. Um TerrainChunk.
4. ArrayMesh.
5. HeightMapShape3D.
6. Camera3D e DirectionalLight3D.

### Fase 2 — textura

1. Control map RGBA.
2. ShaderMaterial.
3. Splat texture.
4. Blend por biome.

### Fase 3 — objetos

1. Scatter determinístico.
2. PackedScene.
3. Pool.
4. MultiMesh para repetidos.

### Fase 4 — editor

1. GraphEdit/GraphNode.
2. Resource Inspector.
3. Link validation.
4. UndoRedo.
5. Save/load/version migration.

### Fase 5 — mobile

1. Draft range.
2. Main range.
3. worker scheduler.
4. memory budget.
5. renderer Mobile.
6. Android pause/resume.

## Fonte principal

- [MapMagic wiki oficial](https://gitlab.com/denispahunov/mapmagic/-/wikis/home)
- [MapMagic repositório oficial](https://gitlab.com/denispahunov/mapmagic)
- [Godot Navigation 3D](https://docs.godotengine.org/en/stable/tutorials/navigation/navigation_introduction_3d.html)
- [Godot MultiMesh](https://docs.godotengine.org/en/latest/tutorials/3d/using_multi_mesh_instance.html)
- [Unity TerrainData](https://docs.unity3d.com/ScriptReference/TerrainData.html)

## O que é fato e o que é proposta

### Confirmado no MapMagic

- Graph, Generator, inlet/outlet, layers e outputs;
- MatrixWorld, TransitionsList e SplineSys como dados de geração;
- Terrain tiles Main/Draft, margins, locks, markers e ranges;
- preparação, geração, finalização, solda e aplicação;
- Graph/Imported Map/Brush Preset como assets do ecossistema Unity;
- integração com Terrain/TerrainData e adapters externos condicionais.

### Proposto para uma implementação Godot

- `MapMagicWorld : Node3D`;
- Graph como `Resource`;
- terrain por `ArrayMesh`;
- colisão por `HeightMapShape3D`;
- árvores por MultiMesh/PackedScene;
- editor por GraphEdit/GraphNode;
- scheduler e cache próprios.

Esses nomes e escolhas da segunda lista não são classes entregues pelo MapMagic nem devem ser apresentados como se fossem funcionalidades já existentes no Godot.
