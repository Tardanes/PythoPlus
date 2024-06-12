using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace PythoPlus.PopScreens
{
    public partial class StatisticsUsr : ContentPage
    {
        private string userId;
        private LocalDbService _localDbService;

        public StatisticsUsr()
        {
            InitializeComponent();
            _localDbService = new LocalDbService(FileSystem.AppDataDirectory);
            Appearing += OnAppearing;
        }

        private void LoadUserId()
        {
            if (Application.Current.Resources.ContainsKey("UserId"))
            {
                userId = Application.Current.Resources["UserId"].ToString();
            }
            else
            {
                throw new Exception("UserId не знайдено у динамічних ресурсах.");
            }
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            LoadUserId();
            await LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            var statsCollection = await _localDbService.GetCollectionAsync<UserStats>("mat_stat");
            var progressCollection = await _localDbService.GetCollectionAsync<ProgressDocument>("mat_progress");

            var statsDocument = statsCollection.FirstOrDefault(stat => stat.UserId == userId);
            var progressDocuments = progressCollection.Where(doc => doc.UserId == userId).ToList();

            int loginsCount = statsDocument != null ? statsDocument.LoginsCount : 0;
            double totalTimeSpentSeconds = statsDocument != null ? statsDocument.TotalTimeSpent : 0.0;
            TimeSpan totalTimeSpent = TimeSpan.FromSeconds(totalTimeSpentSeconds);

            int totalAnswers = statsDocument != null ? statsDocument.TotalAnswers : 0;
            int correctAnswers = statsDocument != null ? statsDocument.CorrectAnswers : 0;
            double correctAnswersPercentage = totalAnswers > 0 ? ((double)correctAnswers / totalAnswers) * 100 : 0;

            LoginsCountLabel.Text = $"Кількість входів: {loginsCount}";
            TotalTimeSpentLabel.Text = $"Загальний час на сторінці: {totalTimeSpent}";
            TotalAnswersLabel.Text = $"Загальна кількість відповідей: {totalAnswers}";
            CorrectAnswersLabel.Text = $"Кількість правильних відповідей: {correctAnswers}";
            CorrectAnswersPercentageLabel.Text = $"Процент правильних відповідей: {correctAnswersPercentage:F2}%";

            var mainLayout = this.FindByName<VerticalStackLayout>("MainLayout");
            mainLayout.Clear();
            foreach (var doc in progressDocuments)
            {
                var materialNumber = doc.MaterialNumber;
                int correctAnswersCount = doc.CorrectAnswers.Count;

                var material = await GetMaterialByNumberAsync(materialNumber);
                if (material != null)
                {
                    int totalQuestions = int.Parse(material.ParagraphCount);
                    int remainingQuestions = totalQuestions - correctAnswersCount;

                    var materialStats = new MaterialStats
                    {
                        MaterialName = material.MaterialName,
                        MaterialDetails = $"Всього питань: {totalQuestions}, Правильних відповідей: {correctAnswersCount}, Залишилось відповісти: {remainingQuestions}"
                    };

                    // Создание Border для каждого материала
                    var border = new Border
                    {
                        BackgroundColor = (Color)Application.Current.Resources["ThemeColor"],
                        Stroke = (Color)Application.Current.Resources["ThemeSupColor"],
                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                        Padding = 5,
                        StrokeThickness = 2
                    };

                    var stackLayout = new VerticalStackLayout();

                    var materialNameLabel = new Label
                    {
                        Text = materialStats.MaterialName,
                        FontSize = 18,
                        TextColor = (Color)Application.Current.Resources["TextColor"],
                        FontFamily = (string)Application.Current.Resources["FontFamily"]
                    };

                    var materialDetailsLabel = new Label
                    {
                        Text = materialStats.MaterialDetails,
                        FontSize = 16,
                        TextColor = (Color)Application.Current.Resources["TextColor"],
                        FontFamily = (string)Application.Current.Resources["FontFamily"],
                        IsVisible = false
                    };

                    var toggleButton = new Button
                    {
                        Text = "Показати/Сховати",
                        BackgroundColor = (Color)Application.Current.Resources["ButtonColor"],
                        TextColor = (Color)Application.Current.Resources["ButtonTextColor"],
                        FontSize = (double)Application.Current.Resources["FontSize"],
                        FontFamily = (string)Application.Current.Resources["FontFamily"]
                    };

                    toggleButton.Clicked += (s, e) =>
                    {
                        materialDetailsLabel.IsVisible = !materialDetailsLabel.IsVisible;
                    };

                    stackLayout.Children.Add(materialNameLabel);
                    stackLayout.Children.Add(materialDetailsLabel);
                    stackLayout.Children.Add(toggleButton);

                    border.Content = stackLayout;

                    mainLayout.Children.Add(border);
                }
            }
        }

        private async Task<Material> GetMaterialByNumberAsync(int materialNumber)
        {
            var superStructure = new List<XmlFields>
            {
                new XmlFields("MaterialFirstNaming", string.Empty),
                new XmlFields("MaterialContextNaming", string.Empty),
                new XmlFields("MaterialContentNaming", string.Empty),
                new XmlFields("MaterialCount", string.Empty),
            };
            var superService = new XmlFileService(superStructure);

            var superInfo = await superService.ReadXmlAsync();

            string matFsn = superInfo.FirstOrDefault(f => f.Name == "MaterialFirstNaming")?.Value;
            string matCxn = superInfo.FirstOrDefault(f => f.Name == "MaterialContextNaming")?.Value;

            var structure = new List<XmlFields>
            {
                new XmlFields("MaterialNumber", string.Empty),
                new XmlFields("MaterialName", string.Empty),
                new XmlFields("MaterialType", string.Empty),
                new XmlFields("MaterialDescription", string.Empty),
                new XmlFields("ParagraphCount", string.Empty)
            };

            var service = new XmlFileService(structure, $"{matFsn}{materialNumber}", $"{matCxn}");
            var fields = await service.ReadXmlAsync();

            string materialName = fields.FirstOrDefault(f => f.Name == "MaterialName")?.Value;
            string materialDescription = fields.FirstOrDefault(f => f.Name == "MaterialDescription")?.Value;
            string materialType = fields.FirstOrDefault(f => f.Name == "MaterialType")?.Value;
            string paragraphCount = fields.FirstOrDefault(f => f.Name == "ParagraphCount")?.Value;

            if (materialName == null)
            {
                return null;
            }

            return new Material
            {
                MaterialNumber = materialNumber,
                MaterialName = materialName,
                MaterialDescription = materialDescription,
                ParagraphCount = paragraphCount,
            };
        }

        private class MaterialStats
        {
            public string MaterialName { get; set; }
            public string MaterialDetails { get; set; }
        }
    }
}
