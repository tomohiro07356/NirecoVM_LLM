using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using OllamaSharp;
using OllamaSharp.Models;

namespace NirecoVM_LLM
{
    public partial class MainForm : Form
    {
        private string? _selectedImagePath;
        private readonly OllamaApiClient _ollamaClient;
        private const string MODEL_NAME = "gemma3:4b";
        private const string OLLAMA_ENDPOINT = "http://localhost:11434";
        private byte[]? _currentImageBytes;
        private bool _isAnalyzing = false;
        private string? _trimmedImagePath;

        public MainForm()
        {
            InitializeComponent();
            _ollamaClient = new OllamaApiClient(OLLAMA_ENDPOINT);
            this.FormClosing += MainForm_FormClosing;
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

            if (!string.IsNullOrEmpty(_trimmedImagePath) && File.Exists(_trimmedImagePath))
            {
                try
                {
                    File.Delete(_trimmedImagePath);
                }
                catch
                {
                }
            }

            _currentImageBytes = null;
            _trimmedImagePath = null;
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
                    _trimmedImagePath = TrimImageIfNeeded(imagePath);
                    
                    string imageForBytes = _trimmedImagePath ?? imagePath;
                    _currentImageBytes = File.ReadAllBytes(imageForBytes);
                    
                    pictureBox.Image = new Bitmap(image, pictureBox.Width, pictureBox.Height);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像の読み込みエラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private string? TrimImageIfNeeded(string originalImagePath)
        {
            try
            {
                using (var originalImage = Image.FromFile(originalImagePath))
                {
                    if (originalImage.Width == 1920 && originalImage.Height == 1080)
                    {
                        string tmpDir = Path.Combine(Application.StartupPath, "tmp");
                        Directory.CreateDirectory(tmpDir);
                        
                        string fileName = $"trimmed_{Path.GetFileNameWithoutExtension(originalImagePath)}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                        string trimmedPath = Path.Combine(tmpDir, fileName);
                        
                        Rectangle cropArea = new Rectangle(300, 100, 1280, 720);
                        using (Bitmap croppedImage = new Bitmap(1280, 720))
                        using (Graphics graphics = Graphics.FromImage(croppedImage))
                        {
                            graphics.DrawImage(originalImage, new Rectangle(0, 0, 1280, 720), cropArea, GraphicsUnit.Pixel);
                            croppedImage.Save(trimmedPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        
                        return trimmedPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像のトリミングエラー: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            return null; // Return null if not 1920x1080 or if trimming failed
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

            try
            {
                _isAnalyzing = true;
                btnAnalyze.Enabled = false;
                txtResult.Text = "画像を分析中...";

                string base64Image = Convert.ToBase64String(_currentImageBytes);

                string prompt = "ナンバープレート情報を読み取って.例えば12-34だったら" +
                    "「ナンバープレート情報：12-34」の形式で回答して";

                var request = new GenerateRequest
                {
                    Model = MODEL_NAME,
                    Prompt = prompt,
                    Stream = false,
                    Options = new RequestOptions
                    {
                        Temperature = 0.0f, // 0.0fは決定的な応答を生成
                        NumPredict = 256
                    },
                    Images = new[] { base64Image }
                };

                var responseStream = _ollamaClient.GenerateAsync(request);
                string responseText = "";
                
                await foreach (var response in responseStream)
                {
                    if (response != null)
                    {
                        responseText += response.Response;
                    }
                }

                txtResult.Text = responseText;
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
    }
}
