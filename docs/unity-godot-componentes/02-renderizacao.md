# 02 — renderização, geometria, materiais e shaders

## O que esta categoria resolve

Renderização transforma dados de cena em pixels. Para um mundo procedural, ela precisa desenhar terrain chunks, materiais, árvores, grass, água, debug draw, LOD e preview sem misturar a lógica do graph com chamadas específicas de GPU.

## Unity: componentes visuais

### Renderer

**O que faz:** base de objetos renderizáveis; possui materials, bounds, shadows, light probes, reflection probes, rendering layer e visibility.

**Onde fica:** Inspector de GameObject visual.

**Precisa de:** MeshFilter/mesh ou geometry equivalente; material/shader; câmera que inclua a layer.

### MeshFilter

**O que faz:** fornece uma `Mesh` para `MeshRenderer`.

**Processo:** MeshFilter referencia o asset; MeshRenderer lê vértices/índices/UVs; GPU desenha submeshes com materials.

**Não faz:** não renderiza sozinho e não cria collider.

### MeshRenderer

**O que faz:** desenha mesh estática com materials e configurações de shadow/probe.

**Precisa de:** MeshFilter no mesmo GameObject ou mesh compatível; pelo menos um material por submesh quando necessário.

### SkinnedMeshRenderer

**O que faz:** desenha mesh deformada por bones, bind poses, weights e blend shapes.

**Uso:** personagens, animais, deformação de veículos e objetos animados.

**Custo:** skinning, bones e overdraw; avaliar LOD/mobile.

### SpriteRenderer

**O que faz:** desenha Sprite/Texture 2D com sorting layer, order, flip, color e material.

### LineRenderer

**O que faz:** desenha uma linha por pontos. Útil para spline preview, estrada temporária, raycast e debug.

### TrailRenderer

**O que faz:** cria geometria seguindo o histórico de posição.

### ParticleSystemRenderer

**O que faz:** desenha o resultado do ParticleSystem como billboard, mesh, trail ou ribbon.

### TilemapRenderer

**O que faz:** desenha tiles 2D; não é o Terrain system.

## Unity: Mesh e dados geométricos

### Mesh

**O que faz:** guarda vertices, indices, normals, tangents, UVs, colors, bone weights e submeshes.

**MapMagic:** pode ser usada para outputs customizados/terrain alternativo, mas Terrain nativo usa TerrainData.

### RenderTexture

**O que faz:** textura renderizável por câmera/shader. Útil para preview, mapas intermediários, minimap e captura.

### Material

**O que faz:** instancia/configura shader e valores. Pode referenciar texturas, floats, colors, buffers e keywords.

**Cuidado:** alterar `material` pode criar instância; alterar `sharedMaterial` altera asset/compartilhado. Documentar ownership.

### Shader

**O que faz:** código de GPU para vertex/fragment/compute conforme pipeline.

**Dependência:** Built-in, URP, HDRP ou shader customizado; propriedades precisam bater com o script/material.

### ShaderVariantCollection

**O que faz:** conserva variantes que serão aquecidas para evitar travadas de compilação no runtime.

## Unity: otimização visual

### LODGroup

**O que faz:** troca renderers por níveis conforme distância.

**MapMagic:** usar em props/árvores instanciadas; terrain tem seus próprios LOD settings.

### OcclusionArea/OcclusionPortal

**OcclusionArea:** define volume usado no bake de occlusion culling.

**OcclusionPortal:** abre/fecha passagem em ambientes com portais.

### ReflectionProbe

Captura ambiente local para reflexos. Não é substituto de iluminação global.

### LightProbeGroup

Fornece probes para iluminação indireta de objetos dinâmicos.

### LightProbeProxyVolume

Usa vários probes para representar iluminação de objeto grande.

### Draw Instanced

Configuração do Terrain/renderer que permite instancing de detalhes/vegetação quando material e pipeline suportam.

## Godot: componentes visuais

### VisualInstance3D

Base de instâncias visuais 3D, visibility layers, bounds e registro no cenário visual.

### GeometryInstance3D

Adiciona material override, cast shadow, transparência, LOD bias, culling e visibility ranges. É base de MeshInstance3D, MultiMeshInstance3D e partículas 3D.

### MeshInstance3D

**O que faz:** renderiza uma `Mesh` com material.

**Onde fica:** `Add Child Node > MeshInstance3D`.

**Precisa de:** `Mesh` (PlaneMesh, ArrayMesh etc.) e material opcional.

### MultiMeshInstance3D

**O que faz:** desenha muitas instâncias da mesma mesh via `MultiMesh`.

**Uso:** árvores, grass, pedras, postes e objetos repetidos.

**Limitação:** não cria um Node independente por instância.

### Sprite3D/AnimatedSprite3D

Renderizam textura 2D no mundo 3D; AnimatedSprite3D alterna frames.

### CSGShape3D

Gera geometria booleana/primitiva para protótipo. Não é a escolha para milhares de chunks de terreno.

### Label3D

Texto no mundo 3D, útil para gizmos, labels de debug e markers.

### Decal

Projeta textura sobre geometria; útil para marcas, sujeira, seleção e debug.

## Godot: materiais e geometria

### Mesh

Resource de geometria. `ArrayMesh` é adequado para dados calculados; `PrimitiveMesh` serve para protótipo.

### StandardMaterial3D

Material PBR configurável pelo Inspector: albedo, metallic, roughness, normal, emission, transparency, culling e UV.

### ShaderMaterial

Material que usa shader customizado. Necessário para splat maps, blend de biomas, triplanar, macro variation e terrain procedural avançado.

### Shader

Programa escrito na linguagem de shader do Godot. Deve respeitar renderer/métodos suportados e limitações mobile.

### ImmediateMesh

Gera geometria imediata; útil para debug/linhas, não ideal para mundo grande persistente.

## Processo completo de renderização procedural

```text
Graph / height data
   ↓
CPU: vértices, normais, UVs, material data
   ↓
Mesh/TerrainData/ArrayMesh
   ↓
Renderer + material/shader
   ↓
Camera culling + LOD + shadows
   ↓
GPU draw
```

## Dependências que precisam ser explícitas

- câmera ativa;
- layer/culling mask ou visibility layer;
- material válido;
- shader compatível com pipeline/renderer;
- bounds corretos para culling;
- normals/tangents quando iluminação exigir;
- textures com formato e import settings adequados;
- instancing compatível com material;
- LOD configurado para distância;
- resolução coerente com GPU/memória.

## Tutorial de validação

### Unity

1. Crie um Plane com `MeshFilter + MeshRenderer`.
2. Crie Material com shader padrão.
3. Atribua albedo e normal.
4. Crie um segundo material transparente.
5. Adicione LODGroup em três meshes.
6. Ative/desative culling mask da câmera.
7. Observe material, shadows, bounds e profiler.

### Godot

1. Crie `Node3D`.
2. Adicione `MeshInstance3D` com PlaneMesh.
3. Atribua StandardMaterial3D.
4. Troque para ShaderMaterial.
5. Adicione MultiMeshInstance3D com cópias.
6. Configure visibility range e material override.
7. Visualize collision/debug e monitore frame time.

## Relação com MapMagic

- Map generators não devem chamar API de renderer a cada operação matemática.
- `Height`/`Textures` são outputs, isto é, a fronteira entre dados e engine visual.
- Direct Matrices/Direct Textures permitem consumidores customizados.
- Draft pode usar mesh/resolução/material mais baratos.
- objetos repetidos devem preferir instancing/pooling.
- debug draw deve ser separado do renderer final.

## Sequência confirmada para o Terrain do MapMagic

O `MapMagicObject.DefaultTerrainMaterial()` procura, nesta ordem declarada no código, o shader do material padrão do Render Pipeline, depois `HDRP/TerrainLit`, depois `Nature/Terrain/Standard` e por fim `Lightweight Render Pipeline/Terrain/Lit`. O resultado é um novo `Material` com o shader encontrado.

Esse fallback não declara que qualquer versão de URP/HDRP é compatível. Ele apenas mostra os nomes que o bundle tenta procurar. A compatibilidade efetiva depende de o shader existir na versão do Unity e de o material aceitar os dados de Terrain.

No lado do graph, `Height`, `Textures`, `Grass`, `Objects` e `Trees` são `OutputGenerator`s. O renderer não calcula Noise/Erosion; recebe dados aplicados pelos outputs. `DirectMatricesHolder` e `DirectTexturesHolder` são holders declarados para receber dados diretos.

## Componentes não declarados pelo núcleo

O código analisado não declara `MeshRenderer`, `SkinnedMeshRenderer`, `VFX Graph` ou um sistema de renderização customizado como requisito do `MapMagicObject`. Eles podem existir em prefabs, demos ou integrações, mas não devem ser chamados de dependência obrigatória sem verificar o graph/output específico.

## Interface de propriedades e Inspector

### Unity

| Tipo | Onde aparece | Interface/propriedades | Observação |
|---|---|---|---|
| `Renderer` | Inspector do `GameObject` | Material, iluminação, sombras, layers, probes e opções comuns do renderer | A lista exata varia por pipeline e versão |
| `MeshFilter` | Inspector | Referência para `Mesh` | A malha é um asset separado; o campo não edita vértices diretamente |
| `MeshRenderer` | Inspector | Materiais e opções de renderização | Não cria uma malha; trabalha com `MeshFilter` ou outra fonte geométrica |
| `SkinnedMeshRenderer` | Inspector | Mesh esquelética, bones, root bone, materials e bounds | A animação depende de outros objetos/recursos |
| `SpriteRenderer` | Inspector | Sprite, cor, flip, sorting e material | É interface de sprite, não de Terrain |
| `LineRenderer`/`TrailRenderer` | Inspector | Pontos/largura/material e propriedades de trilha | A forma pode ser alterada por script/runtime |
| `ParticleSystemRenderer` | Inspector | Mesh/material/render mode associados ao ParticleSystem | O sistema de partículas tem seu próprio componente e módulos |
| `TilemapRenderer` | Inspector | Material, sorting e modo de renderização | O conteúdo vem do `Tilemap` |
| `Mesh` | Inspector do asset | Dados do recurso, quando suportado pelo editor | Não é componente anexado à cena |
| `Material`/`Shader` | Inspector do asset | Shader, parâmetros expostos, texturas e estados definidos pelo shader | Nem todo uniforme interno é exposto como campo editável |
| `RenderTexture` | Inspector do asset | Dimensão, formato e flags do recurso | É recurso de renderização, não renderer de cena |
| `ShaderVariantCollection` | Inspector do asset | Dados da coleção | Não é uma interface de material por si só |
| `LODGroup` | Inspector do `GameObject` | Níveis LOD, thresholds e cross-fade | O renderer precisa existir nos níveis configurados |
| `OcclusionArea`/`OcclusionPortal` | Inspector | Volume/estado relacionado ao occlusion culling | O resultado depende do bake/configuração do Unity |
| `ReflectionProbe` | Inspector | Volume, resolução, refresh e clipping | Não é requisito declarado do MapMagic core |
| `LightProbeGroup`/`LightProbeProxyVolume` | Inspector | Posições/configurações de probes | São recursos auxiliares do Unity |

### Godot

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `VisualInstance3D`/`GeometryInstance3D` | Inspector do nó | Layers, visibilidade, materiais e propriedades herdadas |
| `MeshInstance3D` | Inspector | Recurso `mesh`, materiais por superfície e propriedades visuais |
| `MultiMeshInstance3D` | Inspector | Recurso `MultiMesh`, material e visibilidade |
| `Sprite3D`/`AnimatedSprite3D` | Inspector | Textura/sprite frames, animação, billboard e propriedades visuais |
| `CSGShape3D` | Inspector | Forma, material, operação booleana e propriedades do nó |
| `Label3D` | Inspector | Texto, fonte, tamanho, billboard e modulação |
| `Decal` | Inspector | Texturas, extents, cull mask e parâmetros do decal |
| `Mesh`/`ArrayMesh`/`ImmediateMesh` | Inspector do recurso | Superfícies, materiais e dados que o recurso expõe |
| `StandardMaterial3D` | Inspector do recurso | Albedo, metallic, roughness, normal, emissão, transparência e estados suportados |
| `ShaderMaterial`/`Shader` | Inspector do recurso | Shader atribuído e uniforms expostos pelo shader |

### O que o MapMagic realmente abre

O `MapMagicObject` v2.1.11 usa um `CustomEditor` próprio. O código analisado não declara `MeshRenderer`, `SkinnedMeshRenderer`, `VFX Graph` ou um Inspector de renderer próprio como dependência obrigatória do gerador. A interface de Terrain é a do Unity, enquanto a interface de geração é a do `MapMagicObject`/Graph Window descrita em `11-editor-ferramentas.md`.

## Fontes

- [Unity Renderer](https://docs.unity3d.com/ScriptReference/Renderer.html)
- [Unity MeshRenderer](https://docs.unity3d.com/Manual/class-MeshRenderer.html)
- [Unity LODGroup](https://docs.unity3d.com/Manual/class-LODGroup.html)
- [Godot GeometryInstance3D](https://docs.godotengine.org/en/stable/classes/class_geometryinstance3d.html)
- [Godot MultiMeshInstance3D](https://docs.godotengine.org/en/latest/tutorials/3d/using_multi_mesh_instance.html)
