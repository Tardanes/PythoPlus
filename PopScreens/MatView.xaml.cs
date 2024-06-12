using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PythoPlus.PopScreens
{
    public partial class MatView : ContentPage
    {
        private string userId;
        private int materialNumber;
        private LocalDbService _localDbService;
        private DateTime startTime;

        public event Action<int> TestSolvedCorrectly;

        public MatView(int materialNumber, string filePath)
        {
            InitializeComponent();
            LoadUserId();
            this.materialNumber = materialNumber;
            _localDbService = new LocalDbService(FileSystem.AppDataDirectory);
            Appearing += OnAppearing;
            Disappearing += OnDisappearing;
            LoadContent(filePath);
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

        private void OnAppearing(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
        }

        private async void OnDisappearing(object sender, EventArgs e)
        {
            var timeSpent = DateTime.Now - startTime;
            var stats = await _localDbService.GetCollectionAsync<UserStats>("mat_stat");
            var userStats = stats.FirstOrDefault(stat => stat.UserId == userId);

            if (userStats == null)
            {
                userStats = new UserStats
                {
                    UserId = userId,
                    TotalTimeSpent = timeSpent.TotalSeconds,
                    TotalAnswers = 0,
                    CorrectAnswers = 0
                };
                await _localDbService.AddRecordAsync("mat_stat", userStats);
            }
            else
            {
                userStats.TotalTimeSpent += timeSpent.TotalSeconds;
                await _localDbService.UpdateRecordAsync("mat_stat", stat => stat.UserId == userId, userStats);
            }
        }

        private async void RecordProgress(int questionId, bool isCorrect)
        {
            var progressDocuments = await _localDbService.GetCollectionAsync<ProgressDocument>("mat_progress");
            var progressDocument = progressDocuments.FirstOrDefault(p => p.UserId == userId && p.MaterialNumber == materialNumber);

            if (progressDocument == null)
            {
                progressDocument = new ProgressDocument
                {
                    UserId = userId,
                    MaterialNumber = materialNumber,
                    CorrectAnswers = isCorrect ? new List<int> { questionId } : new List<int>()
                };
                await _localDbService.AddRecordAsync("mat_progress", progressDocument);
            }
            else
            {
                if (isCorrect && !progressDocument.CorrectAnswers.Contains(questionId))
                {
                    progressDocument.CorrectAnswers.Add(questionId);
                    await _localDbService.UpdateRecordAsync("mat_progress", p => p.UserId == userId && p.MaterialNumber == materialNumber, progressDocument);
                }
            }

            var stats = await _localDbService.GetCollectionAsync<UserStats>("mat_stat");
            var userStats = stats.FirstOrDefault(stat => stat.UserId == userId);

            if (userStats == null)
            {
                userStats = new UserStats
                {
                    UserId = userId,
                    TotalTimeSpent = 0,
                    TotalAnswers = 1,
                    CorrectAnswers = isCorrect ? 1 : 0
                };
                await _localDbService.AddRecordAsync("mat_stat", userStats);
            }
            else
            {
                userStats.TotalAnswers++;
                if (isCorrect)
                {
                    userStats.CorrectAnswers++;
                }
                await _localDbService.UpdateRecordAsync("mat_stat", stat => stat.UserId == userId, userStats);
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
                Text = "Перевірити"
            };
            checkButton.Clicked += async (sender, e) =>
            {
                var selected = radioButtons.FirstOrDefault(rb => rb.IsChecked);
                bool isCorrect = selected != null && radioButtons.IndexOf(selected) == correctAnswer;
                RecordProgress(id, isCorrect);
                if (isCorrect)
                {
                    await DisplayAlert("Вірно!", "Ваша відповідь абсолютно вірна!", "Далі");
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
            checkButton.Clicked += async (sender, e) =>
            {
                var selectedIndexes = checkBoxes.Select((cb, index) => cb.IsChecked ? index : -1).Where(index => index != -1).ToList();
                bool isCorrect = selectedIndexes.SequenceEqual(correctAnswers);
                RecordProgress(id, isCorrect);
                if (isCorrect)
                {
                    await DisplayAlert("Вірно!", "Ваша відповідь абсолютно вірна!", "Далі");
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
            checkButton.Clicked += async (sender, e) =>
            {
                bool isCorrect = entry.Text == correctEntry;
                RecordProgress(id, isCorrect);
                if (isCorrect)
                {
                    await DisplayAlert("Вірно!", "Ваша відповідь абсолютно вірна!", "Далі");
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
            var pickers = pairs.Select(p =>
            {
                var picker = new Picker();
                picker.ItemsSource = pairs.Select(pair => pair.Element).ToList(); // Добавить все возможные элементы в Picker
                return picker;
            }).ToList();

            var stackLayout = new VerticalStackLayout();
            for (int i = 0; i < pairs.Count; i++)
            {
                var horizontalLayout = new HorizontalStackLayout();
                horizontalLayout.Children.Add(labels[i]);
                horizontalLayout.Children.Add(pickers[i]);
                stackLayout.Children.Add(horizontalLayout);
            }

            var checkButton = new Button
            {
                Text = "Проверить"
            };
            checkButton.Clicked += async (sender, e) =>
            {
                bool isCorrect = true;
                for (int i = 0; i < pairs.Count; i++)
                {
                    var pair = pairs[i];
                    var selectedElement = pickers[i].SelectedItem?.ToString();
                    if (selectedElement != pair.Element)
                    {
                        pickers[i].BackgroundColor = Colors.Red;
                        isCorrect = false;
                    }
                }

                RecordProgress(id, isCorrect);
                if (isCorrect)
                {
                    await DisplayAlert("Вірно!", "Ваша відповідь абсолютно вірна!", "Далі");
                    TestSolvedCorrectly?.Invoke(id);
                }
            };

            mainLayout.Children.Add(stackLayout);
            mainLayout.Children.Add(checkButton);
        }
    }
    public class UserStats
    {
        public string UserId { get; set; }
        public double TotalTimeSpent { get; set; }
        public int LoginsCount { get; set; }
        public int TotalAnswers { get; set; }
        public int CorrectAnswers { get; set; }
    }
}
