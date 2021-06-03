using DiscordBot.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiscordBot.Windows
{
    /// <summary>
    /// Interaction logic for ImageSorterWindow.xaml
    /// </summary>
    public partial class ImageSorterWindow : Window
    {
        private readonly ImageSorterViewModel imageSorterViewModel;

        public ImageSorterWindow(ImageSorterViewModel viewModel)
        {
            InitializeComponent();

            imageSorterViewModel = viewModel;
            DataContext = imageSorterViewModel;
            //TagSuggestionsList.Visibility = Visibility.Collapsed;
        }

        private void ThumbnailList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imageSorterViewModel.SelectImage((ImageSorterViewModel.ImageInfo)ThumbnailList.SelectedItem);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            imageSorterViewModel.SaveImage();
        }

        private void TagSuggestionsList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    string tag = (string)TagSuggestionsList.SelectedItem;

                    int wordStart = Math.Max(TagsInput.Text.LastIndexOf(' ', TagsInput.CaretIndex - 1) + 1, 0);
                    int wordEnd = TagsInput.Text.IndexOf(' ', TagsInput.CaretIndex);
                    string currentTag = (wordEnd > 0) ?
                        TagsInput.Text.Substring(wordStart, wordEnd - wordStart) :
                        TagsInput.Text.Substring(wordStart);

                    string tagEnd = tag.Substring(currentTag.Trim().Length) + " ";
                    //TagsInput.Text = TagsInput.Text.Insert(Math.Max(TagsInput.CaretIndex, 0), tagEnd);
                    TagsInput.Text = TagsInput.Text.Remove(wordStart, currentTag.Length).Insert(wordStart, tag + " ");
                    TagsInput.CaretIndex = TagsInput.Text.Length;
                    TagsInput.Focus();
                    imageSorterViewModel.ClearTags();
                }
                else if (TagSuggestionsList.Items.Count > 0)
                {
                    if (e.Key == Key.Up)
                    {
                        e.Handled = true;
                        int index = TagSuggestionsList.Items.IndexOf(TagSuggestionsList.SelectedItem);
                        index = (index + TagSuggestionsList.Items.Count - 1) % TagSuggestionsList.Items.Count;

                        TagSuggestionsList.SelectedItem = TagSuggestionsList.Items[index];
                    }
                    else if (e.Key == Key.Down)
                    {
                        e.Handled = true;
                        int index = TagSuggestionsList.Items.IndexOf(TagSuggestionsList.SelectedItem);
                        index = (index + 1) % TagSuggestionsList.Items.Count;

                        TagSuggestionsList.SelectedItem = TagSuggestionsList.Items[index];
                    }
                }
            }
            catch
            {

            }
        }

        private void TagsInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TagsInput.Text))
                {
                    // Get the spaces around the word where the carat is, to separate the current word
                    // This line will include the space before the word if it isn't the start of the string, so we Trim after
                    int wordStart = Math.Max(TagsInput.Text.LastIndexOf(' ', TagsInput.CaretIndex - 1), 0);
                    int wordEnd = TagsInput.Text.IndexOf(' ', TagsInput.CaretIndex);
                    string currentTag = (wordEnd > 0) ? 
                        TagsInput.Text.Substring(wordStart, wordEnd - wordStart) : 
                        TagsInput.Text.Substring(wordStart);

                    if (!string.IsNullOrWhiteSpace(currentTag))
                    {
                        imageSorterViewModel.DebounceGetSimilarTags(currentTag.Trim());
                    }
                }
                else
                {
                    imageSorterViewModel.ClearTags();
                }
            }
            catch
            {

            }
        }

        private void TagsInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && TagSuggestionsList.Items.Count > 0)
            {
                TagSuggestionsList.Focus();
                TagSuggestionsList.SelectedItem = TagSuggestionsList.Items[0];
                e.Handled = true;
            }
        }
    }
}
