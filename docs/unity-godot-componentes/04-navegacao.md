# 04 — navegação, NavMesh e pathfinding

## Função da categoria

Navegação responde à pergunta “por onde um agente pode andar?”. Ela é diferente de física: física detecta contato; navegação calcula caminho e avoidance. O MapMagic possui `Spline Pathfinding` para gerar caminhos no graph, mas isso não substitui a navegação de NPC em runtime.

## Unity

### NavMesh

Dados de superfície caminhável. Pode ser bakeada no editor ou construída/atualizada em runtime.

### NavMeshSurface

Componente do pacote AI Navigation. Coleta geometria, layers, volumes e gera uma NavMesh.

**Onde fica:** normalmente adicionado a um GameObject de mundo/área.

**Precisa de:** pacote AI Navigation, geometria coletável, layer mask, agent type e parâmetros de bake.

### NavMeshAgent

Agente que consulta NavMesh, calcula caminho, evita obstáculos e fornece velocidade/direção desejadas.

**Importante:** o agente pode mover o Transform; não misture sem critério esse movimento com Rigidbody/CharacterController.

### NavMeshObstacle

Obstáculo dinâmico que afeta avoidance e, com carving, pode abrir buraco na NavMesh.

### OffMeshLink/NavMeshLink

Conexão especial para salto, porta, elevador, ponte ou teleporte. Link não torna o agente capaz de executar a ação sozinho; o gameplay precisa controlar a travessia.

### NavMeshModifier

Inclui/exclui ou altera área de geometria coletada. Faz parte do pacote AI Navigation em versões modernas.

### NavMeshModifierVolume

Volume que marca área com custo/tipo de navegação.

### NavMeshData

Asset/dados bakeados. Útil para carregar regiões, streaming e salvar resultados.

### API NavMesh

`NavMesh.SamplePosition`, `NavMesh.CalculatePath`, `NavMesh.Raycast` e consultas de áreas permitem usar navegação sem um NavMeshAgent em casos especiais.

## Godot

### NavigationRegion3D

Região caminhável que usa `NavigationMesh`. Agentes usam essa região para pathfinding.

**Precisa de:** NavigationMesh atribuída, geometria para bake e mapa de navegação compatível.

### NavigationMesh

Resource com geometria e parâmetros de bake: tamanho de agente, altura, inclinação, step e filtros.

### NavigationAgent3D

Helper para chamadas ao NavigationServer3D, pathfinding e avoidance. Não move automaticamente um CharacterBody3D; o script lê a próxima posição/direção e move o corpo.

### NavigationObstacle3D

Afeta avoidance dos agentes. Não altera necessariamente a NavigationMesh/pathfinding.

### NavigationLink3D

Link entre posições/regiões, com custo e direção. A ação do salto/porta/elevador fica no script.

### NavigationServer3D

Servidor baixo nível que administra mapas, regiões, links, agentes e consultas. É serviço, não Node visual.

### Equivalentes 2D

`NavigationRegion2D`, `NavigationPolygon`, `NavigationAgent2D`, `NavigationObstacle2D`, `NavigationLink2D` e `NavigationServer2D` fazem o mesmo em 2D.

## MapMagic Spline Pathfinding x runtime navigation

| Necessidade | MapMagic | Unity runtime | Godot runtime |
|---|---|---|---|
| Gerar estrada/caminho no terreno | `Spline Pathfinding` | Spline/mesh + NavMeshSurface depois | Spline/mesh + NavigationRegion3D depois |
| Caminho de NPC | Não é responsabilidade do output spline | NavMeshAgent/API | NavigationAgent3D/Server |
| Área inacessível por inclinação | Max Elevation no spline | NavMesh bake/area | NavigationMesh bake/filters |
| Obstáculo dinâmico | graph/objects | NavMeshObstacle | NavigationObstacle3D |
| Ponte/elevador | spline/link visual | OffMeshLink/NavMeshLink | NavigationLink3D |

## Processo correto com terreno procedural

1. Gerar chunk/heightmap.
2. Aplicar visual e collider.
3. Marcar geometria como coletável.
4. Rebuild/bake da região navegável.
5. Adicionar agentes somente quando a região estiver pronta.
6. Descarregar NavMesh junto com chunk distante.

Não bakear todo um mundo infinito de uma vez; usar regiões, tiles e streaming.

## Dependências

### Unity

- `NavMeshAgent`/`NavMeshObstacle` são componentes Unity;
- `NavMeshSurface`, `NavMeshModifier`, `NavMeshModifierVolume` e `NavMeshLink` dependem do pacote AI Navigation nas versões atuais;
- Terrain/mesh precisa estar habilitado para coleta;
- layer mask, agent type e area precisam ser consistentes.

### Godot

- nodes de Navigation são nativos;
- `NavigationMesh` é Resource;
- agente precisa de script de movimento;
- regiões devem estar no mesmo navigation map para conectar;
- a geometria visual não vira navegação automaticamente sem bake/configuração.

## Tutorial de validação

### Unity

1. Instale/ative AI Navigation.
2. Crie Plane/Terrain.
3. Adicione NavMeshSurface.
4. Bake.
5. Adicione NavMeshAgent e um alvo.
6. Defina `agent.destination`.
7. Adicione obstáculo e teste carving/avoidance.

### Godot

1. Crie `NavigationRegion3D`.
2. Crie NavigationMesh.
3. Adicione MeshInstance3D de chão.
4. Faça bake.
5. Crie CharacterBody3D + NavigationAgent3D.
6. Leia `get_next_path_position` no script.
7. Mova o CharacterBody3D com a direção retornada.

## Interface de propriedades e Inspector

| Elemento | Onde aparece | Interface/propriedades | Limite no MapMagic v2.1.11 |
|---|---|---|---|
| Unity `NavMeshSurface` | Inspector do `GameObject` | Agent Type, coleta de geometria, layers, volume e bake | Não foi encontrado como componente obrigatório do `MapMagicObject` |
| Unity `NavMeshAgent` | Inspector do agente | Velocidade, aceleração, raio, altura, stopping distance e avoidance | É componente de runtime do NPC, não gerador de terreno |
| Unity `NavMeshObstacle` | Inspector | Forma, carving e tamanho | Não é criado pelo MapMagic core |
| Unity `NavMeshLink`/`OffMeshLink` | Inspector | Pontos, largura, custo e bidirecionalidade conforme o componente | A ligação precisa ser criada/configurada pelo projeto |
| Unity `NavMeshModifier`/`NavMeshModifierVolume` | Inspector | Inclusão/exclusão e área de navegação | Depende do pacote AI Navigation/versão do Unity |
| Unity `NavMeshData` | Inspector do asset/objeto | Dados serializados do navmesh | Não é equivalente automático ao graph do MapMagic |
| Godot `NavigationRegion3D` | Inspector do nó | `NavigationMesh`, layers e propriedades da região | Integração proposta, não saída declarada do bundle |
| Godot `NavigationMesh` | Inspector do recurso | Parâmetros de bake e geometria navegável | Precisa ser associado/baked pelo projeto Godot |
| Godot `NavigationAgent3D` | Inspector do nó | Raio, altura, alvo, avoidance e camadas | Não é componente do MapMagic original |
| Godot `NavigationObstacle3D`/`NavigationLink3D` | Inspector do nó | Obstáculo/link e parâmetros próprios | São recursos do sistema de navegação Godot |
| `NavigationServer3D`/API | Não abre Inspector próprio | Serviço/API de navegação | É acessado por código, não selecionado como componente visual |

### Sequência de interface

1. Gerar/aplicar o terreno.
2. Selecionar o componente ou recurso de navegação no editor.
3. Definir a malha/region/área navegável e os filtros de layers.
4. Fazer o bake ou atualização do navmesh, quando o sistema escolhido exigir.
5. Selecionar o agente e ajustar parâmetros de movimento.
6. Executar e verificar o caminho.

O `Spline Pathfinding` do MapMagic abre/configura um generator dentro do Graph Window; isso não significa que o Inspector do MapMagic crie `NavMeshAgent`, `NavMeshSurface`, `NavigationRegion3D` ou `NavigationAgent3D`. A documentação desses componentes descreve a navegação da engine, enquanto a integração com o MapMagic precisa ser comprovada no graph/output utilizado.

## Fontes

- [Unity AI Navigation](https://docs.unity3d.com/Packages/com.unity.ai.navigation@latest)
- [Unity NavMeshAgent](https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html)
- [Godot introdução à navegação 3D](https://docs.godotengine.org/en/stable/tutorials/navigation/navigation_introduction_3d.html)
- [Godot NavigationRegion3D](https://docs.godotengine.org/en/stable/classes/class_navigationregion3d.html)
- [Godot NavigationAgent3D](https://docs.godotengine.org/en/stable/classes/class_navigationagent3d.html)

## Escopo declarado pelo MapMagic

O bundle v2.1.11 declara `Spline Pathfinding` como generator de graph e possui integração com dados de altura. Isso é geração de spline/path dentro do graph. A leitura do `MapMagicObject`, `TerrainTile` e assemblies não declara `NavMeshAgent`, `NavMeshSurface`, `NavigationRegion3D` ou `NavigationAgent3D` como parte obrigatória do core.

Portanto:

- `Spline Pathfinding` pode produzir uma linha baseada no heightmap;
- a linha pode alimentar Stroke, Stamp, Scatter ou Spline Output;
- navegação de personagem precisa ser adicionada pelo projeto/engine;
- em Unity isso pode ser AI Navigation/NavMesh;
- em Godot isso pode ser NavigationRegion/NavigationAgent.

Não existe no código analisado uma etapa automática declarada que faça bake de NavMesh depois de cada tile. Se um projeto precisar disso, essa integração deve ser implementada e testada separadamente.
