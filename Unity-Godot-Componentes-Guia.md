# Unity e Godot — catálogo de componentes/nodes para a engine e para o MapMagic

> A versão modular, separada por categoria, está em [docs/unity-godot-componentes/README.md](docs/unity-godot-componentes/README.md).

## Como ler este catálogo

Este documento lista os componentes e nodes que realmente formam uma engine de jogo 2D/3D completa e que são relevantes para portar/recriar o fluxo do MapMagic.

Uma lista literal de toda a API interna de Unity e de todas as classes da árvore de classes do Godot mudaria conforme a versão e teria milhares de tipos. Aqui estão todos os componentes de cena e recursos de alto nível usados normalmente em terreno procedural, editor, jogo, física, renderização, input, áudio, partículas, animação, UI e navegação.

### Diferença fundamental

| Unity | Godot |
|---|---|
| `GameObject` é o contêiner | `Node` é a unidade da Scene Tree |
| Todo GameObject tem exatamente um `Transform` | Um `Node3D` possui transformação; `Node2D` possui transformação 2D |
| Componentes são adicionados ao GameObject | Nodes são adicionados como pai/filho; scripts também podem ser anexados |
| `MonoBehaviour` é script/componente | `Node` + script GDScript/C# cumpre papel equivalente |
| `ScriptableObject` é asset de dados | `Resource` é asset de dados |
| Prefab é asset/instância de GameObject | PackedScene é a cena reutilizável/instanciável |
| Inspector edita componentes | Inspector edita propriedades de Nodes e Resources |
| Terrain é componente nativo | Godot 4 não possui um Terrain renderer equivalente nativo; é preciso usar mesh customizada, GridMap ou plugin |

Unity documenta que cada GameObject tem um único Transform, enquanto Godot organiza seus objetos como Nodes em uma árvore. Essa diferença é mais importante que os nomes individuais.

## 1. Fundação da cena

### Unity

| Componente/tipo | Explicação |
|---|---|
| `GameObject` | Contêiner de identidade, layer, tag, active state e componentes. Não renderiza nem simula sozinho. |
| `Transform` | Posição, rotação, escala, parent/children e conversões local/world. É obrigatório em todo GameObject. |
| `RectTransform` | Variante usada em UI; acrescenta anchors, pivots, offsets, size delta e posição ancorada. |
| `MonoBehaviour` | Base para scripts anexáveis, lifecycle (`Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, `LateUpdate`, `OnDisable`, `OnDestroy`) e callbacks. |
| `ScriptableObject` | Asset de dados compartilhável, ideal para configurações, graphs, materiais lógicos, presets e tabelas. Não é componente de GameObject. |
| `Component` | Base de componentes anexados ao GameObject; permite `GetComponent`, `TryGetComponent`, `AddComponent` e acesso pelo Inspector. |
| `Behaviour` | Base para componentes ativáveis/desativáveis com `enabled`. |
| `Tag` | Identificador semântico único por objeto, usado por busca e filtros. |
| `Layer` | Bitmask para câmera, raycast, colisão, rendering e seleção. |
| `Prefab` | Asset/instância reutilizável de GameObject e hierarquia; usado para objetos espalhados, árvores, NPCs e props. |
| `Scene` | Arquivo de ambiente/hierarquia; contém GameObjects e referências a assets. |

### Godot

| Node/tipo | Explicação |
|---|---|
| `Object` | Raiz da hierarquia de classes; fornece signals, metaprogramação e lifecycle básico. |
| `RefCounted` | Objeto gerenciado por contagem de referências; usado em classes que não precisam entrar na Scene Tree. |
| `Resource` | Asset de dados serializável e reutilizável: mesh, material, texture, animation, navigation mesh, configuration. |
| `Node` | Unidade básica da Scene Tree; possui nome, pai, filhos, groups, signals, process callbacks e script. |
| `Node2D` | Node com transformação 2D. |
| `Node3D` | Node com transformação 3D; equivalente aproximado ao Transform + contêiner de Unity. |
| `CanvasItem` | Base visual de Nodes 2D e Control; visibilidade, modulate, z-index, draw callbacks e material. |
| `Control` | Base da UI; layout, anchors, mouse/touch, foco, clipping e tamanho mínimo. |
| `CanvasLayer` | Camada independente da câmera para HUD, menus e overlays. |
| `PackedScene` | Cena reutilizável equivalente a prefab; pode ser instanciada muitas vezes. |
| `NodePath` | Referência serializável a outro Node na árvore. |
| `Groups` | Tags flexíveis: um Node pode pertencer a vários grupos. |

## 2. Terreno procedural e mundo

### Unity

| Componente/recurso | Explicação |
|---|---|
| `Terrain` | Renderiza um terreno baseado em heightmap, texturas, árvores, detalhes, LOD e configurações de material. É o alvo padrão do MapMagic. |
| `TerrainData` | Asset/dados do Terrain: heightmap, alphamaps, TerrainLayers, detalhes, árvores, resolução, tamanho e holes. |
| `TerrainCollider` | Collider baseado no heightmap do TerrainData; usado para chão físico e raycast. |
| `TerrainLayer` | Camada de textura do terreno: diffuse, normal, mask, metallic/specular, tile size e offset. Necessária para Textures Output padrão. |
| `TreePrototype` | Registro de prefab/árvore que pode ser usado pelo sistema de árvores do Terrain. |
| `DetailPrototype` | Registro de grass ou mesh de detalhe para o sistema de detalhes do Terrain. |
| `MapMagicObject` | Componente do pacote; coordena graph, tiles, drafts, markers, ranges, geração e aplicação. |
| `MapMagicBrush` | Componente opcional do bundle; aplica um Brush graph em um ou vários Terrains. |
| `DirectMatricesHolder` | Holder do bundle para receber matrizes diretamente. |
| `DirectTexturesHolder` | Holder do bundle para receber texturas diretamente. |
| `WindZone` | Fornece vento para árvores, grass e alguns shaders. |
| `OcclusionArea` | Volume de bake/occlusion culling. |
| `OcclusionPortal` | Abre/fecha regiões para occlusion culling. |
| `LODGroup` | Troca renderers por níveis de detalhe conforme distância. |
| `ReflectionProbe` | Captura ambiente para reflexos locais. |
| `LightProbeGroup` | Rede de probes para iluminação indireta de objetos dinâmicos. |
| `LightProbeProxyVolume` | Melhora iluminação de objetos grandes usando vários probes. |

### Godot

| Node/recurso | Explicação |
|---|---|
| `MeshInstance3D` | Renderiza uma `Mesh` com material. É a base para uma implementação de terreno mesh-based. |
| `ArrayMesh` | Mesh criada por arrays de vértices, normais, UVs, índices e atributos; adequada para gerar chunks de terreno. |
| `PrimitiveMesh` | Meshes simples como PlaneMesh, BoxMesh, SphereMesh e CylinderMesh. |
| `HeightMapShape3D` | Shape de colisão baseada em heightmap; serve para terreno físico, mas não renderiza o terreno. Não suporta overhangs/cavernas verdadeiros. |
| `GridMap` | Sistema de células 3D baseado em MeshLibrary; útil para mundo modular, mas não substitui automaticamente um Terrain procedural contínuo. |
| `MultiMeshInstance3D` | Instancing eficiente de milhares de cópias de uma mesh; indicado para floresta, grass e props repetidos. |
| `GeometryInstance3D` | Base visual com visibilidade, material override, shadow, LOD/visibility ranges e culling. |
| `MapMagicTerrain` | Não existe nativamente; em Godot seria um Node3D customizado que gera `ArrayMesh`, `HeightMapShape3D`, materiais, chunks e instâncias. |
| `NavigationRegion3D` | Pode receber uma NavigationMesh sobre a geometria do terreno. |
| `WorldEnvironment` | Ambiente, céu, iluminação ambiente e pós-processamento da cena; não gera terreno. |

Godot documenta `HeightMapShape3D` como shape de colisão, não como renderer de terreno. Portanto, o equivalente ao `Terrain` do Unity precisa ser uma camada própria.

## 3. Renderização e geometria

### Unity

| Componente | Explicação |
|---|---|
| `Renderer` | Base para componentes que desenham geometria e possuem materials, bounds, shadows, light probes e rendering layer. |
| `MeshFilter` | Fornece a `Mesh` para um `MeshRenderer`. |
| `MeshRenderer` | Renderiza uma mesh estática. |
| `SkinnedMeshRenderer` | Renderiza mesh deformada por bones, blend shapes e skinning; usado em personagens. |
| `SpriteRenderer` | Renderiza uma textura/sprite 2D. |
| `LineRenderer` | Renderiza linha entre pontos; útil para debug, estradas simples e trajetórias. |
| `TrailRenderer` | Deixa rastro seguindo o movimento. |
| `ParticleSystemRenderer` | Renderiza partículas de um ParticleSystem. |
| `TilemapRenderer` | Renderiza Tilemap 2D. |
| `BillboardRenderer` | Renderiza billboard baseado em BillboardAsset. |
| `CanvasRenderer` | Renderiza elementos gráficos de UI dentro de Canvas. |
| `VFX`/`VisualEffect` | Componente do Visual Effect Graph, pacote/sistema separado do ParticleSystem clássico. |
| `Material` | Instância/configuração de shader e propriedades de superfície. |
| `Shader` | Programa de GPU que define transformação, iluminação e aparência. |
| `ShaderVariantCollection` | Pré-aquece/conserva variantes de shader para reduzir hitch em build. |
| `RenderTexture` | Superfície de renderização usada para câmera, preview, mapas e pós-processamento. |
| `Camera` | Renderiza a cena a partir de posição, projeção, FOV, clipping, culling mask e target texture. |
| `Light` | Luz Directional, Point, Spot ou Area conforme pipeline/configuração. |
| `ReflectionProbe` | Captura cubemap de ambiente para reflexos. |
| `LightProbeGroup` | Pontos de iluminação indireta para objetos móveis. |
| `LightProbeProxyVolume` | Volume de amostragem de probes para meshes grandes. |
| `ParticleSystemForceField` | Campo de força para ParticleSystem. |

### Godot

| Node/recurso | Explicação |
|---|---|
| `VisualInstance3D` | Base de elementos visuais 3D, com layers de visibilidade e bounds. |
| `GeometryInstance3D` | Adiciona material override, shadows, transparência, LOD bias e visibility ranges. |
| `MeshInstance3D` | Desenha uma `Mesh` com materiais. |
| `MultiMeshInstance3D` | Desenha muitas instâncias por GPU usando `MultiMesh`. |
| `CSGShape3D` | Geometria construtiva simples: Box, Sphere, Cylinder, Torus, Polygon e Combiner. Boa para protótipo, não para grandes mundos. |
| `Sprite3D` | Mostra textura 2D no espaço 3D. |
| `AnimatedSprite3D` | Sprite3D com frames/animações. |
| `Label3D` | Texto no mundo 3D. |
| `Decal` | Projeta textura sobre geometria. |
| `GPUParticles3D` | Partículas calculadas na GPU; usa ParticleProcessMaterial ou ShaderMaterial. |
| `CPUParticles3D` | Partículas calculadas na CPU; fallback quando GPU particles não é adequado. |
| `Camera3D` | Câmera perspectiva/ortogonal/frustum; define viewport, projection e culling. |
| `CameraAttributes` | Exposição, DOF e atributos de câmera. |
| `WorldEnvironment` | Ambiente padrão da cena: background, sky, ambient light, tonemapping, SSAO/DOF e efeitos. |
| `Environment` | Resource que contém as configurações ambientais usadas por WorldEnvironment/Camera. |
| `DirectionalLight3D` | Luz direcional global, equivalente aproximado à Directional Light Unity. |
| `OmniLight3D` | Luz pontual omnidirecional. |
| `SpotLight3D` | Luz cônica. |
| `ReflectionProbe` | Captura reflexos locais. |
| `LightmapGI` | Iluminação global baked. |
| `VoxelGI` | GI voxelizada dinâmica/semidinâmica em cenários compatíveis. |
| `LightmapProbe` | Probes de iluminação para objetos dinâmicos. |
| `FogVolume` | Volume de neblina/fog localizado. |
| `GPUParticlesCollision3D` | Formas para colisão de partículas GPU: box, sphere, heightfield etc. |

## 4. Física e colisão 3D

### Unity

| Componente | Explicação |
|---|---|
| `Collider` | Base das formas de colisão 3D. |
| `BoxCollider` | Caixa; barato e adequado para volumes simples. |
| `SphereCollider` | Esfera; barata para objetos redondos. |
| `CapsuleCollider` | Cápsula; boa para personagens. |
| `MeshCollider` | Colisão baseada em mesh; cara, com restrições quando convexa/não convexa. |
| `TerrainCollider` | Colisão de TerrainData. |
| `WheelCollider` | Modelo especializado para roda/suspensão de veículo. |
| `CharacterController` | Movimento de personagem com cápsula e colisão sem Rigidbody tradicional. |
| `Rigidbody` | Corpo rígido 3D simulado por física; forças, massa, gravidade, torque e colisões. |
| `ArticulationBody` | Corpo para articulações robóticas/veículos complexos. |
| `FixedJoint` | Mantém dois rigidbodies unidos. |
| `HingeJoint` | Restrição de dobradiça. |
| `SpringJoint` | Restrição elástica. |
| `ConfigurableJoint` | Junta configurável em eixos, limites, drives e forças. |
| `CharacterJoint` | Junta semelhante a ragdoll. |
| `ConstantForce` | Força/torque constante em Rigidbody. |
| `PhysicsMaterial` | Friction, bounciness e combinação das superfícies. |
| `Physics.IgnoreCollision` | Ignora colisão entre dois colliders específicos. |
| `Layer Collision Matrix` | Configuração global que define quais layers colidem. |

### Unity 2D

`Collider2D`, `BoxCollider2D`, `CircleCollider2D`, `CapsuleCollider2D`, `PolygonCollider2D`, `CompositeCollider2D`, `TilemapCollider2D`, `Rigidbody2D`, `CharacterController` equivalente via script, `HingeJoint2D`, `SpringJoint2D`, `DistanceJoint2D`, `FixedJoint2D`, `SliderJoint2D`, `WheelJoint2D`, `Effector2D`, `PlatformEffector2D`, `AreaEffector2D`, `PointEffector2D`, `SurfaceEffector2D` e `BuoyancyEffector2D` formam o conjunto 2D.

### Godot

| Node/recurso | Explicação |
|---|---|
| `CollisionObject3D` | Base para objetos que participam da colisão e layers/masks. |
| `PhysicsBody3D` | Base dos corpos físicos 3D. |
| `StaticBody3D` | Corpo imóvel; chão, paredes e estruturas estáticas. |
| `AnimatableBody3D` | Corpo movido manualmente/por animação que ainda afeta outros corpos. |
| `CharacterBody3D` | Corpo controlado por script para player/NPC; `move_and_slide` e colisão integrada. |
| `RigidBody3D` | Corpo simulado por forças, impulsos, massa e torque; não deve ser movido diretamente todo frame. |
| `Area3D` | Região que detecta entrada/saída de corpos e áreas. |
| `CollisionShape3D` | Atribui uma `Shape3D` a Area3D/PhysicsBody3D. |
| `CollisionPolygon3D` | Colisão baseada em polígonos/convexidade conforme uso. |
| `BoxShape3D` | Shape caixa. |
| `SphereShape3D` | Shape esfera. |
| `CapsuleShape3D` | Shape cápsula. |
| `CylinderShape3D` | Shape cilíndrica. |
| `ConvexPolygonShape3D` | Shape convexa derivada de pontos. |
| `ConcavePolygonShape3D` | Shape trimesh; adequada para estáticos, mais cara. |
| `HeightMapShape3D` | Colisão de terreno por heightmap; não representa cavernas/overhangs. |
| `Joint3D` | Base para joints. |
| `HingeJoint3D` | Dobradiça. |
| `PinJoint3D` | Junta de pino. |
| `SliderJoint3D` | Movimento em eixo. |
| `ConeTwistJoint3D` | Junta para cone/twist, útil em ragdoll. |
| `Generic6DOFJoint3D` | Restrições configuráveis nos seis graus de liberdade. |
| `PhysicsMaterial` | Friction, bounce e combinação física. |
| `PhysicsRayQueryParameters3D` | Configuração de raycast por script. |
| `PhysicsDirectSpaceState3D` | Consultas diretas ao espaço físico. |

## 5. Física 2D no Godot

`StaticBody2D`, `AnimatableBody2D`, `CharacterBody2D`, `RigidBody2D`, `Area2D`, `CollisionShape2D`, `CollisionPolygon2D`, `RectangleShape2D`, `CircleShape2D`, `CapsuleShape2D`, `ConvexPolygonShape2D`, `SegmentShape2D`, `WorldBoundaryShape2D`, `RayCast2D`, `ShapeCast2D`, `PointCast2D`, `PinJoint2D`, `GrooveJoint2D`, `DampedSpringJoint2D` e `PhysicalBone2D` são os principais nodes 2D de física.

## 6. Navegação e pathfinding

### Unity

| Componente/tipo | Explicação |
|---|---|
| `NavMeshSurface` | Componente do pacote AI Navigation; coleta geometria e gera NavMesh. Não é um componente mínimo do UnityEngine antigo. |
| `NavMeshAgent` | Agente que consulta NavMesh, calcula caminho, evita obstáculos e move personagem. |
| `NavMeshObstacle` | Obstáculo que afeta navegação; pode usar carving. |
| `OffMeshLink` | Ligação especial entre regiões: salto, porta, elevador, ponte. |
| `NavMeshModifier` | Inclui/exclui ou altera área de geometria coletada. Pacote AI Navigation. |
| `NavMeshModifierVolume` | Volume que marca áreas de navegação. Pacote AI Navigation. |
| `NavMeshLink` | Link moderno do pacote AI Navigation. |
| `NavMeshData` | Asset/dados bakeados da NavMesh. |
| `NavMesh` API | Consultas por posição, raycast, sample position e path. |

### Godot

| Node/recurso | Explicação |
|---|---|
| `NavigationRegion3D` | Região caminhável que usa `NavigationMesh`; agentes podem navegar sobre ela. |
| `NavigationMesh` | Resource com geometria/bake e parâmetros de navegação. |
| `NavigationAgent3D` | Helper para pathfinding e avoidance; não move o personagem sozinho. |
| `NavigationObstacle3D` | Afeta avoidance de agentes; não altera automaticamente o pathfinding da mesh. |
| `NavigationLink3D` | Liga dois pontos/regiões com custo e bidirecionalidade. |
| `NavigationServer3D` | Server baixo nível da navegação; gerencia mapas, regiões, links e consultas. |
| `NavigationRegion2D` | Região caminhável 2D. |
| `NavigationPolygon` | Resource de polígonos navegáveis 2D. |
| `NavigationAgent2D` | Helper 2D de pathfinding/avoidance. |
| `NavigationObstacle2D` | Obstacle 2D de avoidance. |
| `NavigationLink2D` | Link especial 2D. |

Para o MapMagic, `Spline Pathfinding` não depende desses componentes: ele calcula em matrizes/graphs durante geração. Para navegação de NPC em runtime, os componentes acima entram depois que o terreno/mesh foi aplicado.

## 7. Câmera, ambiente e iluminação

### Unity

`Camera` controla projeção, FOV, clipping planes, culling mask, depth, viewport, HDR/MSAA e target texture. `Light` fornece Directional, Point, Spot e Area conforme pipeline. `ReflectionProbe`, `LightProbeGroup`, `LightProbeProxyVolume`, `Skybox` e `RenderSettings` controlam ambiente/reflexos/iluminação. `Volume` e `VolumeProfile` são usados por URP/HDRP para pós-processamento; são pipeline-dependent.

### Godot

`Camera3D`, `WorldEnvironment`, `Environment`, `CameraAttributes`, `DirectionalLight3D`, `OmniLight3D`, `SpotLight3D`, `ReflectionProbe`, `LightmapGI`, `VoxelGI`, `LightmapProbe`, `FogVolume`, `Sky`, `ProceduralSkyMaterial`, `PhysicalSkyMaterial` e `Compositor` formam a pilha de câmera/ambiente. `WorldEnvironment` só deve existir uma vez por cena para o ambiente padrão.

## 8. Animação

### Unity

| Componente | Explicação |
|---|---|
| `Animator` | Executa Animator Controller, estados, transições, layers e parâmetros. |
| `RuntimeAnimatorController` | Controller serializado com state machines. |
| `AnimatorOverrideController` | Substitui clips de um controller sem duplicar a lógica. |
| `Avatar` | Mapeia bones de personagem humanoide/genérico. |
| `Animation` | Sistema legado de clips simples. |
| `AnimationClip` | Dados de curvas/eventos de animação. |
| `PlayableDirector` | Executa Timeline/Playable Graph. |
| `TimelineAsset` | Sequência de clips, sinais, animações, áudio e câmeras. |
| `SkinnedMeshRenderer` | Deforma visualmente mesh pelo Avatar/bones. |
| `Cloth` | Simulação de tecido; custo e suporte variam. |
| `SpringJoint`/joints | Movimento físico, não animação keyframed. |

### Godot

| Node/recurso | Explicação |
|---|---|
| `AnimationPlayer` | Reproduz `AnimationLibrary` com tracks de propriedades, métodos, áudio e transform. |
| `AnimationTree` | Mistura animações e executa state machine/blend tree. |
| `AnimationNodeStateMachine` | Estados/transições dentro do AnimationTree. |
| `Skeleton3D` | Hierarquia de bones para personagens. |
| `BoneAttachment3D` | Anexa Node a um bone. |
| `PhysicalBone3D` | Bone integrado à física/ragdoll. |
| `LookAtModifier3D` | Faz bone mirar alvo. |
| `SkeletonIK3D` | IK quando disponível na versão; ajustar conforme o Godot-alvo. |
| `Tween` | Anima propriedades por interpolação sem clip persistente. |
| `AnimatedSprite2D/3D` | Troca frames de sprite. |

## 9. Áudio

### Unity

| Componente/recurso | Explicação |
|---|---|
| `AudioSource` | Reproduz `AudioClip`, com volume, pitch, loop, spatial blend, attenuation, priority e mixer group. |
| `AudioListener` | Ponto que escuta áudio; geralmente na câmera principal. |
| `AudioClip` | Asset de áudio. |
| `AudioMixer` | Roteia e processa áudio em grupos. |
| `AudioMixerGroup` | Canal/grupo dentro do mixer. |
| `AudioMixerSnapshot` | Estado salvável de volumes/efeitos para transições. |
| `AudioReverbZone` | Região de reverb espacial. |
| `Microphone` API | Captura microfone, não é componente de cena. |

### Godot

| Node/recurso | Explicação |
|---|---|
| `AudioStreamPlayer` | Áudio não-posicional, UI/música/efeitos globais. |
| `AudioStreamPlayer2D` | Áudio posicional em 2D. |
| `AudioStreamPlayer3D` | Áudio posicional 3D com atenuação, direção, Doppler e low-pass. |
| `AudioListener3D` | Listener 3D selecionável; por padrão a câmera pode ser o ponto de audição. |
| `AudioStream` | Resource base de áudio; WAV/OGG/MP3 conforme import/suporte. |
| `AudioBus` | Canal de mixagem configurado no projeto. |
| `AudioEffect` | Reverb, chorus, compressor, delay, EQ e outros efeitos de bus. |
| `AudioStreamGenerator` | Geração de áudio via script. |

## 10. Partículas e efeitos

### Unity

`ParticleSystem` é o componente principal; `ParticleSystemRenderer` escolhe billboard, mesh, trail e material. Módulos incluem Main, Emission, Shape, Velocity over Lifetime, Limit Velocity, Inherit Velocity, Force over Lifetime, Color over Lifetime, Color by Speed, Size over Lifetime, Rotation, External Forces, Noise, Collision, Triggers, Texture Sheet Animation, Lights, Trails e Sub Emitters. `ParticleSystemForceField` influencia partículas. Visual Effect Graph usa `VisualEffect` e assets de graph separados.

### Godot

`GPUParticles3D` emite partículas na GPU e aceita `ParticleProcessMaterial` ou `ShaderMaterial`. `CPUParticles3D` processa na CPU e é fallback. Equivalentes 2D são `GPUParticles2D` e `CPUParticles2D`. Recursos auxiliares: `ParticleProcessMaterial`, `Curve`, `Gradient`, `RibbonTrailMesh`, `TubeTrailMesh`, `GPUParticlesCollisionBox3D`, `Sphere3D`, `HeightField3D`, `VectorField3D` e `CollisionSDF3D` conforme renderer/versão.

## 11. UI e input visual

### Unity UI clássico

| Componente | Explicação |
|---|---|
| `Canvas` | Raiz de UI; Screen Space Overlay, Screen Space Camera ou World Space. |
| `CanvasScaler` | Escala UI por resolução/densidade. |
| `GraphicRaycaster` | Detecta clique/toque em Graphics do Canvas. |
| `CanvasGroup` | Alpha, interactable e bloqueio de raycast em subárvore. |
| `Image` | Renderiza Sprite/color, fill, sliced/tiled. |
| `RawImage` | Renderiza Texture/RenderTexture. |
| `Text` | Texto legado; para projetos novos normalmente usa-se TextMeshPro. |
| `Button` | Evento de clique e estado visual. |
| `Toggle` | Estado booleano e grupo de toggles. |
| `Slider` | Valor arrastável entre min/max. |
| `Scrollbar` | Barra de rolagem. |
| `ScrollRect` | Área rolável. |
| `Dropdown` | Seleção de item. |
| `InputField` | Entrada de texto legada. |
| `Selectable` | Base de controles navegáveis. |
| `LayoutGroup` | Base para layouts automáticos. |
| `HorizontalLayoutGroup` | Organiza filhos horizontalmente. |
| `VerticalLayoutGroup` | Organiza filhos verticalmente. |
| `GridLayoutGroup` | Organiza filhos em grade. |
| `ContentSizeFitter` | Ajusta tamanho ao conteúdo. |
| `LayoutElement` | Min/preferred/flexible size. |
| `AspectRatioFitter` | Mantém proporção. |
| `Mask`/`RectMask2D` | Recorta conteúdo filho. |
| `EventSystem` | Estado global de seleção, pointer, teclado e navegação. |
| `StandaloneInputModule` | Input clássico para EventSystem. |
| `InputSystemUIInputModule` | Integração com o novo Input System; pacote Input System. |
| `PhysicsRaycaster` | Raycast de UI/ponte para objetos 3D pelo EventSystem. |
| `Physics2DRaycaster` | Raycast de objetos 2D. |

TextMeshPro adiciona `TMP_Text`, `TextMeshProUGUI`, `TMP_InputField`, `TMP_Dropdown` e recursos de fonte; é pacote normalmente incluído, mas não é a mesma implementação do `Text` legado.

### Godot UI

| Node | Explicação |
|---|---|
| `Control` | Base de todos os controles, layout e interação. |
| `Container` | Base de nodes que posicionam filhos automaticamente. |
| `HBoxContainer` | Layout horizontal. |
| `VBoxContainer` | Layout vertical. |
| `GridContainer` | Layout em grade. |
| `MarginContainer` | Adiciona margens. |
| `CenterContainer` | Centraliza filho. |
| `AspectRatioContainer` | Mantém proporção. |
| `Panel` | Fundo/estilo de painel. |
| `PanelContainer` | Container que dimensiona pelo painel/filho. |
| `ColorRect` | Retângulo de cor. |
| `TextureRect` | Mostra texture com stretch mode. |
| `Label` | Texto simples. |
| `RichTextLabel` | Texto rico, BBCode, links e formatação. |
| `Button` | Botão clicável. |
| `TextureButton` | Botão baseado em texturas. |
| `CheckBox` | Checkbox. |
| `CheckButton` | Toggle em forma de botão. |
| `OptionButton` | Dropdown de opções. |
| `MenuButton` | Botão que abre PopupMenu. |
| `LinkButton` | Botão visual de link. |
| `LineEdit` | Entrada de uma linha. |
| `TextEdit` | Editor multilinha. |
| `SpinBox` | Campo numérico com setas. |
| `HSlider`/`VSlider` | Sliders horizontal/vertical. |
| `ProgressBar` | Barra de progresso. |
| `HScrollBar`/`VScrollBar` | Barras de rolagem. |
| `ScrollContainer` | Container rolável. |
| `TabContainer` | Páginas com abas. |
| `TabBar` | Barra de tabs. |
| `Tree` | Hierarquia/lista em árvore; equivalente útil para Scene Tree. |
| `ItemList` | Lista selecionável. |
| `Popup`, `PopupPanel`, `PopupMenu` | Menus, contextos e popups. |
| `Window` | Janela separada/embutida. |
| `SubViewportContainer` | Mostra SubViewport dentro da UI. |
| `FileDialog` | Seletor de arquivo/pasta. |
| `ColorPicker` | Escolha de cor. |
| `GraphEdit`/`GraphNode` | Base nativa para editor visual de grafos, muito relevante para MapMagic-like editor. |

## 12. Input e interação

### Unity

| API/componente | Explicação |
|---|---|
| `Input` clássico | API estática para teclado, mouse, joystick e touch; legado, mas ainda disponível em versões compatíveis. |
| `Input System` | Pacote moderno com actions, devices, bindings, control schemes e `PlayerInput`. Não é obrigatório para MapMagic. |
| `PlayerInput` | Componente do Input System que conecta actions a callbacks. |
| `Touch`/`Input.touches` | Identidade, posição, fase, pressão e finger id. |
| `EventSystem` | Entrada de UI e seleção. |
| `Raycast` | Consulta física/visual para seleção e interação. |
| `Camera.ScreenPointToRay` | Converte toque/tela em raio no mundo. |
| `Physics.Raycast` | Intersecta raio com colliders 3D. |
| `Physics2D.Raycast` | Intersecta raio com colliders 2D. |
| `Gizmos`/`Handles` | Ferramentas de visualização e edição no Editor, não runtime de gameplay. |

### Godot

| API/node | Explicação |
|---|---|
| `Input` singleton | Consulta ações, teclado, mouse, joypad e touch. |
| `InputMap` | Cadastro central de ações e eventos/bindings. |
| `InputEvent` | Base de eventos; `InputEventKey`, `MouseButton`, `MouseMotion`, `ScreenTouch`, `ScreenDrag`, `JoypadButton`, `JoypadMotion`. |
| `_input(event)` | Recebe eventos antes de `_unhandled_input`. |
| `_unhandled_input(event)` | Recebe eventos não consumidos, bom para gameplay. |
| `_gui_input(event)` | Recebe eventos dentro de Control. |
| `RayCast3D` | Raycast persistente configurável na Scene Tree. |
| `ShapeCast3D` | Sweep de forma para detectar volume. |
| `RayCast2D`/`ShapeCast2D` | Equivalentes 2D. |
| `Camera3D.project_ray_origin/normal` | Converte posição de tela em raio 3D. |
| `TouchScreenButton` | Controle visual para touchscreen. |
| `Virtual joystick` | Normalmente uma cena/script customizado; não é componente obrigatório nativo. |

## 13. Tempo, lifecycle e execução

### Unity

`Time`, `Update`, `FixedUpdate`, `LateUpdate`, `Coroutine`, `Invoke`, `AsyncOperation`, `Application` lifecycle, `OnApplicationPause`, `OnApplicationFocus`, `SceneManager` e `DontDestroyOnLoad` formam a infraestrutura temporal. Física deve usar `FixedUpdate`; UI/câmera normalmente usam `Update`/`LateUpdate`; geração pesada pode usar threads, jobs ou coroutines, mas alterações de UnityEngine objects devem respeitar a main thread.

### Godot

`_enter_tree`, `_ready`, `_process`, `_physics_process`, `_input`, `_unhandled_input`, `_exit_tree`, `Timer`, `SceneTreeTimer`, `SceneTree`, `process_mode`, `pause_mode`/pausing e `await` formam o lifecycle. `_physics_process` é o lugar padrão para lógica sincronizada com física. `WorkerThreadPool`, `Thread`, `Mutex`, `Semaphore` e `call_deferred` ajudam no trabalho assíncrono; Nodes/Scene Tree continuam com restrições de thread.

## 14. Recursos, importação e serialização

### Unity

`Texture2D`, `Texture3D`, `Texture2DArray`, `Cubemap`, `Mesh`, `Material`, `Shader`, `AudioClip`, `AnimationClip`, `Avatar`, `TerrainData`, `TerrainLayer`, `TextAsset`, `ComputeShader`, `RenderTexture`, `ScriptableObject`, `AssetDatabase` e importers são os principais recursos. `AssetDatabase`, `SerializedObject`, `SerializedProperty`, `Undo` e `EditorWindow` são editor-only.

Para MapMagic, `Graph` e `Imported Map` são ScriptableObjects; TerrainLayer é asset Unity; RAW/Texture é entrada; TerrainData é saída persistente opcional.

### Godot

`Texture2D`, `Image`, `ImageTexture`, `Texture3D`, `Texture2DArray`, `Mesh`, `ArrayMesh`, `ImmediateMesh`, `Material`, `ShaderMaterial`, `StandardMaterial3D`, `Shader`, `AudioStream`, `Animation`, `AnimationLibrary`, `NavigationMesh`, `Environment`, `Curve`, `Gradient`, `PackedScene`, `Resource` e `ConfigFile` formam a pilha de assets.

Para MapMagic-like, o equivalente seria um `Resource` de Graph, `Resource` de geração, `Image`/`ImageTexture` como heightmap, `ArrayMesh`/`Mesh` como saída, `HeightMapShape3D` para colisão e `PackedScene` para prefabs.

## 15. Editor e ferramentas

### Unity Editor

`EditorWindow` cria janelas customizadas; `CustomEditor` cria Inspector; `PropertyDrawer` desenha propriedades; `EditorGUILayout`/`EditorGUI` desenham controles; `MenuItem` cria menus; `AssetPostprocessor` intercepta importação; `ScriptedImporter` cria importadores; `SceneView`/`Handles` fazem ferramentas de cena; `Gizmos` desenham debug; `Undo` integra histórico; `SerializedObject` preserva serialização; `PrefabUtility` manipula prefabs; `Selection` controla seleção; `EditorApplication.update` roda callbacks no editor; `AssetDatabase` pesquisa/cria/move assets.

### Godot Editor

`@tool` executa script no editor; `EditorPlugin` cria plugin; `EditorInspectorPlugin` adiciona propriedades/controles; `EditorProperty` customiza editor de campo; `EditorNode3DGizmoPlugin` cria gizmos; `EditorImportPlugin` importa assets; `EditorExportPlugin` altera exportação; `EditorFileSystem` observa assets; `EditorInterface` controla docks/cenas; `EditorUndoRedoManager` integra undo/redo; `GraphEdit`/`GraphNode` fornecem editor visual; `@export` expõe campos no Inspector.

## 16. Equivalências diretas para portar MapMagic

| MapMagic/Unity | Godot provável | Observação |
|---|---|---|
| `MapMagicObject` | `MapMagicWorld : Node3D` customizado | Coordena chunks, graph, ranges, camera markers e apply |
| `Graph : ScriptableObject` | `Graph : Resource` customizado | Guarda nodes, links, layers e defaults |
| `Generator` | `Resource`/classe de node do graph | Não precisa virar Node da cena; pode ser dado serializado |
| `MatrixWorld` | `Image`, `PackedFloat32Array` ou buffer customizado | Para performance, usar buffers/arrays próprios em vez de nodes por pixel |
| `Terrain` | `MeshInstance3D` por chunk | Gerar `ArrayMesh` a partir de heightmap |
| `TerrainData` | Resource customizado + arrays de height/splat/detail | Não há um TerrainData nativo com o mesmo contrato |
| `TerrainCollider` | `StaticBody3D + CollisionShape3D + HeightMapShape3D` | Atualizar shape conforme chunk |
| `TerrainLayer` | `Texture2D + Material/ShaderMaterial` | Splat blending precisa ser escrito no shader/material |
| Trees Output | `MultiMeshInstance3D` ou instâncias de `PackedScene` | MultiMesh é preferível para milhares de árvores |
| Objects Output | `PackedScene.instantiate()` ou MultiMesh | Escolher conforme necessidade de script/collider |
| Grass Output | `MultiMeshInstance3D`, `GPUParticles3D` ou shader | Não existe detail prototype de Terrain igual ao Unity |
| SplineSys | Resource de pontos/curvas | Renderizar com ImmediateMesh, ArrayMesh ou Line3D customizado |
| Biomes Set | Resources/graphs de biome + masks | Mesma ideia de composição e normalização |
| Function | Sub-Resource/Graph instance | Inputs/outputs e overrides expostos |
| Locks | Resource de máscaras persistentes | Aplicar máscara antes/depois da geração |
| Drafts | LOD/chunks de baixa resolução | Usar mesh simplificada e menor resolução |
| Main/Draft Range | Distância de streaming/LOD | Controlar geração conforme câmera/markers |
| Brush | `MapMagicBrush : Node3D` + SubViewport/UI | Usar Image/height buffers e strokes de input |
| Unity Inspector | Inspector Godot + `@export`/EditorInspectorPlugin | Metadata deve alimentar editor, serialização e graph |
| `Undo` | `EditorUndoRedoManager` | Registrar comandos do editor |
| `MenuItem` | `EditorPlugin`/custom dock/menu | Não existe a mesma API textual 1:1 |
| `NavMeshAgent` | `NavigationAgent3D` | O agente Godot ajuda pathfinding/avoidance, mas script move o personagem |
| `Camera.main` | Camera3D current | Godot usa câmera current/viewport; não há exatamente a mesma tag automática |

## 17. O que não deve ser confundido

- `TerrainData` não é `TerrainCollider`: um guarda dados; outro participa da física.
- `MeshRenderer` não é `MeshFilter`: um desenha; outro fornece a mesh.
- `Rigidbody` não é `Collider`: um simula corpo; outro define forma de colisão.
- `AudioSource` não é `AudioListener`: um emite; outro escuta.
- `ParticleSystem` não é `ParticleSystemRenderer`: um simula/emite; outro desenha.
- `NavMeshAgent` não é `NavMeshSurface`: um navega; outro cria/fornece a superfície navegável.
- `ScriptableObject` não é `MonoBehaviour`: o primeiro é asset de dados; o segundo é componente de cena.
- `WorldEnvironment` do Godot não gera terreno; somente configura ambiente.
- `HeightMapShape3D` do Godot não renderiza terreno; somente cria forma de colisão.
- `MultiMeshInstance3D` é instancing de geometria; não cria lógica individual para cada árvore.
- `GraphEdit` é editor de grafos; não é por si só um sistema de execução visual.

## 18. Mínimo para um protótipo MapMagic-like em cada engine

### Unity

```text
Scene
 ├── Main Camera (Transform + Camera + AudioListener)
 ├── Directional Light (Transform + Light)
 └── MapMagicObject
       └── Graph asset (ScriptableObject)
              ├── Noise/height generators
              ├── Height Output
              └── optional Textures/Objects/Trees outputs
```

Componentes mínimos: `MapMagicObject`, `Terrain`, `TerrainData`, `TerrainCollider` quando houver colisão, `Camera`, pelo menos uma `Light`/material de visualização e `TerrainLayer` se houver textura.

### Godot

```text
World (Node3D)
 ├── Camera3D
 ├── DirectionalLight3D
 ├── WorldEnvironment
 └── MapMagicWorld (Node3D customizado)
       ├── Chunk MeshInstance3D(s)
       ├── StaticBody3D
       │    └── CollisionShape3D + HeightMapShape3D
       └── MultiMeshInstance3D / PackedScene instances
```

Nodes mínimos: `Node3D`, `Camera3D`, `DirectionalLight3D`, opcional `WorldEnvironment`, um gerador customizado, `MeshInstance3D`/`ArrayMesh` para visual e `StaticBody3D + CollisionShape3D + HeightMapShape3D` para colisão.

## 19. Fontes técnicas

- [Unity — Introduction to components](https://docs.unity3d.com/Manual/Components.html)
- [Unity — TerrainData API](https://docs.unity3d.com/ScriptReference/TerrainData.html)
- [Unity — TerrainData.SetHeightsDelayLOD](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/TerrainData.SetHeightsDelayLOD.html)
- [Unity — Terrain Layer](https://docs.unity3d.com/Manual/class-TerrainLayer.html)
- [Godot — Node3D e árvore de classes](https://docs.godotengine.org/en/stable/classes/class_node3d.html)
- [Godot — World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html)
- [Godot — CollisionShape3D](https://docs.godotengine.org/en/stable/classes/class_collisionshape3d.html)
- [Godot — HeightMapShape3D](https://docs.godotengine.org/en/4.4/classes/class_heightmapshape3d.html)
- [Godot — GeometryInstance3D](https://docs.godotengine.org/en/stable/classes/class_geometryinstance3d.html)
- [Godot — MultiMeshInstance3D](https://docs.godotengine.org/en/latest/tutorials/3d/using_multi_mesh_instance.html)
- [Godot — Navigation 3D](https://docs.godotengine.org/en/stable/tutorials/navigation/navigation_introduction_3d.html)
- [Godot — GPUParticles3D](https://docs.godotengine.org/en/stable/classes/class_gpuparticles3d.html)
- [Godot — AudioStreamPlayer3D](https://docs.godotengine.org/en/stable/classes/class_audiostreamplayer3d.html)
- [Godot — RigidBody3D](https://docs.godotengine.org/en/stable/classes/class_rigidbody3d.html)
