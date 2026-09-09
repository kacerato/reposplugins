# Path Painter II — runtime, build e exemplos completos

## Editor versus runtime

O modo normal é usar o Path Painter dentro do Editor para alterar `TerrainData` e salvar a cena. A API `Painter` também pode ser usada em runtime, mas o pacote foi otimizado para que a parte do plugin não entre no build por padrão. O `Version Log` v2.1.2 declara explicitamente a janela de Build Settings para escolher essa inclusão.

### Checklist de runtime

1. confirme que a chamada realmente precisa acontecer depois que o jogo inicia;
2. abra `Window -> 3D Haven -> Path Painter II -> Path Painter II Build Settings`;
3. inclua o Path Painter/runtime no build;
4. confirme que a assembly `PathPainter` está disponível para o assembly do script;
5. confirme que os Terrains e TerrainLayers existem no jogador;
6. teste em cada plataforma-alvo, especialmente Android;
7. meça tempo de pintura, memória e alterações do TerrainData.

O manual afirma que o runtime API não é garantido para todas as plataformas/perfis de desempenho e recomenda teste no dispositivo alvo. Não se deve assumir que uma pintura pesada seja adequada a cada frame.

## Exemplo: callback de Undo no editor

O callback recebe o Terrain que precisa ser registrado:

```csharp
using UnityEditor;
using UnityEngine;
using Haven.API.PathPainter2;

public static class EditorPaintWithUndo
{
    public static void Paint(Vector3[] points)
    {
        var painter = new Painter(terrain =>
        {
            Undo.RegisterCompleteObjectUndo(terrain.terrainData, "Path Painter terrain change");
        });

        painter.SetDefaults();
        painter.size = 4f;
        painter.embankmentSize = 18f;
        painter.shaping = true;
        painter.textureStrength = 1f;
        painter.Paint(points);
    }
}
```

Esse callback usa a API do Unity Editor; não o leve para um assembly de jogador. O pacote fornece o callback para facilitar a integração de Undo, mas o modo exato de registrar a operação deve ser compatível com o fluxo de Undo da versão de Unity do projeto.

## Exemplo: terreno relativo com opções parciais

```csharp
var painter = new Painter();
painter.SetDefaults();
painter.shaping = true;
painter.elevation = -1.5f; // leito relativo ao terreno

var options = new[]
{
    Painter.Size(8f),
    Painter.EmbankmentSize(30f),
    Painter.SlopeLimit(15f),
    Painter.EvenRamp(0.5f),
    Painter.ClearGrass(true),
    Painter.GrassClearingDistance(0.4f),
    Painter.GrassThinningDistance(0.9f),
    Painter.ClearTree(true),
    Painter.TreeClearingDistance(0.5f)
};

painter.Paint(points, options);
```

`points` precisa ser `List<Vector3>` ou `Vector3[]` em espaço mundial. `EvenRamp` é normalizado: `0` acompanha o terreno e `1` uniformiza a rampa. O exemplo não seleciona textures; sem `Texture`/`TerrainLayer`, a parte de texturing deve permanecer desligada ou sem layer, conforme a configuração atual.

## Exemplo: trajetória 3D

```csharp
var painter = new Painter();
painter.SetDefaults();
painter.shaping = true;
painter.slopeLimit = 90f; // evita que Auto Ramp substitua os Ys fornecidos
painter.Paint3D(new[]
{
    new Vector3(0, 12, 0),
    new Vector3(20, 13, 5),
    new Vector3(40, 18, 16)
});
```

Use `Paint3D` quando cada ponto tem uma altura que deve conduzir a superfície. A documentação do XML declara que `evenRamp` não participa de `Paint3D` e que `slopeLimit` ainda pode intervir.

## Exemplo: várias pinturas em um grupo

```csharp
using System;

var painter = new Painter();
if (!painter.StartBulkPaint(new Vector3(100, 0, 100)))
    throw new InvalidOperationException("Terrains incompatíveis para bulk painting");

try
{
    painter.Paint(mainPath);
    painter.Paint(branchPath, Painter.Size(3f));
    painter.Paint3D(riverBed);
}
finally
{
    painter.EndBulkPaint();
}
```

Faça bulk somente depois de validar o grupo de Terrain. `try/finally` é uma proteção do código consumidor para fechar o estado; não é uma exigência textual do XML, mas é uma sequência segura para não deixar a operação aberta em caso de exceção.

## Exemplo: line painting incremental

```csharp
var painter = new Painter();
painter.NewLine(firstPoint,
    Painter.Size(6f),
    Painter.EmbankmentSize(20f));

foreach (var point in recordedPoints)
    painter.AddToLine(point);

painter.CompleteLine();
```

O método `NewLine` inicia um estado de linha; `AddToLine` acrescenta pontos; `CompleteLine` encerra. A API foi incluída para pintura ponto a ponto, por exemplo para registrar o movimento do mouse, conforme o histórico do pacote.

## Cuidados de performance

- não chame pintura pesada em todo `Update()` sem medir;
- agrupe pontos e use bulk quando o caso for multiterrain;
- evite recalcular resoluções/heightmaps sem necessidade;
- faça uma pequena área de teste antes de um mapa inteiro;
- no Android, meça memória, tempo de CPU/GPU e travamentos térmicos;
- lembre que `TerrainData` é um asset grande e a alteração pode gerar picos.

## Integração com outros sistemas

Path Painter não declara integração automática com NavMesh, física, veículo, personagem ou MapMagic. Depois de pintar um caminho, esses sistemas podem precisar de sua própria atualização/bake. Essa etapa é uma responsabilidade do projeto, não uma função confirmada do plugin.
