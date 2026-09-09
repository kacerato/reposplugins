# 08 — partículas, emitters e efeitos

## Unity

### ParticleSystem

**O que faz:** simula partículas por emissão, tempo de vida, posição, velocidade, tamanho, cor, rotação e módulos.

**Onde fica:** GameObject com ParticleSystem.

**Precisa de:** ParticleSystemRenderer, material/shader e módulos configurados.

### ParticleSystemRenderer

Desenha partículas como billboard, mesh, stretched billboard, ribbon/trail ou modelo escolhido.

### Módulos principais

- `Main`: duração, loop, lifetime, speed, size, rotation, color, gravity, simulation space;
- `Emission`: taxa e bursts;
- `Shape`: esfera, cone, box, mesh, edge;
- `Velocity over Lifetime`: velocidade por curva;
- `Limit Velocity`: limita velocidade;
- `Force over Lifetime`: força contínua;
- `Color over Lifetime`: gradiente;
- `Size over Lifetime`: escala por curva;
- `Rotation over Lifetime`: rotação;
- `Noise`: turbulência;
- `Collision`: colisão com mundo/planes;
- `Triggers`: callbacks por volume;
- `Sub Emitters`: partículas filhas;
- `Texture Sheet Animation`: sprites em atlas;
- `Lights`: luzes associadas;
- `Trails`: rastros.

### ParticleSystemForceField

Campo de força que influencia partículas na região.

### VisualEffect

Componente do Visual Effect Graph. É uma solução diferente do ParticleSystem clássico e exige o pacote/render pipeline compatível.

## Godot

### GPUParticles3D

Emitter 3D processado na GPU. Usa `ParticleProcessMaterial` ou `ShaderMaterial`; adequado para muitos elementos.

Propriedades: amount, lifetime, one_shot, preprocess, local_coords, draw order, process material, draw pass mesh e trails.

### CPUParticles3D

Emitter processado na CPU. Mais flexível/compatível em alguns casos, porém pode custar mais CPU.

### GPUParticles2D/CPUParticles2D

Equivalentes 2D.

### ParticleProcessMaterial

Resource que controla direção, spread, gravity, damping, initial velocity, scale, color, animation e emissão.

### ShaderMaterial em partículas

Permite comportamento customizado por shader, com maior controle e maior responsabilidade de compatibilidade.

### Colisões de partículas

`GPUParticlesCollisionBox3D`, `GPUParticlesCollisionSphere3D`, `GPUParticlesCollisionHeightField3D`, `GPUParticlesCollisionSDF3D` e `GPUParticlesCollisionVectorField3D` fornecem volumes/campos para GPU particles conforme renderer/versão.

## Processo

```text
Emitter
  ↓
Emission/seed
  ↓
Particle simulation
  ↓
Collision/forces/noise
  ↓
Renderer/material
  ↓
GPU/CPU draw
```

## MapMagic e partículas

MapMagic Grass Output é detalhe de Terrain, não necessariamente ParticleSystem. Para biomas com milhões de blades:

- Unity: Terrain detail/instancing ou VFX/particles conforme visual;
- Godot: MultiMeshInstance3D, GPUParticles3D ou shader;
- partículas não substituem collider individual;
- Draft deve diminuir amount/density;
- tiles distantes podem desligar partículas.

## Dependências

- mesh/material quando o modo não é billboard;
- bounds corretos, pois partículas podem ser cortadas;
- shader que suporte transparência/soft particles;
- collision volumes se houver colisão;
- GPU/renderer compatível para GPU particles;
- limite de partículas por tile em mobile.

## Tutorial de validação

### Unity

1. Crie ParticleSystem.
2. Configure Main e Emission.
3. Use Shape Cone.
4. Adicione Color over Lifetime e Noise.
5. Adicione ParticleSystemForceField.
6. Compare billboard e mesh.
7. Meça overdraw no dispositivo.

### Godot

1. Adicione GPUParticles3D.
2. Defina amount/lifetime.
3. Crie ParticleProcessMaterial.
4. Defina draw pass mesh.
5. Teste CPUParticles3D como fallback.
6. Adicione collision volume.
7. Compare renderer Mobile/Compatibility.

## Limite de declaração do MapMagic

O `Grass Output` do MapMagic é output de detalhe/grass do Terrain; o bundle não declara `ParticleSystem`, `GPUParticles3D` ou `CPUParticles3D` como mecanismo obrigatório para esse output. Partículas podem aparecer em prefabs ou projeto, mas não devem ser tratadas como parte do algoritmo de mapa.

Sequência confirmada:

1. generator produz uma mask/densidade;
2. Grass Output aplica essa informação ao sistema de detalhes do Terrain;
3. renderer do Terrain desenha o detalhe conforme suas configurações;
4. ParticleSystem/GPUParticles só entram se o projeto escolher outro sistema visual.

## Interface de propriedades e Inspector

### Unity

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `ParticleSystem` | Inspector do `GameObject` | Main, emission, shape, velocity, lifetime, color, size, collision e módulos habilitados |
| `ParticleSystemRenderer` | Inspector do mesmo objeto | Render mode, material, mesh, trail e sorting |
| `ParticleSystemForceField` | Inspector do `GameObject` | Tipo, alcance, direção e força do campo |
| `VisualEffect` | Inspector e Visual Effect Graph | Asset `.vfx`, parâmetros expostos e execução |
| Módulos de partículas | Dentro do Inspector do `ParticleSystem` | Cada módulo aparece quando ativado e mostra seus campos | Não são componentes independentes anexáveis |

### Godot

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `GPUParticles3D`/`CPUParticles3D` | Inspector do nó | Amount, lifetime, explosiveness, visibility AABB, material/process material e emissão |
| `GPUParticles2D`/`CPUParticles2D` | Inspector do nó | Quantidade, duração, textura/material e processo 2D |
| `ParticleProcessMaterial` | Inspector do recurso | Gravidade, direção, velocidade, escala, cor e curvas suportadas |
| `ShaderMaterial` em partículas | Inspector do recurso | Shader e uniforms expostos |
| Colisões de partículas | Inspector do nó/recurso, quando suportado | Parâmetros do modo de colisão | A interface e o suporte variam por versão e renderer |

### O que o MapMagic chama de Grass

O `Grass Output` do MapMagic é configurado no Inspector próprio do `MapMagicObject`, dentro de `Outputs Settings`, com os campos de resolução identificados no código (`Resolution Downscale` e `Resolution per Patch`). Ele trabalha com dados de detalhe do Terrain; isso não abre nem exige `ParticleSystem`, `GPUParticles3D` ou `CPUParticles3D`.

## Fontes

- [Unity Particle System](https://docs.unity3d.com/Manual/PartSysUsage.html)
- [Godot GPUParticles3D](https://docs.godotengine.org/en/stable/classes/class_gpuparticles3d.html)
- [Godot CPUParticles3D](https://docs.godotengine.org/en/stable/classes/class_cpuparticles3d.html)
- [Godot criação de partículas 3D](https://docs.godotengine.org/en/stable/tutorials/3d/particles/creating_a_3d_particle_system.html)
