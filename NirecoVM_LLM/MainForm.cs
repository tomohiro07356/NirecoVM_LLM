using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LLama;
using LLama.Native;
using LLama.Common;
using LLama.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace NirecoVM_LLM
{
    public partial class MainForm : Form
    {
        private string? _selectedImagePath;
        private byte[]? _currentImageBytes;
        private bool _isAnalyzing = false;
        private LLama.LLavaWeights? _model;
        private LLama.Context? _context;
        private const string MODEL_PATH = "models/llama-3.2-vision-8b.Q5_K_M.gguf";
        private const string CLIP_MODEL_PATH = "models/llama-3.2-vision-8b-mmproj.Q5_K_M.gguf";
        private const int MAX_TOKENS = 1024;
        private const float TEMPERATURE = 0.7f;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;
            InitializeModel();
        }

        private void InitializeModel()
        {
            try
            {
                if (!File.Exists(MODEL_PATH))
                {
                    MessageBox.Show($"モデルファイルが見つかりません: {MODEL_PATH}\n" +
                                    "モデルをダウンロードして配置してください。", 
                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LLama.Native.NativeLibraryConfig.Instance.WithCuda();

                var parameters = new LLama.ModelParams(MODEL_PATH)
                {
                    ContextSize = 4096,
                    GpuLayerCount = 100, // GPUで処理するレイヤー数
                    Seed = 1337,
                    UseMmap = true,
                    UseMLock = true,
                };

                _model = LLama.LLavaWeights.LoadFromFile(parameters, CLIP_MODEL_PATH);

                var contextParams = new LLama.Common.ContextParams
                {
                    BatchSize = 512,
                    Seed = 1337,
                };
                _context = _model.CreateContext(contextParams);

                txtResult.Text = "モデルの読み込みが完了しました。画像を選択してください。";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"モデルの初期化エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupResources();
        }

        private void CleanupResources()
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }

            _currentImageBytes = null;
            
            _context?.Dispose();
            _model?.Dispose();
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp|すべてのファイル|*.*";
                openFileDialog.Title = "車両画像を選択";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (pictureBox.Image != null)
                    {
                        pictureBox.Image.Dispose();
                        pictureBox.Image = null;
                    }

                    _selectedImagePath = openFileDialog.FileName;
                    LoadAndDisplayImage(_selectedImagePath);
                    txtResult.Clear();
                }
            }
        }

        private void LoadAndDisplayImage(string imagePath)
        {
            try
            {
                using (var image = Image.FromFile(imagePath))
                {
                    _currentImageBytes = File.ReadAllBytes(imagePath);
                    
                    pictureBox.Image = new Bitmap(image, pictureBox.Width, pictureBox.Height);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像の読み込みエラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedImagePath) || !File.Exists(_selectedImagePath) || _currentImageBytes == null)
            {
                MessageBox.Show("画像を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isAnalyzing)
            {
                MessageBox.Show("現在分析中です。しばらくお待ちください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_model == null || _context == null)
            {
                MessageBox.Show("モデルが初期化されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _isAnalyzing = true;
                btnAnalyze.Enabled = false;
                txtResult.Text = "画像を分析中...";

                byte[] processedImageBytes = await Task.Run(() => ProcessImageForModel(_currentImageBytes));

                string prompt = "The vehicle in the image is a Japanese car. Please check the license plate information. All you need is the main 4 digits, no other information is required. For example, if the number is 12-34, just answer License Plate: 12-34. If you cannot read it, just answer License Plate: None.";

                var inferenceParams = new LLama.Common.InferenceParams
                {
                    Temperature = TEMPERATURE,
                    MaxTokens = MAX_TOKENS,
                    AntiPrompt = new List<string> { "User:", "Human:" }
                };

                var multimodalData = new List<LLama.Abstractions.IMultiModalData>
                {
                    new LLama.Common.ImageData(processedImageBytes)
                };

                string responseText = await Task.Run(() => 
                {
                    var executor = new LLama.Executor(_context, inferenceParams);
                    return executor.Infer(prompt, multimodalData);
                });

                txtResult.Text = responseText.Trim();
            }
            catch (Exception ex)
            {
                txtResult.Text = $"エラーが発生しました: {ex.Message}";
            }
            finally
            {
                btnAnalyze.Enabled = true;
                _isAnalyzing = false;
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private byte[] ProcessImageForModel(byte[] imageBytes)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(imageBytes))
            {
                image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Size(512, 512)));
                
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, new JpegEncoder());
                    return ms.ToArray();
                }
            }
        }
    }
}
