# Path Painter II — shaping, ramps, texturas e vegetação

## Brush Settings

### Brush Size

É a largura real da superfície do caminho, em metros/unidades do mundo. O valor deve ser compatível com a resolução do Terrain: heightmap e alphamap/control texture com baixa resolução não representam detalhes finos, mesmo que o número digitado seja pequeno.

### Embankment Size

É a largura total do caminho incluindo o embankment, isto é, a área de transição até o terreno original. Deve ser ligeiramente maior que `Brush Size`. O manual informa que o valor é atualizado automaticamente quando o Brush Size muda e que o tamanho é limitado pelas resoluções `Heightmap Resolution` e `Control Texture Resolution` quando o texturing está ativo.

### Embankment Curves

A curva determina como o embankment sobe/desce ou mistura com o ambiente. O enum existe na API como `Painter.EmbankmentCurve`; o manual mostra formas como `Sharp`, `Smooth`, `Round`, `Square` e `Mound` na interface/fluxos. Os nomes exatos disponíveis devem ser lidos do dropdown da versão importada e da DLL, pois não há fonte C# pública no pacote que permita afirmar uma lista maior.

## Shaping

| Controle | Semântica declarada |
|---|---|
| Slope Limit | limite em graus usado pelo Auto Ramp; `0` produz caminho nivelado e `90` equivale a não impor limite |
| Elevation | elevação relativa; valor negativo pode cavar riverbed/lakebed |
| Terrain Follow | mistura entre seguir o relevo existente e criar uma rampa de inclinação uniforme entre início/fim |
| Auto Ramp | cria ramps onde a trajetória ultrapassaria a inclinação permitida |

O manual recomenda deixar o `Slope Limit` alguns graus abaixo do limite de inclinação do `FPSController` do jogo, quando essa é a intenção de gameplay. Isso é uma recomendação de integração, não uma dependência: o Path Painter não exige que um `FPSController` exista.

`Elevation` e o tamanho do embankment influenciam os limites efetivos: o terreno precisa ter altura disponível para acomodar a geometria. Ao criar um leito, use elevação negativa e verifique o resultado nas bordas.

### Auto Ramp e direção

O sentido em que os pontos são pintados influencia a direção da rampa. Antes de aplicar, decida se a operação deve elevar o caminho ou cavar o terreno. Para conectar montanhas/ilhas/vales/lago/rios, o manual recomenda explorar `Terrain Follow` e `Even Slope`/`evenRamp`.

## Textures

| Controle | Efeito |
|---|---|
| Surface Texture | TerrainLayer aplicada na superfície do caminho |
| Embankment Texture | TerrainLayer aplicada no entorno/embankment |
| Texture Strength/Opacity | força de aplicação entre `0` e `1`; pode servir para pintura parcial e mistura por altura em integrações como CTS |
| Smart Texture Paint | heurística de aplicação; desligue quando quiser que a textura do embankment seja aplicada sobre áreas já pintadas com a textura da superfície |
| Edit Terrain Layers | botão/atalho para abrir a área nativa de Terrain Layers; a edição da camada continua sendo feita pela ferramenta Paint Texture do Terrain |

Com `Texturing` desligado, o `Version Log` v2.1.1/2.1.0b4 registra correções para evitar pintura indevida em certas circunstâncias. Ainda assim, confirme visualmente o toggle antes de aplicar um stroke.

## Vegetation

### Grass/details

O recurso diferencia uma faixa de limpeza central e uma faixa de thinning gradual:

- `Grass Clearing Distance`: razão `0.01–1` da largura de um lado do embankment; details são removidos até essa distância;
- `Grass Thinning Distance`: razão `0.01–1`; a partir do limite de clearing até esse valor, os details são desbastados;
- `Grass Clearing Noise`: tipo de noise usado no thinning;
- toggle `clearGrass`/Vegetation Clearing: liga a operação.

### Árvores

- `Tree Clearing Distance`: razão `0.01–1` da largura de um lado do embankment;
- trees são removidas até essa distância;
- toggle `clearTree`/Vegetation Clearing controla a operação.

O código da API chama o tipo de ruído `Painter.Noise`. A documentação local não deve ser estendida com nomes de noises que não estejam no dropdown/XML da DLL.

## Terrains vizinhos

Para a pintura atravessar tiles:

1. use Terrains quadrados;
2. deixe Width e Length iguais;
3. mantenha compatíveis `Heightmap Resolution`, `Control Texture Resolution` e `Detail Resolution`;
4. posicione os tiles na grade, inclusive os que formam buracos no conjunto;
5. verifique a validação do Path Painter antes de aplicar.

O `Version Log` registra suporte/validação de Grouping IDs, aviso para tiles desviantes, correções em grupos incompatíveis e correções de offsets com heightmap/control texture mismatched. Portanto, “está encostado visualmente” não é suficiente: os parâmetros do grupo também precisam ser compatíveis.

## Texture Auto Propagation

O manual declara que o recurso adiciona automaticamente TerrainLayers aos terrenos onde elas não existem. Layers com configurações diferentes podem ser consideradas diferentes e acabar duplicadas. Depois de uma pintura multiterrain, revise as TerrainLayers no Inspector de cada tile para confirmar a ordem e evitar referências duplicadas.

## Sequência segura de uma pintura

1. Faça backup/commit da cena e TerrainData.
2. Escolha um Terrain representativo.
3. Defina Brush Size e depois Embankment Size.
4. Decida Shaping, Slope Limit, Elevation e Terrain Follow.
5. Configure surface/embankment texture e opacity.
6. Configure clearing de grass/árvores somente se necessário.
7. Faça um stroke curto em `Paint Mode`.
8. Confirme com `Apply Changes`.
9. Inspecione altura, layers e vegetação.
10. Só então aplique no grupo inteiro.
