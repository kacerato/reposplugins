# WaterSystem — Flowmap, fluid simulation, rio e shoreline

## Flowmap Painter

### Preparação

1. Selecione o `WaterSystem`.
2. Na aba `Flowing`, habilite `UseFlowMap`.
3. Ative o botão `Flowmap Painter`.
4. Defina `FlowMapAreaPosition` e `FlowMapAreaSize`; o código ajusta Y para a altura da água.
5. Escolha `Flowmap resolution` (`512`, `1024`, `2048` ou `4096`), `Flow Speed` e `Brush Strength`.
6. A Scene View mostra a área wireframe e o círculo do pincel.

### Pintura

1. Segure o botão esquerdo e arraste para desenhar a direção do fluxo.
2. Segure `Ctrl` + botão esquerdo para apagar/usar erase mode.
3. Use a roda do mouse para alterar o raio do pincel.
4. Pressione `Save All`.

O código `KWS_EditorFlowmap` desenha o círculo em ciano e usa vermelho no modo `Ctrl`. O fluxo é convertido em posição/direção no plano da água ou projetado na superfície do rio.

### Botões

| Botão | Efeito |
|---|---|
| `Flowmap Painter` | ativa/desativa o modo de edição e inicializa recursos ao ligar |
| `Load Latest Saved` | carrega dados salvos; pede confirmação |
| `Delete All` | limpa dados do flowmap; pede confirmação |
| `Save All` | serializa a textura/dados atuais |
| `Flowing Data` | ObjectField para `FlowingScriptableData`; trocar o asset reinitializa o flowmap |

Os dados são salvos em `Assets/KriptoFX/WaterSystem/WaterResources/Resources/SavedData/WaterID`, conforme o README local. O ID aparece na aba/Inspector como `Water unique ID: <cena>.<código>`.

## Fluid Simulation

É uma simulação 2D adicional habilitada com `UseFluidsSimulation`. O README local declara explicitamente que ela calcula fluxo dinâmico ao redor de objetos estáticos.

### Sequência declarada

1. Ative `Flowmap Painter`.
2. Desenhe a direção do fluxo.
3. Use `Save All`.
4. Pressione `Bake Fluids Obstacles`.
5. Aguarde o bake; o Inspector mostra percentual quando disponível.

O editor bloqueia a operação se não houver flowmap salvo e mostra aviso. O bake salva depth/máscaras e a simulação prebaked no `FlowingScriptableData`. Os campos expert `FluidsSimulationIterrations` e `FluidsTextureSize` aumentam custo; o editor calcula e mostra pixels renderizados como indicador “less is better”.

### Limite

O README diz “static objects only”. Isso significa que o bake/fluxo em torno de obstáculos dinâmicos não é uma capacidade declarada dessa ferramenta. Não confunda fluid simulation com `KW_InteractWithWater`, que gera ondas locais em objetos móveis.

## River Spline Editor

Quando `WaterMeshType = River`, a aba Mesh oferece `River Editor`. O código mantém `SplineScriptableData.Spline` e `SplinePoint` em memória e atualiza a mesh.

### Sequência

1. Escolha `Mesh -> Render Mode -> River`.
2. Ative `River Editor`.
3. Pressione `Add River`.
4. Clique no ground para criar o ponto inicial.
5. Segure `Shift` + clique esquerdo para adicionar pontos.
6. Segure `Ctrl` + clique esquerdo para remover o ponto selecionado.
7. Use o `Scale Tool` ou a tecla `R` para alterar a largura.
8. Adicione ao menos três pontos.
9. Mantenha os pontos aproximadamente equidistantes e evite curvas fortes.
10. Pressione `Save Changes`.

O código rejeita pontos muito próximos com aviso de distância mínima, mantém largura do ponto anterior em novos pontos e atualiza a mesh ao terminar o arrasto. Handles de movimento permitem deslocar X/Z; a altura do ponto é projetada no ground mais `RiverSplineNormalOffset`.

### Controles da aba

| Controle | Uso |
|---|---|
| `Spline Normal Offset` | offset normal dos pontos |
| `Selected Spline Depth` | profundidade da spline selecionada |
| `Selected Spline Vertex Count` | vértices entre pontos da spline |
| `Add River` | inicia modo de adicionar spline |
| `Delete Selected River` | pede confirmação e remove a spline selecionada |
| `Save Changes` | salva em `SplineScriptableData` quando há alterações |
| `Spline Data` | ObjectField; trocar o asset recarrega e reconstrói as splines |

O editor desenha Bezier/handles. Cubos vermelhos marcam `EditorBadVertices`, que é um diagnóstico visual de interseções/artefatos da mesh.

## Shoreline Editor

### Sequência declarada no README

1. Ative `UseShorelineRendering`.
2. Entre no `Edit mode`.
3. Pressione `Add Wave` ou `Insert` na posição do cursor.
4. Selecione uma wave e pressione `Delete` para remover.
5. Use Move, Rotate e Scale como em qualquer GameObject.
6. Pressione `Save All`.

O código usa `ShorelineWavesScriptableData.ShorelineWave`, com `WaveID`, `Position`, `EulerRotationY`, `Size`, `TimeOffset`, `Scale`, `Flip` e `WorldMatrix`. A matriz é recalculada por `UpdateMatrix()` sempre que posição/rotação/escala são alteradas.

### Interface e limites

As waves aparecem como caixas/volumes de handle na Scene View. O modo usa o objeto selecionado por proximidade do cursor e bloqueia o click da Scene View para não selecionar outro objeto acidentalmente. A posição Y é forçada para o Y da água quando a wave é movida.

## Caustic depth editor

O editor de Caustic tem `Caustic Depth Scale`/modo de edição e desenha uma área ortográfica. `Bake Caustic Depth` gera o depth usado pela caustic. `CausticOrthoDepthPosition`, `CausticOrthoDepthAreaSize` e `CausticOrthoDepthTextureResolution` são os campos que definem a área/resolução. O código não declara um componente separado de caustic para a Hierarchy.

## Persistência

Flowmap, fluid data, shoreline waves e spline data ficam em `ScriptableObject`s associados ao `WaterInstanceID`. Duplicar/copiar um WaterSystem gera novo ID no editor e instancia settings separadamente, segundo o código de `CheckCopyPastAndReplaceInstanceID`. Não mova ou apague manualmente `Resources/SavedData/WaterID` sem aceitar que o editor possa perder esses dados.
