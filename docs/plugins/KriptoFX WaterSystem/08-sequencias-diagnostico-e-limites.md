# WaterSystem — sequências, diagnóstico e limites

## Fluxo mínimo de água

```text
importar KWS
      ↓
GameObject -> Effects -> Water System
      ↓
Camera + Light + color space
      ↓
selecionar mesh mode
      ↓
ajustar Color/Waves/Mesh
      ↓
Play Mode e Console
```

1. Crie o Water System.
2. Verifique se `Settings` foi criado.
3. Selecione `InfiniteOcean` para o teste inicial ou o modo apropriado.
4. Ajuste cor/transparência e ondas.
5. Confirme que a Camera renderiza a água.
6. Só depois ative reflexões, underwater e efeitos caros.

## Fluxo de rio

1. Selecione `Mesh -> River`.
2. Ative `River Editor`.
3. Clique `Add River`.
4. Adicione no mínimo três pontos.
5. Mantenha distância entre pontos suficiente e evite curvatura brusca.
6. Ajuste largura no Scale Tool/`R`.
7. Ajuste depth/vertices se necessário.
8. Salve `Save Changes`.
9. Verifique `Spline Data` e os cubos vermelhos de bad vertices.

## Fluxo de água corrente

1. Ligue `UseFlowMap`.
2. Entre em `Flowmap Painter`.
3. Defina área/posição/resolução.
4. Desenhe com botão esquerdo.
5. Apague com `Ctrl` + botão esquerdo.
6. Salve `Save All`.
7. Se usar fluides, ligue `UseFluidsSimulation`.
8. Pressione `Bake Fluids Obstacles` e aguarde o percentual.

Se o bake avisar que não há flowmap, volte ao passo 5/6; a ferramenta exige dados salvos.

## Fluxo de shoreline

1. Ligue `UseShorelineRendering`.
2. Entre em `Edit mode`.
3. Use `Add Wave` ou `Insert`.
4. Posicione/rotacione/escale com os handles.
5. Remova selecionada com `Delete`.
6. Salve `Save All`.

## Fluxo de buoyancy

1. Crie um objeto com `Rigidbody`.
2. Garanta um `Collider`.
3. Adicione `KW_Buoyancy`; os `RequireComponent` também podem criar os componentes ausentes.
4. Escolha `VolumeSource` e ajuste Density/slices/voxels.
5. Entre no Play Mode.
6. Espere os dados de superfície ficarem prontos.
7. Ative `DebugForces` apenas para diagnóstico.

## Fluxo de interação/dynamic waves

1. Ligue `UseDynamicWaves`.
2. Adicione `KW_InteractWithWater` a um objeto que se mova.
3. Ajuste `Size`, `Strength`, `Pressure` e `Offset`.
4. Mova o objeto através da superfície.
5. Observe as ondas e o gizmo amarelo.

## Sintomas e causas prováveis

### Nada aparece

Confira GameObject ativo, `WaterSystem.enabled`, Camera, layer da água, `EnabledMeshRendering`, mesh mode, shaders importados e erros no Console. O Inspector desabilita a GUI quando o objeto está inativo/desabilitado ou `IsEditorAllowed()` retorna falso.

### A água aparece rosa/erro de shader

Confirme que esta cópia é Standard Rendering e que os shaders/recursos `WaterResources` foram importados. Não misture arquivos de uma listagem URP/HDRP diferente. Teste no pipeline exato do projeto.

### Demo com aparência ruim

Use Linear color space conforme README. Em Gamma, ajuste intensidade da luz, `Transparent` e `Turbidity`.

### Flowmap não salva/carrega

Confira `FlowMapAreaPosition`, `FlowMapAreaSize`, o `Water unique ID`, a pasta `Resources/SavedData/WaterID` e se o `FlowingScriptableData` foi atribuído. Não apague a pasta de dados.

### Fluids não reage a objetos móveis

O README declara que o bake calcula fluxo em torno de objetos estáticos. Para ondas de objetos móveis, use `KW_InteractWithWater`/Dynamic Waves, não espere que o bake de obstacles siga o objeto.

### River apresenta artefatos

Use pelo menos três pontos, aumente a distância entre pontos, evite curvas fortes, reduza largura/escala se necessário e consulte os cubos vermelhos de `EditorBadVertices`. Salve somente depois de a spline estar estável.

### Buoyancy não funciona

Confirme Rigidbody + Collider, água visível/ativa, posição dentro de `WorldSpaceBounds`, settings inicializados e `IsActualDataReady`. A API de superfície pode entregar fallback por alguns frames.

### Underwater entra/sai incorretamente

`IsCameraUnderwater` é uma aproximação, especialmente com ondas altas. Para gameplay de personagem, use a consulta de posição/sphere e uma margem/histerese no script do jogo.

### Luz volumétrica não influencia

Adicione `KWS_AddLightToWaterRendering` ao mesmo objeto da Light, confira o modo `VolumetricLightRenderingMode`, sombras e se a feature `UseVolumetricLight` está ativa. O componente exige Light e o modo `ShadowAndLight` só mostra downsample em certos tipos.

## Limites e não-promessas

- o pacote local não declara suporte a Godot;
- não há componente nativo de NavMesh/character/vehicle;
- Cinemachine/Post Processing são recomendados para a demo, não requisitos universais;
- a cópia local não fornece uma matriz completa de versões Unity/pipelines;
- `WaterSystem` não é um sistema de mundo/streaming/savegame;
- dados de rede, sincronização de relógio e gameplay precisam ser implementados pelo projeto;
- a existência de `ComputeShader`/tessellation exige validação de GPU;
- o material local não garante que todo efeito funcione em todos os targets.

## Auditoria de Inspector

| Item | Interface confirmada |
|---|---|
| `WaterSystem` | `KWS_Editor` no Inspector; abas, profiles, ObjectFields, sliders, enums, botões e help boxes |
| Flowmap | botão/aba no Inspector + brush/área na Scene View |
| Shoreline | botão/aba no Inspector + handles na Scene View |
| River | aba Mesh + handles Bezier/Move/Scale na Scene View |
| Caustic | aba + área/handles e `Bake Caustic Depth` |
| `KW_Buoyancy` | Inspector Unity com campos públicos; `DebugForces` produz debug visual |
| `KW_InteractWithWater` | Inspector com Size/Strength/Pressure/Offset; gizmo amarelo |
| `KWS_AddLightToWaterRendering` | custom Inspector com Volumetric Mode e Shadow Downsample condicional |
| `WaterPassHandler` | componente auxiliar interno, sem fluxo de configuração manual documentado |
| ScriptableData | Project/Inspector como assets referenciáveis |

O princípio é importante para a engine: interface de água, dados persistidos e subsistemas internos são camadas diferentes. Não trate cada classe `KW_*` encontrada nos scripts como um botão ou como um componente público disponível no `Add Component`.
