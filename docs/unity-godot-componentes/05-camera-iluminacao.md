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
