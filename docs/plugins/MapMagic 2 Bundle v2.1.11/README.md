# MapMagic 2 Bundle v2.1.11 — documentação modular

## Objetivo

Esta pasta contém a análise do MapMagic 2 Bundle v2.1.11 e o catálogo Unity/Godot usado para explicar o fluxo do plugin. Os documentos estão separados por categoria para que cada sistema — terreno, renderização, assets, Inspector, editor, runtime e mobile — possa ser consultado isoladamente.

Uma lista absolutamente literal de todos os tipos da API de duas engines depende da versão e passa de milhares de classes. A documentação abaixo cobre os componentes de cena, recursos e serviços que participam diretamente de um projeto de engine/terrain. Cada categoria possui sua própria explicação, fluxo, localização no editor, dependências, exemplos e tutorial de teste.

As categorias também possuem uma seção de **Interface de propriedades e Inspector**. Ela informa se o elemento aparece na `Hierarchy`/`Scene Tree`, no `Project`/`FileSystem`, em uma janela própria, na `Scene View`/viewport ou somente por API/runtime. Para o MapMagic, o documento de editor lista as interfaces customizadas realmente encontradas no bundle v2.1.11.

## Como a informação foi classificada

Para evitar completar lacunas com suposições, os documentos usam esta separação:

- **Documentação oficial:** comportamento descrito nas referências oficiais do Unity, Godot ou MapMagic.
- **Código confirmado:** comportamento observado no bundle local `MapMagic 2 Bundle v2.1.11`, especialmente em `MapMagicObject`, `TerrainTile`, assemblies e arquivos de editor.
- **Não declarado:** componente, pacote, função ou fluxo que não foi encontrado como requisito no escopo analisado. A ausência não significa que seja incompatível; significa somente que não pode ser afirmado como dependência do MapMagic sem examinar o graph, output, prefab ou integração correspondente.
- **Proposta:** desenho sugerido para uma implementação em Godot ou na engine própria. Uma classe proposta não é um componente existente do Godot nem uma classe original do MapMagic.

Cada exemplo deve ser lido dentro dessa classificação. Os trechos de Unity/Godot explicam o uso documentado das engines; os trechos do MapMagic limitam-se ao que foi declarado pela documentação ou confirmado no pacote v2.1.11.

## Mapa dos documentos

| Documento | Categoria |
|---|---|
| [00-fundamentos.md](00-fundamentos.md) | GameObject, Node, Transform, scripts, prefabs, cenas e identidade |
| [01-terreno.md](01-terreno.md) | Terrain, TerrainData, TerrainLayer, heightmap, chunks e equivalente Godot |
| [02-renderizacao.md](02-renderizacao.md) | Mesh, renderer, materiais, shaders, LOD, culling e instancing |
| [03-fisica.md](03-fisica.md) | RigidBody, colliders, CharacterController, corpos Godot e colisões |
| [04-navegacao.md](04-navegacao.md) | NavMesh, NavigationRegion, agents, obstacles e pathfinding |
| [05-camera-iluminacao.md](05-camera-iluminacao.md) | Câmeras, lights, ambiente, reflexos, probes e pós-processamento |
| [06-animacao.md](06-animacao.md) | Animator, AnimationPlayer, skeletons, state machines e Timeline |
| [07-audio.md](07-audio.md) | AudioSource, AudioListener, AudioStreamPlayer, buses e mixer |
| [08-particulas.md](08-particulas.md) | ParticleSystem, GPUParticles, CPUParticles e forças |
| [09-ui-input.md](09-ui-input.md) | UI, Canvas, Control, EventSystem, Input System e touchscreen |
| [10-recursos-serializacao.md](10-recursos-serializacao.md) | Assets, Resources, ScriptableObject, importadores, cenas e save |
| [11-editor-ferramentas.md](11-editor-ferramentas.md) | Inspector, EditorWindow, plugins, gizmos, graph editor e undo |
| [12-lifecycle-performance-mobile.md](12-lifecycle-performance-mobile.md) | Lifecycle, threads, jobs, memória, Android e frame budget |
| [13-equivalencias-mapmagic.md](13-equivalencias-mapmagic.md) | Arquitetura para transportar MapMagic para Godot/engine própria |

## Regra de classificação

- Unity `Component` = classe anexada a um `GameObject`.
- Unity `Resource`/asset = dado serializado reutilizável, não necessariamente presente na cena.
- Godot `Node` = elemento da Scene Tree, normalmente equivalente a GameObject + componente especializado.
- Godot `Resource` = dado serializado reutilizável.
- API/service = serviço global ou chamada de baixo nível; não deve ser confundido com componente visual.

## Fontes-base

- [Unity — Introduction to components](https://docs.unity3d.com/Manual/Components.html)
- [Unity — TerrainData](https://docs.unity3d.com/ScriptReference/TerrainData.html)
- [Unity — Terrain Layer](https://docs.unity3d.com/Manual/class-TerrainLayer.html)
- [Godot — Node3D](https://docs.godotengine.org/en/stable/classes/class_node3d.html)
- [Godot — World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html)
- [Godot — All classes](https://docs.godotengine.org/en/stable/classes/index.html)
- [MapMagic — wiki oficial](https://gitlab.com/denispahunov/mapmagic/-/wikis/home)
- [MapMagic — repositório oficial](https://gitlab.com/denispahunov/mapmagic)

## Como testar cada documento

Todos os tutoriais devem ser executados primeiro em uma cena mínima, com Console limpo, um objeto de teste, uma câmera e iluminação básica. Depois repetir no Android/Player e medir memória, frame time, draw calls e estabilidade.
