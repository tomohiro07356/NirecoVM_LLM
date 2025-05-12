using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mistral.SDK;
using Mistral.SDK.Models;

namespace NirecoVM_LLM
{
    public partial class MainForm : Form
    {
        private string _selectedImagePath;
        private readonly MistralClient _mistralClient;

        public MainForm()
        {
            InitializeComponent();
            _mistralClient = new MistralClient(
                apiKey: "dummy-api-key", // ローカルLLMを使用する場合は不要かもしれません
                endpoint: "http://localhost:8000" // ローカルLLMのエンドポイント
            );
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

                string base64Image = Convert.ToBase64String(File.ReadAllBytes(_selectedImagePath));

                string prompt = "この画像には日本の車両が写っています。ナンバープレートの情報を抽出して、以下の形式で出力してください：\n" +
                                "地域名：（例：品川）\n" +
                                "分類番号：（例：300）\n" +
                                "ひらがな：（例：さ）\n" +
                                "ナンバー：（例：1234）\n";

                var response = await _mistralClient.ChatAsync(
                    new ChatRequest
                    {
                        Model = "mistral-medium-3",
                        Messages = new[]
                        {
                            new ChatMessage
                            {
                                Role = "user",
                                Content = new[]
                                {
                                    new ChatMessageContent
                                    {
                                        Type = "text",
                                        Text = prompt
                                    },
                                    new ChatMessageContent
                                    {
                                        Type = "image",
                                        ImageUrl = new ChatMessageImageUrl
                                        {
                                            Data = base64Image
                                        }
                                    }
                                }
                            }
                        }
                    });

                txtResult.Text = response.Choices[0].Message.Content;
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
