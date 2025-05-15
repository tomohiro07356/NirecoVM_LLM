# モデルファイルについて

このディレクトリには、ナンバープレート認識に使用するLLamaモデルファイルを配置してください。

## 必要なモデルファイル

アプリケーションは以下のモデルファイルを使用します：

- `llama-3.2-vision-8b.Q5_K_M.gguf`

## モデルのダウンロード方法

1. [HuggingFace](https://huggingface.co/)からLlama 3.2 Visionモデルをダウンロードします。
   - [TheBloke/Llama-3.2-Vision-8B-GGUF](https://huggingface.co/TheBloke/Llama-3.2-Vision-8B-GGUF)

2. ダウンロードしたモデルファイル（`llama-3.2-vision-8b.Q5_K_M.gguf`）をこのディレクトリに配置します。

## 注意事項

- モデルファイルは大きいため、Gitリポジトリには含まれていません。
- アプリケーションを初めて実行する前に、必ずモデルファイルをダウンロードして配置してください。
- NVIDIA RTX 500 Ada Generation Laptop GPUでの動作に最適化されています。
