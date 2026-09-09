# Path Painter II — API completa confirmada no XML

## Namespace e instanciação

O XML da assembly `PathPainter` declara:

```csharp
using Haven.API.PathPainter2;
```

O objeto principal é `Painter`:

```csharp
Painter painter = new Painter();
Painter painterWithUndo = new Painter(RecordUndoAction);
```

O construtor com callback recebe uma `Action<UnityEngine.Terrain>`. O callback é chamado para que o projeto registre Undo nos Terrains afetados. O XML também declara `Init(Action<Terrain>)` para inicializar/reinicializar esse callback.

## Estado do Painter

As propriedades/campos declarados no XML são:

| Membro | Tipo/intervalo documentado | Função |
|---|---|---|
| `size` | `float` | largura da superfície sem embankment |
| `embankmentSize` | `float` | largura total, incluindo embankment; não pode ser menor que `size` |
| `shaping` | `bool` | liga/desliga remodelagem do terreno |
| `elevation` | `float` | elevação relativa; negativo cava |
| `slopeLimit` | `float`, `0–90` | Auto Ramp; `0` nivelado, `90` sem limite |
| `embankmentCurve` | `Painter.EmbankmentCurve` | forma de mistura do embankment |
| `evenRamp` | `float`, normalizado `0–1` | `0` segue terreno; `1` deixa a rampa uniforme entre início/fim |
| `textureStrength` | `float`, `0–1` | força da pintura de textura |
| `texture` | `TerrainLayer` | layer da superfície; `null` desliga texturing |
| `embankmentTexture` | `TerrainLayer` | layer do embankment |
| `smartTexturePaint` | `bool` | controla a aplicação inteligente; desligue para pintar embankment sobre superfície já texturizada |
| `clearGrass` | `bool` | limpa/desbasta Terrain Details |
| `grassClearingDistance` | `0.01–1` normalizado | distância de limpeza de details como razão de um lado do embankment |
| `grassThinningDistance` | `0.01–1` normalizado | distância final de thinning |
| `grassClearingNoise` | `Painter.Noise` | noise do thinning |
| `clearTree` | `bool` | limpa árvores |
| `treeClearingDistance` | `0.01–1` normalizado | distância de limpeza das árvores |
| `smoothPath` | `bool` | usa pintura suave quando ligado |

`SetDefaults()` restaura as opções padrão da instância. O XML documenta que o estado é reutilizado entre chamadas; por isso, se uma pintura precisa ser isolada de outra, configure explicitamente os campos ou chame `SetDefaults()` antes.

## `Paint` — pontos relativos ao terreno

Overloads confirmados:

```csharp
void Paint(List<Vector3> points);
void Paint(Vector3[] points);
void Paint(List<Vector3> points, params PaintOption[] options);
void Paint(Vector3[] points, params PaintOption[] options);
void Paint(List<Vector3> points, float size, float embankmentSize,
           Painter.EmbankmentCurve embankmentCurve, bool shaping,
           float slopeLimit, float elevation, float evenRamp,
           float textureStrength, TerrainLayer texture,
           TerrainLayer embankmentTexture, bool smartTexturePaint,
           bool clearGrass, float grassClearingDistance,
           float grassThinningDistance, Painter.Noise grassClearingNoise,
           bool clearTree, float treeClearingDistance, bool smoothPath);
```

Existe overload equivalente com `Vector3[]` para a assinatura longa. Nos overloads `Paint`, os pontos são posições em espaço mundial e os valores `Y` são ignorados. A altura da superfície é calculada em relação ao Terrain segundo `evenRamp` e `elevation`.

Exemplo mínimo:

```csharp
using System.Collections.Generic;
using UnityEngine;
using Haven.API.PathPainter2;

public static class PaintExample
{
    public static void MakePath(List<Vector3> points)
    {
        var painter = new Painter();
        painter.size = 5f;
        painter.embankmentSize = 25f;
        painter.shaping = true;
        painter.slopeLimit = 30f;
        painter.elevation = 0f;
        painter.Paint(points);
    }
}
```

O exemplo só é operacional se a assembly estiver referenciada, houver Terrain atingido pelos pontos e o build/editor estiver configurado para incluir a API conforme a necessidade. A documentação não declara que `Paint` cria Terrain ou TerrainLayer automaticamente em todos os casos; layers devem existir/ser selecionáveis no Terrain.

## `Paint3D` — pontos com altura explícita

Overloads confirmados:

```csharp
void Paint3D(List<Vector3> points);
void Paint3D(Vector3[] points);
void Paint3D(List<Vector3> points, params PaintOption[] options);
void Paint3D(Vector3[] points, params PaintOption[] options);
void Paint3D(List<Vector3> points, float size, float embankmentSize,
             Painter.EmbankmentCurve embankmentCurve, bool shaping,
             float slopeLimit, float elevation, float textureStrength,
             TerrainLayer texture, TerrainLayer embankmentTexture,
             bool smartTexturePaint, bool clearGrass,
             float grassClearingDistance, float grassThinningDistance,
             Painter.Noise grassClearingNoise, bool clearTree,
             float treeClearingDistance, bool smoothPath);
```

Há assinatura equivalente com `Vector3[]`. Em `Paint3D`, os valores `Y` dos pontos são usados para a superfície. O XML declara que `evenRamp` é ignorado nessa modalidade; `Auto Ramp` ainda pode substituir inclinações muito íngremes, então use `slopeLimit = 90f` se a altura fornecida precisar ser respeitada sem esse limite.

## `PaintOption`

O XML declara o tipo `PaintOption`, o enum aninhado `PaintOption.PaintOptionType`, propriedade `Type`, construtor `(PaintOptionType, object)` e `ApplyOptions(Painter, PaintOption[])`. A intenção é mudar apenas algumas configurações para uma chamada:

```csharp
// Os métodos-fábrica abaixo são membros estáticos declarados em Painter no XML.
var options = new[]
{
    Painter.Size(5f),
    Painter.EmbankmentSize(25f),
    Painter.SlopeLimit(30f),
    Painter.SmoothPath(true)
};

painter.Paint(points, options);
```

Os métodos-fábrica declarados no XML são membros estáticos de `Painter`: `Size`, `EmbankmentSize`, `EmbankCurve`, `Elevation`, `Shaping`, `SlopeLimit`, `EvenRamp`, `TextureStrength`, `Texture`, `EmbankmentTexture`, `SmartTexturePaint`, `ClearGrass`, `GrassClearingDistance`, `GrassThinningDistance`, `GrassClearingNoise`, `ClearTree`, `TreeClearingDistance` e `SmoothPath`. Eles produzem `PaintOption` para as overloads com `params PaintOption[]`. Os tipos dos argumentos devem seguir os campos correspondentes; não passe `string` no lugar de `TerrainLayer`/enum.

## Bulk painting

O XML declara:

```csharp
bool StartBulkPaint(Vector3 targetTerrainsAt);
void EndBulkPaint();
```

Sequência:

1. chame `StartBulkPaint` com uma posição em espaço mundial;
2. verifique o `bool` retornado;
3. faça qualquer número de `Paint()`/`Paint3D()` compatível;
4. chame `EndBulkPaint()` mesmo se o conjunto tiver uma única pintura.

`StartBulkPaint` retorna `false` quando não consegue bloquear o grupo de tiles alvo ou quando faltam propriedades de Terrain compatíveis. O XML/manual descreve que o bulk opera em um grupo compatível de Terrains. Line painting não é a modalidade indicada dentro de bulk.

## Line painting

Métodos confirmados:

```csharp
void NewLine(Vector3 point);
void NewLine3D(Vector3 point);
void AddToLine(Vector3 point);
void CompleteLine();
```

Também existem overloads de `NewLine`/`NewLine3D` com assinatura longa e `params PaintOption[]`. `NewLine` ignora `Y`; `NewLine3D` mantém a altura explicitamente informada. Exemplo:

```csharp
var painter = new Painter();
painter.NewLine(new Vector3(0, 0, 0));
painter.AddToLine(new Vector3(10, 0, 0));
painter.AddToLine(new Vector3(20, 0, 5));
painter.CompleteLine();
```

É possível completar várias linhas, mas complete a linha atual antes de fazer uma pintura que não seja line painting. O `Version Log` diz que a API fornece warnings quando usada incorretamente e tenta inferir a intenção do usuário; warnings não devem ser tratados como prova de que uma chamada incorreta é segura.
