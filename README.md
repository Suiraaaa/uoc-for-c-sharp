# uoc-for-c-sharp

[![License](https://img.shields.io/github/license/Suiraaaa/uoc-for-c-sharp)](./LICENSE)
[![GitHub Release](https://img.shields.io/github/v/release/Suiraaaa/uoc-for-c-sharp?sort=semver)](https://github.com/Suiraaaa/uoc-for-c-sharp/releases)
![Target Framework](https://img.shields.io/badge/.NET%20Standard-2.1-512BD4?logo=dotnet)

UOCフォーマットの譜面データをC#で扱うためのライブラリです。

> [!WARNING]
> 現在はまだ開発中のプレリリース版となります。
> 公開APIや挙動は、今後のリリースで変更される可能性があります。

## UOCフォーマットについて

このライブラリは、以下のUOCフォーマット仕様に準拠して開発されています。

* [UOCフォーマット仕様](https://gist.github.com/Suiraaaa/188f4ec0639fde9834d7cb7ef057bf2c)

## 主な機能

* UOC文字列のパース
* UOC文字列の構築
* 譜面プロパティの管理
* ノートおよびノートグループの管理
* BPM、小節長、スピード倍率などのイベント情報の取得
* 譜面再生向けデータの解析

## 対象環境

* .NET Standard 2.1

## 配布について

現在はプレリリース版です。

公開済みの成果物については、[Releases](https://github.com/Suiraaaa/uoc-for-c-sharp/releases) を参照してください。

## 基本的な使用例

### UOCファイルを読み込む

```csharp
using System.IO;
using Uoc;
using Uoc.Parse;

var text = File.ReadAllText("chart.uoc");
var uocObject = UocParser.Parse(new UocString(text));

var gameId = uocObject.ChartPropertyGroup.GetGameId();
var tpb = uocObject.ChartPropertyGroup.GetTpb();
```

### UOCファイルを書き出す

```csharp
using System.IO;
using Uoc.Parse;

var output = UocBuilder.Build(
    editorName: "SampleEditor",
    chartPropertyGroup: uocObject.ChartPropertyGroup,
    noteDefCollection: uocObject.NoteDefCollection,
    noteGroupDefCollection: uocObject.NoteGroupDefCollection,
    noteProfileCollection: uocObject.NoteProfileCollection);

File.WriteAllText("output.uoc", output.Value);
```

より詳しいAPI仕様については、[SPEC.md](./SPEC.md) を参照してください。

## ドキュメント

* [API仕様書](./SPEC.md)
* [UOCフォーマット仕様](https://gist.github.com/Suiraaaa/188f4ec0639fde9834d7cb7ef057bf2c)

## ライセンス

[MIT License](./LICENSE)
