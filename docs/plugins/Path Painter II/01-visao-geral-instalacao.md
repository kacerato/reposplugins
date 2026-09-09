# Path Painter II — visão geral, estrutura e instalação

## Função

O Path Painter trabalha sobre Unity Terrains. A operação pode combinar, na mesma pintura:

- alteração do heightmap para criar ou cortar o caminho;
- aplicação de uma TerrainLayer na superfície;
- aplicação de outra TerrainLayer no embankment/entorno;
- limpeza ou desbaste de grass/details;
- limpeza de árvores;
- Auto Ramp para limitar inclinações;
- pintura atravessando um grupo compatível de terrenos vizinhos.

O manual cita caminhos, estradas, ramps, riverbeds, lakebeds, play areas e, em alguns casos, montanhas. A classe de API recebe pontos em espaço mundial e escreve nos TerrainData alcançados pela trajetória.

## Estrutura local observada

| Caminho | Papel |
|---|---|
| `Editor/Documentation` | PDF do manual e histórico de versões |
| `Editor/Engine/000` | DLL/XML de uma variante de engine |
| `Editor/Engine/010` | DLL/XML de outra variante de engine |
| `Editor/000` e `Editor/010` | DLL do editor |
| `Editor/Resources` | shaders e imagens usados pela janela/visualização |
| `Editor/Demo` | cena, TerrainData e exemplos de uso |
| `Editor/ImpMgr.cs` | gerenciador de pós-importação/limpeza de versões antigas |

Os nomes `000` e `010` aparecem como variantes no pacote, mas o material local não fornece, em texto, uma tabela que mapeie cada variante a uma versão específica do Unity. Não se deve escolher uma DLL manualmente sem deixar o Unity/importador do pacote fazer a seleção correta.

## Instalação no editor

1. Importe o pacote no projeto Unity.
2. Aguarde a importação das DLLs e dos recursos.
3. Abra `Window -> 3D Haven -> Path Painter -> Path Painter`.
4. Se o menu não aparecer, aguarde o fim da importação/recompile ou reimporte o pacote; esse é o procedimento indicado no manual para menu ausente.
5. Selecione ou crie um `Terrain`.
6. Garanta que o Terrain tenha `TerrainData` válido e que as `TerrainLayer`s desejadas estejam configuradas no componente Terrain.

O Path Painter não substitui o `Terrain` nem cria, por si só, um sistema de terreno procedural. Ele pinta sobre o Terrain existente.

## Dependências confirmadas

### Unity

- `UnityEngine.Terrain` e `TerrainData` são o alvo da pintura.
- `TerrainLayer` é necessária quando se deseja texturizar; uma camada pode ser obtida pela posição `tile.terrainData.terrainLayers[index]` na API.
- o Terrain precisa ter resoluções suficientes para o detalhe desejado: o manual relaciona `Heightmap Resolution` e `Control Texture Resolution` aos limites de largura/precisão.
- para vizinhança, os Terrains precisam ser quadrados, possuir largura/comprimento e resoluções relevantes compatíveis e estar posicionados na grade.
- a edição é uma ferramenta de Editor; menus, Scene View, Undo e `TerrainData` são recursos editoriais.

### Pacote

- `PathPainter.dll` para a API/engine;
- `PathPainterEditor.dll` para a interface do editor;
- shaders e imagens da pasta `Editor/Resources`;
- DLL/XML correspondentes à variante importada.

Não foi declarado no manual que Cinemachine, Post Processing, NavMesh, CharacterController ou qualquer pacote externo sejam necessários. Eles podem consumir o resultado, mas não são requisitos confirmados do Path Painter.

## Build

O `Version Log` v2.1.2 declara que o pacote passou a usar uma estrutura otimizada para build e que, por padrão, nenhum trecho do Path Painter entra no build. Também declara uma janela `Path Painter II Build Settings` para incluir/excluir o plugin.

Use `Window -> 3D Haven -> Path Painter II -> Path Painter II Build Settings` quando a aplicação usar a API em runtime. Nesse caso, inclua a parte de runtime antes de criar o build. Se o plugin for usado somente para editar Terrain no Editor, a configuração padrão lean evita levá-lo para o jogador.

## Interface de propriedades

O usuário não seleciona um componente `PathPainter` no Inspector. A configuração de pintura fica na janela e é aplicada ao TerrainData. O Inspector do Terrain continua sendo o local de configuração das resoluções, TerrainLayers, details, árvores e `Draw Instanced`. A janela do plugin lê/usa esses dados; ela não cria um Inspector customizado de componente público no material analisado.
