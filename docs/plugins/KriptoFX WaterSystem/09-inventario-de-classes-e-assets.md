# WaterSystem — inventário de classes, assets e passes

Este inventário separa o que é componente de cena, recurso serializável, editor, subsistema e infraestrutura. O nome da classe é preservado exatamente como aparece nos scripts locais.

## Componentes anexáveis (`MonoBehaviour`)

| Classe | Arquivo | Interface/uso |
|---|---|---|
| `WaterSystem` | `Scripts/Core/WaterSystem.cs` | componente principal; `CustomEditor`; menu `GameObject/Effects/Water System` |
| `KW_Buoyancy` | `Scripts/Core/KW_Buoyancy.cs` | Rigidbody + Collider exigidos; Inspector de física/flutuação |
| `KW_InteractWithWater` | `Scripts/Core/KW_InteractWithWater.cs` | interação com ondas; `[ExecuteInEditMode]`; gizmo |
| `UndoProvider` | `Scripts/Core/UndoProvider.cs` | auxiliar serializado para Undo; normalmente criado internamente |
| `ComputeBufferObject` | `Scripts/Core/ScriptableDatas/ComputeBufferObject.cs` | criado pelo importador de `.kwsComputeBuffer` |
| `KWS_AddLightToWaterRendering` | `Scripts/Standard/KWS_AddLightToWaterRendering.cs` | `[RequireComponent(Light)]`; Inspector customizado |
| `WaterPassHandler` | `Scripts/Standard/RenderPass/WaterPassHandler.cs` | `[ExecuteAlways]`; criado em objeto temporário pelo backend |

## Recursos `ScriptableObject`

| Classe | Arquivo | Dados |
|---|---|---|
| `WaterSystemScriptableData` | `Scripts/Core/ScriptableDatas/WaterSystemScriptableData.cs` | settings de todas as abas |
| `FlowingScriptableData` | `Scripts/Core/ScriptableDatas/FlowingScriptableData.cs` | flowmap, masks e prebake |
| `ShorelineWavesScriptableData` | `Scripts/Core/ScriptableDatas/ShorelineWavesScriptableData.cs` | lista de ondas shoreline |
| `SplineScriptableData` | `Scripts/Core/ScriptableDatas/SplineScriptableData.cs` | splines e pontos de rios |

## Subsistemas de água

| Classe | Responsabilidade observada |
|---|---|
| `FFT_GPU` | espectro/ondas FFT por LOD |
| `KWS_FFT_ToHeightMap` | leitura/atualização de altura e `WaterSurfaceData` |
| `KW_FlowMap` | textura, desenho, save/load e resolução de flowmap |
| `KW_FluidsSimulation2D` | fluides 2D, bake e render |
| `KW_DynamicWaves` | ondas interativas e chuva |
| `KW_ShorelineWaves` | inicialização, buffers, LOD e save de shoreline |
| `KWS_SplineMesh` | geração/atualização da mesh de rio |
| `MeshQuadTree` | chunks, níveis, visibilidade e culling da mesh |
| `MeshUtils` | criação/processamento de meshes e collider auxiliar |
| `CubemapReflection` | captura/atualização de reflexão cubemap |
| `PlanarReflection` | implementação interna de reflexão planar |
| `ReflectionPass` | base do pass de reflexão |
| `KWS_WaterLights` | registro/coleta de luzes volumétricas e sombras |
| `KW_CustomFixedUpdate` | temporização fixa interna para simulações |
| `KW_WaterDynamicScripts` | registro global de buoyancy/interações |

## Core e dados auxiliares

`KWS_CoreUtils`, `KW_Extensions`, `KWS_ShaderConstants`, `SharedData`, `ReflectionUtils`, `KWS_PyramidBlur`, `KWS_SplineMesh`, `UndoProvider` e os tipos de SRP/RT handle (`KWS_RTHandle`, `KWS_RTHandles`, `KWS_RTHandleSystem`, `KWS_BufferedRTHandleSystem`, `KWS_SRP_CoreUtils`) fornecem operações comuns, IDs de shader, buffers, textura temporária, culling, reflection e compatibilidade de render target.

Essas classes são infraestrutura do assembly `KWS`, não componentes que o usuário precisa adicionar à cena. O fato de uma classe ser `public` não significa que ela tenha um campo no Inspector ou que seja uma unidade de composição de gameplay.

## Passes Standard

O diretório `Scripts/Standard/RenderPass` contém os passes:

`WaterPass`, `WaterPassHandler`, `OrthoDepthPass`, `ShorelineWavesPass`, `MaskDepthNormalPass`, `CausticPass`, `CopyColorPass`, `ReflectionFinalPass`, `ScreenSpaceReflectionPass`, `VolumetricLightingPass`, `DrawMeshPass`, `ShorelineFoamPass`, `ShorelineDrawFoamToScreenPass`, `UnderwaterPass` e `DrawToPosteffectsDepthPass`.

O diretório `Scripts/Core/CommandPass` contém as versões core correspondentes, como `CausticPassCore`, `DrawMeshPassCore`, `DrawToPosteffectsDepthPassCore`, `MaskDepthNormalPassCore`, `OrthoDepthPassCore`, `ReflectionFinalPassCore`, `ScreenSpaceReflectionPassCore`, `ShorelineDrawFoamToScreenPassCore`, `ShorelineFoamPassCore`, `ShorelineWavesPassCore`, `UnderwaterPassCore` e `VolumetricLightingPassCore`.

## Editor

| Classe | Interface |
|---|---|
| `KWS_Editor` | `CustomEditor(typeof(WaterSystem))`; abas, profiles, help e Scene View |
| `KWS_EditorFlowmap` | brush, área, direção, erase e handles de flowmap |
| `KWS_EditorShoreline` | add/delete wave, handles Move/Rotate/Scale |
| `KWS_EditorSplineMesh` | add/delete spline, pontos, Bezier, largura e bad vertices |
| `KWS_EditorCaustic` | área e edição de depth caustic |
| `KWS_EditorProfiles` | perfis de qualidade que escrevem valores em `Settings` |
| `KWS_EditorUtils` | tabs, toggles, sliders, ObjectFields, help boxes e mensagens |
| `KWS_EditorTextDescription` | descrições e tooltips |
| `CustomImporter.TextureImporter` | `.kwsTexture` |
| `CustomImporter.ComputeBufferImporter` | `.kwsComputeBuffer` |
| `KWS_AddLightToWaterRendering_Editor` | Inspector de Light volumétrica |
| `VideoTooltipWindow` | janela de tooltip em vídeo no editor |

## Arquivos e assets de suporte

- `WaterResources/README.txt`: manual inicial, versão local e exemplos de API;
- `WaterResources/SimpleDemo.unity`: cena de teste;
- `WaterResources/Resources`: shaders, texturas, compute buffers e dados salvos;
- `WaterResources/Resources/SavedData/WaterID`: flowmap/shoreline/spline associados ao ID;
- `WaterResources/Scripts/KWS_asmdef.asmdef`: assembly `KWS`;
- `WaterResources/Scripts/Standard`: backend e passes da cópia local;
- `WaterResources/Scripts/Core/Editor`: código do Inspector e ferramentas Scene View.

## Regra para futuras auditorias

Se um novo arquivo for adicionado ao plugin, classifique-o primeiro como `MonoBehaviour`, `ScriptableObject`, `Editor`, `pass`, `resource importer` ou `utility`. Só documente um novo campo como “Inspector” quando houver campo público/serializado ou desenho explícito no `KWS_Editor`; só documente como requisito quando houver `[RequireComponent]`, referência do asmdef, README ou uso obrigatório no fluxo.
