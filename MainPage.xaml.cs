using Microsoft.Maui.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using PythoPlus.PopScreens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PythoPlus
{
    public partial class MainPage : ContentPage
    {
        private ObjectId? userId;
        private MongoDbService _mongoDbService;
        private Dictionary<int, string> materialPaths;

        public MainPage()
        {
            InitializeComponent();
            Navigation.PushModalAsync(new Login());
            _mongoDbService = new MongoDbService();
            materialPaths = new Dictionary<int, string>();
            Appearing += OnAppearing;
        }

        private void LoadUserId()
        {
            if (Application.Current.Resources.ContainsKey("UserId"))
            {
                userId = new ObjectId(Application.Current.Resources["UserId"].ToString());
            }
            else
            {
                userId = null;
            }
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            LoadUserId();
            if (userId != null)
            {
                await GenerateLinksAsync();
            }

        }

        private async Task GenerateLinksAsync()
        {
            mainLayout.Children.Clear();

            var progressCollection = _mongoDbService.GetCollection("mat_progress");
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", userId);
            var progressDocuments = await progressCollection.Find(filter).ToListAsync();

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

                var completedMaterials = await GetCompletedMaterialsAsync(progressDocuments);

                if (completedMaterials.Count > 0)
                {
                    var random = new Random();
                    var randomCompleted = completedMaterials[random.Next(completedMaterials.Count)];

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

        private async Task<Material> GetFirstUnfinishedMaterialAsync(List<BsonDocument> progressDocuments)
        {
            foreach (var doc in progressDocuments)
            {
                var materialNumber = doc["material_number"].AsInt32;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc["correct_answers"].AsBsonArray.Count;
                    int totalQuestions = int.Parse(material.ParagraphCount);

                    if (correctAnswers < totalQuestions)
                    {
                        return material;
                    }
                }
            }

            return null;
        }

        private async Task<Material> GetLastCompletedMaterialAsync(List<BsonDocument> progressDocuments)
        {
            foreach (var doc in progressDocuments.OrderByDescending(d => d["material_number"].AsInt32))
            {
                var materialNumber = doc["material_number"].AsInt32;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc["correct_answers"].AsBsonArray.Count;
                    int totalQuestions = int.Parse(material.ParagraphCount);

                    if (correctAnswers == totalQuestions)
                    {
                        return material;
                    }
                }
            }

            return null;
        }

        private async Task<List<Material>> GetCompletedMaterialsAsync(List<BsonDocument> progressDocuments)
        {
            var completedMaterials = new List<Material>();

            foreach (var doc in progressDocuments)
            {
                var materialNumber = doc["material_number"].AsInt32;
                var material = await GetMaterialByNumberAsync(materialNumber);

                if (material != null)
                {
                    int correctAnswers = doc["correct_answers"].AsBsonArray.Count;
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
}
