# 01 — terreno procedural, TerrainData e chunks

## Função da categoria

O sistema de terreno transforma mapas numéricos em uma superfície renderizável, colidível e texturizada. No MapMagic, os generators trabalham antes do Terrain; os outputs `Height`, `Textures`, `Grass`, `Objects` e `Trees` aplicam o resultado.

## Unity: Terrain

### Terrain

**O que faz:** renderiza uma superfície baseada em heightmap. Também pode exibir TerrainLayers, árvores, detalhes e holes.

**Onde fica:** `GameObject > 3D Object > Terrain` ou criado pelo MapMagic.

**Propriedades principais:** referência a `TerrainData`, material, draw instancing, pixel error, base map distance, tree/detail distance, shadow e reflection probes.

**Processo:**

1. Terrain referencia TerrainData.
2. TerrainData contém heightmap e protótipos.
3. O renderer converte height samples em geometria/LOD.
4. Alphamaps escolhem TerrainLayers.
5. Detail maps instanciam grass/meshes.
6. Tree instances usam tree prototypes.
7. TerrainCollider usa o mesmo relevo para física.

**Precisa de:** `TerrainData`. Para colisão, `TerrainCollider`. Para textura, `TerrainLayer`. Para câmera visível, material/shader compatível.

### TerrainData

**O que faz:** guarda o estado de terreno: heightmap, tamanho, resolução, alphamaps, TerrainLayers, detail resolution, detail prototypes, tree prototypes e tree instances.

**Onde fica:** asset no Project ou objeto criado em runtime.

**Propriedades/API importantes:** `heightmapResolution`, `size`, `alphamapResolution`, `detailResolution`, `terrainLayers`, `treePrototypes`, `detailPrototypes`, `SetHeights`, `SetHeightsDelayLOD`, `SetAlphamaps`, `SetDetailLayer`, `SetTreeInstances`.

**Processo de altura:** MapMagic produz valores normalmente 0..1; o output define altura máxima; os valores são escritos no heightmap; LOD e collider são atualizados conforme o método.

**SetHeightsDelayLOD:** caminho rápido para edição interativa, pois posterga atualizações de LOD/vegetação; depois é necessário sincronizar/atualizar o heightmap.

**Risco:** `TerrainData` grande é caro em memória. Não duplicar TerrainData por tile sem lifecycle e limite.

### TerrainCollider

**O que faz:** cria colisão física a partir do heightmap de TerrainData.

**Onde fica:** no mesmo GameObject do Terrain, normalmente adicionado automaticamente.

**Precisa de:** TerrainData válido; Physics layer matrix; Rigidbody/CharacterController/raycast do outro objeto para interagir.

**MapMagic:** `applyColliders` e geração de tiles precisam ser coordenados. Atualizar visual sem collider cria player atravessando o chão.

### TerrainLayer

**O que faz:** descreve uma camada visual de solo. Guarda diffuse, normal, mask, tile size/offset, metallic/specular e normal scale.

**Onde fica:** `Assets > Create > Terrain Layer`; atribuído ao TerrainData ou ao Textures Output.

**Precisa de:** Terrain material/shader que leia o layer; alphamap/control map; textura importada corretamente.

**MapMagic:** Textures Output normaliza masks; a ordem das layers importa; editar propriedades dentro do node altera o asset TerrainLayer referenciado.

### DetailPrototype e TreePrototype

**DetailPrototype:** define grass ou mesh de detalhe, textura/mesh, modo de render, largura, altura, cor, densidade e resolução.

**TreePrototype:** define prefab/árvore usado por tree instances, bend factor e tint.

**Onde ficam:** dentro de TerrainData, manipulados pelo Terrain Inspector ou output do MapMagic.

## Unity: configuração de tile

### Tamanho e resolução

- `Tile Size`: tamanho mundial do chunk.
- `Main Resolution`: amostras principais.
- `Draft Resolution`: amostras de baixa resolução.
- `Main/Draft Margins`: pixels extras para filtros e continuidade.
- `Height Output Interpolation`: None, Smooth, Scale 2X, Scale 4X.

Um heightmap de 513 representa 512 polígonos por lado. Splat/control maps podem usar resolução 512; essa diferença de vértice/polígono é fonte de desalinhamento.

### Streaming

`Main Range` decide chunks detalhados perto do marker. `Drafts Range` mantém chunks baratos mais distantes. Markers podem ser câmera, objetos, tags ou coordenadas. Chunks fora do alcance podem ser ocultados/destruídos conforme a política.

### Margins e seams

Filtros como Blur, Erosion, Scatter, Stamp, Spline Stroke e Flatten precisam enxergar além da borda. Margins fornecem essa área. `Safe Borders` reduz a influência do filtro na borda. Noise deve usar coordenada mundial para continuar entre tiles.

## Godot: equivalente necessário

Godot não oferece um `Terrain` renderer nativo com o mesmo pacote de heightmap, TerrainLayer, trees e details. A implementação deve ser composta.

### MapMagicWorld customizado

**O que faz:** Node3D que coordena graph, tiles, ranges, drafts, geração assíncrona, materiais, colisão e instancing.

**Onde fica:** script customizado em `res://engine/terrain/` anexado à raiz do mundo.

**Precisa de:** graph Resource, geradores, câmera/markers, chunk manager, `ArrayMesh`, material/shader, colisão e política de cache.

### MeshInstance3D por chunk

**O que faz:** renderiza a mesh de um tile.

**Processo:**

1. Converter heightmap em vértices X/Z/Y.
2. Criar índices, normais e UVs.
3. Criar `ArrayMesh`.
4. Atribuir `ShaderMaterial`/`StandardMaterial3D`.
5. Adicionar como filho do chunk.
6. Trocar por mesh Draft/LOD distante.

### ArrayMesh

**O que faz:** permite montar mesh por arrays; é a escolha comum para terrain chunks gerados.

**Precisa de:** arrays com tamanhos coerentes; índices válidos; normais/UVs; material.

### HeightMapShape3D

**O que faz:** shape de colisão por heightmap. Não renderiza. Não representa overhangs/cavernas verdadeiras.

**Onde fica:** `CollisionShape3D.shape` dentro de `StaticBody3D`.

**Processo:** copiar dados do heightmap, definir largura/profundidade/valores, atualizar shape quando o chunk mudar.

**Custo:** mais rápido que mesh côncava para terreno, mas mais lento que primitivas simples.

### MultiMeshInstance3D

**O que faz:** renderiza muitas cópias de uma mesh para árvores, grass e props.

**Precisa de:** `MultiMesh`, mesh fonte, material e transformes por instância.

**Limitação:** instâncias não são Nodes individuais; scripts/colliders individuais exigem estratégia separada.

### Texturas e splat map

Godot exige shader próprio para misturar splat/control maps. Um shader pode:

1. ler RGBA/control texture;
2. amostrar Terrain textures;
3. combinar por pesos;
4. calcular normal/roughness;
5. aplicar macro variation;
6. produzir resultado no fragment.

### Grass/árvores

- poucas árvores interativas: `PackedScene` instanciada;
- milhares de árvores: `MultiMeshInstance3D`;
- grass visual: `MultiMeshInstance3D`, partículas ou shader;
- grass com colisão: não usar somente MultiMesh; criar representação física simplificada.

## Dependências comparadas

| Recurso | Unity | Godot |
|---|---|---|
| Renderer de terreno | `Terrain` nativo | Mesh customizada/plug-in |
| Dados do relevo | `TerrainData` | `Resource` + arrays/Image |
| Texturas | `TerrainLayer` + alphamaps | shader + textures/control map |
| Collider | `TerrainCollider` | `StaticBody3D + HeightMapShape3D` |
| Grass | DetailPrototype | MultiMesh/particles/shader |
| Árvores | TreePrototype/instances | PackedScene/MultiMesh |
| Tile streaming | MapMagic + Terrain tiles | Node customizado + chunk manager |

## Tutorial de validação

### Unity

1. Crie Terrain.
2. Crie TerrainLayer.
3. Defina heightmap 513 e size 1000 x 600.
4. Use um Graph com Noise > Height.
5. Ative TerrainCollider.
6. Adicione câmera e um CharacterController.
7. Verifique seams criando pelo menos quatro tiles.

### Godot

1. Crie `World : Node3D`.
2. Crie `MapMagicWorld : Node3D`.
3. Gere um chunk plano com `ArrayMesh`.
4. Adicione `MeshInstance3D`.
5. Crie `StaticBody3D > CollisionShape3D > HeightMapShape3D`.
6. Adicione `Camera3D`.
7. Troque heightmap por Noise e confirme mesh/collider juntos.

## Sequência confirmada no código do bundle v2.1.11

No `TerrainTile.StartGenerate`, o bundle declara estes passos para cada nível:

1. Para Draft, cria `TileData` se necessário.
2. Define `Area` usando coordenada, `draftResolution`, `draftMargins` e `tileSize`.
3. Copia `globals` e o random do Graph.
4. Marca `isDraft=true`, `generateStarted=true`, `applyReady=false` e `generateReady=false`.
5. Coloca a tarefa na fila com prioridade de Draft.
6. Para Main, repete o preparo com `tileResolution` e `tileMargins`.
7. Cancela/encerra a tarefa Main anterior antes de enfileirar a nova.
8. Executa `Prepare`, que chama `graph.Prepare(data, terrain)`.
9. Executa `graph.Generate(data, stop)` e depois `graph.Finalize(data, stop)`.
10. Lê e solda as bordas (`Weld.ReadEdges`, `Weld.WeldEdgesInThread`, `Weld.WriteEdges`).
11. Enfileira aplicação Draft imediata ou rotina Main.
12. A aplicação retira cada `IApplyData` da fila e chama `Apply(terrain)`; rotinas podem ceder frames.
13. Quando a aplicação termina, marca `applyReady`, troca LOD e emite `OnTileApplied`.
14. Quando não há mais trabalho de thread/coroutine, emite `OnAllComplete`.

O código também declara eventos `OnBeforeTileStart`, `OnBeforeTilePrepare`, `OnBeforeTileGenerate`, `OnTileFinalized`, `OnTileApplied`, `OnAllComplete`, `OnLodSwitched`, `OnPreviewAssigned` e `OnTileMoved`. Esses eventos são pontos de integração declarados; não devem ser tratados como garantia de que o pacote cria um sistema de gameplay por trás deles.

## O que o Unity fornece e o MapMagic apenas utiliza

- `Terrain` visualiza os dados;
- `TerrainData` armazena heightmap, alphamaps, details e trees;
- `TerrainCollider` participa da física;
- `TerrainLayer` descreve a textura;
- o MapMagic coordena tiles, dados, geração, solda e aplicação.

O MapMagic não declara um renderer de Terrain próprio no núcleo v2.1.11; ele opera sobre os tipos Terrain do Unity.

## Interface de propriedades e Inspector

| Elemento | Interface padrão/costumeira | Campos ou ações documentados | É requisito do MapMagic v2.1.11? |
|---|---|---|---|
| Unity `Terrain` | Inspector do `GameObject`; o Unity fornece as ferramentas de terreno | Referência ao `TerrainData`, material, altura, texturas, árvores, detalhes e ferramentas de pintura conforme a versão do Unity | O bundle usa o tipo `Terrain`; a presença de cada aba do Inspector depende da versão do Unity |
| Unity `TerrainData` | Inspector do asset no `Project` e edição indireta pelo `Terrain` | Resolução/tamanho, heightmap, alphamap, layers, árvores e detalhes | É o dado aplicado aos tiles; o MapMagic não substitui o Inspector do Unity |
| Unity `TerrainCollider` | Inspector do componente no mesmo `GameObject` | Referência ao `TerrainData` e propriedades do collider disponíveis na versão | O `MapMagicObject` possui `applyColliders = true`, mas o componente de gameplay do jogador não é criado pelo gerador |
| Unity `TerrainLayer` | Inspector do asset | Textura difusa/normal, tile size/offset e propriedades de layer conforme a versão | O output de textura depende de layers/configuração do terreno; não confundir com shader customizado |
| `DetailPrototype`/`TreePrototype` | Configurados dentro de `TerrainData`/ferramentas do Terrain | Protótipos de detalhes e árvores | O bundle possui outputs de trees/grass; o uso efetivo depende do output e das configurações do graph |
| Godot `MeshInstance3D` por chunk | Inspector do nó | `mesh`, material override, visibilidade, layers e propriedades herdadas | É uma implementação proposta para Godot, não um componente criado pelo MapMagic original |
| Godot `ArrayMesh` | Inspector do recurso ou campo `mesh` | Superfícies, materiais e dados da malha conforme o recurso | Proposta para representar uma saída de malha; não é requisito original do MapMagic |
| Godot `HeightMapShape3D` | Inspector da `Shape3D` ligada ao `CollisionShape3D` | Dados do heightmap e dimensões da forma | É uma alternativa proposta para colisão de terreno em Godot |
| Godot `MultiMeshInstance3D` | Inspector do nó e do recurso `MultiMesh` | Mesh, transformações por instância e material | Proposta para árvores/grass instanciados; não é saída declarada do bundle |

### Sequência da alteração pelo Inspector

1. Selecionar o `Terrain` na `Hierarchy` ou o asset `TerrainData` no `Project`.
2. Alterar a propriedade na interface do Unity.
3. O Unity marca/aplica os dados conforme a operação e a versão do editor.
4. Se a alteração for feita no contexto do MapMagic, o graph/output pode escrever novamente o dado ao gerar ou aplicar o tile.
5. No caso de uma implementação Godot, o nó/recurso correspondente teria de receber os dados gerados e atualizar mesh e shape; isso é proposta de integração, não comportamento do bundle.

O Inspector não significa que todos os campos estejam editáveis em todos os estados. Alguns dados são internos, derivados, dependem de um asset atribuído ou só são desenhados por uma ferramenta de editor.

## Fontes

- [Unity TerrainData](https://docs.unity3d.com/ScriptReference/TerrainData.html)
- [Unity SetHeightsDelayLOD](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/TerrainData.SetHeightsDelayLOD.html)
- [Unity TerrainLayer](https://docs.unity3d.com/Manual/class-TerrainLayer.html)
- [Godot HeightMapShape3D](https://docs.godotengine.org/en/4.4/classes/class_heightmapshape3d.html)
- [Godot MultiMeshInstance3D](https://docs.godotengine.org/en/latest/tutorials/3d/using_multi_mesh_instance.html)
- [MapMagic Settings wiki](https://gitlab.com/denispahunov/mapmagic/-/wikis/Main/Settings)
