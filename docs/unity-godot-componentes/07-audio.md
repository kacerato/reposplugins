# 07 — áudio, listeners, buses e mixer

## Unity

### AudioSource

**O que faz:** emite um AudioClip. Controla volume, pitch, loop, play on awake, spatial blend, min/max distance, spread, doppler e prioridade.

**Onde fica:** GameObject que emite som.

**Precisa de:** AudioClip ou clip criado em runtime; AudioListener em algum ponto da cena; AudioMixerGroup opcional.

**Processo:** o source lê clip; mixer aplica volume/efeitos; listener recebe o som conforme posição; sistema espacial calcula atenuação e Doppler.

### AudioListener

Ponto de audição. Normalmente fica na Main Camera; deve haver um listener ativo para ouvir áudio 3D.

### AudioClip

Asset de áudio importado. Formato, compressão, load type e sample rate alteram memória/latência.

### AudioMixer

Roteia fontes por grupos e aplica volume, EQ, compressor, reverb e snapshots.

### AudioMixerGroup

Canal do mixer para música, SFX, voz, ambiente ou veículo.

### AudioMixerSnapshot

Estado salvo do mixer; permite transição entre gameplay, pausa, underwater e menu.

### AudioReverbZone

Volume espacial que modifica reverb conforme posição do listener/source.

### Microphone API

Captura áudio de entrada; é API, não componente de cena. Exige permissões e tratamento de plataforma.

## Godot

### AudioStreamPlayer

Reproduz áudio não-posicional, como música e UI.

### AudioStreamPlayer2D

Reproduz áudio posicional em coordenadas 2D.

### AudioStreamPlayer3D

Reproduz áudio espacial 3D com atenuação, direção, Doppler e filtro low-pass.

**Onde fica:** Node3D do objeto emissor.

**Precisa de:** `AudioStream`; bus válido; listener atual ou câmera padrão.

### AudioListener3D

Listener 3D selecionável com `make_current`. Sem ele, a câmera normalmente serve como ponto de audição.

### AudioStream

Resource de áudio importado ou gerado. `AudioStreamGenerator` permite áudio procedural por script.

### AudioBus

Canal global de mixagem. Configurado no projeto, com volume, mute, solo e efeitos.

### AudioEffect

Efeitos de bus: reverb, delay, compressor, chorus, EQ, distortion e limiter, conforme versão.

## Processo de áudio de um objeto gerado

```text
Prefab/PackedScene
  ├── AudioSource/AudioStreamPlayer3D
  └── clip + bus
          ↓
      listener atual
          ↓
      atenuação/mixer/efeitos
          ↓
      saída do dispositivo
```

## MapMagic e áudio

MapMagic não exige áudio para gerar Terrain. Para objetos procedurais, um prefab pode conter áudio de vento, árvore, máquina ou veículo.

Arquitetura recomendada:

- `VehicleComponent` produz RPM/estado;
- `VehicleAudioComponent` consome esse estado;
- AudioSource/AudioStreamPlayer3D apenas reproduz;
- não colocar física e mixer dentro de um único script.

## Dependências e erros

- listener ausente = silêncio;
- bus com nome inexistente = áudio roteado incorretamente;
- clip carregado em memória inteira = consumo alto;
- AudioSource dentro de prefab pooled precisa parar/resetar no unload;
- Android exige testar foco, pausa, volume e interrupções do sistema.

## Tutorial de validação

### Unity

1. Adicione AudioListener na câmera.
2. Crie AudioSource com clip curto.
3. Ative spatial blend 3D.
4. Mova câmera pelo emissor.
5. Crie AudioMixer com grupos Music/SFX.
6. Use snapshot de pausa.

### Godot

1. Crie AudioStreamPlayer3D.
2. Atribua AudioStream.
3. Crie bus SFX.
4. Adicione AudioEffect.
5. Adicione AudioListener3D e torne-o current.
6. Mova câmera e valide atenuação/Doppler.

## Limite de declaração do MapMagic

O bundle v2.1.11 não declara `AudioSource`, `AudioListener`, `AudioMixer`, `AudioStreamPlayer` ou um mixer próprio como dependência de geração. Áudio só entra se um prefab, uma cena demo ou um sistema externo do projeto o utilizar.

Sequência que pode ser afirmada sem inventar integração:

1. Objects/Trees Output escolhe/aplica um prefab ou árvore;
2. se esse asset possuir componente de áudio, a engine de áudio o processa;
3. listener, clip, bus/mixer e lifecycle são responsabilidade da Unity/Godot e do projeto.

O MapMagic não gera sons de vento, motor ou ambiente automaticamente no core v2.1.11.

## Fontes

- [Unity AudioSource](https://docs.unity3d.com/Manual/class-AudioSource.html)
- [Unity Audio Mixer](https://docs.unity3d.com/Manual/AudioMixerOverview.html)
- [Godot AudioStreamPlayer3D](https://docs.godotengine.org/en/stable/classes/class_audiostreamplayer3d.html)
- [Godot Audio buses](https://docs.godotengine.org/en/stable/tutorials/audio/audio_buses.html)
