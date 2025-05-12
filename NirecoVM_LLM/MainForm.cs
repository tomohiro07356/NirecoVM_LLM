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
        private const string MODEL_NAME = "mistral";
        private const string OLLAMA_ENDPOINT = "http://localhost:11434";

        public MainForm()
        {
            InitializeComponent();
            _ollamaClient = new OllamaApiClient(OLLAMA_ENDPOINT);
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp|すべてのファイル|*.*";
                openFileDialog.Title = "車両画像を選択";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
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
                    pictureBox.Image?.Dispose();
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
            if (string.IsNullOrEmpty(_selectedImagePath) || !File.Exists(_selectedImagePath))
            {
                MessageBox.Show("画像を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnAnalyze.Enabled = false;
                txtResult.Text = "画像を分析中...";

                byte[] imageBytes = File.ReadAllBytes(_selectedImagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                string prompt = "この画像には日本の車両が写っています。ナンバープレートの情報を抽出して、以下の形式で出力してください：\n" +
                                "地域名：（例：品川）\n" +
                                "分類番号：（例：300）\n" +
                                "ひらがな：（例：さ）\n" +
                                "ナンバー：（例：1234）\n";

                var request = new GenerateRequest
                {
                    Model = MODEL_NAME,
                    Prompt = prompt,
                    Stream = false,
                    Options = new RequestOptions
                    {
                        Temperature = 0.7f,
                        NumPredict = 1024
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
            }
        }
    }
}
