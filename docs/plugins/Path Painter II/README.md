# Path Painter II — documentação técnica do pacote local

Esta pasta documenta o Path Painter II encontrado em `Assets/3D Haven/Path Painter II`. O plugin modifica dados de `UnityEngine.Terrain` para desenhar caminhos, estradas, rampas, leitos de rios/lago e áreas de jogo. A documentação foi montada a partir do manual PDF local, `Version Log.txt`, `PathPainter.dll`, `PathPainter.xml`, `ImpMgr.cs`, shaders, recursos e demos incluídos no projeto.

## Mapa da documentação

| Documento | Conteúdo |
|---|---|
| [01-visao-geral-instalacao.md](01-visao-geral-instalacao.md) | O que o plugin é, estrutura da pasta, instalação, build e dependências confirmadas |
| [02-interface-paint.md](02-interface-paint.md) | Janela, abas, botões, atalhos, Scene View e estado do Inspector |
| [03-pintura-terreno.md](03-pintura-terreno.md) | Shaping, Auto Ramp, texturas, vegetação, terrenos vizinhos e sequência de pintura |
| [04-modescaler-e-fluxos.md](04-modescaler-e-fluxos.md) | MapScaler, workflows do manual, exemplos e ordem correta das operações |
| [05-api-completa.md](05-api-completa.md) | Namespace, `Painter`, propriedades, overloads, `PaintOption`, bulk e line painting |
| [06-runtime-build-e-exemplos.md](06-runtime-build-e-exemplos.md) | API em runtime, inclusão no build, exemplos de código e limites de uso |
| [07-limitacoes-versoes-e-diagnostico.md](07-limitacoes-versoes-e-diagnostico.md) | Limitações, incompatibilidades declaradas, logs de versão e troubleshooting |
| [08-inventario-de-assets-e-camadas.md](08-inventario-de-assets-e-camadas.md) | DLLs, XML, recursos visuais, demos e dados de Terrain que o plugin lê |

## Componentes, assets e interfaces

| Elemento | Tipo confirmado | Aparece onde |
|---|---|---|
| Path Painter | janela de editor do plugin | `Window -> 3D Haven -> Path Painter -> Path Painter` |
| Paint / MapScaler / More... | abas da janela | janela principal |
| `Painter` | classe de API em `PathPainter.dll` | script/editor ou runtime quando incluído no build |
| `PaintOption` | tipo de opções da API | script |
| `Terrain` / `TerrainData` / `TerrainLayer` | componentes e assets nativos do Unity usados pelo plugin | Hierarchy, Inspector e Project |
| `PathPainter.dll` | assembly de engine/API | `Editor/Engine/000` e `Editor/Engine/010` |
| `PathPainterEditor.dll` | assembly de editor | `Editor/000` e `Editor/010` |
| `Path Painter Documentation.pdf` | manual | `Editor/Documentation` |
| `Version Log.txt` | histórico de versão | `Editor/Documentation` |
| shaders `3dhpp2_*` e imagens `3dhpp2_*` | recursos visuais do editor | `Editor/Resources` |
| `Demo Scene.unity` e `Terrains` | demonstração e TerrainData | `Editor/Demo` |

Não foi encontrado no material examinado um `MonoBehaviour` público do Path Painter para ser adicionado a um GameObject. Portanto, a interface principal não é um componente no Inspector: é uma ferramenta `EditorWindow`. A API `Painter` é consumida por código. Isso não impede que o usuário use os componentes nativos `Terrain`, `TerrainCollider` e `TerrainData` no Inspector.

## Como interpretar as afirmações

Os valores, botões e sequências descritos como “confirmados” vêm do manual/PDF, XML ou código local. Quando o manual v2.1.2 e o `Version Log` v2.1.5 divergem sobre a versão mínima do Unity, ambos são preservados e a divergência é explicada em [07](07-limitacoes-versoes-e-diagnostico.md). Nenhum componente da Godot é apresentado como requisito original deste plugin.
