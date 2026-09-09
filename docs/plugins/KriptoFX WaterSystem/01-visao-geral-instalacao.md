# WaterSystem — visão geral, estrutura e instalação

## O que o plugin monta

`WaterSystem` concentra uma superfície de água que pode usar ocean infinito, box finito, river spline ou mesh customizada. A classe inicializa recursos de ondas FFT, altura para buoyancy, flowmap, fluid simulation, shoreline, dynamic waves, cubemap/planar/SSR reflection, caustics, underwater, mesh/quadtree e passes de renderização.

O pacote local é a variante de Standard Rendering: há uma pasta `Standard`, classes `WaterPass`, `WaterPassHandler`, iluminação e integração com comandos/render targets. O asmdef `KWS_asmdef.asmdef` declara apenas `{ "name": "KWS" }` e não lista referências externas.

## Instalação mínima

1. Importe a pasta do plugin no projeto Unity.
2. Aguarde a compilação do assembly `KWS` e a importação dos recursos.
3. Na Hierarchy, clique com o botão direito em `Effects -> Water system`.
4. Se preferir, adicione `WaterSystem` a um GameObject; o caminho do menu é o fluxo recomendado pelo código.
5. Garanta que exista uma `Camera` válida e uma `Light`/skybox apropriada para avaliar a renderização.
6. Selecione o Water System e abra seu Inspector customizado.
7. Comece com um perfil de qualidade baixo/médio e ative os recursos adicionais um por vez.

O código do menu posiciona o objeto aproximadamente três unidades à frente da Scene View, define sua layer para `KWS_Settings.Water.WaterLayer`, registra Undo e seleciona o objeto.

## Demo local

O `README.txt` local pede, para a demo:

1. `Edit -> Project Settings -> Player -> Other Settings -> Color Space -> Linear`.
2. Instalação de `Cinemachine` e `Post Processing` pelo `Window -> Package Manager`.
3. Reinício do Unity depois da instalação de Post Processing.

O mesmo README diz que em Gamma pode ser necessário ajustar intensidade da luz e `Transparent`/`Turbidity` para obter aparência melhor. Essas recomendações são para a demo; o código do `KWS` não declara Cinemachine como `RequireComponent` de `WaterSystem`.

## Recursos e pastas relevantes

| Pasta/arquivo | Função observada |
|---|---|
| `Scripts/Core` | WaterSystem, lógica comum, FFT, flowmap, fluid simulation, shoreline, mesh, reflection e ScriptableData |
| `Scripts/Standard` | backend Standard, lights, shaders/constantes e render passes |
| `Scripts/Core/Editor` | custom Inspector, flowmap/shoreline/spline/caustic Scene View e importadores |
| `Resources/SavedData/WaterID` | dados salvos de flowmap/shoreline/river associados ao ID da água, segundo README |
| `SimpleDemo.unity` | cena de demonstração local |
| `KWS_asmdef.asmdef` | assembly `KWS` |

## Dependências Unity confirmadas

### Pelo código

- `WaterSystem : MonoBehaviour` com `[ExecuteAlways]`;
- `Rigidbody` e `Collider` exigidos por `KW_Buoyancy`;
- `Light` exigido por `KWS_AddLightToWaterRendering`;
- `Camera` para passes, underwater, reflexões e culling;
- `Mesh`, `Material`, `Shader`, `ComputeShader`, `RenderTexture`, `ComputeBuffer` e `UnityEngine.Rendering`;
- `ScriptableObject` para settings, flow data, shoreline data e spline data.

### Pelo README/demo

- Linear color space recomendado para a cena demo;
- Cinemachine e Post Processing recomendados para a demo, não declarados como dependências universais;
- fog de terceiros é opcional e selecionado na aba Rendering.

## Inspector e Scene View

Selecionar `WaterSystem` abre `KWS_Editor`, que sincroniza/edita `Settings` e pode salvar/carregar um `WaterSystemScriptableData` em `Profile`. Os modos `Flowmap Painter`, `Shoreline`, `Caustic` e `River Editor` desenham na Scene View e alteram dados que depois podem ser salvos como ScriptableData. O sistema não usa uma janela separada para cada feature; a maior parte é uma aba do Inspector com modo de edição contextual.
