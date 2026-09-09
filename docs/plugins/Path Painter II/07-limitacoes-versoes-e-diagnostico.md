# Path Painter II — limitações, versões e diagnóstico

## Divergência de versão mínima

Há duas declarações locais que precisam ser preservadas:

- o manual PDF é identificado como v2.1.2 e possui uma seção para Unity antigo, mencionando suporte a Unity 2018.3/2018.4 com runtime scripting .NET 3.5 Equivalent;
- o `Version Log.txt` na entrada v2.1.5 diz: “Minimum Unity version is now 2019.4 as per Unity requirements”.

Como o material local contém DLLs de variantes `000`/`010`, mas não traz uma tabela textual que associe a DLL à versão do Editor, não é correto concluir uma versão mínima única para qualquer instalação. Para este projeto, use a declaração do `Version Log` v2.1.5 como a mais recente e valide a importação no Unity concreto; não use o trecho antigo do manual para justificar compatibilidade sem teste.

## Limitações declaradas

- terrains não quadrados não são suportados pelo Path Painter;
- para pintar vizinhos, Width/Length e resoluções de heightmap/control texture/details precisam ser compatíveis;
- mapas com baixa resolução não conseguem representar caminhos finos;
- `Paint` ignora `Y`; `Paint3D` usa `Y` e ignora `evenRamp`;
- Auto Ramp pode alterar uma trajetória 3D se o Slope Limit ainda limitar a inclinação;
- o runtime API não é garantido para toda plataforma/desempenho;
- o Path Painter normalmente é editor-only e precisa ser incluído explicitamente no build para uso de API runtime;
- layers com configurações diferentes podem ser duplicadas durante Texture Auto Propagation;
- alterações de TerrainData podem ser grandes e devem ser testadas com backup.

## Mensagens e sintomas

### Menu não aparece

Espere a importação/recompilação. Se continuar ausente, reimporte o pacote. Confirme se as DLLs e os recursos estão na pasta correta e se a versão do Unity satisfaz a declaração do `Version Log`.

### Caminho não acompanha o relevo

Verifique `Terrain Follow`/`evenRamp`, a ordem dos pontos e se você está usando `Paint` ou `Paint3D`. `Paint` ignora Y; se a intenção era seguir alturas explícitas, use `Paint3D`.

### Caminho fica muito serrilhado ou não tem detalhe

Confira `Heightmap Resolution` e `Control Texture Resolution` no Terrain. Use MapScaler se a escala do mundo for grande demais para a resolução atual. Faça isso antes de ampliar Brush Size esperando que a resolução crie detalhe.

### Rampa fica diferente do esperado

Confira Slope Limit, direção dos pontos, Elevation e Auto Ramp. Para aprendizado, use `90`; para gameplay, ajuste abaixo da inclinação tolerada pelo controlador. Em `Paint3D`, use `90` quando precisar respeitar exatamente os Ys fornecidos, desde que o terreno/algoritmo permita.

### Textura aparece quando Texturing deveria estar desligado

Confira o toggle `Texturing`, as layers selecionadas e se está editando um stroke antigo no `Edit Mode`. O histórico registra correções para casos em que texturing desligado ainda pintava em certas circunstâncias; se o comportamento continuar, reduza o caso a um Terrain e registre a versão/Console.

### Vizinhos falham ou ficam desalinhados

Compare formato quadrado, Width/Length, Heightmap Resolution, Control Texture Resolution, Detail Resolution, posições de grade e Grouping IDs. O histórico registra correções justamente para grupos incompatíveis, tiles ausentes e offsets de resoluções divergentes.

### Vegetação não aparece enquanto pinto

Isso pode ser visualização normal: o manual informa que trees/grass podem ficar ocultos durante a pintura. Confira `Draw Vegetation` depois do stroke e confirme se `Vegetation Clearing` estava ligado ou desligado conforme a intenção.

### API não entra no build

Abra `Path Painter II Build Settings` e inclua o plugin/runtime. Confirme que a chamada está em um assembly de runtime válido e que a referência da `PathPainter.dll` escolhida pelo pacote foi importada. Não copie DLLs entre `000` e `010` por conta própria.

## Histórico relevante do pacote

O `Version Log` local registra, entre outros pontos:

- v2.1.10: correção relacionada a custom importer em versões Unity recentes; clean install pode ser necessário ao atualizar versão antiga;
- v2.1.9: correção do tratamento de caps em `Paint3D` line;
- v2.1.7: otimização adaptativa para pinturas grandes e correção de detalhes com resolução zero;
- v2.1.6: Grouping IDs, validação de tiles, offsets e Auto Ramp;
- v2.1.5: Unity mínimo 2019.4, tooltips de layer e demo atualizada;
- v2.1.4: solução temporária para bug do Unity que afetava build Android;
- v2.1.2: estrutura lean para build, Build Settings, API/Undo e suporte ao caso Unity 2018.3/2018.4 descrito acima;
- v2.1.0b3/b4: Auto Ramp, API XML, line painting e correções de visualização/GC.

## O que não foi encontrado

No escopo local examinado não foi encontrada uma lista oficial que diga que o Path Painter:

- cria NavMesh automaticamente;
- cria collider ou controlador de personagem;
- atualiza MapMagic automaticamente;
- oferece um componente `MonoBehaviour` de cena;
- exige Cinemachine, Post Processing ou um pacote Godot;
- possui uma janela de prioridades/priority tab.

Esses pontos devem ser tratados como integração/proposta do projeto, não como funcionalidade declarada do plugin.
