using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PythoPlus.PopScreens
{
    public partial class MatCatalog : ContentPage
    {
        // VERY IMPORTANT INFO
        List<XmlFields> fieldsTask;
        private ObjectId userId;
        private MongoDbService _mongoDbService;

        public MatCatalog()
        {
            InitializeComponent();
            _mongoDbService = new MongoDbService();
            Appearing += OnAppearing; // Добавляем обработчик для события Appearing
        }

        private void LoadUserId()
        {
            if (Application.Current.Resources.ContainsKey("UserId"))
            {
                userId = new ObjectId(Application.Current.Resources["UserId"].ToString());
            }
            else
            {
                throw new Exception("UserId не знайдено у динамічних ресурсах.");
            }
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            NavigationPage.SetHasBackButton(this, false);
            LoadUserId(); // Обновляем UserId каждый раз, когда страница становится активной
            await LoadMaterialsAsync();
        }

        private async Task LoadMaterialsAsync()
        {
            try
            {
                mainLayout.Children.Clear(); // Очищаем текущие элементы перед загрузкой новых данных

                var superStructure = new List<XmlFields>
                {
                    new XmlFields("MaterialFirstNaming", string.Empty),
                    new XmlFields("MaterialContextNaming", string.Empty),
                    new XmlFields("MaterialContentNaming", string.Empty),
                    new XmlFields("MaterialCount", string.Empty),
                };
                var superService = new XmlFileService(superStructure);

                var superInfo = await superService.ReadXmlAsync();

                fieldsTask = superInfo;

                string matFsn = superInfo.FirstOrDefault(f => f.Name == "MaterialFirstNaming")?.Value;
                string matCxn = superInfo.FirstOrDefault(f => f.Name == "MaterialContextNaming")?.Value;
                string matCntForm = superInfo.FirstOrDefault(f => f.Name == "MaterialCount")?.Value;

                int matCnt = int.Parse(matCntForm);

                for (int i = 1; i <= matCnt; i++)
                {
                    var structure = new List<XmlFields>
                    {
                        new XmlFields("MaterialNumber", string.Empty),
                        new XmlFields("MaterialName", string.Empty),
                        new XmlFields("MaterialType", string.Empty),
                        new XmlFields("MaterialDescription", string.Empty),
                        new XmlFields("ParagraphCount", string.Empty)
                    };
                    var service = new XmlFileService(structure, $"{matFsn}{i}", $"{matCxn}");
                    var fields = await service.ReadXmlAsync();

                    string materialName = fields.FirstOrDefault(f => f.Name == "MaterialName")?.Value;
                    string materialDescription = fields.FirstOrDefault(f => f.Name == "MaterialDescription")?.Value;
                    string materialType = fields.FirstOrDefault(f => f.Name == "MaterialType")?.Value;
                    string paragraphCount = fields.FirstOrDefault(f => f.Name == "ParagraphCount")?.Value;

                    var material = new Material
                    {
                        MaterialNumber = i,
                        MaterialName = materialName,
                        MaterialDescription = materialDescription,
                        ParagraphCount = paragraphCount
                    };

                    await AddMaterialToLayout(material, materialType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке материалов: {ex.Message}");
                await DisplayAlert("Помилка", "Не вдалося завантажити матеріали. Зверніться до підтримки.", "OK");
            }
        }

        private async Task AddMaterialToLayout(Material material, string isTest)
        {
            var materialLayout = new VerticalStackLayout();

            string BorderColor = "ThemeColor";
            Color backgroundColor = (Color)Application.Current.Resources[BorderColor];
            string percentageText = "--%";
            Color percentageBorderColor = Colors.DarkGray;

            switch (isTest)
            {
                case "practice":
                    BorderColor = "ButtonColor";
                    break;
                default:
                    break;
            }

            try
            {
                // Fetch user progress
                var progressCollection = _mongoDbService.GetCollection("mat_progress");
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", userId) & Builders<BsonDocument>.Filter.Eq("material_number", material.MaterialNumber);
                var progressDocument = await progressCollection.Find(filter).FirstOrDefaultAsync();

                int correctAnswers = 0;
                int totalQuestions = int.Parse(material.ParagraphCount);

                if (progressDocument != null)
                {
                    var correctAnswersArray = progressDocument["correct_answers"].AsBsonArray;
                    correctAnswers = correctAnswersArray.Count;

                    double percentage = ((double)correctAnswers / totalQuestions) * 100;
                    percentageText = $"{percentage:F2}%";

                    if (correctAnswers == totalQuestions)
                    {
                        percentageBorderColor = Colors.Green;
                    }
                    else
                    {
                        percentageBorderColor = backgroundColor;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении прогресса пользователя: {ex.Message}");
                await DisplayAlert("Помилка", "Не вдалося під'єднатись до бази даних. Спробуйте пізніше", "OK");
            }

            var border = new Border
            {
                BackgroundColor = ((isTest == "summary") ? Colors.OrangeRed : (Color)Application.Current.Resources[BorderColor]),
                Stroke = (Color)Application.Current.Resources["ThemeSupColor"],
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                Padding = 5,
                StrokeThickness = 2
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var verticalLayout = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var nameLabel = new Label
            {
                Text = material.MaterialName,
                TextColor = (Color)Application.Current.Resources["BackColor"]
            };

            var descriptionLabel = new Label
            {
                Text = material.MaterialDescription
            };

            verticalLayout.Children.Add(nameLabel);
            verticalLayout.Children.Add(descriptionLabel);

            var percentageLabel = new Label
            {
                Text = percentageText,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 5, 0) // Adjust margin to align it with the border's padding
            };

            var percentageBorder = new Border
            {
                BackgroundColor = percentageBorderColor,
                Stroke = (Color)Application.Current.Resources["BackColor"],
                StrokeThickness = 2,                
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                Padding = 5,
                Content = percentageLabel
            };

            grid.Children.Add(verticalLayout);
            Grid.SetColumn(verticalLayout, 0);

            grid.Children.Add(percentageBorder);
            Grid.SetColumn(percentageBorder, 1);

            var line = new Line
            {
                BackgroundColor = (Color)Application.Current.Resources["ThemeSupColor"],
                StrokeThickness = 2
            };

            bool isEmpty = percentageText == "--%";

            double progressValue = 0;
            if (!isEmpty)
            {
                if (double.TryParse(percentageText.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture, out double percentage))
                {
                    progressValue = percentage / 10000;
                }
            }

            var progressBar = new ProgressBar
            {
                Progress = progressValue
            };

            var innerLayout = new VerticalStackLayout();
            innerLayout.Children.Add(grid);
            innerLayout.Children.Add(line);
            innerLayout.Children.Add(progressBar);

            border.Content = innerLayout;

            materialLayout.Children.Add(border);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => OnItemTapped(material);
            materialLayout.GestureRecognizers.Add(tapGestureRecognizer);

            mainLayout.Children.Add(materialLayout);
        }

        private void OnItemTapped(Material material)
        {
            string matFsn = fieldsTask.FirstOrDefault(f => f.Name == "MaterialFirstNaming")?.Value;
            string matCtn = fieldsTask.FirstOrDefault(f => f.Name == "MaterialContentNaming")?.Value;
            // Условный обработчик нажатия, например, навигация к детальной информации о материале
            Console.WriteLine($"Tapped on material number: {material.MaterialNumber}");
            var matView = new MatView(material.MaterialNumber, $"{matFsn}{material.MaterialNumber}/{matCtn}");
            matView.Title = material.MaterialName;
            Navigation.PushAsync(matView);
        }
    }
}
