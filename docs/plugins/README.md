# Documentação por plugin

Esta pasta contém a documentação técnica dos plugins encontrados no projeto. Cada plugin possui sua própria pasta para que o manual, a API, a lista de componentes, as interfaces do Inspector/Editor e as dependências permaneçam junto do plugin que foi analisado.

## Catálogo geral Unity/Godot

O catálogo geral de componentes Unity, nodes Godot, uso, sequência, necessidades e interfaces do Inspector está em [docs/unity-godot/README.md](../unity-godot/README.md). Ele é separado dos manuais específicos dos plugins porque descreve as APIs das engines e suas equivalências.

## Plugins documentados

| Plugin | Pasta | Evidência principal |
|---|---|---|
| MapMagic 2 Bundle v2.1.11 | [MapMagic 2 Bundle v2.1.11](MapMagic%202%20Bundle%20v2.1.11/README.md) | Bundle local, scripts, assemblies, XML e documentação do pacote |
| Path Painter II | [Path Painter II](Path%20Painter%20II/README.md) | Manual PDF local, Version Log, DLL/XML e demos |
| KriptoFX WaterSystem | [KriptoFX WaterSystem](KriptoFX%20WaterSystem/README.md) | README local v1.4.03, scripts C#, shaders, asmdef e cena de demo |

O relatório técnico completo do MapMagic, com dependências Unity, graph, outputs, objetos, splines, biomes, Brush, assets e diagnóstico, está em [14-relatorio-tecnico-completo.md](MapMagic%202%20Bundle%20v2.1.11/14-relatorio-tecnico-completo.md).

## Regra de leitura

- **Confirmado no pacote:** nome, tipo, campo, enum, método, botão, menu, shader, asset ou dependência que aparece no arquivo local analisado.
- **Declarado pelo manual:** comportamento descrito no manual/README do próprio plugin.
- **Não declarado:** não foi encontrado como requisito no escopo local examinado; não significa que seja impossível.
- **Inferência:** relação deduzida a partir do código, sempre identificada como tal.
- **Proposta:** adaptação sugerida para a engine mobile/Godot; não é uma API original do plugin.

As páginas evitam transformar uma possibilidade de integração em requisito. Quando existe conflito entre a documentação e a versão atual do pacote, o conflito fica explícito.
