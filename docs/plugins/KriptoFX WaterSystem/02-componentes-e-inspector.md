# WaterSystem — componentes, recursos e Inspector

## `WaterSystem`

O componente principal é `KWS.WaterSystem`. A classe é `[ExecuteAlways]`, portanto o pacote executa lógica no Editor e no Play Mode. O código declara dois campos serializados públicos:

| Campo | Tipo | Uso |
|---|---|---|
| `Profile` | `WaterSystemScriptableData` | asset/perfil salvo; o Inspector pode salvar settings atuais nele ou carregar dele |
| `Settings` | `WaterSystemScriptableData` | cópia/estado efetivo usado pelo WaterSystem |

Ao habilitar, o sistema inicializa FFT, reflection, shaders, passes e settings. Se `Settings` estiver nulo, cria uma instância ou clona `Profile`. O Inspector mostra aviso quando `Settings` e `Profile` não estão sincronizados.

### Campos de API que não são necessariamente controles visualizados

`IsWaterRenderingActive`, `UseNetworkTime`, `NetworkTime`, `WorldSpaceBounds`, `IsCameraUnderwater`, `ScreenSpaceBounds` e `OnWaterRender` são membros públicos da classe. O `KWS_Editor` usa e expõe parte da configuração por abas, mas não há evidência de que todos sejam desenhados como campos editáveis. `IsCameraUnderwater` tem setter privado; é leitura de estado, não entrada do Inspector.

## `KW_Buoyancy`

O componente é anexável e exige automaticamente:

```csharp
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
```

| Campo no Inspector | Faixa/valor no código | Significado |
|---|---|---|
| `VolumeSource` | `Collider` ou `Mesh` | fonte usada para volume/voxels |
| `OvverideCenterOfMass` | `Transform` | centro de massa opcional |
| `Density` | `100–1000` | densidade usada no cálculo |
| `SlicesPerAxisX/Y/Z` | `1–6` | subdivisão do volume em cada eixo |
| `isConcave` | bool | marca o caso côncavo |
| `VoxelsLimit` | `2–32` | limite de voxels |
| `AngularDrag` | float, default `0.25` | arrasto angular aplicado |
| `Drag` | float, default `0.25` | arrasto linear aplicado |
| `NormalForce` | `0–1`, default `0.2` | força relacionada à normal da superfície |
| `DebugForces` | bool | habilita visualização das forças |

O `README` diz “adicione `KW_Buoyancy` a um objeto com Rigidbody”; o atributo também exige Collider. O componente registra-se no conjunto dinâmico do WaterSystem e lê dados de superfície de modo assíncrono; o script consumidor precisa tolerar dados ainda não prontos.

## `KW_InteractWithWater`

É `[ExecuteInEditMode]` e pode ser anexado a objetos móveis para criar interação com ondas dinâmicas. O editor desenha uma esfera gizmo amarela no ponto `TransformPoint(Offset)`.

| Campo | Faixa/valor | Uso |
|---|---|---|
| `Size` | `0.025–10`, default `0.15` | tamanho da área de interação |
| `Strength` | `0.05–1`, default `1` | força derivada do deslocamento |
| `Pressure` | `-1–1`, default `0` | pressão subtraída da força |
| `Offset` | `Vector3.zero` | ponto local de interação |

O script calcula a distância percorrida desde o frame anterior, limita-a e registra a instância em `KW_WaterDynamicScripts`. Não é um collider nem um sistema de movimento: o objeto precisa ser movido por outro sistema.

## `KWS_AddLightToWaterRendering`

É `[ExecuteAlways]`, exige `Light` e possui Inspector customizado com suporte a múltiplos objetos:

| Campo | Opções | Visibilidade |
|---|---|---|
| `VolumetricLightRenderingMode` | `LightOnly` ou `ShadowAndLight` conforme enum `KWS_WaterLights.VolumeLightRenderMode` | sempre no Inspector customizado |
| `ShadowDownsample` | enum `ShadowDownsampleEnum` | aparece/é relevante em `ShadowAndLight` com Light Directional ou Spot |

O script registra a luz para coleta de luzes volumétricas e cria/limpa command buffers de shadowmap. A luz continua sendo o componente Unity original; este script é uma ponte específica do WaterSystem.

## `WaterPassHandler`

É um `MonoBehaviour` `[ExecuteAlways]`, mas o `WaterSystem` o adiciona a um GameObject temporário interno no backend Standard. Ele mantém passes como `OrthoDepthPass`, `ShorelineWavesPass`, `MaskDepthNormalPass`, `CausticPass`, `CopyColorPass`, `ReflectionFinalPass`, `ScreenSpaceReflectionPass`, `VolumetricLightingPass`, `DrawMeshPass`, `ShorelineFoamPass`, `UnderwaterPass` e `DrawToPosteffectsDepthPass`.

Não adicione manualmente esse handler em uma cena sem entender o lifecycle do WaterSystem; no fluxo local ele é infraestrutura automática.

## `UndoProvider` e `ComputeBufferObject`

`UndoProvider` é um componente auxiliar em objeto temporário e serializa listas de `ShorelineWavesScriptableData.ShorelineWave` e `SplineScriptableData.Spline`. `ComputeBufferObject` é gerado pelo `ComputeBufferImporter`; seus arrays são `[HideInInspector]` e o método `GetComputeBuffer()` cria o buffer a partir de `PackedSize`. Em geral, nenhum deles é um componente de gameplay para o usuário configurar diretamente.

## Recursos ScriptableObject

| Recurso | Campos principais | Aparece como |
|---|---|---|
| `WaterSystemScriptableData` | todas as abas de água, mesh e render | `Settings`/`Profile` |
| `FlowingScriptableData` | `AreaSize`, `AreaPosition`, `FlowmapResolution`, `FlowmapTexture`, `FluidsMaskTexture`, `FluidsPrebakedTexture` | campo `Flowing Data` na aba Flowing |
| `ShorelineWavesScriptableData` | lista `Waves` com matriz, ID, posição, rotação, size, offset, scale e flip | campo de shoreline/salvamento |
| `SplineScriptableData` | lista de `Spline`; cada spline tem pontos, profundidade e vértices entre pontos | campo `Spline Data` na aba Mesh |

Esses recursos são dados serializáveis e não `MonoBehaviour`s. Podem aparecer no Project/Inspector e ser referenciados pelo `WaterSystem`.
