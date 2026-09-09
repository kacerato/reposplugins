# Path Painter II — janela, botões e atalhos

## Abrir e abas

Abra `Window -> 3D Haven -> Path Painter -> Path Painter`. A janela possui as abas:

- `Paint`: controles da pintura;
- `MapScaler`: ferramenta auxiliar para redimensionar mapas do Terrain;
- `More...`: opções e links adicionais, inclusive documentação.

O manual também menciona `Help`, tooltips e uma área de informação que apresenta dicas/notícias. O status bar exibe atalhos, estado de `Paint Mode`/`Edit Mode` e funções disponíveis.

## Main Controls

| Controle | Ação | Quando produz efeito |
|---|---|---|
| Shaping | liga/desliga alteração do relevo | com o botão ligado, a superfície do caminho é moldada; desligado, o terreno não é elevado/cortado por essa opção |
| Texturing | liga/desliga a pintura de TerrainLayer | com ligado, as camadas selecionadas são pintadas; com desligado, o material do terreno não deve ser alterado pela pintura |
| Paint | entra/sai do modo de pintura; o manual associa o atalho `G` | precisa estar em `Paint Mode` para desenhar com o pincel |
| Vegetation Clearing | liga/desliga limpeza de vegetação | controla a remoção/desbaste de details e a limpeza de árvores conforme os controles de Vegetation |
| Draw Vegetation | alterna o desenho de vegetação da própria configuração do Terrain | é uma configuração do Terrain usada para aliviar a visualização/performance durante a pintura |
| Apply Changes | aplica em conjunto as alterações do stroke ativo | fica habilitado quando o stroke atual possui mudanças; é usado para confirmar o traço configurado |

Shaping, Texturing e Vegetation Clearing podem ser combinados em qualquer combinação. Uma sequência recomendada pelo manual é desligar `Draw Vegetation`, fazer várias pinturas e ligá-lo novamente depois.

## Paint Mode e Scene View

1. Ative `Paint` ou pressione `G`.
2. Mova o cursor sobre o Terrain para ver a visualização do brush.
3. O brush de superfície aparece em roxo e o embankment em verde, segundo o manual.
4. Arraste/click conforme o estado indicado no status bar.
5. Use `Apply Changes` para aplicar o stroke quando o botão estiver disponível.

O ajuste de Brush Size/Embankment Size pela Scene View exige Paint Mode, Scene View focada e brush visível. O status bar informa a tecla; o procedimento descrito é mover o mouse para ajustar, clicar com o botão esquerdo para aceitar e usar botão direito ou `Esc` para cancelar.

## Edit Mode

O atalho padrão indicado é `SHIFT`.

- permite editar o stroke ativo com feedback ao vivo;
- adiciona pontos ao stroke ativo, útil para Terrain Follow e ramps;
- cria linhas retas por cliques;
- se já existe um stroke ativo, continua nele em vez de começar outro;
- as mudanças de configuração são aplicadas imediatamente, sem depender de `Apply Changes`.

Ao soltar `SHIFT`, a ferramenta volta ao fluxo normal indicado na barra de status. A seleção do modo e o resultado visual ocorrem na Scene View, não no Inspector de um componente Path Painter.

## Help, Info e Status Bar

O manual afirma que clicar na caixa com `?` ou passar o cursor sobre uma configuração mostra descrição. A área `Info` informa dicas/tips/news. O status bar deve ser lido antes de usar um atalho, porque ele indica as teclas disponíveis para a versão/estado atual da janela.

## Estado salvo

O Path Painter lembra configurações por projeto. O identificador é o `Project Name` em `Project Settings -> Player -> Project Name`. Isso é persistência de configuração da ferramenta, não um asset de cena nem uma propriedade de `TerrainData`.

## O que aparece no Inspector

| Local | O que se edita |
|---|---|
| Inspector do Terrain | resoluções, TerrainData, layers, details, árvores e opções nativas do Unity |
| Inspector de TerrainLayer | textura, normal, mask, tile size/offset e demais dados da camada |
| janela Path Painter | brush, shaping, texturing, ramp, vegetação, modos e aplicação |
| Scene View | visualização do brush, embankment, handles e edição dos strokes |
| Project | DLLs/XML, manual, shaders, demo e TerrainData |

No pacote examinado, a interface principal não aparece como um componente customizado anexável na lista `Add Component`.
