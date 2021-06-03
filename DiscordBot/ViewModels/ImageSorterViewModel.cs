using DiscordBot.Models;
using DiscordBot.Utility;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using DiscordBot.Extensions;
using System.Threading;
using Microsoft.Extensions.Options;
using DiscordBot.Configuration;

namespace DiscordBot.ViewModels
{
    public class ImageSorterViewModel : INotifyPropertyChanged
    {
        //TODO: move to configuration
        private readonly string[] imageExtensions = { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff" };

        public ObservableCollection<ImageInfo> Images { get; private set; }
        public ImageInfo CurrentImage { get => currentImage; private set { currentImage = value; NotifyPropertyChanged(); } }
        public string CurrentDirectory { get => currentDirectory.FullName; private set { LoadImagesFromDirectory(value); NotifyPropertyChanged(); } }
        public bool CurrentImageIsSaved { get => currentImageIsSaved; private set { currentImageIsSaved = value; NotifyPropertyChanged(); }  }
        public string Tags { get => tags; set { tags = value; NotifyPropertyChanged(); } }
        public List<string> SuggestedTags { get => suggestedTags; set { suggestedTags = value; NotifyPropertyChanged(); } }

        private readonly DatabaseContext dbContext;
        private ILogger Logger { get; set; }
        private ImageInfo currentImage;
        private DirectoryInfo currentDirectory;
        private bool currentImageIsSaved;
        private string tags;
        public List<string> suggestedTags;
        private Action<string> debounceGetTagsAction;

        public ImageSorterViewModel(DatabaseContext databaseContext, ILogger logger, IOptions<DirectoriesConfiguration> options)
        {
            dbContext = databaseContext;
            Logger = logger;
            Images = new ObservableCollection<ImageInfo>();

            Action<string> getTagsAction = async (tag) => SuggestedTags = await GetSimilarTags(tag);
            debounceGetTagsAction = getTagsAction.Debounce(300);

            LoadImagesFromDirectory(options.Value.DefaultImagesFolder);
        }

        public Task LoadImagesFromDirectory(string directory)
        {
            try
            {
                currentDirectory = new DirectoryInfo(directory);

                return LoadImagesFromDirectory();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Task.FromException(ex);
            }
        }

        public async Task LoadImagesFromDirectory()
        {
            try
            {
                var files = currentDirectory.GetFiles();
                var imageInfos = files.Where(f => imageExtensions.Any(i => i.ToLower() == f.Extension.ToLower()))
                    .Select(f =>
                    {
                        var image = new ImageInfo
                        {
                            FileInfo = f,
                            ShellFile = ShellFile.FromFilePath(f.FullName),
                            ImageUri = f.FullName,                          
                        };

                        // This seems to cause the thumbnails to load faster than if ThumbnailSource is defined as:
                        // ThumbnailSource => Thumbnail?.BitmapSource
                        // no idea why though.
                        image.ThumbnailSource = image.Thumbnail.BitmapSource;

                        return image;
                    });

                foreach (var image in imageInfos)
                {
                    Images.Add(image);
                }

                CurrentImage = Images.FirstOrDefault();
                NotifyPropertyChanged(nameof(Images));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void SelectImage(ImageInfo image)
        {
            CurrentImage = image;
            var savedImage = dbContext.Images.Include(i => i.Tags).FirstOrDefault(i => image.FullName == i.PhysicalUrl);
            CurrentImageIsSaved = (savedImage != null);
            Tags = (savedImage != null) ? string.Join(" ", savedImage.Tags.Select(t => t.Name)) + " " : "";

            NotifyPropertyChanged(nameof(CurrentImage));
        }

        public async void SaveImage()
        {
            try
            {
                if (CurrentImage != null)
                {
                    var dbImage = dbContext.Images.FirstOrDefault(i => i.PhysicalUrl == CurrentImage.FullName);
                    if (dbImage != null)
                    {
                        dbImage.PhysicalUrl = CurrentImage.FullName; //TODO: Change to different variable to change filename
                        var tags = Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var dbTags = tags.Select(t => dbContext.Tags.GetOrAddIfNotExists(new Tag { Name = t }, (tag) => tag.Name == t));
                        dbImage.Tags.Clear();
                        foreach (var tag in dbTags)
                        {
                            dbImage.Tags.Add(tag);
                        }
                    }
                    else
                    {
                        dbImage = new Models.Image()
                        {
                            PhysicalUrl = CurrentImage.FullName,
                            Extension = CurrentImage.FileInfo.Extension,
                            FileSize = CurrentImage.FileInfo.Length,
                            CreatedBy = "Application",
                            Width = (int)(CurrentImage.ShellFile.Properties.System.Image.HorizontalSize.Value ?? 0),
                            Height = (int)(CurrentImage.ShellFile.Properties.System.Image.VerticalSize.Value ?? 0),
                        };

                        var tags = Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var dbTags = tags.Select(t => dbContext.Tags.GetOrAddIfNotExists(new Tag { Name = t }, (tag) => tag.Name == t));
                        dbImage.Tags = dbTags.ToList();
                        dbContext.Images.Add(dbImage);
                    }
                    await dbContext.SaveChangesAsync();
                    CurrentImageIsSaved = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                //TODO: Do we need to clear changes?
            }
        }

        public async Task DebounceGetSimilarTags(string tag)
        {
            debounceGetTagsAction(tag);
        }

        public async Task<List<string>> GetSimilarTags(string tag)
        {
            try
            {
                var tags = dbContext.Tags.Where(t => t.Name.Contains(tag))
                .OrderBy(t => t.Name.IndexOf(tag))
                .Select(t => t.Name);

                return await tags.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;
        }

        public void AddTag(string tag)
        {
            try
            {
                if (!Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries).Any(t => t.ToLower() == tag.Trim().ToLower()))
                {
                    Tags += $"{tag.Trim().ToLower()} ";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void ClearTags()
        {
            SuggestedTags?.Clear();
            NotifyPropertyChanged(nameof(SuggestedTags));
        }

        public class ImageInfo
        {
            public string ImageUri { get; set; }
            public FileInfo FileInfo { get; set; }
            public ShellFile ShellFile { get; set; }
            public ShellThumbnail Thumbnail => ShellFile?.Thumbnail;
            public BitmapSource ThumbnailSource { get; set; }
            public string FileName => FileInfo.Name;
            public string FullName => FileInfo.FullName;
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
