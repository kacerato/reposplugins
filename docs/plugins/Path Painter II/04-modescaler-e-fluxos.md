# Path Painter II — MapScaler e fluxos do manual

## MapScaler

`MapScaler` é uma aba/utilitário do Path Painter para escalar mapas do Terrain para cima ou para baixo. O manual usa como motivação um terreno muito grande em que um caminho de 4 m em um mapa de 2 km com heightmap `513` e control texture `512` fica visualmente ruim: aumentar as resoluções permite representar o detalhe, ao custo de mais memória/processamento.

### Fluxo

1. Abra a janela do Path Painter.
2. Entre na aba `MapScaler`.
3. Leia as resoluções atuais mostradas.
4. Escolha uma resolução suportada pelo Unity para o tipo de mapa.
5. Execute a operação de escala indicada pela aba.
6. Aguarde o processamento.
7. Confirme no Inspector do Terrain/ TerrainData as novas resoluções e teste um caminho pequeno.

O manual declara que versões mais novas do Unity também oferecem mecanismos semelhantes de alteração de resolução. O MapScaler não permite resoluções que o Unity não suporta. Alterar resolução pode modificar dados grandes; faça backup antes.

## Workflow 1 — primeiro caminho

1. Abra Path Painter.
2. Ative `Paint Mode` com o botão ou `G`.
3. Clique/arraste no Terrain para criar uma trajetória.
4. Para um primeiro teste, o manual usa `Elevation = 6`.
5. Aplique com `Apply Changes`.
6. Use `Ctrl+Z` para testar o Undo do Unity.
7. Crie outro traço e revise o relevo.

Para aprendizado, o manual sugere `Slope Limit = 90` e Auto Ramp desligado/sem limitação, de modo que a primeira experiência não seja alterada por uma rampa automática.

## Workflow 2 — caminho com textura e embankment

Configuração exemplificada no manual:

- `Brush Size = 2`;
- `Embankment Size = 50–55`;
- curva de embankment `Sharp`;
- `Elevation = 8–9`;
- uma Grass TerrainLayer na superfície;
- edição posterior pelo `Edit Mode` (`SHIFT`) para ajustar Texture Strength e Embankment Curves.

O objetivo do exemplo é mostrar que a superfície estreita e a faixa de transição podem ter escalas muito diferentes. Não trate esses números como defaults obrigatórios.

## Workflow 3 — linha reta

1. Entre em `Edit Mode`/mantenha `SHIFT` conforme o status bar.
2. Clique no ponto inicial.
3. Clique no ponto final.
4. Revise o stroke reto criado.
5. Aplique/complete conforme o estado mostrado na janela.

## Workflow 4 — ramp com Terrain Follow

O exemplo do manual usa, entre outros valores, `Brush Size = 12`, `Elevation = 0` e `Slope Limit = 90`, depois ajusta `Terrain Follow`. A ideia é comparar a trajetória seguindo o terreno com uma inclinação uniforme. Em um projeto real, reduza o Slope Limit se a rampa precisar obedecer um controlador de personagem.

## Workflow 5 — adicionar pontos mantendo o mesmo path

Quando a câmera é girada e é necessário continuar um caminho:

1. entre em Edit Mode;
2. segure `SHIFT`;
3. continue clicando para adicionar pontos ao stroke ativo;
4. solte/complete conforme a interface.

Sem a continuação do stroke, a ferramenta pode iniciar uma nova linha, o que produz uma rampa/undo separado.

## Workflow 6 — conectar caminhos elevados ou leitos

O manual descreve uma estratégia para evitar um resultado inadequado ao cruzar caminhos:

1. crie os caminhos sem que se atravessem;
2. coloque `Elevation = 0`;
3. use `Even Slope`/`evenRamp` para a transição;
4. conecte os caminhos após as duas superfícies estarem estáveis;
5. revise a área de interseção e as texturas.

## Workflow 7 — embankment somente com textura

Para misturar textura sem remodelar o relevo:

1. desligue `Shaping`;
2. desligue `Vegetation Clearing` se não quiser alterar details/árvores;
3. deixe `Embankment Texture = None` se a intenção for apenas sobrepor a camada escolhida;
4. pinte sobre a área.

## Workflow 8 — vegetação

Durante a pintura, trees/grass podem ficar ocultos para melhorar a visualização. Depois, com `Draw Vegetation` ligado, a vegetação volta a ser desenhada. Use as distâncias e o noise para criar a faixa de transição; faça uma amostra pequena antes de limpar uma área grande.

## Ordem recomendada para produção

```text
resoluções do Terrain
        ↓
MapScaler, se o detalhe não for representável
        ↓
grupo/vizinhança compatível
        ↓
shaping + Auto Ramp/Terrain Follow
        ↓
texturas e embankment
        ↓
vegetação
        ↓
revisão no Inspector, Scene View e gameplay
```

Essa ordem separa alteração destrutiva do heightmap, pintura de layers e limpeza de vegetação. O plugin permite combinar operações, mas separar os testes torna o Undo e o diagnóstico mais legíveis.
