using Microsoft.Maui.Controls;
using PythoPlus.PopScreens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PythoPlus
{
    public partial class MainPage : ContentPage
    {
        private string userId;
        private LocalDbService _localDbService;
        private Dictionary<int, string> materialPaths;

        public MainPage()
        {
            InitializeComponent();
            Navigation.PushModalAsync(new Login());
            _localDbService = new LocalDbService(FileSystem.AppDataDirectory);
            materialPaths = new Dictionary<int, string>();
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
                userId = null;
            }
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            LoadUserId();
            mainLayout.Children.Clear();
            if (userId != null)
            {
                await GenerateLinksAsync();
            }
        }

        private async Task GenerateLinksAsync()
        {
            mainLayout.Children.Clear();

            // Заголовок
            var titleLabel = new Label
            {
                Text = "Вітаємо! Учбової Вам наснаги!",
                FontSize = (double)Application.Current.Resources["FontSize"] * 1.40,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontFamily = (string)Application.Current.Resources["FontFamily"],
                HorizontalOptions = LayoutOptions.Center
            };
            mainLayout.Children.Add(titleLabel);

            // Подзаголовок
            var subtitleLayout = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                VerticalOptions = LayoutOptions.Center
            };

            var subtitleLabel = new Label
            {
                Text = "Ваш учбовий прогрес складає:",
                FontSize = (double)Application.Current.Resources["FontSize"] * 1.20,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            Grid.SetColumn(subtitleLabel, 0);

            var progressLabel = new Label
            {
                FontSize = (double)Application.Current.Resources["FontSize"] * 1.20,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontFamily = (string)Application.Current.Resources["FontFamily"],
                FontAttributes = FontAttributes.Bold
            };
            Grid.SetColumn(progressLabel, 1);

            subtitleLayout.Children.Add(subtitleLabel);
            subtitleLayout.Children.Add(progressLabel);

            mainLayout.Children.Add(subtitleLayout);

            // Получение данных для прогресса
            int totalMaterials = await GetTotalMaterialsAsync();
            int completedMaterials = await GetCompletedMaterialsCountAsync();

            double progressPercentage = totalMaterials > 0 ? ((double)completedMaterials / totalMaterials) : 0;
            progressLabel.Text = $"{progressPercentage * 100:F2}%";

            // ProgressBar с прогрессом
            var progressBar = new ProgressBar
            {
                Progress = progressPercentage,
                BackgroundColor = (Color)Application.Current.Resources["BackColor"],
                ProgressColor = (Color)Application.Current.Resources["ThemeColor"],
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 20,
                Margin = new Thickness(0, 10, 0, 10)
            };

            mainLayout.Children.Add(progressBar);

            // Заголовок и подзаголовок добавлены, теперь добавляем генерируемые элементы
            var progressDocuments = await _localDbService.GetCollectionAsync<ProgressDocument>("mat_progress");

            if (progressDocuments.Count == 0)
            {
                AddLink("Ви тут вперше?", "Перейдіть до вкладинки 'Матеріали', щоб розпочати навчання! Рекомендуємо обрати перший матеріал.", async () =>
                {
                    await Shell.Current.GoToAsync("MatCatalog");
                });
            }
            else
            {
                var firstUnfinished = await GetFirstUnfinishedMaterialAsync(progressDocuments);

                if (firstUnfinished != null)
                {
                    AddLink("Недопройдений матеріал! Ви зупинились на ньому:", $"Матеріал: {firstUnfinished.MaterialName}", async () =>
                    {
                        await Shell.Current.GoToAsync("MatCatalog");
                        var matView = new MatView(firstUnfinished.MaterialNumber, materialPaths[firstUnfinished.MaterialNumber]);
                        matView.Title = $"{firstUnfinished.MaterialName}";
                        await Shell.Current.Navigation.PushAsync(matView);
                    });
                }

                var lastCompleted = await GetLastCompletedMaterialAsync(progressDocuments);

                if (lastCompleted != null)
                {
                    var nextMaterialNumber = lastCompleted.MaterialNumber + 1;
                    var nextMaterial = await GetMaterialByNumberAsync(nextMaterialNumber);

                    if (nextMaterial != null)
                    {
                        AddLink("Наступний матеріал для опрацювання:", $"Матеріал: {nextMaterial.MaterialName}", async () =>
                        {
                            await Shell.Current.GoToAsync("MatCatalog");
                            var matView = new MatView(nextMaterialNumber, materialPaths[nextMaterialNumber]);
                            matView.Title = $"{nextMaterial.MaterialName}";
                            await Shell.Current.Navigation.PushAsync(matView);
                        });
                    }
                }

                var completedMaterialsList = await GetCompletedMaterialsAsync(progressDocuments);

                if (completedMaterialsList.Count > 0)
                {
                    var random = new Random();
                    var randomCompleted = completedMaterialsList[random.Next(completedMaterialsList.Count)];

                    AddLink("Повторення - основа навчання! Освіжіть пам'ять, згадавши:", $"Матеріал: {randomCompleted.MaterialName}", async () =>
                    {
                        await Shell.Current.GoToAsync("MatCatalog");
                        var matView = new MatView(randomCompleted.MaterialNumber, materialPaths[randomCompleted.MaterialNumber]);
                        matView.Title = $"{randomCompleted.MaterialName}";
                        await Shell.Current.Navigation.PushAsync(matView);
                    });
                }
            }

            AddLink("Не знаєте, що робити?", "Перегляньте інструкцію, щоб дізнатись, як користуватись додатком!", async () =>
            {
                await Shell.Current.GoToAsync("Help");
            });
        }

        private async Task<int> GetTotalMaterialsAsync()
        {
            var superStructure = new List<XmlFields>
            {
                new XmlFields("MaterialCount", string.Empty)
            };
            var superService = new XmlFileService(superStructure);

            var superInfo = await superService.ReadXmlAsync();
            string materialCountString = superInfo.FirstOrDefault(f => f.Name == "MaterialCount")?.Value;

            return int.TryParse(materialCountString, out int materialCount) ? materialCount : 0;
        }

        private async Task<int> GetCompletedMaterialsCountAsync()
        {
            var progressDocuments = await _localDbService.GetCollectionAsync<ProgressDocument>("mat_progress");

            int completedCount = 0;

            foreach (var doc in progressDocuments)
            {
                var materialNumber = doc.MaterialNumber;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc.CorrectAnswers.Count;
                    int totalQuestions = int.Parse(material.ParagraphCount);

                    if (correctAnswers == totalQuestions)
                    {
                        completedCount++;
                    }
                }
            }

            return completedCount;
        }

        private void AddLink(string title, string description, Func<Task> onClick)
        {
            var border = new Border();
            var layout = new VerticalStackLayout { Padding = 10, Spacing = 10 };

            var titleLabel = new Label
            {
                Text = title,
                FontAttributes = FontAttributes.Bold
            };

            var descriptionLabel = new Label
            {
                Text = description
            };

            var buttonLayout = new VerticalStackLayout
            {
                Padding = 3,
                Spacing = 5,
                HorizontalOptions = LayoutOptions.Center
            };

            var buttonLabel = new Label
            {
                Text = "Перейти",
                TextColor = (Color)Application.Current.Resources["ButtonTextColor"]
            };

            var buttonBorder = new Border
            {
                Stroke = (Color)Application.Current.Resources["ThemeSupColor"],
                StrokeThickness = 2,
                BackgroundColor = (Color)Application.Current.Resources["ButtonColor"]
            };

            buttonLayout.Children.Add(buttonLabel);
            buttonBorder.Content = buttonLayout;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => await onClick();

            buttonLayout.GestureRecognizers.Add(tapGestureRecognizer);

            layout.Children.Add(titleLabel);
            layout.Children.Add(descriptionLabel);
            layout.Children.Add(buttonBorder);

            border.Content = layout;

            mainLayout.Children.Add(border);
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
            string matCntn = superInfo.FirstOrDefault(f => f.Name == "MaterialContentNaming")?.Value;

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

            var materialPath = $"{matFsn}{materialNumber}/{matCntn}";
            materialPaths[materialNumber] = materialPath;

            return new Material
            {
                MaterialNumber = materialNumber,
                MaterialName = materialName,
                MaterialDescription = materialDescription,
                ParagraphCount = paragraphCount,
            };
        }

        private async Task<Material> GetFirstUnfinishedMaterialAsync(List<ProgressDocument> progressDocuments)
        {
            foreach (var doc in progressDocuments)
            {
                var materialNumber = doc.MaterialNumber;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc.CorrectAnswers.Count;
                    int totalQuestions = int.Parse(material.ParagraphCount);

                    if (correctAnswers < totalQuestions)
                    {
                        return material;
                    }
                }
            }

            return null;
        }

        private async Task<Material> GetLastCompletedMaterialAsync(List<ProgressDocument> progressDocuments)
        {
            foreach (var doc in progressDocuments.OrderByDescending(d => d.MaterialNumber))
            {
                var materialNumber = doc.MaterialNumber;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc.CorrectAnswers.Count;
                    int totalQuestions = int.Parse(material.ParagraphCount);

                    if (correctAnswers == totalQuestions)
                    {
                        return material;
                    }
                }
            }

            return null;
        }

        private async Task<List<Material>> GetCompletedMaterialsAsync(List<ProgressDocument> progressDocuments)
        {
            var completedMaterials = new List<Material>();

            foreach (var doc in progressDocuments)
            {
                var materialNumber = doc.MaterialNumber;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc.CorrectAnswers.Count;
                    int totalQuestions = int.Parse(material.ParagraphCount);

                    if (correctAnswers == totalQuestions)
                    {
                        completedMaterials.Add(material);
                    }
                }
            }

            return completedMaterials;
        }

        internal void OnUserLoggedIn()
        {
            OnAppearing(new object(), new EventArgs());
            DisplayAlert("Вітаємо", "Вхід виконано успішно!", "OK");
        }
    }

    public class ProgressDocument
    {
        public string UserId { get; set; }
        public int MaterialNumber { get; set; }
        public List<string> CorrectAnswers { get; set; }
    }
}
