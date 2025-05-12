# NirecoVM_LLM

日本の車両ナンバープレート認識アプリケーション

## 概要

NirecoVM_LLMは、車両が1台写っている画像を入力すると、日本のナンバープレート情報を出力するWindowsのネイティブアプリケーションです。

## 機能

- 車両画像の読み込み
- ローカルLLM（Mistral Medium 3）を使用したナンバープレート認識
- 認識結果の表示（地域名、分類番号、ひらがな、ナンバー）

## 技術仕様

- フレームワーク: .NET 8
- UI: Windows Forms
- 言語: C#
- LLM: Mistral（マルチモーダル対応）
- ライブラリ: OllamaSharp

## ファイル構造

```
NirecoVM_LLM/
├── NirecoVM_LLM.sln
└── NirecoVM_LLM/
    ├── MainForm.cs
    ├── MainForm.Designer.cs
    ├── NirecoVM_LLM.csproj
    ├── Program.cs
    └── SampleImages/
        └── Cap_20250501082858.jpg
```

## セットアップ方法

### 前提条件

- Windows 10/11
- .NET 8 SDK
- Ollama（ローカルLLMランナー）

### インストール手順

1. Ollamaをインストールして起動
   - [Ollama公式サイト](https://ollama.ai/)からダウンロードしてインストール
   - 以下のコマンドでMistralモデルをダウンロード：
     ```
     ollama pull mistral
     ```

2. リポジトリをクローン
   ```
   git clone https://github.com/tomohiro07356/NirecoVM_LLM.git
   ```

3. プロジェクトディレクトリに移動
   ```
   cd NirecoVM_LLM
   ```

4. NuGetパッケージを復元
   ```
   dotnet restore
   ```

5. アプリケーションをビルド
   ```
   dotnet build
   ```

6. アプリケーションを実行
   ```
   dotnet run --project NirecoVM_LLM/NirecoVM_LLM.csproj
   ```

### ローカルLLMの設定

MainForm.csファイル内の`OLLAMA_ENDPOINT`と`MODEL_NAME`の値を、必要に応じて環境に合わせて変更してください：

```csharp
private const string MODEL_NAME = "mistral";
private const string OLLAMA_ENDPOINT = "http://localhost:11434";
```

## 使用方法

1. アプリケーションを起動
2. 「画像を選択」ボタンをクリックして車両画像を選択
3. 「ナンバープレート認識」ボタンをクリックして分析を実行
4. 認識結果が表示されます

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。
