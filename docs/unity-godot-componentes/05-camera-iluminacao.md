# 05 — câmera, iluminação, ambiente e reflexos

## Unity

### Camera

**O que faz:** escolhe o que será renderizado e como será projetado.

**Propriedades:** perspective/orthographic, FOV/size, near/far clip, culling mask, depth, viewport rect, HDR, MSAA, target texture e output display.

**Onde fica:** GameObject com `Camera`.

**Precisa de:** Transform, viewport/render pipeline e pelo menos um objeto visível. `Camera.main` exige câmera com tag `MainCamera`.

**MapMagic:** modo de marker `Around Main Camera` depende de câmera principal; em editor e mobile é mais robusto permitir markers explícitos.

### Light

- **Directional**: sol/luz global, não depende de posição;
- **Point**: luz em todas as direções;
- **Spot**: cone;
- **Area**: luz de área, conforme pipeline/render path.

Propriedades comuns: color, intensity, range, angle, shadows, culling mask, baking/mixed/realtime.

### ReflectionProbe

Captura ambiente local para reflexos. Possui volume, resolução, clipping, importance e modo baked/realtime.

### LightProbeGroup

Distribui pontos de iluminação indireta para objetos dinâmicos.

### LightProbeProxyVolume

Amostra vários probes sobre objeto grande; melhora resultado de iluminação indireta.

### Skybox/RenderSettings

Definem ambiente global, skybox, ambient light, fog e reflection source. Algumas opções variam por pipeline.

### Volume/VolumeProfile

Sistema de pós-processamento e ambiente de URP/HDRP. É pipeline-dependent e não é requisito do MapMagic core.

## Godot

### Camera3D

**O que faz:** câmera perspective/orthogonal/frustum. Possui near/far, FOV/size, cull mask, current e atributos.

**Onde fica:** filho de Node3D no Scene Tree.

**Precisa de:** viewport e ser `current`/selecionada para renderizar daquela posição.

### WorldEnvironment

Configura Environment padrão da cena: background, sky, ambient lighting, tonemapping, fog e pós-processamento. Deve haver uma instância de ambiente padrão por cena.

### Environment

Resource com configurações ambientais. Pode ser usado por WorldEnvironment e, em alguns casos, sobrescrito por Camera3D.

### DirectionalLight3D

Luz direcional global, equivalente ao sol.

### OmniLight3D

Luz pontual omnidirecional.

### SpotLight3D

Luz em cone, com range e ângulo.

### ReflectionProbe

Captura reflexos locais no cenário.

### LightmapGI

Iluminação global baked/lightmap. Exige bake e UV/configuração adequados.

### VoxelGI

Iluminação global voxelizada para cenários compatíveis; custo maior e limitações mobile.

### LightmapProbe

Probes para objetos dinâmicos receberem iluminação baked.

### FogVolume

Neblina localizada em volume.

### Sky

Resource de céu. Pode usar `ProceduralSkyMaterial`, `PhysicalSkyMaterial` ou panorama.

## Processo de iluminação de um chunk procedural

```text
Chunk/mesh
   ↓
Normais/tangentes/UVs
   ↓
Material/shader
   ↓
Camera culling
   ↓
Light/probes/reflections
   ↓
Shadow/ambient/post-process
   ↓
Imagem final
```

## Dependências e erros comuns

- sem câmera current/Main Camera, nada é visto;
- sem luz, material lit pode ficar preto;
- shader de Terrain incompatível com pipeline causa rosa/erro;
- reflection probe não gera luz global;
- LightProbeGroup não substitui Light;
- sombras, GI e pós-processamento podem custar caro em mobile;
- culling mask/layers podem esconder Terrain;
- bounds errados fazem chunks sumirem.

## Tutorial de validação

### Unity

1. Crie Main Camera com tag `MainCamera`.
2. Crie Directional Light.
3. Crie Terrain/mesh.
4. Adicione ReflectionProbe e LightProbeGroup.
5. Teste culling mask.
6. Troque material Built-in/URP/HDRP conforme projeto.
7. Compare sombras/reflexos em dispositivo Android.

### Godot

1. Crie World Node3D.
2. Adicione Camera3D e marque current.
3. Adicione DirectionalLight3D.
4. Adicione WorldEnvironment + Environment.
5. Crie MeshInstance3D.
6. Adicione ReflectionProbe/FogVolume se necessário.
7. Verifique renderer Forward+/Mobile/Compatibility para o alvo.

## Interface de propriedades e Inspector

### Unity

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `Camera` | Inspector do `GameObject` | Projection, FOV/ortho size, clipping planes, viewport, depth/priority e clear flags conforme a versão |
| `Light` | Inspector | Tipo, cor, intensidade, alcance/ângulo, sombras e culling mask |
| `ReflectionProbe` | Inspector | Bounds, clipping, resolução, HDR, intensidade e modo de atualização |
| `LightProbeGroup` | Inspector + Scene View | Pontos da sonda e edição visual no Scene View |
| `LightProbeProxyVolume` | Inspector | Volume, resolução e modo de atualização |
| Skybox/`RenderSettings` | Lighting/Environment e propriedades do projeto/cena | Skybox, ambient lighting, fog e reflexos conforme o pipeline |
| `Volume`/`VolumeProfile` | Inspector do objeto e asset | Profile, prioridade, peso, blend distance e overrides expostos pelo pipeline |

### Godot

| Tipo | Onde aparece | Interface/propriedades |
|---|---|---|
| `Camera3D` | Inspector do nó | Projection, FOV/size, near/far, cull mask e propriedades de viewport |
| `WorldEnvironment`/`Environment` | Inspector do nó/recurso | Background, sky, ambient light, fog, tonemap, glow e ajustes suportados |
| `DirectionalLight3D`, `OmniLight3D`, `SpotLight3D` | Inspector do nó | Cor, energia, alcance/ângulo, sombras e cull mask |
| `ReflectionProbe` | Inspector do nó | Bounds e atualização/reflexos conforme o recurso |
| `LightmapGI`/`VoxelGI`/`LightmapProbe` | Inspector do nó | Parâmetros de bake/volume e propriedades do sistema |
| `FogVolume`/`Sky` | Inspector do nó/recurso | Forma, material de fog e dados do céu |

### Sequência prática

1. Selecionar a câmera e definir a projeção.
2. Selecionar a luz/ambiente e configurar a iluminação.
3. Verificar se o objeto está na layer/cull mask correta.
4. Adicionar probes/volumes somente se o projeto realmente utilizar esses sistemas.
5. No MapMagic, confirmar o `Material Template` e as `Terrain Properties` no Inspector próprio; a câmera e as luzes continuam sendo componentes da cena do Unity.

O código do bundle possui um fallback de material de Terrain, mas não declara `Cinemachine`, `WorldEnvironment`, `ReflectionProbe`, `LightProbeGroup` ou `Volume` como requisitos do `MapMagicObject`.

## Fontes

- [Unity Camera](https://docs.unity3d.com/Manual/CamerasOverview.html)
- [Unity Light](https://docs.unity3d.com/Manual/LightingOverview.html)
- [Unity Reflection Probe](https://docs.unity3d.com/Manual/RefProbe.html)
- [Godot WorldEnvironment](https://docs.godotengine.org/en/latest/classes/class_worldenvironment.html)
- [Godot World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html)
- [Godot Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html)

## Declarações específicas do bundle

O MapMagic possui modo de geração por markers que pode usar Around Main Camera, Around Objects, Around Objects Tagged ou Around Coordinates. O modo Around Main Camera pressupõe que exista uma câmera principal localizável pelo projeto; os outros modos não dependem dela da mesma forma.

O material padrão é resolvido pelo código de fallback documentado no arquivo de renderização. O bundle não declara `Cinemachine`, `WorldEnvironment`, `ReflectionProbe`, `LightProbeGroup` ou `Volume` como requisitos do MapMagicObject.

Sequência de diagnóstico confirmada:

1. validar se o tile está ativo/visível;
2. validar câmera/marker quando a geração for guiada por câmera;
3. validar material/shader encontrado;
4. validar luz/ambiente do projeto;
5. só então analisar culling, LOD ou pós-processamento.
