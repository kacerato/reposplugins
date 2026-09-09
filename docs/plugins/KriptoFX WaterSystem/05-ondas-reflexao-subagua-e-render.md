# WaterSystem — ondas, espuma, caustics, underwater e passes

## FFT waves

O WaterSystem inicializa três objetos `FFT_GPU` (`Lod0`, `Lod1`, `Lod2`) e um `KWS_FFT_ToHeightMap`. `FFT_SimulationSize` escolhe o detalhamento do espectro; `WindSpeed`, `WindRotation`, `WindTurbulence` e `TimeScale` alimentam os parâmetros globais dos shaders. O resultado é usado tanto para renderização quanto para a consulta assíncrona de altura.

`GetWaterSurfaceData` pode retornar `IsActualDataReady = false` durante a inicialização/atualização. O código devolve posição de fallback e normal `Vector3.up` quando a leitura ainda não terminou.

## Dynamic Waves

`KW_DynamicWaves` é um subsistema interno, não um component. Ele mantém render textures e áreas de simulação. O `WaterSystemScriptableData` controla:

- `UseDynamicWaves`;
- área, FPS e resolução por metro;
- velocidade de propagação;
- chuva e força da chuva.

`KW_InteractWithWater` registra objetos móveis. A força é calculada pela distância percorrida entre frames, limitada e ajustada por `Strength`, `Pressure` e `Size`. O código não move o objeto nem substitui o Rigidbody.

## Foam e shoreline foam

Há dois níveis de espuma no settings:

- `UseFoamRendering`, `FoamColor`, `FoamFadeDistance`, `FoamSize` para foam geral;
- `UseShorelineRendering`, `ShorelineColor`, `ShorelineFoamLodQuality`, `UseShorelineFoamFastMode`, `ShorelineFoamReceiveDirShadows` para shoreline.

No Standard backend, os passes `ShorelineFoamPass` e `ShorelineDrawFoamToScreenPass` desenham a espuma em momentos diferentes. Isso é implementação interna; não há botões individuais para adicionar esses passes.

## Refraction

`RefractionModeEnum` declara `Simple` e `PhysicalAproximationIOR`. No modo Simple, o editor exibe `Strength`; em ambos pode haver dispersão com `UseRefractionDispersion`/`RefractionDispersionStrength`. A cena precisa de geometria/depth adequados para o resultado; o pacote não declara um componente extra de refração.

## Reflexões

O sistema possui caminhos para:

- Screen Space Reflection;
- Planar Reflection;
- Cubemap Reflection;
- reflexo anisotrópico;
- sol refletido;
- preenchimento de buracos/estiramento de bordas.

O renderer escolhe resoluções por enums e máscaras de culling. `UsePlanarReflection` cria/usa `PlanarReflection` interno; `UseScreenSpaceReflection` usa o pass SSR; cubemap tem intervalo de atualização. O componente de água não exige uma `Reflection Probe` como componente Unity no código analisado.

## Underwater

`UseUnderwaterEffect` liga o pass `UnderwaterPass`; `UseUnderwaterBlur`, `UnderwaterBlurRadius` e `UnderwaterQueue` controlam o tratamento. `WaterSystem.IsCameraUnderwater` é uma propriedade de leitura atualizada pelo sistema. A API também declara `IsPositionUnderWater` e `IsSphereUnderWater` para gameplay.

O teste de câmera é uma aproximação e o comentário do código avisa que ondas altas podem causar falso positivo. O teste de posição percorre instâncias visíveis e verifica `WorldSpaceBounds` antes de consultar a superfície; fora das bounds retorna false.

## Caustics

`CausticPass`/`CausticPassCore` usa textura, mesh, LOD, dispersão, bicubic e depth. O editor expõe `Bake Caustic Depth` e a área ortográfica. A qualidade tem custo de GPU/VRAM proporcional à textura e à resolução; não existe no código uma promessa de custo fixo.

## Mesh e quadtree

Os quatro modos de mesh são:

| Modo | Uso confirmado |
|---|---|
| `InfiniteOcean` | oceano que acompanha a posição da câmera; bounds calculadas muito grandes |
| `FiniteBox` | volume com `MeshSize` |
| `River` | mesh gerada por `SplineScriptableData` |
| `CustomMesh` | usa um `Mesh` selecionado no ObjectField |

O sistema usa `MeshQuadTree` para chunks/LOD/culling. `Debug Quadtree` é um toggle do Inspector para visualizar/diagnosticar o quadtree. Tessellation só é habilitada se `UseTesselation` estiver ligado, o shader level for `>=46` e o editor não estiver em Spline mode.

## Ordem dos passes Standard

`WaterPassHandler` registra callbacks de câmera e, quando a água está visível, executa uma sequência interna que inclui:

```text
OrthoDepth
ShorelineWaves
MaskDepthNormal
Caustic
CopyColor
SSR
ReflectionFinal
VolumetricLighting
ShorelineFoam
DrawMesh
ShorelineDrawFoamToScreen
Underwater
DrawToPosteffectsDepth
```

Nem todos são executados a cada instância/frame da mesma maneira: o código faz culling e limita alguns recursos por instância visível. Essa lista é arquitetura do backend, não uma ordem que o projeto deve reproduzir manualmente.

## Debug visual

- `Debug Quadtree` mostra o particionamento da mesh;
- Scene View Flowmap mostra área, brush e direção;
- Shoreline mostra caixas e handles;
- River mostra Bezier, pontos e cubos vermelhos para bad vertices;
- `KW_Buoyancy.DebugForces` desenha forças;
- `KW_InteractWithWater` desenha esfera gizmo amarela.
