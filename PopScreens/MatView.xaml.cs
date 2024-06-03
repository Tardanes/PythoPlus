using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace PythoPlus.PopScreens
{
    public partial class MatView : ContentPage
    {
        public event Action<int> TestSolvedCorrectly;

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
                        AddTextParagraph(paragraph.ID, paragraph.Content, paragraph.IsBold);
                        break;
                    case "Code":
                        AddCodeParagraph(paragraph.ID, paragraph.Content);
                        break;
                    case "TestRadio":
                        AddTestRadioParagraph(paragraph.ID, paragraph.TestAsk, paragraph.TestOptions, paragraph.CorrectAnswer);
                        break;
                    case "TestCheck":
                        AddTestCheckParagraph(paragraph.ID, paragraph.TestAsk, paragraph.TestOptions, paragraph.CorrectAnswers);
                        break;
                    case "Image":
                        AddImageParagraph(paragraph.ID, paragraph.Content);
                        break;
                    case "Compliance":
                        AddComplianceParagraph(paragraph.ID, paragraph.TestAsk, paragraph.CompliancePairs);
                        break;
                    case "Entry":
                        AddEntryParagraph(paragraph.ID, paragraph.TestAsk, paragraph.CorrectEntry);
                        break;
                }
            }
        }

        private void AddTextParagraph(int id, string content, bool isBold)
        {
            var label = new Label
            {
                Text = content,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"],
                FontAttributes = isBold ? FontAttributes.Bold : FontAttributes.None
            };
            mainLayout.Children.Add(label);
        }

        private void AddCodeParagraph(int id, string content)
        {
            var label = new Label
            {
                Text = content,
                TextColor = Color.FromArgb("#FFFFFF"),
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = "Consolas"
            };
            var border = new Border
            {
                BackgroundColor = Color.FromArgb("#1F1F1F"),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(5) },
                StrokeThickness = 2
            };
            var innerLayout = new VerticalStackLayout();
            innerLayout.Children.Add(label);
            border.Content = innerLayout;
            mainLayout.Children.Add(border);
        }

        private void AddTestRadioParagraph(int id, string ask, List<TestRadioOption> options, int correctAnswer)
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
            var radioButtons = new List<RadioButton>();

            foreach (var option in options)
            {
                var radioButton = new RadioButton
                {
                    Content = option.Text,
                    GroupName = "TestRadioGroup"
                };
                radioButtons.Add(radioButton);
                stackLayout.Children.Add(radioButton);
            }

            var checkButton = new Button
            {
                Text = "Проверить"
            };
            checkButton.Clicked += (sender, e) =>
            {
                var selected = radioButtons.FirstOrDefault(rb => rb.IsChecked);
                if (selected != null && radioButtons.IndexOf(selected) == correctAnswer)
                {
                    DisplayAlert("Правильно!", "Ваш ответ правильный.", "OK");
                    TestSolvedCorrectly?.Invoke(id);
                }
                else
                {
                    foreach (var rb in radioButtons)
                    {
                        if (rb.IsChecked)
                        {
                            rb.TextColor = Colors.Red;
                        }
                    }
                }
            };

            mainLayout.Children.Add(stackLayout);
            mainLayout.Children.Add(checkButton);
        }

        private void AddTestCheckParagraph(int id, string ask, List<TestRadioOption> options, List<int> correctAnswers)
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
            var checkBoxes = new List<CheckBox>();

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

                checkBoxes.Add(checkBox);
                stackLayout.Children.Add(horizontalLayout);
            }

            var checkButton = new Button
            {
                Text = "Проверить"
            };
            checkButton.Clicked += (sender, e) =>
            {
                var selectedIndexes = checkBoxes.Select((cb, index) => cb.IsChecked ? index : -1).Where(index => index != -1).ToList();
                if (selectedIndexes.SequenceEqual(correctAnswers))
                {
                    DisplayAlert("Правильно!", "Ваш ответ правильный.", "OK");
                    TestSolvedCorrectly?.Invoke(id);
                }
                else
                {
                    foreach (var cb in checkBoxes)
                    {
                        if (cb.IsChecked)
                        {
                            cb.Color = Colors.Red;
                        }
                    }
                }
            };

            mainLayout.Children.Add(stackLayout);
            mainLayout.Children.Add(checkButton);
        }

        private void AddImageParagraph(int id, string content)
        {
            var image = new Image
            {
                Source = content,
                Aspect = Aspect.AspectFit
            };
            mainLayout.Children.Add(image);
        }

        private void AddEntryParagraph(int id, string ask, string correctEntry)
        {
            var askLabel = new Label
            {
                Text = ask,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            mainLayout.Children.Add(askLabel);

            var entry = new Entry();
            mainLayout.Children.Add(entry);

            var checkButton = new Button
            {
                Text = "Проверить"
            };
            checkButton.Clicked += (sender, e) =>
            {
                if (entry.Text == correctEntry)
                {
                    DisplayAlert("Правильно!", "Ваш ответ правильный.", "OK");
                    TestSolvedCorrectly?.Invoke(id);
                }
                else
                {
                    entry.TextColor = Colors.Red;
                }
            };

            mainLayout.Children.Add(checkButton);
        }

        private void AddComplianceParagraph(int id, string ask, List<CompliancePair> pairs)
        {
            var askLabel = new Label
            {
                Text = ask,
                TextColor = (Color)Application.Current.Resources["TextColor"],
                FontSize = (double)Application.Current.Resources["FontSize"],
                FontFamily = (string)Application.Current.Resources["FontFamily"]
            };
            mainLayout.Children.Add(askLabel);

            var labels = pairs.Select(p => new Label { Text = p.Label }).ToList();
            var entries = pairs.Select(p => new Entry { Placeholder = "Введите соответствие" }).ToList();

            var stackLayout = new VerticalStackLayout();
            for (int i = 0; i < pairs.Count; i++)
            {
                var horizontalLayout = new HorizontalStackLayout();
                horizontalLayout.Children.Add(labels[i]);
                horizontalLayout.Children.Add(entries[i]);
                stackLayout.Children.Add(horizontalLayout);
            }

            var checkButton = new Button
            {
                Text = "Проверить"
            };
            checkButton.Clicked += (sender, e) =>
            {
                bool isCorrect = true;
                for (int i = 0; i < pairs.Count; i++)
                {
                    var pair = pairs[i];
                    if (entries[i].Text != pair.Element)
                    {
                        entries[i].TextColor = Colors.Red;
                        isCorrect = false;
                    }
                }

                if (isCorrect)
                {
                    DisplayAlert("Правильно!", "Ваш ответ правильный.", "OK");
                    TestSolvedCorrectly?.Invoke(id);
                }
            };

            mainLayout.Children.Add(stackLayout);
            mainLayout.Children.Add(checkButton);
        }
    }
}
