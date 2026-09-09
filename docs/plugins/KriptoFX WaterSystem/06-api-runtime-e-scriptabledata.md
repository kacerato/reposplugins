# WaterSystem — API runtime, callbacks e ScriptableData

## Namespace

Os scripts do pacote usam o namespace `KWS`. O tipo principal é `KWS.WaterSystem`.

## Consultar a superfície

Para obter posição e normal da água em uma posição mundial:

```csharp
using KWS;
using UnityEngine;

public static class WaterQueries
{
    public static bool TryGetSurface(Vector3 position, out Vector3 surface, out Vector3 normal)
    {
        var data = WaterSystem.GetWaterSurfaceData(position);
        surface = data.Position;
        normal = data.Normal;
        return data.IsActualDataReady;
    }
}
```

`GetWaterSurfaceData` é estático e verifica as instâncias visíveis. Para rio, consulta a spline; para os demais modos, usa a altura FFT. Durante a atualização assíncrona, `IsActualDataReady` pode ser falso e a posição pode ser o valor solicitado, com normal de fallback. Não transforme um resultado não pronto em estado “submerso” sem decidir o fallback do gameplay.

O próprio objeto também possui `GetCurrentWaterSurfaceData(Vector3 worldPosition)`, que consulta a instância atual. Use esse método quando a referência à instância já estiver disponível; use o estático para deixar o sistema procurar entre instâncias.

## Detectar submersão

```csharp
if (WaterSystem.IsPositionUnderWater(transform.position))
{
    // transição para swimming/submerged é responsabilidade do projeto
}

bool sphereUnderwater = WaterSystem.IsSphereUnderWater(transform.position, 0.75f);
```

`IsPositionUnderWater` procura a instância cuja `WorldSpaceBounds` contém a posição. `IsSphereUnderWater` consulta pontos ao redor do centro. `WaterSystem.IsCameraUnderwater` é estado da câmera renderizada e possui setter privado.

## Renderização manual

```csharp
water.IsWaterRenderingActive = false;
// ... água em caverna/oclusão lógica ...
water.IsWaterRenderingActive = true;
```

O README local descreve esse campo para controlar manualmente a renderização. Isso não desativa necessariamente toda lógica de settings, física ou dados; o código consumidor deve testar o efeito no caso alvo.

## Tempo de rede

```csharp
water.UseNetworkTime = true;
water.NetworkTime = synchronizedSeconds;
```

O campo é usado para sincronizar animação temporal entre clientes. Definir `UseNetworkTime` não implementa transporte de rede, relógio autoritativo ou rollback; a fonte de `synchronizedSeconds` pertence ao jogo.

## Evento `OnWaterRender`

`WaterSystem.OnWaterRender` é um `Action` chamado quando a água visível é renderizada. Assine/desassine respeitando o lifecycle:

```csharp
void OnEnable()  => water.OnWaterRender += OnWaterRender;
void OnDisable() => water.OnWaterRender -= OnWaterRender;

void OnWaterRender()
{
    // trabalho curto; não faça alocação pesada por frame sem medir
}
```

## Dados persistidos

### `WaterSystemScriptableData`

É o perfil/estado completo de color, waves, reflection, refraction, flowing, dynamic waves, shoreline, foam, volumetric lighting, caustic, underwater, mesh e rendering. O Inspector usa `Profile` como asset e `Settings` como instância em execução/editor.

### `FlowingScriptableData`

Declara:

```csharp
int AreaSize;
Vector3 AreaPosition;
int FlowmapResolution;
Texture2D FlowmapTexture;
Texture2D FluidsMaskTexture;
Texture2D FluidsPrebakedTexture;
```

### `ShorelineWavesScriptableData`

Possui `List<ShorelineWave> Waves`. Cada wave serializa `WorldMatrix`, `WaveID`, `Position`, `EulerRotationY`, `Size`, `TimeOffset`, `Scale`, `Flip` e `Pad`. O construtor e `UpdateMatrix()` usam `Matrix4x4.TRS`.

### `SplineScriptableData`

Possui `List<Spline> Splines`. Cada `Spline` tem `ID`, `SplinePoints`, `VertexCountBetweenPoints` e `Depth`; cada `SplinePoint` tem `ID`, `WorldPosition` e `Width`.

## Criar shoreline em runtime

O README local mostra este padrão:

```csharp
var data = ScriptableObject.CreateInstance<ShorelineWavesScriptableData>();
for (int i = 0; i < 100; i++)
{
    var wave = new ShorelineWavesScriptableData.ShorelineWave(
        typeID: 0,
        pos,
        rotationY,
        scale,
        timeOffset,
        flip);
    wave.UpdateMatrix();
    data.Waves.Add(wave);
}
water.Settings.ShorelineWavesScriptableData = data;
water.ForceUpdateWaterSettings();
```

O README original contém um typo (`I < 100` em vez de `i < 100`) e usa nomes abreviados; o exemplo acima corrige somente a sintaxe e qualifica o tipo. `typeID` precisa corresponder a um tipo de wave existente no pacote; o material local não declara uma lista pública completa desses IDs.

## Alterar settings por script

O código declara `ForceUpdateWaterSettings()` e documenta que ele deve ser chamado depois de alterar parâmetros:

```csharp
water.Settings.Transparent = 5f;
water.Settings.UseVolumetricLight = false;
water.ForceUpdateWaterSettings();
```

A chamada força `UpdateState()`. Não altere muitos campos por frame sem medir; preferencialmente agrupe alterações e atualize uma vez.

## `KW_Buoyancy` e `KW_InteractWithWater`

Esses componentes são a API de composição recomendada pelo pacote para gameplay físico/interativo. `KW_Buoyancy` exige Rigidbody/Collider e obtém a superfície via sistema interno; `KW_InteractWithWater` registra o movimento do objeto para dynamic waves. Eles não transformam automaticamente um personagem em controlador de natação.

## API que não foi declarada

Não foi encontrado no código local um contrato público para: criar WaterSystem por código com todos os assets; gerar NavMesh; criar CharacterController; sincronizar rede; importar modelos; ou converter um Terrain. Essas responsabilidades continuam no projeto consumidor.
