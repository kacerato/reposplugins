# Path Painter II — inventário de assets e camadas

## Assets de código/documentação

| Asset local | Tipo | Função |
|---|---|---|
| `Editor/Engine/000/PathPainter.dll` | assembly | implementação de engine/API para uma variante |
| `Editor/Engine/000/PathPainter.xml` | XML docs | documentação de `Painter`, overloads e opções |
| `Editor/Engine/010/PathPainter.dll` | assembly | implementação de engine/API para outra variante |
| `Editor/Engine/010/PathPainter.xml` | XML docs | mesma API documentada para a variante |
| `Editor/000/PathPainterEditor.dll` | assembly editor | ferramenta de editor da variante |
| `Editor/010/PathPainterEditor.dll` | assembly editor | ferramenta de editor da variante |
| `Editor/Documentation/Path Painter Documentation.pdf` | PDF | manual de uso, API e troubleshooting |
| `Editor/Documentation/Version Log.txt` | texto | mudanças e compatibilidade por versão |
| `Editor/ImpMgr.cs` | script editor | limpeza/renomeação/pós-importação; contém GUIDs de recursos antigos |

O `ImpMgr.cs` executa limpeza por GUID sob `InitializeOnLoadMethod` quando não está em `HAVEN_REL` e pode remover assets de versões antigas. Não é um componente de cena. O usuário não deve mover/renomear DLLs manualmente para contornar esse gerenciador.

## Recursos da interface

`Editor/Resources` contém shaders com prefixo `3dhpp2_` e imagens usadas por botões, headers, ícones, visualização do brush, curves, ajuda, edição de layers, vegetação e estado linear. Esses arquivos sustentam a janela do plugin, mas não são TerrainLayers de gameplay nem materials que o usuário precisa adicionar à cena.

## Demos e TerrainData

| Asset | Uso |
|---|---|
| `Editor/Demo/Demo Scene.unity` | cena de demonstração do manual |
| `Editor/Demo/Terrains/DocsTerrain/PP Documentations Terrain 1.asset` | TerrainData usado nos exemplos documentais |
| `Editor/Demo/Terrains/FFT-F/FTT-F Demo Terrain 0-1.asset` até `2-3.asset` | conjunto de tiles do demo FFT-F |

Os TerrainData de demo são exemplos de dados de Unity, não dependências obrigatórias de um projeto que já possui seus próprios Terrains. O plugin trabalha com `Terrain`/`TerrainData` do projeto consumidor.

## Camadas e dados que o plugin lê

| Dado Unity | Como o Path Painter usa |
|---|---|
| `Terrain` | alvo de seleção/pintura e fonte de altura/vegetação |
| `TerrainData` | recebe heightmap, alphamap/control texture, details e árvores alterados |
| `TerrainLayer` | surface/embankment texture; pode ser obtida por `terrainData.terrainLayers[index]` |
| Heightmap Resolution | limita precisão geométrica do shaping |
| Control Texture Resolution | limita detalhe/precisão do texturing |
| Detail Resolution | relaciona-se à limpeza/desbaste de grass/details e compatibilidade entre vizinhos |
| Terrain Grouping IDs | considerados pelo plugin ao formar grupos, conforme Version Log v2.1.6 |

## O que não é asset do Path Painter

O pacote não declara que suas imagens `3dhpp2_*` sejam texturas de superfície, que seus TerrainData de demo sejam obrigatórios, ou que o `PathPainter.dll` seja um componente adicionável na Hierarchy. O resultado final fica nos dados do Terrain e nas TerrainLayers do projeto.

## Regras de backup

Antes de MapScaler, pintura com shaping, propagação de layer, limpeza de vegetação ou operação em grupo:

1. salve a cena;
2. faça cópia/commit dos TerrainData;
3. faça um teste em um tile;
4. verifique Undo/resultado;
5. somente então use os demais tiles.
