using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.Generic;

namespace PythoPlus.PopScreens
{
    public partial class MatView : ContentPage
    {
        public MatView(string filePath)
        {
            InitializeComponent();
            // Вызов асинхронного метода в конструкторе
            LoadContent(filePath);
        }

        private async void LoadContent(string filePath)
        {
            var contentService = new XmlContentService();
            var paragraphs = await contentService.LoadContentAsync(filePath);

            foreach (var paragraph in paragraphs)
            {
                switch (paragraph.ParagraphType)
                {
                    case "Text":
                        AddTextParagraph(paragraph.Content);
                        break;
                    case "Code":
                        AddCodeParagraph(paragraph.Content);
                        break;
                    case "TestRadio":
                        AddTestRadioParagraph(paragraph.TestAsk, paragraph.TestOptions, paragraph.CorrectAnswer);
                        break;
                    case "TestCheck":
                        AddTestCheckParagraph(paragraph.TestAsk, paragraph.TestOptions, paragraph.CorrectAnswers);
                        break;
                    case "Image":
                        AddImageParagraph(paragraph.Content);
                        break;
                }
            }
        }

        private void AddTextParagraph(string content)
        {
            var label = new Label
            {
                Text = content,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            mainLayout.Children.Add(label);
        }

        private void AddCodeParagraph(string content)
        {
            var label = new Label
            {
                Text = content,
                TextColor = Color.FromArgb("#FFFFFF"),
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            var border = new Border
            {
                BackgroundColor = Color.FromArgb("#1F1F1F"),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                Padding = 5,
                StrokeThickness = 2
            };
            var innerLayout = new VerticalStackLayout();
            innerLayout.Children.Add(label);
            border.Content = innerLayout;
            mainLayout.Children.Add(border);
        }

        private void AddTestRadioParagraph(string ask, List<TestRadioOption> options, int correctAnswer)
        {
            var askLabel = new Label
            {
                Text = ask,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            mainLayout.Children.Add(askLabel);

            var stackLayout = new VerticalStackLayout();

            foreach (var option in options)
            {
                var radioButton = new RadioButton
                {
                    Content = option.Text,
                    GroupName = "TestRadioGroup"
                };
                stackLayout.Children.Add(radioButton);
            }

            mainLayout.Children.Add(stackLayout);
        }

        private void AddTestCheckParagraph(string ask, List<TestRadioOption> options, List<int> correctAnswers)
        {
            var askLabel = new Label
            {
                Text = ask,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            mainLayout.Children.Add(askLabel);

            var stackLayout = new VerticalStackLayout();

            foreach (var option in options)
            {
                var checkBox = new CheckBox();
                var checkBoxLabel = new Label
                {
                    Text = option.Text,
                    VerticalOptions = LayoutOptions.Center
                };

                var horizontalLayout = new HorizontalStackLayout();
                horizontalLayout.Children.Add(checkBox);
                horizontalLayout.Children.Add(checkBoxLabel);

                stackLayout.Children.Add(horizontalLayout);
            }

            mainLayout.Children.Add(stackLayout);
        }

        private void AddImageParagraph(string content)
        {
            var image = new Image
            {
                Source = content,
                Aspect = Aspect.AspectFit
            };
            mainLayout.Children.Add(image);
        }
    }
}
