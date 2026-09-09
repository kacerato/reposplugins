# WaterSystem — pipeline, assets, shaders e mobile

## Pipeline da cópia local

O pacote analisado contém `Scripts/Standard` e `KWS_asmdef.asmdef` com apenas o nome `KWS`. O código usa tipos do renderer clássico/Standard, `Camera.onPreRender`, `Camera.onPostRender`, `CommandBuffer`, `LightEvent`, `RenderTexture`, `ComputeShader` e utilitários próprios de RT handle.

O código ainda contém comentários TODO para adicionar defines de `KWS_STANDARD/KWS_HDRP/KWS_URP` na classe principal, enquanto há uma pasta `Standard`. Portanto, não se deve concluir compatibilidade universal com URP/HDRP apenas porque o Asset Store hoje possui listagens separadas. A cópia local precisa ser validada no pipeline concreto.

Uma listagem pública atual do produto Standard identifica versões posteriores à cópia local; isso reforça a necessidade de manter a análise deste repositório ancorada em `Version 1.4.03` do README local, não em recursos de uma versão online diferente. [KWS Water System — Standard Rendering no Asset Store](https://assetstore.unity.com/packages/tools/particles-effects/kws-water-system-standard-rendering-191771) e [KWS Water System — URP Rendering](https://assetstore.unity.com/packages/tools/particles-effects/kws-water-system-urp-rendering-203144) são referências externas de produto, não prova de que esta cópia local contenha os mesmos arquivos.

## Shaders e nomes observados

O código carrega shaders por nomes `KWS_ShaderConstants`/`KWS_Settings` e usa recursos de água, compute shaders e materiais temporários. O README também declara explicitamente o shader de mask:

```text
KriptoFX/Water/KW_WaterHoleMask
```

Para ignorar a renderização da água dentro de um barco, o README manda criar uma mesh mask e usar esse shader. A mesh mask é fornecida pelo projeto; o WaterSystem não cria uma geometria de barco.

## Importadores customizados

Em `Scripts/Core/Editor/CustomImporter.cs` existem dois `ScriptedImporter`s:

### `TextureImporter`

- extensão: `.kwsTexture`;
- lê arquivo gzip/binário com versão `0`;
- reconstrói `Texture2D`, wrap/filter, mip e raw data;
- pode comprimir automaticamente conforme target e canais;
- para targets móveis (`Android`, `Switch`, `iOS`), escolhe formatos como ASTC/EAC; para desktop usa BC4/BC5/BC6H/BC7 conforme HDR/canais.

Isso é pipeline de importação do Editor, não um componente da Hierarchy. A escolha final depende do `BuildTarget` selecionado.

### `ComputeBufferImporter`

- extensão: `.kwsComputeBuffer`;
- lê dados gzip/binários;
- suporta `FormatUint`, `FormatUint2`, `FormatUint3`, `FormatUint4`;
- cria `ComputeBufferObject` como objeto importado;
- o componente expõe arrays ocultos e `GetComputeBuffer()` cria o buffer em runtime.

Se o arquivo estiver corrompido, a importação falha; não trate `.kwsTexture` como uma Texture2D comum editável pelo Inspector.

## Compatibilidade com hardware

O código detecta capacidades para recursos específicos:

- tessellation só é permitida quando `SystemInfo.graphicsShaderLevel >= 46`;
- fast shoreline foam é desligado se atomics não forem suportados;
- o backend usa compute shaders e render textures;
- o importador escolhe compressão diferente para mobile.

Isso não é uma matriz oficial de aparelhos. Para Android, valide Vulkan/OpenGL ES, formatos de textura, compute support, memória e custo de passes no dispositivo real.

## Qualidade e perfis

`WaterProfileEnum` declara `Custom`, `Ultra`, `High`, `Medium`, `Low` e `PotatoPC`. Cada grupo do Inspector tem um perfil de performance como `ReflectionProfile`, `FlowmapProfile`, `MeshProfile` etc. O código em `KWS_EditorProfiles` fornece perfis para Reflection, Color/Refraction, Flowing, DynamicWaves, Shoreline, Foam, VolumetricLight, Caustic, Mesh e Rendering.

Use `Low/Medium` para validar lógica primeiro. Depois aumente a resolução específica que traz ganho visual; não habilite SSR, planar, volumetric, caustics, FFT grande, fluids e tessellation simultaneamente sem perfil de custo.

## Custos importantes

- FFT e dynamic waves usam render textures/compute;
- fluid simulation multiplica custo por iterações e tamanho de textura;
- SSR depende da resolução relativa à tela;
- planar/cubemap criam renderizações auxiliares;
- volumetric lighting usa iterações e blur;
- caustic usa textura, mesh, LOD e depth bake;
- quadtree/mesh detalhada aumentam triângulos;
- `KWS_AddLightToWaterRendering` pode adicionar trabalho de shadowmap por luz;
- `KW_Buoyancy` aumenta custo com slices/voxels.

## Mobile-first: limites práticos

Para a engine mobile do projeto:

1. teste o WaterSystem no aparelho Android alvo, não somente na Scene View;
2. comece com `Flowmap`, `Dynamic Waves`, `Foam`, `Volumetric` e `Caustic` desligados;
3. reduza FFT, mesh quality, SSR resolution e shadow settings;
4. limite quantidade de WaterSystems visíveis;
5. use compressão apropriada ao GPU/target;
6. observe temperatura, memória, draw calls, bandwidth e frame time;
7. escolha fallback quando shader/compute/tessellation não for suportado.

Esses passos são recomendações arquiteturais para o projeto. O pacote local fornece alguns mecanismos de fallback/detecção, mas não declara um preset Android universal.

## Dependências opcionais da demo

O README menciona projetos open-source para volumetric lighting/clouds nas demos:

- [VolumetricLights](https://github.com/SlightlyMad/VolumetricLights)
- [VolumeCloud](https://github.com/yangrc1234/VolumeCloud)

Esses repositórios são referências incluídas na demo; não aparecem como referência no asmdef `KWS` e não devem ser tratados como dependência de toda instalação.
