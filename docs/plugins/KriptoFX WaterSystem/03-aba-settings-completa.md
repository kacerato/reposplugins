# WaterSystem — abas e propriedades do Inspector

O `KWS_Editor` é um `CustomEditor` de `WaterSystem`. Ele mostra primeiro `Settings Profile`, os botões `Save to Profile` e `Load from Profile`, depois as abas abaixo. Algumas abas possuem toggle de ativação, botão de modo expert e perfil de performance. Os nomes desta página vêm de `WaterSystemScriptableData.cs` e dos métodos de desenho do Inspector.

## Perfil e sincronização

| Controle | Uso |
|---|---|
| `Settings Profile` | referencia `WaterSystemScriptableData` no Project |
| `Save to Profile` | salva os settings atuais no profile; se já houver profile, pede confirmação antes de sobrescrever |
| `Load from Profile` | clona o profile para `Settings`; pede confirmação porque substitui o estado atual |
| aviso de sincronização | aparece quando `Settings.CompareValues(Profile)` indica diferenças |
| `Water unique ID` | mostra o identificador usado para dados de flowmap/shoreline/spline salvos |

O Profile não é automaticamente o mesmo objeto que Settings: o código cria uma instância de settings e, ao carregar, instancia uma cópia do profile.

## Color Settings

| Campo | Default no ScriptableData | Efeito declarado pelo nome/descrição do editor |
|---|---:|---|
| `Transparent` | `5` | transparência/espessura visual da água; o editor usa slider `0.1–50` |
| `WaterColor` | azul claro | cor base da água |
| `Turbidity` | `0.25` | quantidade de turbidez; slider `0.05–1` |
| `TurbidityColor` | verde/azul | cor usada na turbidez |

Os dois Color fields abrem o color picker do Unity, portanto têm interface no Inspector. O README recomenda ajustar `Transparent` e `Turbidity` se a cena estiver em Gamma.

## Waves

| Campo | Default | Interface/uso |
|---|---:|---|
| `FFT_SimulationSize` | `Size_256` | enum `FFT_GPU.SizeSetting`; detalhamento das ondas |
| `WindSpeed` | `1.5` | slider `0.1–15`; influencia vento/altura das ondas |
| `WindRotation` | `0` | slider `0–360` graus |
| `WindTurbulence` | `0.5` | slider `0–1` |
| `TimeScale` | `1` | slider `0–?` no código de editor; controla a escala temporal usada por ondas/shoreline |

O código tem três instâncias FFT internas (`FFTComponentLod0/1/2`) e uma estrutura de leitura de altura. Isso não cria três componentes na Hierarchy: é estado interno do WaterSystem.

## Reflection

| Campo | Default/enum | O que controla |
|---|---|---|
| `ReflectionProfile` | `High` | preset de performance da reflexão |
| `UseScreenSpaceReflection` | `true` | liga SSR |
| `ScreenSpaceReflectionResolutionQuality` | `High` (`50%`) | resolução SSR relativa à tela |
| `UseScreenSpaceReflectionHolesFilling` | `true` | preenchimento de falhas do SSR |
| `ScreenSpaceBordersStretching` | `0.015` | estiramento das bordas do SSR |
| `UsePlanarReflection` | `false` | liga reflexão planar |
| `PlanarCullingMask` | `~0` | layers desenhadas na câmera planar |
| `PlanarReflectionResolutionQuality` | `Medium` (`368`) | resolução planar |
| `ReflectionClipPlaneOffset` | `0.0025` | offset do plano de clipping |
| `RenderPlanarShadows` | `false` | sombras na reflexão planar |
| `RenderPlanarVolumetricsAndFog` | `false` | volumetria/fog planar quando integração existir |
| `RenderPlanarClouds` | `false` | clouds na reflexão planar quando integração existir |
| `CubemapUpdateInterval` | `10` | intervalo de atualização do cubemap |
| `CubemapCullingMask` | valor de `KWS_Settings` | layers capturadas pelo cubemap |
| `CubemapCullingMaskWithIndoorSkylingReflectionFix` | `~0` sem layer da água | máscara alternativa para skylight indoor |
| `CubemapReflectionResolutionQuality` | `Medium` (`256`) | resolução cubemap |
| `FixCubemapIndoorSkylightReflection` | `false` | troca a máscara/comportamento para o caso indoor |
| `UseAnisotropicReflections` | `true` | reflexões anisotrópicas |
| `AnisotropicReflectionsHighQuality` | `false` | variante de qualidade |
| `AnisotropicReflectionsScale` | `0.75` | escala anisotrópica |
| `UseAnisotropicCubemapSkyForSSR` | `false` | usa sky anisotrópico no SSR |
| `ReflectSun` | `true` | reflexo do sol |
| `ReflectedSunCloudinessStrength` | `0.04` | força de cloudiness do sol refletido |
| `ReflectedSunStrength` | `1` | força do sol refletido |

Os enums de resolução declarados no código são: planar `Ultra=768`, `High=512`, `Medium=368`, `Low=256`, `VeryLow=128`; SSR `Ultra=75`, `High=50`, `Medium=35`, `Low=25`, `VeryLow=20`; cubemap `High=512`, `Medium=256`, `Low=128`.

## Color Refraction

| Campo | Default | Condição/uso |
|---|---:|---|
| `RefractionProfile` | `High` | preset |
| `RefractionMode` | `PhysicalAproximationIOR` | enum; modo físico aproximado ou `Simple` |
| `RefractionAproximatedDepth` | `2` | profundidade aproximada |
| `RefractionSimpleStrength` | `0.25` | mostrado quando mode é `Simple`; slider `0.02–1` |
| `UseRefractionDispersion` | `true` | liga dispersão |
| `RefractionDispersionStrength` | `0.35` | mostrado se dispersão ligada; editor usa `0.25–1` |

## Flowing

| Campo | Default | Uso |
|---|---:|---|
| `FlowmapProfile` | `High` | preset |
| `UseFlowMap` | `false` | ativa Flowmap Painter/flowing |
| `FlowMapAreaPosition` | `(0,0,0)` | origem/posição da área; no editor Y é ajustado para o Y da água |
| `FlowMapAreaSize` | `200` | tamanho da área; editor usa `10–16000` |
| `FlowMapTextureResolution` | `_2048` | enum `512/1024/2048/4096` |
| `FlowMapSpeed` | `1` | velocidade do fluxo; editor usa `0.1–5` |
| `UseFluidsSimulation` | `false` | liga fluido 2D adicional |
| `FluidsAreaSize` | `40` | editor usa `10–80` |
| `FluidsSimulationIterrations` | `2` | expert; editor usa `1–3` |
| `FluidsTextureSize` | `1024` | expert; editor usa `368–2048` |
| `FluidsSimulationFPS` | `60` | frequência de simulação |
| `FluidsSpeed` | `1` | velocidade do fluido |
| `FluidsFoamStrength` | `0.5` | força da espuma do fluido |
| `FlowingScriptableData` | nulo | referência a flowmap/máscaras/prebake salvos |

A aba oferece `Flowmap Painter`, `Load Latest Saved`, `Delete All`, `Save All` e `Bake Fluids Obstacles`. Ver [04](04-flowing-fluides-rio-e-shoreline.md).

## Dynamic Waves

| Campo | Default | Uso |
|---|---:|---|
| `DynamicWavesProfile` | `High` | preset |
| `UseDynamicWaves` | `false` | liga ondas por interação |
| `DynamicWavesAreaSize` | `25` | área da simulação |
| `DynamicWavesSimulationFPS` | `60` | frequência |
| `DynamicWavesResolutionPerMeter` | `40` | resolução por metro |
| `DynamicWavesPropagationSpeed` | `1` | propagação |
| `UseDynamicWavesRainEffect` | `false` | chuva/impacto automático |
| `DynamicWavesRainStrength` | `0.2` | força da chuva |

O recurso recebe objetos registrados por `KW_InteractWithWater`; o componente e seus campos estão em [02](02-componentes-e-inspector.md).

## Shoreline

| Campo | Default | Uso |
|---|---:|---|
| `ShorelineProfile` | `High` | preset |
| `UseShorelineRendering` | `false` | liga shoreline |
| `ShorelineColor` | cinza com alpha | cor |
| `ShorelineFoamLodQuality` | `High` | enum `High/Medium/Low/VeryLow` |
| `UseShorelineFoamFastMode` | `false` | caminho rápido; o código desativa se atomics não forem suportados |
| `ShorelineFoamReceiveDirShadows` | `true` | recebe sombras direcionais |
| `ShorelineWavesScriptableData` | nulo | dados salvos de ondas |

O modo `Edit mode` abre handles de posição/rotação/escala na Scene View. `Add Wave`, `Insert`, `Delete` e `Save All` aparecem conforme o modo.

## Foam(beta)

| Campo | Default | Uso |
|---|---:|---|
| `FoamProfile` | `High` | preset |
| `UseFoamRendering` | `false` | liga espuma geral |
| `FoamColor` | cinza com alpha | cor |
| `FoamFadeDistance` | `1.5` | distância de fade |
| `FoamSize` | `20` | escala da espuma |

## Volumetric Lighting

| Campo | Default | Uso |
|---|---:|---|
| `VolumetricLightProfile` | `High` | preset |
| `UseVolumetricLight` | `true` | liga iluminação volumétrica |
| `VolumetricLightResolutionQuality` | `High` (`50%`) | resolução relativa |
| `VolumetricLightIteration` | `6` | iterações/ray-march |
| `VolumetricLightBlurRadius` | `2` | blur |
| `VolumetricLightFilter` | `Bilateral` | enum `Bilateral/Gaussian` |

Para uma Light participar com o script de extensão, use `KWS_AddLightToWaterRendering`, que exige `Light` e possui Inspector próprio.

## Caustic

| Campo | Default | Uso |
|---|---:|---|
| `CausticProfile` | `High` | preset |
| `UseCausticEffect` | `true` | liga caustic |
| `UseCausticBicubicInterpolation` | `true` | interpolação bicúbica |
| `UseCausticDispersion` | `true` | dispersão |
| `CausticTextureSize` | `768` | textura |
| `CausticMeshResolution` | `320` | mesh |
| `CausticActiveLods` | `3` | LODs ativos |
| `CausticStrength` | `1` | força |
| `UseDepthCausticScale` | `false` | usa escala pela profundidade |
| `CausticDepthScale` | `1` | escala |
| `CausticOrthoDepthPosition` | infinito | posição do depth area |
| `CausticOrthoDepthAreaSize` | `512` | área |
| `CausticOrthoDepthTextureResolution` | `2048` | resolução |

O botão `Bake Caustic Depth` aparece no editor de Caustic. O modo `Caustic Depth Scale` desenha área/handles na Scene View.

## Underwater

| Campo | Default | Uso |
|---|---:|---|
| `UnderwaterProfile` | `High` | preset |
| `UseUnderwaterEffect` | `true` | liga efeito |
| `UseUnderwaterBlur` | `false` | liga blur |
| `UnderwaterBlurRadius` | `2.6` | raio; editor usa `0.1–5` |
| `UnderwaterQueue` | `AfterTransparent` | enum `BeforeTransparent/AfterTransparent` |

## Mesh

| Campo | Default/enum | Uso |
|---|---|---|
| `MeshProfile` | `High` | preset |
| `WaterMeshType` | enum | `InfiniteOcean`, `FiniteBox`, `River`, `CustomMesh` |
| `OceanDetailingFarDistance` | `5000` | distância de detalhamento do oceano; editor usa `1000–10000` |
| `RiverSplineNormalOffset` | `1` | offset dos pontos do rio; editor usa `0.1–10` |
| `RiverSplineVertexCountBetweenPoints` | `20` | vértices entre pontos |
| `RiverSplineDepth` | `10` | profundidade da spline |
| `CustomMesh` | nulo | Mesh usada em `CustomMesh` |
| `WaterMeshQualityInfinite` | `High` | qualidade do oceano infinito |
| `WaterMeshQualityFinite` | `High` | qualidade do mesh finito |
| `MeshSize` | `(10,10,10)` | dimensões do box/custom mesh |
| `UseTesselation` | `false` | tessellation; código exige shader level `>=46` e não estar editando spline |
| `TesselationFactor` | `0.6` | fator |
| `TesselationInfiniteMeshMaxDistance` | `2000` | distância máxima infinita |
| `TesselationOtherMeshMaxDistance` | `100` | distância máxima demais meshes |
| `SplineScriptableData` | nulo | dados salvos do rio |

Quando `WaterMeshType = River`, o editor mostra o botão/estado `River Editor`, `Add River`, `Delete Selected River`, `Save Changes`, `Spline Data`, além de controles da spline. Quando `CustomMesh`, o `ObjectField` de Mesh é relevante.

## Rendering

| Campo | Default | Uso |
|---|---:|---|
| `RenderingProfile` | `High` | preset |
| `EnabledMeshRendering` | `true` | habilita desenho da mesh |
| `UseFiltering` | `true` | filtragem |
| `UseAnisotropicFiltering` | `false` | normais anisotrópicas |
| `DrawToPosteffectsDepth` | false | desenha a profundidade para pós-efeitos; pode ficar bloqueado se fog de terceiro já faz isso |
| `WireframeMode` | false | campo interno/experimental; o trecho de editor está comentado na cópia local |

A aba também possui `Third-Party Fog Support`, um popup global que tenta localizar include/asset do fog selecionado e altera defines de shader. Ver [07](07-pipeline-assets-shaders-e-mobile.md).
