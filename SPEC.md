# uoc-for-c-sharp 仕様書

---

## リファレンス

- README: （準備中）
- UOCフォーマット仕様: https://gist.github.com/teaaa0406/188f4ec0639fde9834d7cb7ef057bf2c

---

## 目次

- [1. 全体像](#1-全体像)
- [2. 公開API仕様](#2-公開api仕様)
  - [2.1 Uoc](#21-uoc)
    - [UocObject](#uocobject)
    - [UocString](#uocstring)
  - [2.2 Uoc.Parse](#22-uocparse)
    - [UocParser](#uocparser)
    - [UocBuilder](#uocbuilder)
  - [2.3 Uoc.Analyze](#23-uocanalyze)
    - [AnalysisSetting](#analysissetting)
    - [2.3.1 Uoc.Analyze.Playback](#231-uocanalyzeplayback)
      - [ChartPlaybackData](#chartplaybackdata)
      - [NotePlaybackProvider](#noteplaybackprovider)
      - [NoteGroupPlaybackProvider](#notegroupplaybackprovider)
    - [2.3.2 Uoc.Analyze.Speed](#232-uocanalyzespeed)
      - [BasicSpeed](#basicspeed)
      - [SpeedMultiplier](#speedmultiplier)
  - [2.4 Uoc.Chart](#24-uocchart)
    - [Bpm](#bpm)
    - [Layer](#layer)
    - [Tick](#tick)
    - [Tpb](#tpb)
    - [Position](#position)
    - [Distance](#distance)
    - [MeasureIndex](#measureindex)
    - [MeasureLength](#measurelength)
    - [2.4.1 Uoc.Chart.Event](#241-uocchartevent)
      - [EventProviders](#eventproviders)
      - [BpmProvider](#bpmprovider)
      - [BpmChangeEvent](#bpmchangeevent)
      - [MeasureLengthProvider](#measurelengthprovider)
      - [MeasureLengthChangeEvent](#measurelengthchangeevent)
      - [SpeedMultiplierProvider](#speedmultiplierprovider)
      - [SpeedMultiplierChangeEvent](#speedmultiplierchangeevent)
    - [2.4.2 Uoc.Chart.Notes](#242-uocchartnotes)
      - [NoteProfile](#noteprofile)
      - [NoteProfileCollection](#noteprofilecollection)
      - [NoteGroupProfile](#notegroupprofile)
      - [NoteGroupProfileCollection](#notegroupprofilecollection)
      - [NoteId](#noteid)
      - [NoteGroupId](#notegroupid)
      - [NoteGuid](#noteguid)
      - [Channel](#channel)
      - [ChannelProvider](#channelprovider)
      - [2.4.2.1 Uoc.Chart.Notes.Definition](#2421-uocchartnotesdefinition)
        - [NoteDef](#notedef)
        - [NoteDefCollection](#notedefcollection)
        - [NoteGroupDef](#notegroupdef)
        - [NoteGroupDefCollection](#notegroupdefcollection)
        - [NoteDefIndex](#notedefindex)
    - [2.4.3 Uoc.Chart.Property](#243-uocchartproperty)
      - [Property](#property)
      - [PropertyKey](#propertykey)
      - [PropertyValue](#propertyvalue)
      - [PropertyGroup](#propertygroup)
      - [ChartPropertyGroup](#chartpropertygroup)
- [3. 変更履歴](#3-変更履歴)

---

## 1. 全体像

- **Uoc（ルート）**：UOCを扱うための基本オブジェクト
- **Uoc.Parse**：UOC文字列のパース/構築
- **Uoc.Analyze**：再生向け派生情報の解析
- **Uoc.Chart**：譜面情報を表すドメインモデル（プロパティ/ノート/イベント等）

---

## 2. 公開API仕様

## 2.1 Uoc

### UocObject

- 概要：UOC文字列に含まれるすべての情報（譜面プロパティ/ノート定義/ノーツ情報等）を保持するクラス
- 利用方法：`UocParser` 経由で取得（new は不可）

#### プロパティ

| 名前                         | 型                           | アクセス | 内容                                   |
| ---------------------------- | ---------------------------- | -------- | -------------------------------------- |
| `ChartPropertyGroup`         | `ChartPropertyGroup`         | get      | 譜面プロパティ                         |
| `NoteDefCollection`          | `NoteDefCollection`          | get      | ノート定義コレクション                 |
| `NoteGroupDefCollection`     | `NoteGroupDefCollection`     | get      | ノートグループ定義コレクション         |
| `NoteProfileCollection`      | `NoteProfileCollection`      | get      | ノートプロファイルコレクション         |
| `NoteGroupProfileCollection` | `NoteGroupProfileCollection` | get      | ノートグループプロファイルコレクション |

#### コンストラクタ

- なし

#### メソッド

##### CreateChartPlaybackData

```csharp
public ChartPlaybackData CreateChartPlaybackData(AnalysisSetting analysisSetting)
```
- 役割：保持している情報群から譜面再生データを作成する
- 引数：
  - `analysisSetting`：解析設定
- 戻り値：
  - `ChartPlaybackData`：譜面再生データ

---

### UocString

- 概要：UOC文字列を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成

#### プロパティ

| 名前    | 型       | アクセス | 内容      |
| ------- | -------- | -------- | --------- |
| `Value` | `string` | get      | UOC文字列 |

#### コンストラクタ

##### UocString

```csharp
public UocString(string value)
```
- 役割：UOC文字列を保持するインスタンスを生成する
- 引数：
  - `value`：UOC文字列
- 例外/注意：
  - `value` が `null` / 空文字 / 空白のみの場合、`ArgumentException` を送出する

#### メソッド

- なし

---

## 2.2 Uoc.Parse

### UocParser

- 概要：UOC文字列のパースを行う静的クラス
- 利用方法：静的メソッドを直接使用

#### プロパティ

- なし
  
#### コンストラクタ

- なし

#### メソッド

##### Parse（Static）

```csharp
public static UocObject Parse(UocString uocString)
```
- 役割：UOC文字列をパースし、UocObjectを作成する
- 引数：
  - `uocString`：UOC文字列
- 戻り値：
  - `UocObject`：作成された `UocObject`

---

### UocBuilder

- 概要：UOC文字列の構築を行う静的クラス
- 利用方法：静的メソッドを直接使用

#### プロパティ

- なし

#### コンストラクタ

- なし

#### メソッド

##### Build（Static）

```csharp
public static UocString Build(string editorName, ChartPropertyGroup chartPropertyGroup, NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection)
```
- 役割：初期化時に与えられた情報からUOC文字列を構築する
- 引数：
  - `editorName`：ビルドに必要な情報群を作成したエディタの名称
    - 作成されたUOC文字列の先頭に記述される
  - `chartPropertyGroup`：譜面プロパティ
  - `noteDefCollection`：ノート定義コレクション
  - `noteGroupDefCollection`：ノートグループ定義コレクション
  - `noteProfileCollection`：ノートプロファイルコレクション
- 戻り値：
  - `UocString`：作成された `UocString`

---

## 2.3 Uoc.Analyze

### AnalysisSetting

- 概要：UOCオブジェクトの解析設定を保持するクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前                               | 型           | アクセス | 内容                                           |
| ---------------------------------- | ------------ | -------- | ---------------------------------------------- |
| `BasicSpeed`                       | `BasicSpeed` | get      | ノートの基本移動速度                           |
| `MinimumTiming`                    | `long`       | get      | 譜面の最小タイミング                           |
| `IgnoreSpeedChangesAfterJudgeLine` | `bool`       | get      | 判定ライン以降のスピード変動を無視するかどうか |
| `NotesInstantiationInterval`       | `int`        | get      | ノート生成タイミングの間隔(ミリ秒)             |
> 各プロパティの詳細な挙動はプログラム中のコメントを参照してください

#### コンストラクタ

##### AnalysisSetting

```csharp
public AnalysisSetting(BasicSpeed basicSpeed, long minimumTiming, bool ignoreSpeedChangesAfterJudgeLine, int notesInstantiationInterval)
```
- 役割：解析設定を保持するインスタンスを生成する
- 引数：
  - `basicSpeed`：ノートの基本移動速度
  - `minimumTiming`：譜面の最小タイミング
  - `ignoreSpeedChangesAfterJudgeLine`：判定ライン以降のスピード変動を無視するかどうか
  - `notesInstantiationInterval`：ノート生成タイミングの間隔(ミリ秒) 
- 例外/注意：
  - `notesInstantiationInterval` が `1` より小さい場合、`ArgumentOutOfRangeException` を送出する
  - `basicSpeed` が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

- なし

---

## 2.3.1 Uoc.Analyze.Playback

### ChartPlaybackData

- 概要：譜面の再生に必要な情報群を保持するクラス
- 利用方法：`UocObject` 経由で取得（new は不可）

#### プロパティ

| 名前                     | 型                       | アクセス | 内容                           |
| ------------------------ | ------------------------ | -------- | ------------------------------ |
| `NoteDefCollection`      | `NoteDefCollection`      | get      | ノート定義コレクション         |
| `NoteGroupDefCollection` | `NoteGroupDefCollection` | get      | ノートグループ定義コレクション |

#### コンストラクタ

- なし

#### メソッド

##### GetSingleNotePlaybackProviders

```csharp
public IReadOnlyList<NotePlaybackProvider> GetSingleNotePlaybackProviders()
```
- 役割：単体ノートの再生プロバイダのリストを取得する
  - いずれかのグループに所属するノートは含まれない
  - 生成タイミングで昇順
- 引数：
  - なし
- 戻り値：
  - `IReadOnlyList<NotePlaybackProvider>`：単体ノートの再生プロバイダのリスト

##### GetNoteGroupPlaybackProviders

```csharp
public IReadOnlyList<NoteGroupPlaybackProvider> GetNoteGroupPlaybackProviders()
```
- 役割：すべてのノートグループの再生プロバイダのリストを取得する
  - 生成タイミングで昇順
- 引数：
  - なし
- 戻り値：
  - `IReadOnlyList<NoteGroupPlaybackProvider>`：すべてのノートグループの再生プロバイダのリスト

---

### NotePlaybackProvider

- 概要：単体ノートの再生に関する情報を提供するクラス
- 利用方法：`ChartPlaybackData`, `NoteGroupPlaybackProvider` 経由で取得（new は不可）

#### プロパティ

| 名前                | 型              | アクセス | 内容                   |
| ------------------- | --------------- | -------- | ---------------------- |
| `NoteId`            | `NoteId`        | get      | ノートID               |
| `NoteProperties`    | `PropertyGroup` | get      | ノートが持つプロパティ |
| `InstantiateTiming` | `long`          | get      | ノートの生成タイミング |
| `EnabledTiming`     | `long`          | get      | ノートの有効タイミング |
| `NoteGuid`          | `NoteGuid`      | get      | ノートのGUID           |

#### コンストラクタ

- なし

#### メソッド

##### CalculateNotePosition

```csharp
public float CalculateNotePosition(long timing)
```
- 役割：指定されたタイミングからノートの位置を求める
- 引数：
  - `timing`：タイミング
- 戻り値：
  - `float`：ノートの位置
    - ノート生成位置を1、判定位置を0とし、それ以降は負の値をとる

---

### NoteGroupPlaybackProvider

- 概要：ノートグループの再生に関する情報を提供するクラス
- 利用方法：`ChartPlaybackData` 経由で取得（new は不可）

#### プロパティ

| 名前                     | 型                                    | アクセス | 内容                             |
| ------------------------ | ------------------------------------- | -------- | -------------------------------- |
| `NoteGroupId`            | `NoteGroupId`                         | get      | ノートグループID                 |
| `BelongsNotes`           | `IReadOnlyList<NotePlaybackProvider>` | get      | グループに所属するノートのリスト |
| `FirstInstantiateTiming` | `long`                                | get      | グループ始点の生成タイミング     |

#### コンストラクタ

- なし

#### メソッド

- なし

---

## 2.3.2 Uoc.Analyze.Speed

### BasicSpeed

- 概要：ノートの基本移動速度を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成

#### プロパティ

| 名前           | 型      | アクセス | 内容                                                                 |
| -------------- | ------- | -------- | -------------------------------------------------------------------- |
| `MoveDuration` | `float` | get      | ノートが生成位置から判定位置まで移動するのにかかる時間（ミリ秒単位） |

#### コンストラクタ

##### BasicSpeed

```csharp
public BasicSpeed(float moveDuration)
```
- 役割：ノートの基本移動速度を保持するインスタンスを生成する
- 引数：
  - `moveDuration`：ノートが生成位置から判定位置まで移動するのにかかる時間（ミリ秒単位）
- 例外/注意：
  - `moveDuration` が `0` 以下の場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

### SpeedMultiplier

- 概要：ノートの移動速度倍率を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<SpeedMultiplier>`

#### プロパティ

| 名前            | 型                | アクセス | 内容                                         |
| --------------- | ----------------- | -------- | -------------------------------------------- |
| `One`（Static） | `SpeedMultiplier` | get      | 倍率が `1` の `SpeedMultiplier` インスタンス |
| `Multiplier`    | `float`           | get      | ノートの移動速度倍率                         |

#### コンストラクタ

##### SpeedMultiplier

```csharp
public SpeedMultiplier(float multiplier)
```
- 役割：ノートの移動速度倍率を保持するインスタンスを生成する
- 引数：
  - `multiplier`：ノートの移動速度倍率

#### メソッド

- なし

---

## 2.4 Uoc.Chart

### Bpm

- 概要：BPM情報を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<Bpm>`

#### プロパティ

| 名前    | 型      | アクセス | 内容  |
| ------- | ------- | -------- | ----- |
| `Value` | `float` | get      | BPM値 |

#### コンストラクタ

##### Bpm

```csharp
public Bpm(float value)
```
- 役割：BPM値を保持するインスタンスを生成する
- 引数：
  - `value`：BPM値
- 例外/注意：
  - `value` が `0` 以下の場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

### Layer

- 概要：レイヤー情報を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<Layer>`

#### プロパティ

| 名前    | 型    | アクセス | 内容       |
| ------- | ----- | -------- | ---------- |
| `Value` | `int` | get      | レイヤー値 |

#### コンストラクタ

##### Layer

```csharp
public Layer(int value)
```
- 役割：レイヤー値を保持するインスタンスを生成する
- 引数：
  - `value`：レイヤー値
- 例外/注意：
  - `value` が `0` 未満の場合または `30` より大きい場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

### Tick

- 概要：Tick情報を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<Tick>`

#### プロパティ

| 名前    | 型    | アクセス | 内容   |
| ------- | ----- | -------- | ------ |
| `Value` | `int` | get      | Tick値 |

#### コンストラクタ

##### Tick

```csharp
public Tick(int value)
```
- 役割：Tick値を保持するインスタンスを生成する
- 引数：
  - `value`：Tick値
- 例外/注意：
  - `value` が `0` 未満の場合、`ArgumentOutOfRangeException` を送出する

##### Tick

```csharp
public Tick(float value)
```
- 役割：Tick値を保持するインスタンスを生成する
- 引数：
  - `value`：Tick値
- 例外/注意：
  - `value` が `0` 未満の場合、`ArgumentOutOfRangeException` を送出する
  - 小数点以下切り捨て

#### メソッド

- なし

---

### Tpb

- 概要：TPB（一拍の分解能）情報を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<Tpb>`

#### プロパティ

| 名前    | 型    | アクセス | 内容  |
| ------- | ----- | -------- | ----- |
| `Value` | `int` | get      | TPB値 |

#### コンストラクタ

##### Tpb

```csharp
public Tpb(int value)
```
- 役割：Tpb値を保持するインスタンスを生成する
- 引数：
  - `value`：Tpb値
- 例外/注意：
  - `value` が `0` 以下の場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

### Position

- 概要：譜面の位置情報を保持するクラス
- 利用方法：`new` で生成
- 実装：`IEquatable<Position>`, `IComparable<Position>`

#### プロパティ

| 名前                     | 型             | アクセス | 内容                                   |
| ------------------------ | -------------- | -------- | -------------------------------------- |
| `ChartStart`（Static）   | `Position`     | get      | 譜面の始点を表す位置                   |
| `MeasureStart`（Static） | `Position`     | get      | 引数で指定された小節内の始点を表す位置 |
| `MeasureIndex`           | `MeasureIndex` | get      | 小節番号                               |
| `Position01`             | `float`        | get      | 小節内での位置を 0~1 で表した値        |
| `SectionCount`           | `int`          | get      | 小節のセクション数                     |
| `ActiveIndex`            | `int`          | get      | 有効セクション位置（0始まり）          |

#### コンストラクタ

##### Position

```csharp
public Position(MeasureIndex measureIndex, int sectionCount, int activeIndex)
```
- 役割：譜面の位置情報を保持するインスタンスを生成する
- 引数：
  - `measureIndex`：小節番号
  - `sectionCount`：小節のセクション数
  - `activeIndex`：有効セクション位置（0始まり）
- 例外/注意：
  - `sectionCount` が `1` 未満の場合、`ArgumentOutOfRangeException` を送出する
  - `activeIndex` が `0` 未満の場合、`ArgumentOutOfRangeException` を送出する
  - `activeIndex` が `sectionCount` 以上の場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

##### IsMeasureStart

```csharp
public bool IsMeasureStart()
```
- 役割：位置が小節の始点であるかどうかを返す
- 引数：
  - なし
- 戻り値：
  - `bool`：位置が小節の始点であるかどうか

##### IsChartStart

```csharp
public bool IsChartStart()
```
- 役割：位置が譜面の始点であるかどうかを返す
- 引数：
  - なし
- 戻り値：
  - `bool`：位置が譜面の始点であるかどうか

##### CalculateTick

```csharp
public Tick CalculateTick(MeasureLength measureLength, Tpb tpb)
```
- 役割：位置のティックを計算する
- 引数：
  - `measureLength`：位置の小節長
  - `tpb`：TPB（一拍の分解能）
- 戻り値：
  - `Tick`：計算されたティック

##### CalculateTickInt

```csharp
public int CalculateTickInt(MeasureLength measureLength, Tpb tpb)
```
- 役割：位置のティックをint型で計算する
  - 小数点以下切り捨て
- 引数：
  - `measureLength`：位置の小節長
  - `tpb`：TPB（一拍の分解能）
- 戻り値：
  - `int`：計算されたティック（int）

##### AddDistance

```csharp
public Position AddDistance(Distance distance, MeasureLengthProvider measureLengthProvider)
```
- 役割：指定された距離をこの位置に加算した新しい `Position` を返す
- 引数：
  - `distance`：追加する距離
  - `measureLengthProvider`：小節長プロバイダ
- 戻り値：
  - `Position`：距離が加算された `Position` 

##### RecalculatePosition

```csharp
public Position RecalculatePosition(MeasureLengthProvider oldMeasureLengthProvider, MeasureLengthProvider newMeasureLengthProvider)
```
- 役割：小節長に変更があった際に、絶対的な位置が変動しないようPositionを再計算する
- 引数：
  - `oldMeasureLengthProvider`：変更前の小節長プロバイダ
  - `newMeasureLengthProvider`：変更後の小節長プロバイダ
- 戻り値：
  - `Position`：再計算された `Position` 

##### GetTotalQuarterNoteCount

```csharp
public float GetTotalQuarterNoteCount(MeasureLengthProvider measureLengthProvider)
```
- 役割：譜面位置までの四分音符の数を求める
- 引数：
  - `measureLengthProvider`：小節長プロバイダ
- 戻り値：
  - `float`：譜面位置までの四分音符の数

---

### Distance

- 概要：`Position` 同士の距離を表すクラス
- 利用方法：
  - `new` で生成
  - `CreateFromDifference` メソッドで生成
- 実装：`IEquatable<Distance>`

#### プロパティ

| 名前               | 型      | アクセス | 内容               |
| ------------------ | ------- | -------- | ------------------ |
| `QuarterNoteCount` | `float` | get      | 四分音符単位の距離 |

#### コンストラクタ

##### Distance

```csharp
public Distance(float quarterNoteCount)
```
- 役割：四分音符単位の距離を保持するインスタンスを生成する
- 引数：
  - `quarterNoteCount`：四分音符の数

#### メソッド

##### CreateFromDifference（Static）

```csharp
public static Distance CreateFromDifference(Position start, Position end, MeasureLengthProvider measureLengthProvider)
```
- 役割：指定された二点間の距離を持つ `Distance` インスタンスを作成する
- 引数：
  - `start`：始点
  - `end`：終点
  - `measureLengthProvider`：小節長プロバイダ
- 戻り値：
  - `Distance`：二点間の距離を持つ新たな `Distance` インスタンス

##### Absolute

```csharp
public Distance Absolute()
```
- 役割：保持する距離の絶対値を求め、新たな `Distance` オブジェクトとして返す
- 引数：
  - なし
- 戻り値：
  - `Distance`：保持する距離の絶対値を持つ新たな `Distance` インスタンス

---





### MeasureIndex

- 概要：小節番号値を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<MeasureIndex>`

#### プロパティ

| 名前    | 型    | アクセス | 内容     |
| ------- | ----- | -------- | -------- |
| `Value` | `int` | get      | 小節番号 |

#### コンストラクタ

##### MeasureIndex

```csharp
public MeasureIndex(int value)
```
- 役割：小節番号値を保持するインスタンスを生成する
- 引数：
  - `value`：小節番号値
- 例外/注意：
  - `value` が `0` 未満の場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

### MeasureLength

- 概要：小節長情報を保持するクラス
  - 分子は小節内の拍数、分母は1拍に相当する音符の種類を示す
- 利用方法：`new` で生成
- 実装：`IEquatable<MeasureLength>`

#### プロパティ

| 名前          | 型    | アクセス | 内容                                                      |
| ------------- | ----- | -------- | --------------------------------------------------------- |
| `Numerator`   | `int` | get      | 小節内の拍数（例: 4/4拍子なら4）                          |
| `Denominator` | `int` | get      | 1拍として扱う音符の種類（例: 4/4拍子なら4、4/2拍子なら2） |

#### コンストラクタ

##### MeasureLength

```csharp
public MeasureLength(int numerator, int denominator)
```
- 役割：小節長情報を保持するインスタンスを生成する
- 引数：
  - `numerator`：分子
  - `denominator`：分母
- 例外/注意：
  - いずれかの引数が `0` 以下の場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

##### GetBeatCount

```csharp
public int GetBeatCount()
```
- 役割：小節内の拍数を返す
- 引数：
  - なし
- 戻り値：
  - `int`：小節内の拍数

##### GetQuarterNoteCount

```csharp
public float GetQuarterNoteCount()
```
- 役割：小節内の四分音符換算の拍数を返す
- 引数：
  - なし
- 戻り値：
  - `float`：小節内の四分音符換算の拍数
    - 4/4拍子: 4 * (4/4) = 4
    - 4/2拍子: 4 * (4/2) = 8
    - 2/4拍子: 2 * (4/4) = 2
    - 2/2拍子: 2 * (4/2) = 4

---

## 2.4.1 Uoc.Chart.Event

### EventProviders

- 概要：各種イベントを提供するクラス
- 利用方法：`NoteProfileCollection` 経由で取得（new は不可）

#### プロパティ

| 名前                      | 型                        | アクセス | 内容                       |
| ------------------------- | ------------------------- | -------- | -------------------------- |
| `BpmProvider`             | `BpmProvider`             | get      | BPM情報プロバイダ          |
| `SpeedMultiplierProvider` | `SpeedMultiplierProvider` | get      | スピード倍率情報プロバイダ |
| `MeasureLengthProvider`   | `MeasureLengthProvider`   | get      | 小節長情報プロバイダ       |

#### コンストラクタ

- なし

#### メソッド

- なし

---

### BpmProvider

- 概要：BPM情報を提供するクラス
- 利用方法：
  - `EventProviders` 経由で取得（new は不可）
  - `NoteProfileCollection` 経由で作成（new は不可）

#### プロパティ

- なし

#### コンストラクタ

- なし

#### メソッド

##### GetMeasureStartBpm

```csharp
public Bpm GetMeasureStartBpm(int measureIndex)
```
- 役割：指定された小節の始点BPMを取得する
- 引数：
  - `measureIndex`：小節番号（単純化のため `int` 型）
- 戻り値：
  - `Bpm`：指定された小節の始点BPM

##### GetBpmChangeEventsAt

```csharp
public IReadOnlyList<BpmChangeEvent> GetBpmChangeEventsAt(int measureIndex)
```
- 役割：指定された小節内のBPM変動イベントリストを取得する
  - 小節始点の変動イベントは含まれない
- 引数：
  - `measureIndex`：小節番号（単純化のため `int` 型）
- 戻り値：
  - `IReadOnlyList<BpmChangeEvent>`：指定された小節内のBPM変動イベントリスト

---

### BpmChangeEvent

- 概要：単体のBPM変動イベントを表すクラス
- 利用方法：`BpmProvider` 経由で取得（new は不可）

#### プロパティ

| 名前           | 型             | アクセス | 内容                                 |
| -------------- | -------------- | -------- | ------------------------------------ |
| `MeasureIndex` | `MeasureIndex` | get      | 小節番号                             |
| `Bpm`          | `Bpm`          | get      | イベントが適用するBPM                |
| `Tick`         | `Tick`         | get      | イベントが適用される小節内のティック |

#### コンストラクタ

- なし

#### メソッド

- なし

---

### MeasureLengthProvider

- 概要：小節長情報を提供するクラス
- 利用方法：
  - `EventProviders` 経由で取得（new は不可）
  - `NoteProfileCollection` 経由で作成（new は不可）

#### プロパティ

- なし

#### コンストラクタ

- なし

#### メソッド

##### GetMeasureLengthAt

```csharp
public MeasureLength GetMeasureLengthAt(int measureIndex)
```
- 役割：指定された小節の小節長を取得する
- 引数：
  - `measureIndex`：小節番号（単純化のため `int` 型）
- 戻り値：
  - `MeasureLength`：指定された小節の小節長

---

### MeasureLengthChangeEvent

- 概要：単体の小節長変動イベントを表すクラス
- 利用方法：`MeasureLengthProvider` 経由で取得（new は不可）

#### プロパティ

| 名前            | 型              | アクセス | 内容                     |
| --------------- | --------------- | -------- | ------------------------ |
| `MeasureIndex`  | `MeasureIndex`  | get      | 小節番号                 |
| `MeasureLength` | `MeasureLength` | get      | イベントが適用する小節長 |

#### コンストラクタ

- なし

#### メソッド

- なし

---

### SpeedMultiplierProvider

- 概要：スピード倍率情報を提供するクラス
- 利用方法：
  - `EventProviders` 経由で取得（new は不可）
  - `NoteProfileCollection` 経由で作成（new は不可）

#### プロパティ

- なし

#### コンストラクタ

- なし

#### メソッド

##### GetMeasureStartSpeedMultiplier

```csharp
public SpeedMultiplier GetMeasureStartSpeedMultiplier(int measureIndex, Layer layer)
```
- 役割：指定された小節の始点スピード倍率を取得する
- 引数：
  - `measureIndex`：小節番号（単純化のため `int` 型）
  - `layer`：検索対象レイヤー
- 戻り値：
  - `SpeedMultiplier`：指定された小節の始点スピード倍率

##### GetSpeedMultiplierChangeEventsAt

```csharp
public IReadOnlyList<SpeedMultiplierChangeEvent> GetSpeedMultiplierChangeEventsAt(int startMeasureIndex, int endMeasureIndex, Layer layer)
```
- 役割：指定された小節範囲内のスピード変動イベントリストを取得する
  - 範囲開始小節始点の変動イベントは含まれない
- 引数：
  - `startMeasureIndex`：開始小節番号（範囲に含む）
  - `endMeasureIndex`：終了小節番号（範囲に含む）
  - `layer`：検索対象レイヤー
- 戻り値：
  - `IReadOnlyList<SpeedMultiplierChangeEvent>`：指定された小節範囲内のスピード変動イベントリスト

---

### SpeedMultiplierChangeEvent

- 概要：単体のスピード倍率変動イベントを表すクラス
- 利用方法：`SpeedMultiplierProvider` 経由で取得（new は不可）

#### プロパティ

| 名前              | 型                | アクセス | 内容                                 |
| ----------------- | ----------------- | -------- | ------------------------------------ |
| `MeasureIndex`    | `MeasureIndex`    | get      | 小節番号                             |
| `Layer`           | `Layer`           | get      | イベントが適用されるレイヤー         |
| `SpeedMultiplier` | `SpeedMultiplier` | get      | イベントが適用するスピード倍率       |
| `Tick`            | `Tick`            | get      | イベントが適用される小節内のティック |

#### コンストラクタ

- なし

#### メソッド

- なし

---

## 2.4.2 Uoc.Chart.Notes

### NoteProfile

- 概要：単体のノートを構成する情報を持つクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前            | 型              | アクセス | 内容         |
| --------------- | --------------- | -------- | ------------ |
| `NoteDef`       | `NoteDef`       | get      | ノート定義   |
| `Position`      | `Position`      | get      | ノートの位置 |
| `PropertyGroup` | `PropertyGroup` | get      | プロパティ値 |
| `Layer`         | `Layer`         | get      | レイヤー     |
| `Channel`       | `Channel`       | get      | チャンネル   |
| `NoteGuid`      | `NoteGuid`      | get      | ノートのGUID |

#### コンストラクタ

##### NoteProfile

```csharp
public NoteProfile(NoteDef noteDef, Position position, IReadOnlyList<string> propertyValues, Layer layer, Channel channel, NoteGuid guid)
```
- 役割：ノートの構成からインスタンスを生成する
- 引数：
  - `noteDef`：ノート定義
  - `position`：ノートの位置
  - `propertyValues`：プロパティ値のリスト
  - `layer`：レイヤー
  - `channel`：チャンネル
  - `guid`：ノートのGUID
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する

##### NoteProfile

```csharp
public NoteProfile(NoteDef noteDef, Position position, IReadOnlyList<string> propertyValues, Layer layer, Channel channel)
```
- 役割：ノートの構成からインスタンスを生成する
- 引数：
  - `noteDef`：ノート定義
  - `position`：ノートの位置
  - `propertyValues`：プロパティ値のリスト
  - `layer`：レイヤー
  - `channel`：チャンネル
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する
  - ランダムな `NoteGuid` が新規に生成される

#### メソッド

##### UpdatePosition

```csharp
public NoteProfile UpdatePosition(Position position)
```
- 役割：ノート位置を更新し、新しいインスタンスを返す
- 引数：
  - `position`：新しいノート位置
- 戻り値：
  - `NoteProfile`：ノート位置が更新された新しい `NoteProfile` インスタンス

##### UpdatePropertyGroup

```csharp
public NoteProfile UpdatePropertyGroup(PropertyGroup propertyGroup)
```
- 役割：プロパティグループを更新し、新しいインスタンスを返す
- 引数：
  - `propertyGroup`：新しいプロパティグループ
- 戻り値：
  - `NoteProfile`：プロパティグループが更新された新しい `NoteProfile` インスタンス


##### UpdateChannel

```csharp
public NoteProfile UpdateChannel(Channel channel)
```
- 役割：チャンネルを更新し、新しいインスタンスを返す
- 引数：
  - `channel`：新しいチャンネル
- 戻り値：
  - `NoteProfile`：チャンネルが更新された新しい `NoteProfile` インスタンス

---

### NoteProfileCollection

- 概要：複数の `NoteProfile` を管理するファーストコレクションクラス
- 利用方法：
  - `UocObject` 経由で取得（new は不可）
  - `CreateMinimum` メソッドで作成（new は不可）

#### プロパティ

| 名前           | 型                           | アクセス | 内容                                             |
| -------------- | ---------------------------- | -------- | ------------------------------------------------ |
| `NoteProfiles` | `IReadOnlyList<NoteProfile>` | get      | 管理するノートプロファイルのリスト（位置で昇順） |

#### コンストラクタ

- なし

#### メソッド

##### CreateMinimum（Static）

```csharp
public static NoteProfileCollection CreateMinimum(MeasureLength measureLength, Bpm bpm, SpeedMultiplier speedMultiplier, Layer layer)
```
- 役割：必要最低限の情報のみを含むノートプロファイルコレクションを作成する
- 引数：
  - `measureLength`：初期小節長
  - `bpm`：初期BPM
  - `speedMultiplier`：初期スピード倍率（推奨値：1.0）
  - `layer`：初期情報ノーツを配置するレイヤー
- 戻り値：
  - `NoteProfileCollection`：作成された `NoteProfileCollection` インスタンス

##### CreateNoteGroupProfileCollection

```csharp
public NoteGroupProfileCollection CreateNoteGroupProfileCollection(NoteGroupDefCollection noteGroupDefCollection)
```
- 役割：保持するノーツ情報から `NoteGroupProfileCollection` を作成する
- 引数：
  - `noteGroupDefCollection`：ノートグループ定義コレクション
- 戻り値：
  - `NoteGroupProfileCollection`：作成された `NoteGroupProfileCollection` インスタンス

##### GetNoteProfileByGuid

```csharp
public NoteProfile GetNoteProfileByGuid(NoteGuid guid)
```
- 役割：指定された `NoteGuid` を持つ `NoteProfile` を探索して返します
- 引数：
  - `guid`：探索するノートGUID
- 戻り値：
  - `NoteProfile`：指定された `NoteGuid` を持つ `NoteProfile` インスタンス
- 例外/注意：
  - 対象ノートが見つからなかった場合、`KeyNotFoundException` を送出する

##### PutOrReplace

```csharp
public NoteProfileCollection PutOrReplace(NoteProfile putNoteProfile)
```
- 役割：ノートを追加もしくは置換する
- 引数：
  - `putNoteProfile`：追加もしくは置換するノート
- 戻り値：
  - `NoteProfileCollection`：ノートが追加もしくは置換された新しい `NoteProfileCollection` インスタンス

##### PutOrReplace

```csharp
public NoteProfileCollection PutOrReplace(IReadOnlyList<NoteProfile> putNoteProfiles)
```
- 役割：ノートを追加もしくは置換する
- 引数：
  - `putNoteProfiles`：追加もしくは置換するノートリスト
- 戻り値：
  - `NoteProfileCollection`：ノートが追加もしくは置換された新しい `NoteProfileCollection` インスタンス

##### Remove

```csharp
public NoteProfileCollection Remove(NoteGuid guid)
```
- 役割：指定されたGUIDを持つノートを削除する
- 引数：
  - `guid`：削除対象ノートのノートGUID
- 戻り値：
  - `NoteProfileCollection`：ノートが削除された新しい `NoteProfileCollection` インスタンス

##### Remove

```csharp
public NoteProfileCollection Remove(IReadOnlyList<NoteGuid> guids)
```
- 役割：指定されたGUIDを持つノートを削除する
- 引数：
  - `guids`：削除対象ノートのノートGUIDリスト
- 戻り値：
  - `NoteProfileCollection`：ノートが削除された新しい `NoteProfileCollection` インスタンス

##### GetMaxMeasureIndex

```csharp
public MeasureIndex GetMaxMeasureIndex()
```
- 役割：譜面の最大小節番号を取得する
- 引数：
  - なし
- 戻り値：
  - `MeasureIndex`：譜面の最大小節番号

##### CreateEventProviders

```csharp
public EventProviders CreateEventProviders(Tpb tpb)
```
- 役割：イベントプロバイダ群を作成する
- 引数：
  - `tpb`：TPB（一拍の分解能）
- 戻り値：
  - `EventProviders`：イベントプロバイダ群

##### CreateMeasureLengthProvider

```csharp
public MeasureLengthProvider CreateMeasureLengthProvider()
```
- 役割：小節長プロバイダを作成する
- 引数：
  - なし
- 戻り値：
  - `MeasureLengthProvider`：小節長プロバイダ

##### CreateBpmProvider

```csharp
public BpmProvider CreateBpmProvider(MeasureLengthProvider measureLengthProvider, Tpb tpb)
```
- 役割：BPMプロバイダを作成する
- 引数：
  - `measureLengthProvider`：小節長プロバイダ
  - `tpb`：TPB（一拍の分解能）
- 戻り値：
  - `BpmProvider`：BPMプロバイダ

##### CreateSpeedMultiplierProvider

```csharp
public SpeedMultiplierProvider CreateSpeedMultiplierProvider(MeasureLengthProvider measureLengthProvider, Tpb tpb)
```
- 役割：スピード倍率プロバイダを作成する
- 引数：
  - `measureLengthProvider`：小節長プロバイダ
  - `tpb`：TPB（一拍の分解能）
- 戻り値：
  - `SpeedMultiplierProvider`：作成された `SpeedMultiplierProvider` インスタンス
---

### NoteGroupProfile

- 概要：単体のノートグループを構成する情報を持つクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前           | 型                           | アクセス | 内容                     |
| -------------- | ---------------------------- | -------- | ------------------------ |
| `NoteGroupDef` | `NoteGroupDef`               | get      | ノートグループ定義       |
| `BelongsNotes` | `IReadOnlyList<NoteProfile>` | get      | グループに所属するノーツ |

#### コンストラクタ

##### NoteGroupProfile

```csharp
public NoteGroupProfile(NoteGroupDef noteGroupDef, IReadOnlyList<NoteProfile> belongsNotes)
```
- 役割：ノートグループを構成する情報からインスタンスを生成する
- 引数：
  - `noteGroupDef`：ノートグループ定義
  - `belongsNotes`：グループに所属するノーツ
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

- なし

---

### NoteGroupProfileCollection

- 概要：複数の `NoteGroupProfile` を管理するファーストコレクションクラス
- 利用方法：
  - `NoteProfileCollection` 経由で取得（new は不可）

#### プロパティ

| 名前                | 型                                | アクセス | 内容                                       |
| ------------------- | --------------------------------- | -------- | ------------------------------------------ |
| `NoteGroupProfiles` | `IReadOnlyList<NoteGroupProfile>` | get      | 管理するノートグループプロファイルのリスト |

#### コンストラクタ

- なし

#### メソッド

##### TryGetNoteBelongingGroup

```csharp
public bool TryGetNoteBelongingGroup(NoteProfile note, [MaybeNullWhen(false)] out NoteGroupProfile noteGroup)
```
- 役割：指定されたノーツがいずれかのノートグループに所属している場合、そのノートグループを取得する
- 引数：
  - `note`：検索対象ノート
  - `noteGroup`：ノートが所属しているノートグループ（戻り値が `false` だった場合は `null`）
- 戻り値：
  - `bool`：指定されたノーツがいずれかのノートグループに所属していた場合は `true`

---

### NoteId

- 概要：ノートIDを保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<NoteId>`

#### プロパティ

| 名前    | 型       | アクセス | 内容     |
| ------- | -------- | -------- | -------- |
| `Value` | `string` | get      | ノートID |

#### コンストラクタ

##### NoteId

```csharp
public NoteId(string value)
```
- 役割：ノートIDを保持するインスタンスを生成する
- 引数：
  - `value`：ノートID
- 例外/注意：
  - `value` が `null` / 空文字 / 空白のみの場合、`ArgumentException` を送出する

#### メソッド

- なし

---

### NoteGroupId

- 概要：ノートグループIDを保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<NoteGroupId>`

#### プロパティ

| 名前    | 型       | アクセス | 内容             |
| ------- | -------- | -------- | ---------------- |
| `Value` | `string` | get      | ノートグループID |

#### コンストラクタ

##### NoteGroupId

```csharp
public NoteGroupId(string value)
```
- 役割：ノートグループIDを保持するインスタンスを生成する
- 引数：
  - `value`：ノートグループID
- 例外/注意：
  - `value` が `null` / 空文字 / 空白のみの場合、`ArgumentException` を送出する

#### メソッド

- なし

---

### NoteGuid

- 概要：ノートのGUIDを保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<NoteGuid>`

#### プロパティ

| 名前    | 型     | アクセス | 内容         |
| ------- | ------ | -------- | ------------ |
| `Value` | `Guid` | get      | ノートのGUID |

#### コンストラクタ

##### NoteGuid

```csharp
public NoteGuid(Guid guid)
```
- 役割：ノートのGUIDを保持するインスタンスを生成する
- 引数：
  - `guid`：ノートのGUID

#### メソッド

- なし

---

### Channel

- 概要：ノートのチャンネルを保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<Channel>`

#### プロパティ

| 名前              | 型        | アクセス | 内容                                            |
| ----------------- | --------- | -------- | ----------------------------------------------- |
| `Empty`（Static） | `Channel` | get      | チャンネル情報を持たない `Channel` インスタンス |
| `IsEmpty`         | `bool`    | get      | チャンネル情報を持っていないかどうか            |
| `Value`           | `int`     | get      | チャンネル値                                    |

#### コンストラクタ

##### Channel

```csharp
public Channel(int value)
```
- 役割：ノートのチャンネルを保持するインスタンスを生成する
- 引数：
  - `value`：ノートのチャンネル
- 例外/注意：
  - `value` が `0` 未満もしくは `1224` より大きい場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

### ChannelProvider

- 概要：ノートグループのチャンネルを提供するクラス
- 利用方法：`new` で生成

#### プロパティ

- なし

#### コンストラクタ

##### ChannelProvider

```csharp
public ChannelProvider(NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection)
```
- 役割：ノーツ情報からインスタンスを生成する
- 引数：
  - `noteGroupDefCollection`：ノートグループ定義コレクション
  - `noteProfileCollection`：ノートプロファイルコレクション
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

##### GetAvailableChannel

```csharp
public Channel GetAvailableChannel(Position startPosition, Position endPosition, Layer layer)
```
- 役割：指定された範囲内で利用可能なチャンネルを取得する
- 引数：
  - `startPosition`：範囲始点
  - `endPosition`：範囲終点
  - `layer`：対象レイヤー
- 戻り値：
  - `Channel`：指定された範囲内で利用可能なチャンネル

---

## 2.4.2.1 Uoc.Chart.Notes.Definition

### NoteDef

- 概要：単体のノート定義を表すクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前                            | 型                       | アクセス | 内容                               |
| ------------------------------- | ------------------------ | -------- | ---------------------------------- |
| `BpmChange`（Static）           | `NoteDef`                | get      | BPM変更ノートのノート定義          |
| `SpeedChange`（Static）         | `NoteDef`                | get      | スピード倍率変更ノートのノート定義 |
| `MeasureLengthChange`（Static） | `NoteDef`                | get      | 小節長変更ノートのノート定義       |
| `NoteId`                        | `NoteId`                 | get      | ノートID                           |
| `PropertyNames`                 | `IReadOnlyList<string>`  | get      | ノートプロパティ名のリスト         |
| `CommonNoteDefs`（Static）      | `IReadOnlyList<NoteDef>` | get      | 仕様で定義されたノート定義リスト   |

#### コンストラクタ

##### NoteDef

```csharp
public NoteDef(NoteId noteId, IReadOnlyList<string> propertyNames)
```
- 役割：ノート定義情報を保持するインスタンスを作成する
- 引数：
  - `noteId`：ノートID
  - `propertyNames`：ノートプロパティ名のリスト
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

- なし

---

### NoteDefCollection

- 概要：複数の `NoteDef` を管理するファーストコレクションクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前       | 型                       | アクセス | 内容                       |
| ---------- | ------------------------ | -------- | -------------------------- |
| `NoteDefs` | `IReadOnlyList<NoteDef>` | get      | 管理対象のノート定義リスト |

#### コンストラクタ

##### NoteDefCollection

```csharp
public NoteDefCollection(IReadOnlyList<NoteDef> noteDefs)
```
- 役割：ノート定義リストを保持するインスタンスを作成する
- 引数：
  - `noteDefs`：ノート定義リスト
- 例外/注意：
  - `noteDefs` が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

##### GetNoteDefByIndex

```csharp
public NoteDef GetNoteDefByIndex(NoteDefIndex index)
```
- 役割：ノート定義番号からノート定義を取得する
- 引数：
  - `index`：ノート定義番号
- 戻り値：
  - `NoteDef`：指定されたノート定義番号に対応する `NoteDef` インスタンス
- 例外/注意：
  - ノート定義が見つからなかった場合、`KeyNotFoundException` を送出する

##### GetNoteDefById

```csharp
public NoteDef GetNoteDefById(NoteId noteId)
```
- 役割：ノートIDからノート定義を取得する
- 引数：
  - `noteId`：ノートID
- 戻り値：
  - `NoteDef`：指定されたノートIDを持つ `NoteDef` インスタンス
- 例外/注意：
  - ノート定義が見つからなかった場合、`KeyNotFoundException` を送出する

##### GetNoteDefById

```csharp
public NoteDef GetNoteDefById(string noteId)
```
- 役割：ノートIDからノート定義を取得する
- 引数：
  - `noteId`：ノートID
- 戻り値：
  - `NoteDef`：指定されたノートIDを持つ `NoteDef` インスタンス
- 例外/注意：
  - ノート定義が見つからなかった場合、`KeyNotFoundException` を送出する

##### GetNoteDefIndexById

```csharp
public NoteDefIndex GetNoteDefIndexById(NoteId noteId)
```
- 役割：ノートIDからノート定義番号を取得する
- 引数：
  - `noteId`：ノートID
- 戻り値：
  - `NoteDefIndex`：指定されたノートIDに対応する `NoteDefIndex` インスタンス
- 例外/注意：
  - ノート定義番号が見つからなかった場合、`KeyNotFoundException` を送出する

---

### NoteGroupDef

- 概要：単体のノートグループ定義を表すクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前             | 型                      | アクセス | 内容                             |
| ---------------- | ----------------------- | -------- | -------------------------------- |
| `NoteGroupId`    | `NoteGroupId`           | get      | ノートグループID                 |
| `BelongsNoteIds` | `IReadOnlyList<NoteId>` | get      | グループに所属するノートIDリスト |
| `StartNoteId`    | `NoteId`                | get      | グループの始点となるノートID     |
| `EndNoteId`      | `NoteId`                | get      | グループの終点となるノートID     |

#### コンストラクタ

##### NoteGroupDef

```csharp
public NoteGroupDef(NoteGroupId noteGroupId, IReadOnlyList<NoteId> belongsNoteIds)
```
- 役割：ノートグループ定義情報を保持するインスタンスを作成する
- 引数：
  - `noteGroupId`：ノートグループID
  - `belongsNoteIds`：ノートグループに所属するノートのIDのリスト
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

- なし

---

### NoteGroupDefCollection

- 概要：複数の `NoteGroupDef` を管理するファーストコレクションクラス
- 利用方法：`new` で生成

#### プロパティ

| 名前            | 型                            | アクセス | 内容                               |
| --------------- | ----------------------------- | -------- | ---------------------------------- |
| `NoteGroupDefs` | `IReadOnlyList<NoteGroupDef>` | get      | 管理対象のノートグループ定義リスト |

#### コンストラクタ

##### NoteGroupDefCollection

```csharp
public NoteGroupDefCollection(IReadOnlyList<NoteGroupDef> noteGroupDefs)
```
- 役割：ノートグループ定義リストを保持するインスタンスを作成する
- 引数：
  - `noteGroupDefs`：ノートグループ定義リスト
- 例外/注意：
  - `noteGroupDefs` が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

##### GetNoteGroupById

```csharp
public NoteGroupDef GetNoteGroupById(NoteGroupId noteGroupId)
```
- 役割：ノートグループIDからノートグループ定義を取得する
- 引数：
  - `noteGroupId`：ノートグループID
- 戻り値：
  - `NoteGroupDef`：指定されたノートグループIDを持つ `NoteGroupDef` インスタンス

##### GetNoteGroupDefByStartNoteId

```csharp
public NoteGroupDef GetNoteGroupDefByStartNoteId(NoteId noteId)
```
- 役割：始点ノートIDからノートグループ定義を取得する
- 引数：
  - `noteId`：始点ノートID
- 戻り値：
  - `NoteGroupDef`：始点ノートが指定されたノートIDを持つ `NoteGroupDef` インスタンス

##### BelongsToAnyGroup

```csharp
public bool BelongsToAnyGroup(NoteId noteId)
```
- 役割：指定されたノートIDを持つノートがいずれかのグループに所属しているかどうかを返す
- 引数：
  - `noteId`：ノートID
- 戻り値：
  - `bool`：グループに所属している場合は `true`

##### BelongsToAnyGroup

```csharp
public bool BelongsToAnyGroup(string noteId)
```
- 役割：指定されたノートIDを持つノートがいずれかのグループに所属しているかどうかを返す
- 引数：
  - `noteId`：ノートID
- 戻り値：
  - `bool`：グループに所属している場合は `true`

---

### NoteDefIndex

- 概要：ノート定義番号を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成

#### プロパティ

| 名前    | 型    | アクセス | 内容           |
| ------- | ----- | -------- | -------------- |
| `Value` | `int` | get      | ノート定義番号 |

#### コンストラクタ

##### NoteDefIndex

```csharp
public NoteDefIndex(int value)
```
- 役割：ノート定義番号を保持するインスタンスを生成する
- 引数：
  - `value`：ノート定義番号
- 例外/注意：
  - `value` が `0` 未満もしくは `35` より大きい場合、`ArgumentOutOfRangeException` を送出する

#### メソッド

- なし

---

## 2.4.3 Uoc.Chart.Property

### Property

- 概要：プロパティを表すクラス
  - キー（`PropertyKey`）と値（`PropertyValue`）を持つ
- 利用方法：`new` で生成

#### プロパティ

| 名前            | 型              | アクセス | 内容           |
| --------------- | --------------- | -------- | -------------- |
| `PropertyKey`   | `PropertyKey`   | get      | プロパティキー |
| `PropertyValue` | `PropertyValue` | get      | プロパティ値   |

#### コンストラクタ

##### Property

```csharp
public Property(PropertyKey key, PropertyValue value)
```
- 役割：プロパティのキーと値を保持するインスタンスを生成する
- 引数：
  - `key`：プロパティキー
  - `value`：プロパティ値
- 例外/注意：
  - いずれかの引数が `null` の場合、`ArgumentNullException` を送出する

##### Property

```csharp
public Property(string key, string value)
```
- 役割：プロパティのキーと値を保持するインスタンスを生成する
- 引数：
  - `key`：プロパティキー
  - `value`：プロパティ値

#### メソッド

##### UpdateValue

```csharp
public Property UpdateValue(PropertyValue value)
```
- 役割：プロパティ値を更新し、新しいインスタンスを返す
- 引数：
  - `value`：新しいプロパティ値
- 戻り値：
  - `Property`：プロパティ値が更新された新しい `Property` インスタンス

---

### PropertyKey

- 概要：プロパティキー値を保持するクラス（値オブジェクト）
- 利用方法：`new` で生成
- 実装：`IEquatable<PropertyKey>`

#### プロパティ

| 名前    | 型       | アクセス | 内容             |
| ------- | -------- | -------- | ---------------- |
| `Value` | `string` | get      | プロパティキー値 |

#### コンストラクタ

##### PropertyKey

```csharp
public PropertyKey(string value)
```
- 役割：プロパティキー値を保持するインスタンスを生成する
- 引数：
  - `value`：プロパティキー値
- 例外/注意：
  - `value` が `null` / 空文字 / 空白のみの場合、`ArgumentException` を送出する
  - `value` に空白が含まれる場合、`ArgumentException` を送出する

#### メソッド

- なし

---

### PropertyValue

- 概要：プロパティ値を保持するクラス
- 利用方法：`new` で生成
- 実装：`IEquatable<PropertyValue>`

#### プロパティ

| 名前              | 型              | アクセス | 内容                                      |
| ----------------- | --------------- | -------- | ----------------------------------------- |
| `Empty`（Static） | `PropertyValue` | get      | 空の値を持つ `PropertyValue` インスタンス |

#### コンストラクタ

##### PropertyValue

```csharp
public PropertyValue()
```
- 役割：空のプロパティ値を保持するインスタンスを生成する
- 引数：
  - なし

##### PropertyValue

```csharp
public PropertyValue(string value)
```
- 役割：プロパティ値を保持するインスタンスを生成する
- 引数：
  - `value`：プロパティ値
- 例外/注意：
  - `value` が `null` / 空文字 / 空白のみの場合、`ArgumentException` を送出する

#### メソッド

##### AsString

```csharp
public string AsString()
```
- 役割：プロパティ値を `string` として取得する
- 引数：
  - なし
- 戻り値：
  - `string`：プロパティ値
- 例外/注意：
  - インスタンスがプロパティ値を持っていない場合、`InvalidOperationException` を送出する

##### AsInt

```csharp
public int AsInt()
```
- 役割：プロパティ値を `int` として取得する
- 引数：
  - なし
- 戻り値：
  - `int`：プロパティ値
- 例外/注意：
  - インスタンスがプロパティ値を持っていない場合、`InvalidOperationException` を送出する

##### AsFloat

```csharp
public float AsFloat()
```
- 役割：プロパティ値を `float` として取得する
- 引数：
  - なし
- 戻り値：
  - `float`：プロパティ値
- 例外/注意：
  - インスタンスがプロパティ値を持っていない場合、`InvalidOperationException` を送出する

##### AsBoolean

```csharp
public bool AsBoolean()
```
- 役割：プロパティ値を `bool` として取得する
- 引数：
  - なし
- 戻り値：
  - `bool`：プロパティ値
- 例外/注意：
  - インスタンスがプロパティ値を持っていない場合、`InvalidOperationException` を送出する

##### HasValue

```csharp
public bool HasValue()
```
- 役割：インスタンスがプロパティ値を持っているかどうかを取得する
- 引数：
  - なし
- 戻り値：
  - `bool`：インスタンスがプロパティ値を持っているかどうか

---

### PropertyGroup

- 概要：複数のプロパティをまとめて管理するクラス
- 利用方法：`new` で生成
- 実装：インデクサ（`public Property this[int index]`）

#### プロパティ

| 名前    | 型    | アクセス | 内容                   |
| ------- | ----- | -------- | ---------------------- |
| `Count` | `int` | get      | 保持するプロパティの数 |

#### コンストラクタ

##### PropertyGroup

```csharp
public PropertyGroup()
```
- 役割：プロパティを保持しないインスタンスを生成する
- 引数：
  - なし

##### PropertyGroup

```csharp
public PropertyGroup(IReadOnlyList<Property> properties)
```
- 役割：複数のプロパティを保持するインスタンスを生成する
- 引数：
  - `properties`：プロパティリスト
- 例外/注意：
  - `properties` が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

##### MergeKeysAndValues（Static）

```csharp
public static PropertyGroup MergeKeysAndValues(IReadOnlyList<string> keys, IReadOnlyList<string> values)
```
- 役割：キー配列と値配列から `PropertyGroup` を作成する
  - それぞれの配列の番号同士が対応する
- 引数：
  - `keys`：プロパティキー配列
  - `values`：プロパティ値配列
- 戻り値：
  - `PropertyGroup`：作成されたインスタンス
- 例外/注意：
  - `keys` と `values` の要素数が一致しない場合、 `ArgumentException` を送出する

##### CreateFromPropertyNames（Static）

```csharp
public static PropertyGroup CreateFromPropertyNames(IReadOnlyList<string> propertyNames)
```
- 役割：キーの配列から値を持たないプロパティグループを作成する
- 引数：
  - `propertyNames`：キーの配列
- 戻り値：
  - `PropertyGroup`：作成されたインスタンス

##### GetPropertyByKey

```csharp
public Property GetPropertyByKey(string key)
```
- 役割：指定されたキーに対応するプロパティを取得する
- 引数：
  - `key`：検索するキー
- 戻り値：
  - `Property`：キーに対応する `Property` インスタンス
- 例外/注意：
  - `key` に対応する `Property` インスタンスが存在しない場合、 `ArgumentException` を送出する

##### GetPropertyByKey

```csharp
public Property GetPropertyByKey(PropertyKey key)
```
- 役割：指定されたキーに対応するプロパティを取得する
- 引数：
  - `key`：検索するキー
- 戻り値：
  - `Property`：キーに対応する `Property` インスタンス
- 例外/注意：
  - `key` に対応する `Property` インスタンスが存在しない場合、 `ArgumentException` を送出する

##### HasKey

```csharp
public bool HasKey(string key)
```
- 役割：指定されたキーを持つプロパティが存在するかどうかを返す
- 引数：
  - `key`：検索するキー
- 戻り値：
  - `bool`：キーに対応するプロパティが存在するかどうか

##### HasKey

```csharp
public bool HasKey(PropertyKey key)
```
- 役割：指定されたキーを持つプロパティが存在するかどうかを返す
- 引数：
  - `key`：検索するキー
- 戻り値：
  - `bool`：キーに対応するプロパティが存在するかどうか

##### AllPropertiesHasValue

```csharp
public bool AllPropertiesHasValue()
```
- 役割：すべてのプロパティが値を持つかどうかを返す
- 引数：
  - なし
- 戻り値：
  - `bool`：すべてのプロパティが値を持つかどうか

##### AddOrUpdateProperty

```csharp
public PropertyGroup AddOrUpdateProperty(Property property)
```
- 役割：プロパティを追加または更新し、新しいインスタンスを返す
  - 同一キーを持つプロパティが存在しない場合、プロパティを追加する
  - 同一キーを持つプロパティがすでに存在している場合、そのプロパティの値を更新する
- 引数：
  - `property`：追加/更新するプロパティ
- 戻り値：
  - `PropertyGroup`：プロパティが更新された `PropertyGroup` インスタンス

##### AddOrUpdateProperties

```csharp
public PropertyGroup AddOrUpdateProperties(IReadOnlyList<Property> properties)
```
- 役割：複数のプロパティを追加または更新し、新しいインスタンスを返す
  - 同一キーを持つプロパティが存在しない場合、プロパティを追加する
  - 同一キーを持つプロパティがすでに存在している場合、そのプロパティの値を更新する
- 引数：
  - `properties`：追加/更新するプロパティリスト
- 戻り値：
  - `PropertyGroup`：プロパティが更新された `PropertyGroup` インスタンス

##### GetPropertyValueList

```csharp
public IReadOnlyList<string> GetPropertyValueList()
```
- 役割：すべてのプロパティの値を文字列のリストとして取得する
- 引数：
  - なし
- 戻り値：
  - `IReadOnlyList<string>`：プロパティ値文字列のリスト

### ChartPropertyGroup

- 概要：譜面が持つプロパティを管理するクラス
  - `PropertyGroup` クラスのラッパークラス
- 利用方法：`new` で生成
- 実装：インデクサ（`public Property this[int index]`）

#### プロパティ

| 名前            | 型              | アクセス | 内容                   |
| --------------- | --------------- | -------- | ---------------------- |
| `Count`         | `int`           | get      | 保持するプロパティの数 |
| `PropertyGroup` | `PropertyGroup` | get      | プロパティグループ     |

#### コンストラクタ

##### ChartPropertyGroup

```csharp
public ChartPropertyGroup(PropertyGroup propertyGroup)
```
- 役割：譜面プロパティを保持するインスタンスを生成する
- 引数：
  - `propertyGroup`：プロパティグループ
- 例外/注意：
  - `propertyGroup` が `null` の場合、`ArgumentNullException` を送出する

#### メソッド

##### GetPropertyByKey

```csharp
public Property GetPropertyByKey(string key)
```
- 役割：指定されたキーに対応するプロパティを取得する
- 引数：
  - `key`：検索するキー
- 戻り値：
  - `Property`：キーに対応する `Property` インスタンス

##### GetGameId

```csharp
public string GetGameId()
```
- 役割：ゲームIDを取得する
- 引数：
  - なし
- 戻り値：
  - `string`：ゲームID

##### GetTpb

```csharp
public Tpb GetTpb()
```
- 役割：TPBを取得する
- 引数：
  - なし
- 戻り値：
  - `Tpb`：TPB

---

## 3. 変更履歴
- 2026-02-17: 初版作成
