# 03 — física, corpos e colisões

## Função da categoria

Física responde a colisões, forças, gravidade, juntas, raycasts e movimento de personagens. MapMagic pode gerar o chão, mas não deve acoplar a geração à lógica do player: Terrain/mesh fornece a superfície; o sistema de física consulta essa superfície.

## Unity 3D

### Collider

Base de formas físicas. Define o volume que participa de colisão, trigger e raycast.

**Precisa de:** shape válida, layer collision matrix e outro collider/consulta.

### BoxCollider

Caixa alinhada ao objeto. Barato; use para paredes, caixas, áreas simples.

### SphereCollider

Esfera. Barato; bom para projéteis e volumes redondos.

### CapsuleCollider

Cápsula. Bom para personagens porque desliza melhor em degraus e bordas.

### MeshCollider

Usa a geometria da mesh como colisão. `Convex` é necessário para várias interações dinâmicas; mesh côncava é adequada principalmente a objetos estáticos e pode ser cara.

### TerrainCollider

Lê heightmap do `TerrainData` e fornece o chão para física.

**MapMagic:** precisa ser atualizado junto com TerrainData. Se o visual for regenerado e o collider não, o jogador verá um chão diferente do chão físico.

### WheelCollider

Modelo especializado de roda com suspensão, slip, motor e freio. Não é um collider visual genérico.

### Rigidbody

Corpo rígido 3D simulado. Recebe forças, impulsos, torque, gravidade, massa, drag e constraints.

**Processo:** Physics engine calcula movimento no timestep fixo; colliders geram contatos; callbacks/notificações informam colisões; o Transform é atualizado.

**Regra:** não mover Rigidbody dinâmico diretamente em `Update`; usar forças, `MovePosition`/`MoveRotation` quando apropriado e `FixedUpdate`.

### CharacterController

Cápsula controlada por script para personagens. Não funciona como Rigidbody dinâmico; o script determina deslocamento e o controller resolve colisões.

### ArticulationBody

Corpo articulado para robótica, braços, veículos e hierarquias com joints mais determinísticos.

### Joints

- `FixedJoint`: prende corpos;
- `HingeJoint`: gira em torno de eixo;
- `SpringJoint`: ligação elástica;
- `CharacterJoint`: ragdoll;
- `ConfigurableJoint`: seis graus configuráveis;
- `ConstantForce`: força/torque contínuo.

### PhysicsMaterial

Define friction, bounciness e combinação entre superfícies. Deve ser aplicado ao Collider.

### Triggers

Collider com `Is Trigger` não bloqueia; dispara `OnTriggerEnter/Stay/Exit`. Exige configuração de layers e, para certos callbacks, Rigidbody.

### Consultas

- `Physics.Raycast`: raio;
- `Physics.SphereCast`: esfera deslizando;
- `Physics.BoxCast`: caixa deslizando;
- `Physics.OverlapSphere/Box/Capsule`: volumes;
- `RaycastNonAlloc`: evita allocations quando reutiliza buffer;
- `Collider.Raycast`: consulta contra collider específico.

## Unity 2D

`Rigidbody2D` simula corpo; `Collider2D` é a base; `BoxCollider2D`, `CircleCollider2D`, `CapsuleCollider2D`, `PolygonCollider2D`, `CompositeCollider2D` e `TilemapCollider2D` são formas. `HingeJoint2D`, `SpringJoint2D`, `DistanceJoint2D`, `FixedJoint2D`, `SliderJoint2D` e `WheelJoint2D` conectam corpos. Effectors alteram forças/áreas.

## Godot 3D

### CollisionObject3D

Base de objetos que usam collision layer/mask e shapes.

### StaticBody3D

Corpo imóvel, usado em Terrain, paredes e cenário.

**MapMagic:** `StaticBody3D > CollisionShape3D > HeightMapShape3D` é o equivalente básico do TerrainCollider.

### AnimatableBody3D

Corpo movido manualmente/animação que afeta a simulação física; ideal para plataformas móveis, portas e elevadores.

### CharacterBody3D

Corpo controlado por script. Fornece helpers como `move_and_slide`, detecção de chão, paredes e teto.

### RigidBody3D

Corpo governado pela simulação. Use forças/impulsos; não reposicione diretamente a cada frame.

### Area3D

Volume que detecta corpos/áreas entrando e saindo. Ideal para triggers, água, zonas de dano e interação.

### CollisionShape3D

Atribui uma `Shape3D` a `Area3D` ou corpo físico. Sem shape ativa, o corpo não tem forma de colisão.

### CollisionPolygon3D

Define colisão por polígonos/convex hull conforme configuração.

### Shapes

- `BoxShape3D`;
- `SphereShape3D`;
- `CapsuleShape3D`;
- `CylinderShape3D`;
- `ConvexPolygonShape3D`;
- `ConcavePolygonShape3D`;
- `HeightMapShape3D`;
- `SeparationRayShape3D`.

`HeightMapShape3D` é limitado a um valor de altura por posição X/Z; cavernas/overhangs precisam de mesh/concave shape ou representação própria.

### Joints

- `HingeJoint3D`;
- `PinJoint3D`;
- `SliderJoint3D`;
- `ConeTwistJoint3D`;
- `Generic6DOFJoint3D`.

### Casts

`RayCast3D` é consulta persistente na cena; `ShapeCast3D` testa swept volume. Para consultas pontuais em script, use `PhysicsDirectSpaceState3D` e parâmetros de query.

## Layers, masks e tags

Unity usa layers e matriz global. Godot usa collision layers/masks. Em ambos:

1. defina quais grupos o objeto pertence;
2. defina o que ele pode detectar;
3. filtre raycast/overlap;
4. valide com debug collision.

## Processo de colisão do terreno

```text
Heightmap gerado
   ↓
TerrainData/Terrain mesh
   ↓
TerrainCollider ou HeightMapShape3D
   ↓
Physics world
   ↓
Raycast/Character/Rigidbody
   ↓
Contato, chão, trigger ou movimento
```

## Dependências para MapMagic

- Unity: TerrainCollider e TerrainData; CharacterController/Rigidbody do jogo são opcionais.
- Godot: StaticBody3D, CollisionShape3D e HeightMapShape3D para chão.
- Collider de terreno deve acompanhar o mesmo tile, transform, escala e resolução do visual.
- Não gerar collider detalhado para tiles distantes se o gameplay não precisa dele.

## Tutorial de validação

### Unity

1. Crie Terrain com TerrainCollider.
2. Crie uma cápsula com Rigidbody.
3. Faça a cápsula cair.
4. Use Raycast do player para detectar chão.
5. Gere novo heightmap e confirme atualização visual/física.
6. Ative Physics Debugger e revise layers.

### Godot

1. Crie `StaticBody3D`.
2. Adicione `CollisionShape3D`.
3. Atribua BoxShape3D e teste com `RigidBody3D`.
4. Troque para HeightMapShape3D.
5. Adicione `CharacterBody3D`.
6. Teste `move_and_slide` sobre o heightmap.

## Fontes

- [Unity Collider](https://docs.unity3d.com/Manual/CollidersOverview.html)
- [Unity Rigidbody](https://docs.unity3d.com/Manual/rigidbody-physics.html)
- [Unity TerrainData](https://docs.unity3d.com/ScriptReference/TerrainData.html)
- [Godot CollisionShape3D](https://docs.godotengine.org/en/stable/classes/class_collisionshape3d.html)
- [Godot RigidBody3D](https://docs.godotengine.org/en/stable/classes/class_rigidbody3d.html)
- [Godot HeightMapShape3D](https://docs.godotengine.org/en/4.4/classes/class_heightmapshape3d.html)

## Escopo declarado pelo MapMagic

O `MapMagicObject` v2.1.11 possui o campo `applyColliders = true` e os tiles possuem `Terrain`/`TerrainData`; portanto, o bundle declara aplicação de collider do Terrain como opção do pipeline. Ele não declara `Rigidbody`, `CharacterController`, `WheelCollider`, `PhysicsMaterial` ou joints como dependências do gerador.

Sequência segura, limitada ao que o código declara:

1. o tile cria/possui Terrain;
2. outputs aplicam dados no Terrain;
3. a opção `applyColliders` controla a aplicação do collider;
4. o player/projeto consulta esse chão usando seus próprios componentes físicos.

A última etapa é responsabilidade do projeto Unity. Não se deve afirmar que o MapMagic cria player, veículo ou NPC automaticamente.
