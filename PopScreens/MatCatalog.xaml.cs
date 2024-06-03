using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PythoPlus.PopScreens
{
    public partial class MatCatalog : ContentPage
    {
        // VERY IMPORTANT INFO
        List<XmlFields> fieldsTask;

        public MatCatalog()
        {
            InitializeComponent();
            LoadMaterialsAsync();
        }

        private async void LoadMaterialsAsync()
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
                    new XmlFields("MaterialDescription", string.Empty)
                };
                var service = new XmlFileService(structure, $"{matFsn}{i}", $"{matCxn}");
                var fields = await service.ReadXmlAsync();

                string materialName = fields.FirstOrDefault(f => f.Name == "MaterialName")?.Value;
                string materialDescription = fields.FirstOrDefault(f => f.Name == "MaterialDescription")?.Value;
                string materialType = fields.FirstOrDefault(f => f.Name == "MaterialType")?.Value;

                var material = new Material
                {
                    MaterialNumber = i,
                    MaterialName = materialName,
                    MaterialDescription = materialDescription
                };

                AddMaterialToLayout(material, materialType);
            }
        }

        private void AddMaterialToLayout(Material material, string isTest)
        {
            var materialLayout = new VerticalStackLayout();

            string BorderColor = "ThemeColor";

            switch (isTest)
            {
                case "practice": BorderColor = "ButtonColor"; break;
                default: break;
            }

            var border = new Border
            {
                BackgroundColor = ((isTest == "summary") ? Colors.OrangeRed : (Color)Application.Current.Resources[BorderColor] ),
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
                Text = "--%", // Example static value
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 5, 0) // Adjust margin to align it with the border's padding
            };

            grid.Children.Add(verticalLayout);
            Grid.SetColumn(verticalLayout, 0);

            grid.Children.Add(percentageLabel);
            Grid.SetColumn(percentageLabel, 1);

            var line = new Line
            {
                BackgroundColor = (Color)Application.Current.Resources["ThemeSupColor"],
                StrokeThickness = 2
            };

            var progressBar = new ProgressBar();

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
            var matView = new MatView($"{matFsn}{material.MaterialNumber}/{matCtn}");
            Navigation.PushAsync(matView);
        }
    }
}
