# 06 — animação, skeletons e sequências

## Unity

### Animator

Executa um `RuntimeAnimatorController` com estados, transitions, parameters e layers.

**Onde fica:** GameObject do personagem.

**Precisa de:** controller, Avatar quando humanoide, clips e normalmente SkinnedMeshRenderer.

**Processo:** parâmetros mudam; state machine avalia transitions; clips são misturados; bones recebem poses; SkinnedMeshRenderer deforma a mesh.

### Animator Controller

Asset de state machines, blend trees, transitions e parameters. Não é componente; é recurso usado pelo Animator.

### AnimatorOverrideController

Substitui clips de um controller mantendo a mesma lógica. Útil para variações de personagem.

### AnimationClip

Curvas de propriedades, bones, eventos e frames.

### Avatar

Mapeia bones para retarget humanoide/genérico.

### SkinnedMeshRenderer

Renderiza mesh deformada por bones, weights, bind poses e blend shapes.

### Animation

Componente do sistema legado para tocar clips simples. Evitar em arquitetura nova salvo compatibilidade.

### PlayableDirector/Timeline

`PlayableDirector` executa uma `TimelineAsset`. Timeline organiza animações, áudio, sinais, ativação de objetos e câmeras.

### Cloth

Simulação de tecido sobre SkinnedMeshRenderer; custo e suporte devem ser testados em mobile.

## Godot

### AnimationPlayer

Reproduz `AnimationLibrary` e anima tracks de propriedades, transforms, métodos e áudio.

**Onde fica:** Node da cena que controla qualquer alvo.

### AnimationTree

Sistema de mistura, blend tree e state machine conectado a AnimationPlayer.

### AnimationNodeStateMachine

Resource/node interno do AnimationTree para estados e transitions.

### Skeleton3D

Hierarquia de bones. Meshes skinned usam Skeleton3D para deformação.

### BoneAttachment3D

Prende Node a um bone, por exemplo arma na mão ou acessório na cabeça.

### PhysicalBone3D

Integra bone a física/ragdoll.

### AnimatedSprite2D/3D

Troca frames de sprite sem skeleton.

### Tween

Anima propriedades por interpolação temporal, bom para UI, portas e efeitos simples sem criar clip.

## Comparação

| Função | Unity | Godot |
|---|---|---|
| Clip | AnimationClip | Animation |
| Player | Animator/Animation | AnimationPlayer |
| State machine | Animator Controller | AnimationTree/StateMachine |
| Bone hierarchy | Avatar + bones | Skeleton3D |
| Mesh skinned | SkinnedMeshRenderer | MeshInstance3D + skin/skeleton |
| Sequência | PlayableDirector/Timeline | AnimationPlayer/AnimationTree/scene scripts |
| Acessório no bone | Transform/bone | BoneAttachment3D |

## Processo de animação procedural

1. Objetos gerados recebem prefab/scene.
2. Prefab contém Animator ou AnimationPlayer.
3. O output instancia o objeto.
4. O script inicializa parâmetros.
5. State machine toca animação.
6. LOD pode trocar mesh/rig distante.
7. Ao remover tile, parar/destruir ou devolver ao pool.

## Dependências para objetos MapMagic

- Trees Output do MapMagic pode usar árvores Terrain, que não são Animator completos;
- Objects Output pode instanciar prefabs com Animator/Animation;
- prefabs devem ter clips/controller/rig válidos;
- pooling precisa resetar estado de animação;
- animação não deve depender de referência destruída ao descarregar tile.

## Tutorial de validação

### Unity

1. Crie um prefab com Animator.
2. Crie Idle/Walk clips.
3. Adicione bool `IsWalking`.
4. Faça transitions.
5. Instancie pelo Objects Output.
6. Remova/recrie o tile e confira reset.

### Godot

1. Crie uma cena de personagem.
2. Adicione Skeleton3D/MeshInstance3D.
3. Adicione AnimationPlayer.
4. Crie Idle/Walk.
5. Adicione AnimationTree se houver blends.
6. Instancie PackedScene pelo chunk manager.
7. Descarregue e recarregue conferindo estado.

## Limite de declaração do MapMagic

O MapMagic v2.1.11 recebe prefabs nos outputs `Objects`/`Trees`, mas o núcleo analisado não declara um `Animator`, `Animation`, `AnimationPlayer`, `AnimationTree` ou sistema de animação próprio. Se o prefab fornecido tiver Animator/AnimationPlayer, ele poderá seguir o lifecycle normal da engine; isso pertence ao prefab/projeto, não ao generator de terreno.

Assim, a sequência confirmada é apenas:

1. generator produz transições/objetos;
2. output recebe prefab/árvore;
3. output aplica/instancia conforme suas opções;
4. qualquer animação fica a cargo do objeto instanciado e de seu script/controller.

Não atribuir ao MapMagic recursos de state machine, blend tree ou Timeline sem uma integração adicional comprovada.

## Interface de propriedades e Inspector

### Unity

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `Animator` | Inspector do `GameObject` | Controller, Avatar, apply root motion, culling e parâmetros de atualização |
| `Animator Controller` | Inspector do asset e janela Animator | States, transitions, layers, parameters e state machine |
| `AnimatorOverrideController` | Inspector do asset | Controller base e clips substituídos |
| `AnimationClip` | Inspector do asset | Duração, curvas e opções de importação/clip conforme o asset |
| `Avatar` | Inspector do asset | Configuração/validação do avatar humanoide quando aplicável |
| `SkinnedMeshRenderer` | Inspector do `GameObject` | Mesh, bones, root bone, materials e bounds |
| `Animation` | Inspector do `GameObject` | Clips e modo de reprodução do componente legado |
| `PlayableDirector`/Timeline | Inspector do `GameObject` e janela Timeline | Asset Timeline, binding, tempo e modo de reprodução |
| `Cloth` | Inspector | Parâmetros de simulação e colisão suportados pela versão |

### Godot

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `AnimationPlayer` | Inspector do nó | Biblioteca, autoplay, velocidade e animações do nó |
| `AnimationTree` | Inspector do nó e painel próprio | Árvore/blend tree, ativo, player e parâmetros |
| `AnimationNodeStateMachine` | Editor da árvore de animação | States e transitions; não é normalmente um componente da Scene Tree sozinho |
| `Skeleton3D` | Inspector do nó e painel de esqueleto | Bones, pose e edição do esqueleto |
| `BoneAttachment3D`/`PhysicalBone3D` | Inspector do nó | Bone alvo e propriedades físicas/transformações |
| `AnimatedSprite2D`/`AnimatedSprite3D` | Inspector do nó | Sprite frames, animação, autoplay, velocidade e reprodução |
| `Tween` | API/runtime | Não é um nó persistente com Inspector padrão; é criado/gerenciado por código |

### Regra de interface

Selecionar o componente mostra seus dados editáveis; a janela Animator/Timeline/AnimationTree é uma interface adicional para editar o recurso de animação. Clicar em `Play`/autoplay ou alterar parâmetros do Inspector não significa que a animação esteja integrada ao MapMagic.

O MapMagic v2.1.11 pode receber prefabs nos outputs de objetos/árvores, mas o núcleo analisado não declara `Animator`, `Animation`, `AnimationPlayer`, `AnimationTree` ou Timeline. Se um prefab tiver um desses componentes, o Inspector pertence ao prefab/projeto.

## Fontes

- [Unity Animator](https://docs.unity3d.com/Manual/class-Animator.html)
- [Unity Timeline](https://docs.unity3d.com/Manual/TimelineSection.html)
- [Godot AnimationPlayer](https://docs.godotengine.org/en/stable/classes/class_animationplayer.html)
- [Godot AnimationTree](https://docs.godotengine.org/en/stable/classes/class_animationtree.html)
- [Godot Skeleton3D](https://docs.godotengine.org/en/stable/classes/class_skeleton3d.html)
