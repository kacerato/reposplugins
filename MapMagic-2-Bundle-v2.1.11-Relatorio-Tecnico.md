# MapMagic 2 Bundle v2.1.11 — relatório técnico completo

## Escopo e conclusão rápida

Este relatório documenta o conteúdo do pacote local `MapMagic 2 Bundle v2.1.11.unitypackage`, a documentação oficial, o código-fonte presente no pacote e o repositório oficial do MapMagic no GitLab.

Conclusão: o MapMagic 2 v2.1.11 é um gerador procedural de mundos baseado em grafos. Ele usa mapas matriciais para produzir altura, texturas e detalhes de Terrain; pode distribuir prefabs, árvores e splines; possui biomas, funções reutilizáveis, locks, preview/drafts, geração infinita por tiles e brush destrutiva/editável. A base obrigatória é o próprio MapMagic + Den.Tools + APIs de Terrain/Editor da Unity. URP, HDRP, CTS, MegaSplat, MicroSplat, RTP e Vegetation Studio Pro são integrações opcionais.

O bundle não é um projeto Unity completo: ele não contém `ProjectSettings`, `Packages/manifest.json` ou cenas de usuário. Portanto, ele não escolhe sozinho a versão do Unity, pipeline, renderizador, Input System, física ou pacote de navegação. Ele adiciona código e assets a um projeto Unity existente.

### Bases analisadas

| Base | O que representa | Tratamento |
|---|---|---|
| Bundle local v2.1.11 | Código, manuais, demos, presets, shaders, prefabs e meta-arquivos entregues no pacote | Fonte principal para a versão pedida |
| Wiki oficial | Explicação operacional dos nós, janelas, opções e tutoriais | Fonte principal de comportamento/documentação |
| Repositório oficial GitLab | Linha de desenvolvimento pública; atualmente contém código mais novo | Usado para comparação, não para inventar recursos do v2.1.11 |
| Asset Store | Página comercial e compatibilidade publicada atualmente | Indicada separadamente, pois a página hoje anuncia versão mais nova |
| Vídeo fornecido | Demonstração de uso e contexto visual | Fonte secundária; não substitui código/manual |

### Diferença de versão que precisa ser respeitada

O arquivo local confirma `MapMagicObject.version = 2.1.11`. O repositório oficial consultado no GitLab está no `master` com `MapMagicObject.version = 3.0.0`. A página atual da Asset Store anuncia uma versão ainda mais nova. Assim, exemplos, nomes ou dependências encontrados no GitLab atual não devem ser copiados automaticamente para o bundle v2.1.11.

## Fontes

- [Página do MapMagic 2 Bundle na Unity Asset Store](https://assetstore.unity.com/packages/tools/terrain/mapmagic-2-bundle-178682)
- [Repositório oficial do MapMagic no GitLab](https://gitlab.com/denispahunov/mapmagic)
- [Wiki oficial do MapMagic 2](https://gitlab.com/denispahunov/mapmagic/-/wikis/home)
- [Repositório da wiki oficial](https://gitlab.com/denispahunov/mapmagic.wiki)
- [Vídeo fornecido — Generate Infinite Procedural Terrains in Unity for Free | MapMagic 2](https://www.youtube.com/watch?v=e2HvAGPVRpc)
- [Unity Terrain](https://docs.unity3d.com/Manual/class-Terrain.html)
- [Unity TerrainData.SetHeightsDelayLOD](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/TerrainData.SetHeightsDelayLOD.html)
- [Unity Terrain Layer](https://docs.unity3d.com/Manual/class-TerrainLayer.html)
- [Unity TextureImporter.isReadable](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/TextureImporter-isReadable.html)

## 1. O que existe no pacote

### Inventário físico do bundle

O pacote foi reconstituído do formato interno de Unity Package e contém aproximadamente:

- 1.383 entradas `.meta`;
- 225 arquivos C#;
- 685 imagens PNG;
- 76 assets Unity `.asset`;
- 36 TIFF;
- 35 referências de assembly `.asmref`;
- 28 prefabs;
- 26 modelos FBX;
- 26 shaders;
- 15 cenas Unity `.unity`;
- 15 Terrain Layers;
- 14 materiais;
- 11 mapas RAW;
- 9 assembly definitions `.asmdef`;
- 5 manuais PDF;
- 3 arquivos `.lighting`;
- 3 includes `.cginc`;
- 2 DLLs nativas/compiladas;
- arquivos auxiliares de projeto e diagnóstico.

O diretório `Assets/MapMagic` tem aproximadamente 2.603 arquivos no inventário expandido, incluindo meta-arquivos, e cerca de 428 MB de conteúdo extraído. O tamanho físico do `.unitypackage` é aproximadamente 273 MB.

### Estrutura de diretórios

| Diretório | Papel |
|---|---|
| `Brush` | Brush de edição de terreno, presets, graphs de pintura e nós especiais de entrada/saída |
| `Compatibility` | Adaptadores opcionais para CTS, MegaSplat, MicroSplat, RTP e Vegetation Studio Pro |
| `Core` | `MapMagicObject`, geração, settings, editor, inspector, lifecycle e integração com Terrain |
| `Demo` | Cenas, graphs, assets e exemplos para estudar o fluxo |
| `Expose` | Dados e infraestrutura de valores expostos/funções |
| `Generators` | Implementação dos geradores de mapas, objetos, splines, biomas, funções e outputs |
| `Locks` | Dados e suporte de áreas travadas |
| `Nodes` | Grafo, portas, portais, serialização e editor de nós |
| `Popup` | Menus contextuais e seleção de geradores |
| `Preview` | Preview de mapas, terreno draft, shaders e visualização |
| `Products` | Integrações/saídas de produto |
| `Terrains` | Criação, atualização, aplicação e manutenção dos Terrain tiles |
| `Tools` | Den.Tools: matrizes, splines, threads, GUI, serialização, gizmos, logging e utilitários |

### Manuais incluídos

| Arquivo | Páginas | Conteúdo |
|---|---:|---|
| `Manual.pdf` | 48 | Instalação, graph, nós de mapa, outputs, settings, locks e fluxo geral |
| `ObjectsManual.pdf` | 22 | Distribuição de objetos, árvores, posicionamento e modificadores |
| `BrushManual.pdf` | 23 | Brush, presets, resolução e pintura em Terrain |
| `BiomesManual.pdf` | 6 | Biomes Set, Whittaker, funções e composição |
| `SplinesManual.pdf` | 9 | Splines, interlink, pathfinding, relax, stroke e output |

## 2. Dependências Unity: obrigatórias, condicionais e dispensáveis

### Obrigatórias para o núcleo

| Dependência | Necessidade | Evidência/uso |
|---|---|---|
| Unity Editor | Obrigatória para criar graphs, usar inspector, menus e janela visual | O editor está em assemblies `*.Editor` e usa `UnityEditor` |
| UnityEngine | Obrigatória | `MonoBehaviour`, `ScriptableObject`, `Terrain`, `TerrainData`, `Texture2D`, `GameObject`, `Prefab`, `Shader`, `Material`, `AnimationCurve`, gizmos |
| Unity Terrain/TerrainData | Obrigatória para gerar o mundo padrão | O núcleo cria e atualiza Terrain tiles e TerrainData |
| API Compatibility Level `.NET 4.x` | Requisito documentado para a versão 2 | A instalação oficial do v2 exige `.NET 4.x API Compatibility Level` |
| Den.Tools | Obrigatória e incluída no bundle | Assembly `Den.Tools`; contém matrizes, splines, threads, GUI e utilitários |
| MapMagic runtime | Obrigatória e incluída no bundle | Assembly `MapMagic` referencia `Den.Tools` |
| MapMagic editor | Obrigatória para editar graphs | Assembly editor-only `MapMagic.Editor` |

### Não são obrigatórias para o core

O core v2.1.11 não exige, por si só:

- URP;
- HDRP;
- pacote Input System;
- Cinemachine;
- NavMesh Components;
- Physics package externo;
- Addressables;
- DOTS/ECS;
- Timeline;
- sistema de vegetação externo;
- CTS, MegaSplat, MicroSplat ou RTP.

O MapMagic usa APIs nativas do Unity e pode gerar Terrain padrão sem um desses pacotes. A página atual da Asset Store lista Built-in, URP e HDRP para a versão atual; no bundle v2.1.11, a escolha do material e do pipeline é feita pelos caminhos/fallbacks existentes no código e deve ser validada no editor-alvo.

### Compatibilidade opcional

| Integração | Símbolo/assembly | O que adiciona | O que precisa existir |
|---|---|---|---|
| CTS | `CTS_PRESENT` | Output de controle/textura CTS e inspector CTS | CTS instalado e seus componentes/assemblies |
| MegaSplat | `__MEGASPLAT__` | Output MegaSplat e editor de layers | MegaSplat instalado e tipos do pacote disponíveis |
| MicroSplat | `__MICROSPLAT__` | Output MicroSplat, configuração e editor | MicroSplat instalado; assemblies referenciados por GUID |
| RTP | `RTP` | Output para Relief Terrain Pack | RTP instalado; componente/renderer RTP no objeto MapMagic |
| Vegetation Studio Pro | `VEGETATION_STUDIO_PRO` | Maps e objetos para VS Pro | Assemblies `AwesomeTechnologies.VegetationStudioPro.Runtime/Editor` |
| Aceleração nativa | `MM_NATIVE` | Rotinas nativas para matrizes/erosão quando suportadas | DLL nativa correta para a plataforma; não é requisito do algoritmo gerenciado |

Se a integração opcional não estiver instalada, o pacote inclui placeholders ou código condicionado. O nó compatível pode aparecer como placeholder, aviso ou ficar desabilitado; isso não significa que o output externo funcione sem o produto correspondente.

### `UnityEditor` e build de jogador

O editor está separado em assemblies editor-only. O core contém muito código de runtime e usa `#if UNITY_EDITOR` em pontos de editor. Existe, porém, um ponto que merece validação no projeto real: `Tools/ThreadManager/ThreadManager.cs` possui `using UnityEditor` no arquivo de runtime, enquanto usos efetivos estão condicionados. Isso é uma observação estática do v2.1.11; deve ser confirmado com um build Android/Player porque o comportamento final depende da compilação de assemblies e símbolos do projeto.

## 3. Assembly definitions e acoplamento

| Arquivo | Assembly | Plataforma | Referências |
|---|---|---|---|
| `Tools/Tools.asmdef` | `Den.Tools` | runtime | nenhuma explícita |
| `Tools/Editor/Tools.Editor.asmdef` | `Den.Tools.Editor` | Editor | `Den.Tools` |
| `Core/MapMagic.asmdef` | `MapMagic` | runtime | `Den.Tools` |
| `Core/Editor/MapMagic.Editor.asmdef` | `MapMagic.Editor` | Editor | `MapMagic`, `Den.Tools`, `Den.Tools.Editor`; exige não estar em `NET_STANDARD_2_0` |
| `Core/Plugins/MapMagic.Settings.asmdef` | `MapMagic.Settings` | Editor | nenhuma; `autoReferenced=false` |
| `Compatibility/MicroSplat/MapMagic.MicroSplat.asmdef` | `MapMagic.MicroSplat` | runtime condicionado | GUIDs de MapMagic/Den.Tools/MicroSplat |
| `Compatibility/MicroSplat/Editor/...asmdef` | `MapMagic.MicroSplat.Editor` | Editor condicionado | assemblies MicroSplat e MapMagic |
| `Compatibility/VegetationStudio/Runtime/...asmdef` | `MapMagic.VegetationStudioPro` | runtime condicionado | VS Pro e MapMagic |
| `Compatibility/VegetationStudio/Editor/...asmdef` | `MapMagic.VegetationStudioPro.Editor` | Editor condicionado | VS Pro, MapMagic e Den.Tools |

Há 35 `.asmref` no inventário expandido para ligar submódulos à base. Isso confirma que o pacote foi organizado como módulos, não como um único script gigante.

## 4. Modelo mental de execução

O fluxo é:

```text
Graph asset
   ↓
Generators e conexões
   ↓
Prepare: dependências, tiles, margins, biomas, locks
   ↓
Generate: matrizes, objetos, splines e dados intermediários
   ↓
Apply na main thread
   ↓
TerrainData / GameObjects / árvores / detalhes / integrações
```

Cada tile pode ter um nível `Main` e um nível `Draft`. O Draft é uma representação de baixa resolução para editor, preview e distância. O Main é a geração detalhada usada na área próxima. Margins são áreas extras fora do tile ativo para evitar descontinuidade em filtros, scatter, splines e outputs.

### Tipos de dados internos

| Tipo conceitual | Uso |
|---|---|
| `MatrixWorld` | Mapa numérico espacial: altura, máscara, slope, temperatura, umidade, textura, densidade |
| `TransitionsList` | Lista/hashes de objetos com posição, rotação, escala, prefab e dados de transição |
| `SplineSys` | Sistema de linhas/splines usado em estradas, caminhos, contornos e estampas |
| `Graph` | `ScriptableObject` serializável com generators, links, layers e defaults |
| `TerrainData` | Dados nativos do Unity que recebem heightmap, alphamaps, details e trees |

### Interfaces importantes do core

O código define interfaces e contratos para:

- unidade de geração (`IUnit`);
- inlet/outlet simples e múltiplos;
- preparação (`IPrepare`);
- gizmo de cena (`ISceneGizmo`);
- níveis de output (`OutputLevel`);
- output final (`OutputGenerator`);
- aplicação normal e em rotina (`IApplyData`, `IApplyDataRoutine`);
- complexidade personalizada;
- biome e função;
- dependência personalizada;
- relevância por tile;
- layers múltiplas;
- limpeza personalizada.

Isso explica por que novos nós podem ser adicionados usando a mesma infraestrutura do Inspector, do grafo e da serialização.

## 5. Criação, menus e janelas

### Menus de projeto

| Menu | Resultado |
|---|---|
| `Assets > Create > MapMagic > Empty Graph` | Cria graph vazio via `CreateAssetMenu` |
| `Assets > Create > MapMagic > Simple Graph` | Cria graph de exemplo simples |
| `Assets > Create > MapMagic > PerfTest Graph` | Cria graph para teste/performance |
| `Assets > Create > MapMagic > Brush Graph` | Cria graph especial de Brush |
| `Assets > Create > MapMagic > Imported Map` | Cria asset intermediário para RAW/texture importada |
| `Assets > Create > MapMagic > Brush Preset` | Cria preset serializável do Brush |
| `Assets > Assemble MapMagic` | Monta/atualiza plugins e compatibilidades condicionais |
| `Window > MapMagic > Editor` | Abre a janela do graph/editor |
| `Window > MapMagic > Settings` | Abre configurações de plugins/símbolos |
| `Window > MapMagic > About` | Janela sobre o produto |
| `Window > MapMagic > DocScreens` | Tela/documentação visual incluída |
| `Window > Log` | Console/log customizado do Den.Tools |
| `Window > Timers` | Medição/timers do Den.Tools |
| `Assets > To Matrix Preview` | Abre asset matricial em preview quando aplicável |
| `GameObject > 3D Object > MapMagic` | Cria GameObject com `MapMagicObject` |

### Criação do objeto MapMagic

Há duas rotas documentadas:

1. Criar `Template Graph`/`Simple Graph` e arrastá-lo para a cena; ou
2. usar `GameObject > 3D Object > MapMagic`, depois atribuir o Graph no Inspector.

O componente `MapMagicObject` é `MonoBehaviour`, `[ExecuteInEditMode]`, `[DisallowMultipleComponent]`, `[SelectionBase]` e possui `HelpURL`. Ele inicia/atualiza a geração no editor e no runtime.

### Botões da barra do Graph Window

Da esquerda para a direita, a barra principal possui:

1. **Nome do Graph asset** — mostra o graph atual; clicar destaca o asset no Project View.
2. **Biomes Tree** — navega pela hierarquia de biomes/funções.
3. **Graph Seed** — seed global do graph para resultados determinísticos.
4. **Progress Gauge** — indica progresso; `Ready` quando tiles pendentes terminaram. Sem graph atribuído, não há geração.
5. **Force Re-Generate** — força nova geração mesmo quando o sistema entende que nada mudou.
6. **Generate Changed Only** — recalcula somente o que foi alterado/relevante.
7. **Scroll to Center** — centraliza a visão do grafo.
8. **Zoom** — aproxima/afasta a área do graph.

### Anatomia de um nó

- **Header**: nome/identidade e seleção do nó.
- **Inlets**: entradas; recebem links de outros nós.
- **Outlets**: saídas; fornecem mapas, objetos ou splines.
- **Fields**: valores editáveis.
- **Preview foldout**: visualização do resultado quando aplicável.
- **Add drag handle**: botão de adição no canto superior direito; ao arrastar pode criar, inserir em um link ou anexar a um nó.
- **Remove**: aparece durante operações de remoção/conexão e leva o nó/link à área de descarte.

### Interações do graph

- botão do meio: pan;
- `Alt` + botão de arraste: pan alternativo conforme a versão/ambiente;
- roda do mouse: zoom;
- `Shift` + `=`/`-`: zoom alternativo;
- botão esquerdo: arrastar nós, editar valores e criar links;
- arrastar inlet/outlet para espaço vazio: desfaz ou solta link;
- arrastar pelo handle `Add` para espaço vazio: cria nó;
- arrastar `Add` para um link: insere nó na conexão;
- arrastar `Add` para outlet/nó: acrescenta nó compatível;
- `Shift` na seleção: seleção múltipla;
- `Group Selected`: cria grupo dos nós selecionados;
- arrastar valor: ajuste rápido;
- abrir/expandir parâmetros: edição precisa e exposição de valor.

### Menu de botão direito

O menu muda conforme o contexto:

| Contexto | Menu/ação |
|---|---|
| Espaço vazio | Create/Add generator |
| Sobre link | Insert generator no link |
| Sobre nó/outlet | Append generator |
| Nó | Disable/Enable, Duplicate, Update legacy node, Unlink, Remove, Reset |
| Grupo | Create, Group Selected, Ungroup, Remove |
| Graph | Export Selected Nodes, Import Graph, Update All legacy nodes |
| Campo de valor | `Value > Expose` e `Value > UnExpose` |

### Layers

- `+`: adiciona layer;
- ícone de arraste: reordena layer;
- área vermelha `Remove`: remove layer por arraste;
- chevron: expande/fecha;
- lápis: renomeia;
- normalmente uma layer fica aberta por vez.

O funcionamento de layer é importante para `Blend`, `Normalize`, `Textures`, `Grass`, `Biomes Set`, `Split` e outros nós multi-entrada. A ordem das layers pode mudar o resultado porque muitas usam composição de baixo para cima.

## 6. Inspector do MapMagicObject e settings

### Graph e geração

- **Graph**: asset `Graph` que contém a lógica.
- **Seed**: fonte de determinismo global.
- **Generate Terrain in Playmode**: habilita geração dinâmica durante o jogo.
- **Instant Generate**: tenta aplicar/generar imediatamente; pode reduzir suavidade da interação.
- **Max Threads/Auto Max Threads**: limite de workers de geração.
- **Apply Time per Frame**: orçamento de aplicação na main thread.
- **Clear Generated on Node Remove**: limpa dados produzidos quando o output/nó é removido.

### Tiles

- **Pin New Tile**: cria/fixa Terrain na coordenada escolhida.
- **Pin As Draft**: fixa somente um rascunho de baixa resolução.
- **Select Preview**: escolhe tile usado para preview de nó/shader.
- **Save Terrain Data**: salva o `TerrainData` conectado; regeneração pode alterar esse asset.
- **Save As Copy**: cria uma cópia/snapshot do `TerrainData`.
- **Unpin**: remove o tile/terrain fixado.

Os gizmos precisam estar habilitados para operar essas ferramentas visualmente.

### Ranges e markers

- **Main Range**: distância/raio de tiles Main.
- **Drafts Range**: distância/raio de tiles Draft.
- **Hide Out of Range Chunks**: oculta chunks fora do alcance.
- **Generate Terrain Markers: Around Main Camera**: segue a câmera principal.
- **Around Objects**: segue objetos adicionados à lista.
- **Around Objects Tagged**: segue objetos que possuam uma tag Unity.
- **Around Coordinates**: gera coordenadas de tile explicitamente.

O MapMagic não precisa de uma “aba Prioridades” do Unity para funcionar. Existem prioridades internas de tarefas/nós e a ordem de layers, mas o controle operacional é feito por ranges, markers, drafts, threads e orçamento de apply.

### Resolução e margins

- **Tile Size**: tamanho mundial do tile.
- **Main Resolution**: resolução do heightmap/dados Main.
- **Main Margins**: pixels extras usados para cálculo nas bordas.
- **Draft Resolution**: resolução Draft.
- **Draft Margins**: margem Draft.
- **Use Draft Terrains in Editor/Playmode**: ativa Draft em cada contexto.

Exemplo: resolução 513 com margem 16 gera uma área calculada de `513 + 2*16 = 545` amostras. O Terrain final usa a área ativa; a margem é calculada para filtros e continuidade.

### Outputs settings

#### Height

- **Height**: altura máxima em unidades de mundo.
- **Interpolation**: `None`, `Smooth`, `Scale 2X`, `Scale 4X`.
- **Out level**: `Draft`, `Main` ou `Both`.

`Scale 2X` e `Scale 4X` aumentam a densidade aplicada; a resolução máxima de Terrain do Unity deve ser respeitada. O manual alerta que Terrain resolution tem limite e que MapMagic 513 com `Scale 4X` pode alcançar o limite de 2049.

#### Grass/details

- **Grass Output Resolution Downscale**: reduz resolução de dados de detalhe.
- **Res/patch**: resolução de detalhe por patch.
- **Objects Output Num per Frame**: limita quantos objetos entram por frame.

#### Terrain settings expostos

O `TerrainSettings` replica propriedades do Terrain Inspector:

- Auto Connect e Grouping ID;
- Base Map Distance;
- Show Base Map;
- Base Map Resolution;
- Draw Instanced;
- Pixel Error;
- Shadow Casting Mode quando suportado;
- Reflection Probe Usage;
- Editor Render Flags;
- Heightmap Maximum LOD;
- Material Template;
- Detail Draw, Distance e Density;
- Tree Distance, Billboard Start, Fade Length e Full LOD;
- Tree LOD Bias Multiplier;
- Bake Light Probes For Trees;
- Remove Light Probe Ringing;
- Wind Speed, Wind Size, Wind Bending e Grass Tint;
- cópia opcional de layers, tags e componentes.

### Locks

1. Adicionar uma lock layer.
2. Mover/redimensionar o gizmo.
3. Pressionar/ativar `Lock` para confirmar a região.
4. O círculo interno fica preservado.
5. O círculo externo funciona como transição suave.
6. Alterar graph/regenerar preserva a área confirmada.

**Relative Height** move a área travada junto com o terreno; pode ser inadequado sobre seams de tiles. **Sync Draft** replica o lock para o nível Draft. Locks são layers: podem ser reordenadas e removidas. Faça backup; uma lock não confirmada/ativada pode ser perdida quando os dados forem liberados.

## 7. Catálogo dos nós de mapa

Os nós de mapa produzem `MatrixWorld` ou usam mapas como máscara. Valores normalmente variam entre 0 e 1; outputs convertem isso para a faixa de Terrain/texture/detail correspondente.

### Map/Initial

| Nó | Função e controles principais | Dependência/observação |
|---|---|---|
| `Constant` | Preenche mapa com um `Level` constante de 0 a 1 | Nenhuma externa; usado como base, máscara e background |
| `Import` | Importa `Imported Map`; fonte RAW ou Texture; `Map Source`, `Channel`, `Reload`, `Wrap Mode` (`Once`, `Tile`, `PingPong`), `Scale`, `Offset` | RAW quadrado, grayscale, 16-bit, PC byte order; texture dentro de `Assets` com Read/Write |
| `Noise` | `Type` Unity/Linear/Perlin/Simplex, `Seed`, `Intensity`, `Size`, `Detail`, `Turbulence`, `Offset` | Algoritmo gerenciado ou nativo opcional; offset mundial permite continuidade |
| `Simple Form` | `Gradient X`, `Gradient Z`, `Pyramid`, `Cone`; `Intensity`, `Scale`, `Ratio`, `Offset`, `Wrap Mode` | Não precisa de asset externo |
| `Spot` | Ponto circular com posição, raio e hardness; base de brush/falloff | Usado principalmente em Brush |
| `Voronoi` | Gera padrão de células/pontos Voronoi para formas, regiões e máscaras | Não depende de Unity Terrain até o output |

### Map/Input

| Nó | Função |
|---|---|
| `Height In` | Lê altura existente do Terrain/estado atual para continuar processamento |
| `Splats In` | Lê mapas de controle/alphamaps de textura do Terrain |
| `Textures In` | Importa textura/layers conforme o modo do graph/integração |

### Map/Modifiers

| Nó | O que faz | Controles/nota |
|---|---|---|
| `Blend` | Composição de várias maps | `Mix`, `Add`, `Subtract`, `Multiply`, `Divide`, `Difference`, `Min`, `Max`, `Overlay`, opacity por layer |
| `Blur` | Suaviza mapa | Input, mask, `Intensity`, `Iterations`, `Loss`, `Safe Borders`; iterações são caras |
| `Cavity` | Destaca convexidades/concavidades | `Convex`, `Concave`, `Both`, `Intensity`, `Spread` |
| `Contrast` | Ajusta brilho/intensidade e contraste | `Intensity` -1..1; contraste pode produzir máscara binária em valores altos |
| `Curve` | Remapeia valor por curva customizada do próprio MapMagic | Pontos editáveis; pode invertir, clamp e criar degraus |
| `Direction` | Modifica/analisa segundo direção/iluminação | Atenção à direção da luz e ao ângulo configurado |
| `Erosion` | Simula fluxo, erosão, transporte e sedimentação | `Iterations`, `Durability`, `Sediment`, `Fluidity`; é o nó mais pesado |
| `Ledge` | Produz/realça ledges/arestas | Usado em relevo e combinação com beach/erosion |
| `Levels` | Input low/high, gamma, output low/high | Mais compacto e rápido que Curve; propriedades podem ser expostas |
| `Mask` | Mistura Input A/B usando máscara | `Invert`; A em máscara 1, B em 0 conforme convenção documentada |
| `Normalize` | Normaliza layers para soma 1 | Background constante 1; ordem e opacity importam |
| `Parallax` | Desloca mapa como Smudge | Offset X/Z, intensity map e interpolation `None`, `Always`, `OnTransitions` |
| `Sediment` | Trabalha o sedimento/depósito derivado | Normalmente usado junto de erosão e máscaras |
| `Selector` | Seleciona intervalo de valores | `Set Range`/Min-Max, `Units` map/world, `From`, `To`, `Transition` |
| `Slope` | Calcula diferença de altura/ângulo | `From`, `To`, `Smooth Range` em graus |
| `Terrace` | Cria degraus/terraços | `Num`, `Uniformity`, `Steepness`; frequentemente seguido de Erosion |
| `Unity Curve` | Mesmo propósito de Curve usando editor `AnimationCurve` do Unity | Mais flexível, porém mais pesado |

### Map/Output

| Nó | Função e dependência |
|---|---|
| `Height` | Aplica map ao heightmap do Terrain; `Height`, interpolation e out level; deve terminar a cadeia de altura |
| `Textures` | Aplica layers e alphamaps; normaliza internamente; exige `TerrainLayer` assets |
| `Grass` | Aplica detalhe/grass; modos Grass, Billboard, Mesh Vertex Lit, Mesh Unlit |
| `Direct Matrices` | Envia matrizes diretamente para holders/consumidores |
| `Direct Textures` | Envia texturas diretamente para holders/consumidores |
| `Custom Material` | Saída para material/shader customizado |
| `CTS` | Saída opcional condicionada a CTS |
| `MegaSplat` | Saída opcional condicionada a MegaSplat |
| `MicroSplat` | Saída opcional condicionada a MicroSplat |
| `RTP` | Saída opcional para Relief Terrain Pack |
| `VS Pro Maps` | Saída opcional para Vegetation Studio Pro |

### Map/Portals

`Matrix Enter` e `Matrix Exit` conectam regiões diferentes do grafo sem desenhar um link longo. São portais tipados: ambos precisam representar o mesmo tipo de mapa.

### Map Set

Map Set opera sobre vários mapas associados a protótipos, como um dicionário `<Texture, Map>`. É central no Brush e permite ler, escolher, modificar e escrever conjuntos de textura/detalhe sem perder canais.

Nós encontrados:

- `Add`: define/adiciona mapa a um conjunto;
- `Blur`;
- `Cavity`;
- `Combine`;
- `Contrast`;
- `Create`;
- `Curve`;
- `Erosion`;
- `Levels`;
- `Mask`;
- `Parallax`;
- `Pick`: escolhe mapa por textura/protótipo e índice;
- `Selector`;
- `Slope`;
- `Terrace`;
- `Unity Curve`.

## 8. Outputs de textura, grass e TerrainLayer

### Textures Output

Para usar o output padrão:

1. `Assets > Create > Terrain Layer`.
2. Atribuir cada `TerrainLayer` na layer do nó.
3. Conectar masks.
4. Definir opacity e ordem.
5. Gerar.

O node normaliza as camadas internamente. O Background é uma layer implícita de valor 1. Alterar Diffuse, Normal, Mask, Specular, Metallic, Normal Scale, Tile Size ou Offset pelo node altera o asset TerrainLayer referenciado; isso pode modificar todos os Terrains que o utilizam.

O `TerrainLayer` do Unity é um requisito do output padrão, mas não do core de mapas. Outputs externos usam seus próprios materiais/shaders/configurações.

### Grass Output

- **Grass**: plano estático padrão;
- **Billboard**: plano que encara a câmera;
- **Mesh Vertex Lit**: mesh de detalhe sem opacity;
- **Mesh Unlit**: mesh customizado com opacity;
- **Texture**: textura de grass para modos grass/billboard;
- **Object**: prefab/mesh de detalhe para modos mesh;
- **Density**;
- **Dry/Healthy**;
- **Width/Height**;
- **Noise**;
- **Downscale**;
- **Res/patch**;
- **Out level**.

Esses dados passam pelas estruturas de detail prototype do Unity. O usuário precisa configurar corretamente os assets/protótipos que o Terrain pode aceitar.

## 9. Catálogo dos nós de objetos

### Objects/Initial

| Nó | Função e propriedades |
|---|---|
| `Positions` | Recebe/expõe posições iniciais para distribuição e transições |
| `Scatter` | Grade ordenada com deslocamento aleatório; `Seed`, `Density`, `Uniformity`, `Relax`, `Add. Margins`; rápido |
| `Random` | Distribuição aleatória não baseada em grade; `Seed`, `Density`, `Uniformity`; aceita máscara |
| `Get by Tag` | Lê GameObjects já existentes pela tag; `Tag`, `Add. Margins` |

`Scatter` é mais previsível e rápido; `Random` é mais orgânico, mas pode produzir agrupamentos. Densidade é expressa por área de 100 x 100 unidades, isto é, um quilômetro quadrado no sistema documentado.

### Objects/Modifiers

| Nó | Função e controles |
|---|---|
| `Adjust` | Ajusta `Height`, `Front`, `Right`, `Rotation`, `Scale`; `Random Range`, `Seed`, `Size Factor`, `Relativity` Absolute/Relative |
| `Randomize` | Randomiza propriedades/valores; `Seed`; útil para variar várias saídas |
| `Mask` | Remove objetos segundo mapa; `Seed`, `Invert` |
| `Lerp` | Interpola propriedades/posições de objetos entre fontes |
| `Floor` | Coloca objetos na altura do heightmap; `Relativity` Absolute/Relative |
| `Split` | Separa objetos em layers por condições ou probabilidade; `Match`, `Seed`, `Probability` |
| `Rarefy` | Remove objetos próximos; `Distance`, `Size Factor`, `Use Self`, layers adicionais |
| `Combine` | Junta listas de objetos |
| `Spread` | Cria clones em torno dos objetos; `Retain Originals`, `Seed`, `Growth`, `Distance`, `Size Factor` |
| `Blob` | Cria/usa área de influência ao redor de objetos |
| `Flatten` | Achata terreno sob posições; `Radius`, `Hardness`, `Use Noise on Falloff`, `Noise Amount`, `Noise Size` |
| `Stroke` | Gera área circular de influência/ajuste; radius, hardness e noise no falloff |
| `Stamp` | Usa objetos como carimbos sobre mapa; `Size`, `Intensity`, fatores de tamanho/intensidade, blend, rotation, falloff |
| `Forest` | Simula crescimento: seedlings, other trees, soil, `Years`, `Density`, `Fecundity`, `Seed Dist`, `Reproductive Age`, `Survival Rate`, `Max Age`, `Size is Age` |
| `Slide` | Desliza objetos pela inclinação do terreno; `Blur`, `Iterations`, `Move Factor`, `Stop Slope` |
| `Move` | Desloca objetos em direção usando mapa de intensidade |
| `Shrink Scale` | Reduz escala para caber/evitar colisão/limites conforme configuração do nó |
| `Safe Borders` | Remove objetos próximos da borda; `Edge Dist` |

### Objects/Outputs

| Nó | Função |
|---|---|
| `Objects` | Instancia prefabs; single/multi-prefab, pool, clones, biome blend, Num/Frame, altura, rotação e escala |
| `Trees` | Usa o sistema de árvores do Terrain; prefab(s), seed, cor, bend, altura, normal, rotação e escala |
| `VS Pro Objs` | Saída opcional para Vegetation Studio Pro |

No `Objects` e `Trees` existem grupos de controles de transformação:

- **Height**: Relative Height e Object Height;
- **Rotation**: Use Rotation, Terrain Normal, Rotate Y Only, Regards Prefab Rotation;
- **Scale**: Use Scale, Scale Y Only, Regards Prefab Scale.

`Objects` pode aplicar por frame para suavizar travamentos da main thread. `Use Pool` evita destruir e instanciar tudo a cada mudança de tile. Em Playmode, objetos são sempre instanciados como clones.

### Objects/Portals

`Objects Enter`/`Exit` transportam `TransitionsList` entre regiões do graph.

## 10. Catálogo dos nós de spline

Splines são linhas com nós/segmentos, usadas para estradas, rios, trilhos, contornos e caminhos de objetos.

| Nó | Função e controles |
|---|---|
| `Manual` | Entrada manual de pontos/linhas |
| `Interlink` | Liga nós usando grafo geométrico; `Iterations`, `Max Links`, `Within Tile`, `Clamp` |
| `Pathfinding` | A* sobre heightmap; `Draft`, `Height`, `Resolution`, `Distance Factor`, `Elevation Factor`, `Straighten Factor`, `Max Elevation`, `Weld Endpoints` |
| `Stroke` | Converte spline em máscara; `Width`, `Hardness` |
| `Align` | Alinha/ajusta splines a referência/terreno |
| `Stamp` | Achata faixa do terreno ao longo da spline; `Flat Range`, `Blend Range` |
| `Optimize` | Remove nós mantendo forma; `Split`, `Deviation` |
| `Relax` | Suaviza linha; `Blur`, `Iterations` |
| `Combine` | Junta sistemas sem weld/merge automático |
| `Weld Close` | Une splines próximas; `Threshold` |
| `Floor` | Coloca spline na altura da superfície |
| `Avoid` | Desvia de obstáculos/áreas proibidas |
| `Push` | Empurra/desloca pontos/linhas |
| `Isoline` | Extrai linha de nível a partir de mapa |
| `Silhouette` | Extrai contorno/silhueta de área |
| `Scatter` | Coloca objetos ao longo da linha; `Distance`, `Min Obj/Seg`, `Max Obj/Seg`, `Rotate` |
| `Output` | Envia spline ao sistema de aplicação/holder |

`Pathfinding` é um dos nós mais lentos; resolução maior melhora precisão, mas custa tempo e memória. `Interlink` usa o grafo de Gabriel para criar conexões; `Within Tile` ignora pontos nas margins e `Clamp` corta conexões que saem do tile.

## 11. Biomes e Functions

### Biomes Set

Mistura graphs de biome como layers de textura. Possui normalização embutida; Background vale 1; a ordem das layers importa. Para transições coerentes, os biomes devem compartilhar relevo-base compatível — normalmente o mesmo `Noise Seed`, `Size` e `Offset`.

### Whittaker

Recebe:

- **Temperature**: 0 equivale a 0 °C anual e 1 a 30 °C anuais na escala documentada;
- **Moisture**: 0 sem precipitação e 1 a 400 cm anuais.

Distribui dez biomes esperados: tropic rainforest, mild rainforest, tropic forest, mild forest, taiga, savanna, grassland, tundra, hot desert e cold desert. Cada biome tem Graph Slot e Influence. `Sharpness` controla a transição. Também gera outlets de máscara.

### Biome Mask

Retorna a máscara do biome atual dentro de `Biomes Set`. No graph de topo retorna mapa preenchido com 1. Pode reduzir grass/objects perto da borda para evitar mistura visual.

### Function

Uma Function é um subgraph reutilizável com `Function Input` e `Function Output`. O parent graph ganha inlets/outlets correspondentes aos portais internos.

- **Graph Slot**: subgraph da função;
- **Open**: entra no subgraph como biome/contexto;
- **Folder Up**: retorna ao parent;
- **Folder Tree**: navega na hierarquia;
- **Override Exposed**: permite alterar localmente os valores expostos;
- valores override não alteram o asset compartilhado nem outras instâncias;
- overrides têm pequeno custo extra de geração.

### Expose

Qualquer campo de valor pode usar `Value > Expose`. O valor passa a aparecer nos defaults/overrides da graph. Em código, a documentação usa o conceito:

```csharp
mmObject.graph.defaults["Nome"] = valor;
```

Também existe acesso por `TryGetValue`, preservando o tipo. O default fica no graph asset; portanto, alterar em Playmode pode persistir no asset se o código não restaurar.

## 12. Brush

### O que é diferente

Um Brush graph não é um graph normal. Ele precisa de nodes especiais de input/output para ler e escrever somente a região pintada. Um graph de Terrain comum não funciona automaticamente como Brush graph.

### Componente e alvo

`MapMagicBrush` é um `MonoBehaviour` `ExecuteInEditMode`, `DisallowMultipleComponent` e `SelectionBase`. Pode ficar em qualquer GameObject. Terrains são adicionados à lista independentemente da hierarquia:

- `This Component`: Terrain do mesmo objeto;
- `Child Terrains`: todos os Terrains filhos;
- `All Terrains`: todos da cena;
- `Custom`: Terrain escolhido/arrastado;
- `X`: remove da lista.

Os Terrains precisam ter mesmo tamanho mundial e mesmas resoluções de heightmap, splat/control map e detail/grass. Terrains incompatíveis são rejeitados com aviso.

### Brush Menu

- **Preset Source**: asset preset usado como fonte;
- **Save**: grava valores atuais no preset;
- **Save As**: cria novo preset;
- **Graph**: graph de Brush;
- **Radius**: tamanho;
- **Hardness**: parte interna com efeito 100%;
- **Spacing**: distância máxima entre stamps durante stroke;
- **Intensity/Texture/TerrainLayer/etc.**: valores expostos do graph;
- **Used Auto-values**: valores realmente fornecidos no último stroke;
- **All Auto-values**: catálogo de nomes/tipos disponíveis;
- **Position**: posição atual do cursor/stroke;
- **PrevPosition**: posição anterior;
- **CapturedPosition**: posição capturada com Ctrl-click;
- **TerrainHeight**: altura mundial para converter Y em faixa 0..1;
- **Shift**: boolean/int indicando Shift, usado para modo apagar.

O teclado `[` e `]` ajusta Radius quando a Scene View está focada. Presets de teclado usam `~`, `1`, `2`, `3`, etc., na ordem da lista.

### Brush input/output nodes

- Height In/Out;
- Textures In/Out;
- Texture Set In/Out;
- Grass In/Out;
- Grass Set In/Out;
- Trees In/Out;
- Objects In/Out;
- Transfer Trees;
- Transfer Objects.

### Presets incluídos

`Add`, `AddBlur`, `Blur`, `Cavity`, `Erosion`, `Forest`, `Grass`, `Level`, `Move`, `Noise`, `Polish`, `Ridge`, `Road`, `Slope`, `SlopeCavity`, `Texture`.

Os presets são assets, não apenas botões: armazenam graph, radius, hardness, spacing e valores expostos. Copiar um preset para o Brush copia os valores; editar o Brush não altera a fonte até usar Save.

### Map Sets no Brush

Map Set é um array/dicionário de mapas por protótipo. O fluxo completo pode ser:

```text
Texture Set In
  ↓
Pick por TerrainLayer/índice
  ↓
Mask/Blend/Curve/etc.
  ↓
Add ou Set
  ↓
Texture Set Out
```

Isso preserva todos os canais e permite, por exemplo, pintar grass somente sobre a textura GreenGrass e apagar com Shift.

### Resolução: detalhe importante

Heightmap representa vértices; uma malha de 512 polígonos tem 513 vértices. Control maps frequentemente são 512. Essa diferença causa deslocamento/seams entre height e splat. O Brush pode upsample/downsample para processar, mas a melhor compatibilidade é usar resoluções coerentes e verificar limites do Unity.

## 13. Assets e como cada família é usada

### Assets de graph e dados

| Asset | Uso |
|---|---|
| `Graph.asset` | Serializa nodes, links, layers, defaults e subgraphs |
| `Imported Map.asset` | Guarda dados importados de RAW/texture para o nó Import |
| `Brush.asset` | Guarda preset de Brush |
| `TerrainLayer` | Difuse/normal/mask e parâmetros de textura do Terrain |
| `.terrainlayer` | Arquivos de layer usados pelo Terrain/Textures Output |
| `.raw` | Heightmaps e máscaras importáveis |
| `.mat` | Materiais de Terrain, demos e integrações |
| `.prefab` | Objetos distribuídos, árvores, grass meshes e componentes demo |
| `.unity` | Cenas de tutorial/demo |
| `.lighting` | Configurações de iluminação de cenas demo |
| `.asset` adicional | Presets, configurações, protótipos, objetos serializados e dados de demonstração |

### Assets visuais

PNG/TIFF aparecem em quatro papéis principais:

1. ícones da janela/nós/portais/menus;
2. imagens de documentação e telas;
3. texturas de Terrain, grass e materiais;
4. mapas e recursos de demo.

Não se deve atribuir qualquer PNG como heightmap sem conferir import settings, canal e Read/Write.

### Shaders/includes

Os shaders e `.cginc` pertencem a preview, terrain/material customizado, integrações ou visualização. Eles não significam que o bundle exige uma render pipeline específica; o material efetivamente usado determina a compatibilidade.

### DLL/native

O pacote contém binários nativos/compilados para aceleração opcional. O código seleciona rotinas por `MM_NATIVE` e desabilita caminhos incompatíveis com Android/IL2CPP em condicionais. Não se deve copiar DLL de desktop para Android sem checar arquitetura e plugin import settings.

## 14. API Unity usada diretamente

### Componentes/tipos Unity essenciais

O código depende conceitualmente de:

- `MonoBehaviour`;
- `ScriptableObject`;
- `ISerializationCallbackReceiver`;
- `Terrain`;
- `TerrainData`;
- `TerrainLayer`;
- `Texture2D`;
- `Texture2DArray` em pontos de utilitário;
- `Material` e `Shader`;
- `GameObject`, `Transform`, `Component`, `Prefab`;
- `AnimationCurve`;
- `Color`, `Vector2`, `Vector3`, `Quaternion`, `Bounds`, `Rect`;
- `GraphicsSettings.renderPipelineAsset`;
- `EditorApplication.update`/callbacks no editor;
- `Undo`, `Selection`, `AssetDatabase`, `EditorGUI` e `EditorWindow` no editor;
- `System.Threading.Thread` e filas de trabalho;
- gizmos/handles para Scene View.

### TerrainData e aplicação

O output de altura precisa de APIs de `TerrainData` que recebem arrays normalizados de height samples. O caminho rápido de edição usa a ideia de `SetHeightsDelayLOD` e posterior sincronização/atualização de LOD. Isso é compatível com a documentação Unity: `SetHeightsDelayLOD` evita atualizar LOD/vegetação a cada alteração e deve ser sincronizado depois.

Texturas exigem alphamaps/control maps e `TerrainLayer`. Grass exige detail prototypes/resolution. Trees exigem tree prototypes e parâmetros de árvore do Terrain.

### Import de mapa

Para Texture mode, a textura deve estar dentro de `Assets` e com Read/Write habilitado. A razão é que o MapMagic lê pixels do lado CPU; Unity explica que uma texture não-readable não fornece seus dados para scripts e que Read/Write mantém uma cópia descompactada em memória.

### Cena e câmera

Geração infinita pode seguir `Camera.main`, objetos, tags ou coordenadas. `Camera.main` precisa existir para o modo Around Main Camera; isso é uma dependência de configuração, não uma biblioteca externa.

## 15. Prioridade, threads e performance

### Não existe dependência de “aba Prioridades”

Não foi encontrada exigência de uma aba Unity chamada Prioridades. Há três conceitos distintos:

1. `priority` no atributo `GeneratorMenuAttribute`, usado para ordenar categorias/menus;
2. prioridade/ordem interna de tarefas do thread manager;
3. prioridade visual/ordem de layers e outputs.

Nenhum deles exige criar uma aba externa no Unity.

### Thread manager

O sistema mantém workers, fila, limite de threads e aplicação em rotina. O processamento pesado pode ocorrer fora da main thread; mudanças em Unity objects/TerrainData precisam voltar para a main thread. Por isso `Apply Time per Frame`, `Num/Frame` e `Instant Generate` têm efeitos diferentes:

- mais orçamento: termina mais rápido, pode gerar spikes;
- menos orçamento: suaviza frame, termina mais tarde;
- instant: privilegia resposta imediata, não necessariamente FPS;
- objetos por frame: controla especificamente instanciação/aplicação.

### Nós caros

- Erosion: itera fluxo, erosão e sedimentação;
- Blur com muitas iterações;
- Pathfinding com alta resolução;
- Forest;
- Scatter/Random com densidade alta e margins grandes;
- Stamp/Flatten/Stroke em resolução alta;
- outputs com Scale 4X;
- criação de muitos GameObjects em vez de pooling/instancing.

### Mobile

O pacote não é automaticamente mobile-first. Para Android, testar:

- número de tiles Main/Draft;
- memória de TerrainData e control maps;
- resolução e margins;
- Draw Instanced;
- densidade de árvores/grass/objects;
- material/shader no GPU-alvo;
- geração durante Playmode;
- IL2CPP e arquitetura ARM64;
- presença/ausência de `MM_NATIVE`;
- pausa/retorno de Activity e perda de Surface.

## 16. Compatibilidades e integrações em detalhe

### CTS

O código de CTS é protegido por `CTS_PRESENT` e acessa tipos CTS somente quando o pacote externo está presente. O inspector mostra warning se o símbolo foi ativado sem componentes compatíveis.

### MegaSplat

`MegaSplatOutput` e editor dependem de `MAPMAGIC2` e `__MEGASPLAT__`. Sem MegaSplat, o arquivo não deve ser tratado como output funcional; pode ficar em placeholder/warning.

### MicroSplat

Há assembly runtime/editor separado, constraints `__MICROSPLAT__` e referências GUID para assemblies externos. A integração pode exigir módulos MicroSplat específicos conforme a versão do MicroSplat.

### RTP

O output RTP lê configuração/renderer externo e o editor mostra aviso quando RTP ou renderer não estão atribuídos ao MapMagicObject.

### Vegetation Studio Pro

As assemblies referenciam explicitamente `AwesomeTechnologies.VegetationStudioPro.Runtime` e `.Editor`. Maps e objects não funcionam como Terrain padrão sem o produto.

### Pipeline Built-in/URP/HDRP

O `DefaultTerrainMaterial` tenta selecionar o material padrão do render pipeline e possui fallback para nomes de shaders Built-in, HDRP e Lightweight/URP. Isso é uma estratégia de fallback; o material final, shader e versão do Unity ainda precisam ser validados em um projeto real.

## 17. Instalação recomendada do v2.1.11

1. Criar projeto Unity de teste, preferencialmente usando a versão compatível da época do bundle.
2. Em Player/Other Settings, selecionar `.NET 4.x API Compatibility Level`.
3. Importar o `.unitypackage` completo.
4. Se já existir MapMagic 1, removê-lo antes de importar v2 para evitar colisão.
5. Deixar `Demo` e `Compatibility` fora somente se tiver motivo; para aprender, manter demos.
6. Aguardar o Unity recompilar assemblies.
7. Abrir `Window > MapMagic > Settings` e verificar símbolos somente para integrações instaladas.
8. Criar `MapMagic > Simple Graph` ou `Template Graph`.
9. Arrastar o graph para a cena ou criar `GameObject > 3D Object > MapMagic`.
10. Conferir Graph, Seed, Tile Size, Main/Draft Resolution, Margins e ranges.
11. Abrir o graph e verificar se há pelo menos um output final.
12. Para textura, criar e atribuir `TerrainLayer` assets.
13. Para objetos, atribuir prefabs válidos e escolher pooling/clone/transform.
14. Gerar primeiro com resolução baixa e sem integrações externas.
15. Validar Console, warnings, seams, memory e tempo por tile.
16. Só depois ativar Erosion, Pathfinding, Forest, alta densidade e compatibilidades.

## 18. Checklist de diagnóstico

### Nada aparece

- Graph atribuído ao MapMagicObject?
- Há output `Height`, `Textures`, `Grass`, `Objects` ou `Trees`?
- Output está em `Main`, `Draft` ou `Both` compatível com o que está visível?
- Main/Draft ranges não estão em zero?
- Generate Terrain in Playmode está habilitado quando necessário?
- Existe câmera/marker para o modo escolhido?
- Console acusa assembly ou shader?

### Textura não funciona

- `TerrainLayer` foi criado e atribuído?
- Máscaras estão conectadas?
- Background/Normalize deixa soma válida?
- O material do Terrain suporta o modo esperado?
- Tile Size é compatível com o tamanho do Terrain para evitar seams?

### Import não funciona

- RAW é quadrado, grayscale, 16-bit e PC byte order?
- Texture está em `Assets`?
- Read/Write está habilitado?
- Canal correto foi selecionado?
- `Imported Map` foi criado e atribuído?

### Objetos somem ou atravessam bordas

- `Floor` foi usado quando necessário?
- `Safe Borders`/margins estão adequados?
- Scatter/Random têm margins?
- `Terrain Normal`, `Object Height`, `Regards Prefab Rotation/Scale` estão corretos?
- Pool/clones estão destruindo ou reaproveitando objetos como esperado?

### Seam entre tiles

- Main/Draft Margins suficientes?
- `Safe Borders` ativado em filtros de altura?
- Noise usa offset mundial e mesmo seed/size?
- Textures tile size é múltiplo coerente do tamanho mundial?
- Control map e heightmap têm resolução compatível?
- Spline/objects estão sendo cortados por `Within Tile`, `Clamp` ou `Safe Borders`?

### Build Android falha

- Projeto está usando API Compatibility correta?
- Algum assembly externo opcional ficou habilitado sem pacote?
- DLL nativa tem arquitetura e import settings corretos?
- Algum `UnityEditor` vazou em assembly runtime?
- `MM_NATIVE` está habilitado em uma plataforma não suportada?
- Shader/material escolhido compila para a GPU/API do dispositivo?

## 19. Observações de arquitetura

O design do v2.1.11 é relativamente modular:

- `Core` conhece o ciclo de geração;
- `Den.Tools` concentra operações de matrizes, splines, threads e GUI;
- outputs são plugins/geradores finais;
- metadata `GeneratorMenu` registra nós sem um grande `if` manual no menu;
- `Graph` e generators são serializáveis;
- biomes/functions reutilizam graphs;
- compatibilidades usam símbolos e assemblies separados.

Limitações/risco observados:

- bundle antigo, enquanto Unity e integrações atuais mudaram;
- package não inclui projeto Unity de validação;
- integrações externas dependem de GUIDs, nomes de assembly e símbolos corretos;
- TerrainData/control maps podem consumir muita memória;
- resolução e margins exigem calibração por projeto;
- geração infinita e objetos ainda precisam de estratégia de lifecycle no Android;
- integração nativa requer teste por plataforma;
- `ThreadManager.cs` merece build de Player real por causa do `using UnityEditor` no arquivo de runtime.

## 20. Resposta direta às dependências citadas

### “Ele precisa que componente X exista?”

Sim, conforme o recurso:

- para o objeto principal: `MapMagicObject`;
- para Terrain padrão: `Terrain`/`TerrainData` e APIs correspondentes;
- para seguir câmera: uma câmera acessível como `Camera.main` ou marker alternativo;
- para Textures Output: `TerrainLayer` assets;
- para Objects Output: prefabs/`GameObject` válidos;
- para Trees Output: tree prefabs/prototypes compatíveis com Terrain;
- para Brush: `MapMagicBrush` e Terrains com resoluções/tamanho compatíveis;
- para CTS/MegaSplat/MicroSplat/RTP/VS Pro: componentes e assemblies do produto externo;
- para `Get by Tag`: GameObjects existentes com a tag selecionada.

### “Ele precisa de uma função X da Unity?”

Sim, internamente usa famílias de API, não uma única função mágica:

- serialização de `ScriptableObject`/Unity serialization;
- leitura de assets via `AssetDatabase` no editor;
- criação/alteração de `TerrainData`;
- aplicação de alturas, alphamaps, details e árvores;
- `AnimationCurve` para Unity Curve;
- `EditorWindow`, `MenuItem`, `Undo`, Scene View/gizmos;
- threads/rotinas para geração e apply;
- shaders/materials/graphics settings.

### “Ele precisa da aba Prioridades?”

Não. Não há dependência de uma aba Prioridades do Unity. O que existe é prioridade interna de menu/tarefa, ordem de layers, ranges, drafts, markers e orçamento por frame.

## 21. Resultado final da leitura

Para reproduzir o comportamento essencial do MapMagic 2 v2.1.11, o mínimo é:

```text
Unity Editor
  + .NET 4.x API Compatibility
  + UnityEngine/Terrain/TerrainData
  + Assets/MapMagic/Tools  -> Den.Tools
  + Assets/MapMagic/Core   -> MapMagic runtime/editor
  + um Graph
  + um MapMagicObject
  + pelo menos um Output
```

O bundle completo inclui muito mais: objetos, árvores, splines, biomes, functions, brush, demos, locks, preview, materiais, shaders, presets e adaptadores. O produto externo só entra quando um output compatível é selecionado e seus símbolos/componentes/assemblies existem.

Esta separação é a forma segura de levar o conteúdo para outro projeto, para Android ou para uma engine própria: primeiro reproduzir o contrato de Terrain, graph, matrizes, outputs, serialização e lifecycle; depois adicionar Objects, Splines, Biomes, Brush e integrações como módulos.

