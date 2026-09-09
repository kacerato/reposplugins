# 10 — recursos, assets, importação e serialização

## Unity

### Asset

Arquivo dentro de `Assets` reconhecido pelo Unity: texture, mesh, material, audio, animation, scene, prefab, ScriptableObject, TerrainData ou custom importer.

### ScriptableObject

Asset de dados compartilhável. Use para `Graph`, `Imported Map`, `Brush Preset`, biome, input map, material config e vehicle config.

### Scene

Arquivo serializado de hierarquia GameObjects, componentes e referências.

### Prefab

Hierarquia reutilizável com overrides/aplicar/reverter.

### Texture2D/Texture3D/Texture2DArray/Cubemap

Texturas 2D, volumétricas, arrays e cubemaps. Import settings determinam formato, mipmaps, compressão, sRGB e Read/Write.

### Mesh

Dados de geometria. Pode ser asset importado ou gerado.

### Material/Shader/ComputeShader

Material configura shader; shader roda vertex/fragment; ComputeShader processa buffers/texturas em compute.

### AudioClip/AnimationClip/Avatar

Assets de áudio, curvas de animação e rig/retarget.

### TerrainData/TerrainLayer

Dados de terreno e suas camadas visuais.

### AssetDatabase

Editor-only: encontra, cria, move, importa, salva e atualiza assets.

### SerializedObject/SerializedProperty

API editor-safe para desenhar e alterar propriedades mantendo serialização/undo.

### AssetPostprocessor/ScriptedImporter

Interceptam importação e transformam arquivos em assets Unity.

## Godot

### Resource

Base de assets serializáveis. Pode ser salvo `.tres`/`.res` e referenciado em vários Nodes.

### PackedScene

Cena/prefab instanciável.

### Texture2D/Image/ImageTexture

`Image` é dado de pixels editável no CPU; `ImageTexture` envia para GPU; Texture2D é referência de textura.

### ArrayMesh/PrimitiveMesh/ImmediateMesh

Meshes geradas por arrays, primitivas ou desenho imediato.

### StandardMaterial3D/ShaderMaterial/Shader

Materiais PBR ou custom shader.

### AudioStream

Asset de áudio; pode ser gerador procedural.

### Animation/AnimationLibrary

Clips e bibliotecas usados pelo AnimationPlayer.

### NavigationMesh

Resource de navegação bakeável.

### Environment/Sky/Curve/Gradient

Recursos de ambiente, céu, curvas e gradientes.

### ImportFile/EditorImportPlugin

Importação de assets nativos e customizada.

### ConfigFile/JSON/FileAccess

Configuração e save/load. `FileAccess` trabalha com paths `res://` e `user://`, respeitando sandbox do projeto.

## MapMagic como pipeline de assets

```text
Fonte RAW/PNG/FBX
   ↓ importação
Asset/Resource intermediário
   ↓ graph/generator
Matrix/objects/spline
   ↓ output
TerrainData/Terrain ou Mesh/ArrayMesh
   ↓ runtime
cache/streaming/save
```

## Regras de serialização

- salvar versão do formato;
- persistir IDs estáveis, não índices temporários;
- separar source, imported e cache;
- guardar dependencies;
- validar referências quebradas;
- migrar versões antigas;
- não salvar estado gigante em cada frame;
- separar editor metadata de runtime data.

## Import de heightmap

### Unity

`Imported Map` do MapMagic guarda dados importados. RAW precisa ser quadrado, grayscale, 16-bit e PC byte order. Texture precisa ficar em Assets e ter Read/Write habilitado.

### Godot

Carregar Image/EXR/RAW por ResourceLoader/FileAccess, converter para formato de float adequado e validar largura, altura, canal e range. Depois gerar ArrayMesh e HeightMapShape3D.

## Cache e streaming

Cache pode ser apagado e reconstruído. Um tile gerado deve ter:

- coordenada;
- seed/graph hash;
- resolução;
- margins;
- versão do generator;
- assets dependentes;
- estado Main/Draft;
- status `Unloaded`, `Loading`, `Ready`, `Failed`, `Unloading`.

## Tutorial de validação

### Unity

1. Crie ScriptableObject `TerrainConfig`.
2. Salve asset.
3. Crie Graph e Imported Map.
4. Altere uma propriedade pelo Inspector.
5. Feche/reabra Unity.
6. Confirme referência e versão.

### Godot

1. Crie `TerrainConfig : Resource`.
2. Exponha campos com `@export`.
3. Salve `.tres`.
4. Referencie em MapMagicWorld.
5. Feche/reabra projeto.
6. Teste migration ao aumentar versão.

## Interface de propriedades e Inspector

### Unity

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `ScriptableObject`/asset | Inspector do asset no `Project` | Campos serializáveis, referências e dados do recurso |
| `Scene` | Hierarchy/Inspector e arquivo no `Project` | Objetos da cena, referências e settings dos objetos |
| `Prefab` | Inspector do asset/instância | Dados do prefab, overrides e referências |
| `Texture2D`/`Texture3D`/`Texture2DArray`/`Cubemap` | Inspector do asset | Import settings, formato, compressão, mipmaps e uso conforme o tipo |
| `Mesh` | Inspector do asset | Dados/import settings suportados pelo Unity |
| `Material`/`Shader`/`ComputeShader` | Inspector do asset | Shader, propriedades expostas e referências |
| `AudioClip`/`AnimationClip`/`Avatar` | Inspector do asset | Import settings e dados específicos do recurso |
| `TerrainData`/`TerrainLayer` | Inspector do asset | Heightmap, layers, protótipos e dados de terreno |
| `SerializedObject`/`SerializedProperty` | Não são painéis de usuário | APIs que alimentam Inspectors customizados e Undo |
| `AssetDatabase`/importers | Não são componentes visuais | Serviços e callbacks de editor para importar/localizar assets |

### Godot

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `Resource` | Inspector do asset ou campo do nó | Propriedades serializadas/exportadas |
| `PackedScene` | `FileSystem` e instância na `Scene Tree` | Asset da cena, nó raiz e overrides quando aplicáveis |
| Texturas/imagens | Inspector do recurso | Importação e propriedades do recurso |
| `ArrayMesh`/`PrimitiveMesh`/`ImmediateMesh` | Inspector do recurso | Superfícies, material e dados geométricos |
| `StandardMaterial3D`/`ShaderMaterial`/`Shader` | Inspector do recurso | Parâmetros e uniforms expostos |
| `AudioStream`/`Animation`/`AnimationLibrary` | Inspector do recurso | Dados do stream/animação e bibliotecas |
| `NavigationMesh`/`Environment`/`Sky`/`Curve`/`Gradient` | Inspector do recurso | Parâmetros específicos do recurso |
| `FileAccess`/`ConfigFile`/JSON | Não são painéis de cena | APIs de leitura/escrita e dados carregados por código |
| import plugins | Não são recursos editáveis comuns | Ferramentas de editor que transformam arquivos em recursos |

### Sequência de asset até Inspector

1. O arquivo é importado pelo pipeline do engine.
2. O resultado aparece no `Project`/`FileSystem`.
3. Selecionar o asset mostra sua interface própria.
4. Referenciar o asset em um componente mostra um campo de referência; os dados continuam pertencendo ao recurso.
5. Alterar o asset pode afetar todas as instâncias que o referenciam; alterar uma instância/prefab pode criar override.

No MapMagic, `Graph`, `Preset`, `MatrixAsset` e `Imported Map` têm interfaces específicas descritas no documento de editor. O Inspector do asset não deve ser confundido com o Graph Window nem com um savegame do jogador.

## Fontes

- [Unity ScriptableObject](https://docs.unity3d.com/Manual/class-ScriptableObject.html)
- [Unity AssetDatabase](https://docs.unity3d.com/ScriptReference/AssetDatabase.html)
- [Unity TextureImporter.isReadable](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/TextureImporter-isReadable.html)
- [Godot Resource](https://docs.godotengine.org/en/stable/classes/class_resource.html)
- [Godot PackedScene](https://docs.godotengine.org/en/stable/classes/class_packedscene.html)
- [Godot FileAccess](https://docs.godotengine.org/en/stable/classes/class_fileaccess.html)

## Sequência confirmada para Graph e Imported Map

O bundle declara `Graph : ScriptableObject, ISerializationCallbackReceiver` e um `CreateAssetMenu` para `MapMagic/Empty Graph`. O fluxo documentado é:

1. criar Graph pelo menu `Assets > Create > MapMagic`;
2. arrastar Graph para cena ou atribuí-lo no `MapMagicObject`;
3. abrir o asset no Graph Editor;
4. serializar generators/links/layers;
5. preparar dados por tile;
6. gerar e finalizar;
7. aplicar outputs no Terrain;
8. limpar/regerar quando Graph ou propriedades mudarem.

`Imported Map` é um asset intermediário. O nó Import mantém referência ao asset, em vez de colocar todo RAW/Texture diretamente no generator. A documentação declara que Texture mode exige fonte dentro de Assets com Read/Write habilitado; RAW mode exige o formato indicado no manual.

## O que não está declarado

O bundle não declara Addressables, Resources.Load obrigatório, banco externo, cloud save ou sistema de savegame de jogador. `TerrainData` salvo, cache/intermediate data e Graph asset não devem ser confundidos com savegame do gameplay.
