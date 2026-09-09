# KriptoFX WaterSystem — documentação técnica do pacote local

Esta pasta documenta `Assets/KriptoFX/WaterSystem/WaterResources`. A cópia analisada contém `README.txt` com a identificação `Version 1.4.03`, o assembly definition `KWS`, scripts C#, shaders, assets de recursos, importadores e `SimpleDemo.unity`.

## Mapa da documentação

| Documento | Conteúdo |
|---|---|
| [01-visao-geral-instalacao.md](01-visao-geral-instalacao.md) | estrutura, criação do sistema, instalação e dependências |
| [02-componentes-e-inspector.md](02-componentes-e-inspector.md) | componentes anexáveis, campos no Inspector e objetos internos |
| [03-aba-settings-completa.md](03-aba-settings-completa.md) | abas e campos do `WaterSystemScriptableData` |
| [04-flowing-fluides-rio-e-shoreline.md](04-flowing-fluides-rio-e-shoreline.md) | flowmap, fluid simulation, river spline e shoreline editor |
| [05-ondas-reflexao-subagua-e-render.md](05-ondas-reflexao-subagua-e-render.md) | FFT, dynamic waves, foam, caustics, underwater e reflexos |
| [06-api-runtime-e-scriptabledata.md](06-api-runtime-e-scriptabledata.md) | API pública, dados persistidos, callbacks e exemplos |
| [07-pipeline-assets-shaders-e-mobile.md](07-pipeline-assets-shaders-e-mobile.md) | render pipeline, shaders, importadores, qualidade e Android |
| [08-sequencias-diagnostico-e-limites.md](08-sequencias-diagnostico-e-limites.md) | fluxos completos, avisos, performance e limites comprovados |
| [09-inventario-de-classes-e-assets.md](09-inventario-de-classes-e-assets.md) | classificação dos componentes, ScriptableObjects, subsistemas, passes e editor |

## Componentes públicos encontrados

| Tipo | Base/atributos | Papel |
|---|---|---|
| `KWS.WaterSystem` | `MonoBehaviour`, `[ExecuteAlways]`, `[Serializable]` | superfície de água, settings, mesh e coordenação dos subsistemas |
| `KWS.KW_Buoyancy` | `MonoBehaviour`, `[RequireComponent(typeof(Rigidbody))]`, `[RequireComponent(typeof(Collider))]` | forças de flutuação em objeto físico |
| `KWS.KW_InteractWithWater` | `MonoBehaviour`, `[ExecuteInEditMode]` | informa interação/movimento ao sistema de ondas dinâmicas |
| `KWS.KWS_AddLightToWaterRendering` | `MonoBehaviour`, `[ExecuteAlways]`, `[RequireComponent(typeof(Light))]` | registra Light para iluminação volumétrica/sombras na água |
| `KWS.WaterPassHandler` | `MonoBehaviour`, `[ExecuteAlways]` | objeto auxiliar interno que executa passes de renderização |
| `KWS.UndoProvider` | `MonoBehaviour` | guarda listas serializadas de shoreline/splines para Undo do editor |
| `KWS.ComputeBufferObject` | `MonoBehaviour` | asset importado para alimentar `ComputeBuffer` |

Também há `WaterSystemScriptableData`, `FlowingScriptableData`, `ShorelineWavesScriptableData` e `SplineScriptableData` como `ScriptableObject`. FFT, flowmap, fluid simulation, shoreline, mesh/quadtree, reflexão e passes `WaterPass`/`WaterPassCore` aparecem como classes internas auxiliares; não são componentes que o usuário adiciona livremente à Hierarchy.

## Interface principal

O menu confirmado no código é `GameObject -> Effects -> Water System`. O objeto criado recebe `WaterSystem` e a layer `KWS_Settings.Water.WaterLayer`. O `KWS_Editor` é `[CustomEditor(typeof(WaterSystem))]` e desenha as abas `Color Settings`, `Waves`, `Reflection`, `Color Refraction`, `Flowing`, `Dynamic Waves`, `Shoreline`, `Foam(beta)`, `Volumetric Lighting`, `Caustic`, `Underwater`, `Mesh` e `Rendering`.

O Inspector não é um conjunto de campos nativos simples: ele possui abas expansíveis, perfis de performance, botões, mensagens e modos que desenham handles na Scene View. A documentação de cada aba marca o campo, a unidade/faixa que o código usa, o efeito e a condição de visibilidade.

## Classificação de dependências

- **Confirmado pelo código:** Unity `MonoBehaviour`, `Rigidbody`, `Collider`, `Light`, `Camera`, `Shader`, `ComputeShader`, `RenderTexture`, `Mesh`, `ScriptableObject`, `UnityEngine.Rendering` e o assembly `KWS`.
- **Declarado pelo README local:** Linear color space para a demo; Cinemachine e Post Processing para a demo; suporte opcional a fog de terceiros; shader de depth mask; API de buoyancy/underwater.
- **Não declarado como requisito universal:** Cinemachine/Post Processing para toda cena, NavMesh, CharacterController, Godot ou um pacote de física externo.
- **Atualidade:** listagens públicas do Asset Store podem ter versões posteriores à cópia local. Esta documentação descreve a cópia local v1.4.03; não substitui o manual da versão atual.
