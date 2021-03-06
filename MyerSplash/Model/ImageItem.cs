﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using JP.Utils.Data.Json;
using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplash.Data;
using MyerSplash.ViewModel;
using MyerSplashShared.Service;
using MyerSplashShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Model
{
    public class ImageItem : ModelBase
    {
        [IgnoreDataMember]
        public DownloadsViewModel DownloadsVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<DownloadsViewModel>();
            }
        }

        private UnsplashImage _image;
        public UnsplashImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    RaisePropertyChanged(() => Image);
                }
            }
        }

        private CachedBitmapSource _listImageBitmap;
        [IgnoreDataMember]
        public CachedBitmapSource ListImageBitmap
        {
            get
            {
                return _listImageBitmap;
            }
            set
            {
                if (_listImageBitmap != value)
                {
                    _listImageBitmap = value;
                    RaisePropertyChanged(() => ListImageBitmap);
                }
            }
        }

        private CachedBitmapSource _largeBitmap;
        [IgnoreDataMember]
        public CachedBitmapSource LargeBitmap
        {
            get
            {
                return _largeBitmap;
            }
            set
            {
                if (_largeBitmap != value)
                {
                    _largeBitmap = value;
                    RaisePropertyChanged(() => LargeBitmap);
                }
            }
        }

        [IgnoreDataMember]
        public Thickness NameThickness
        {
            get
            {
                if (Image.IsUnsplash)
                {
                    return new Thickness(0, 0, 0, 2);
                }
                else
                {
                    return new Thickness(0);
                }
            }
        }

        private SolidColorBrush _majorColor;
        [IgnoreDataMember]
        public SolidColorBrush MajorColor
        {
            get
            {
                return _majorColor;
            }
            set
            {
                if (_majorColor != value)
                {
                    _majorColor = value;
                    RaisePropertyChanged(() => MajorColor);
                    if (ColorConverter.IsLight(value.Color))
                    {
                        InfoForeColor = new SolidColorBrush(Colors.Black);
                        InfoForeColor = new SolidColorBrush(Colors.Black);
                        InfoForeColor = new SolidColorBrush(Colors.Black);
                        BtnForeColor = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        InfoForeColor = new SolidColorBrush(Colors.White);
                        InfoForeColor = new SolidColorBrush(Colors.White);
                        InfoForeColor = new SolidColorBrush(Colors.White);
                        BtnForeColor = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }

        private SolidColorBrush _infoForeColor;
        [IgnoreDataMember]
        public SolidColorBrush InfoForeColor
        {
            get
            {
                return _infoForeColor;
            }
            set
            {
                if (_infoForeColor != value)
                {
                    _infoForeColor = value;
                    RaisePropertyChanged(() => InfoForeColor);
                }
            }
        }

        private SolidColorBrush _btnForeColor;
        [IgnoreDataMember]
        public SolidColorBrush BtnForeColor
        {
            get
            {
                return _btnForeColor;
            }
            set
            {
                if (_btnForeColor != value)
                {
                    _btnForeColor = value;
                    RaisePropertyChanged(() => BtnForeColor);
                }
            }
        }

        private SolidColorBrush _backColorBrush;
        [IgnoreDataMember]
        public SolidColorBrush BackColorBrush
        {
            get
            {
                return _backColorBrush;
            }
            set
            {
                if (_backColorBrush != value)
                {
                    _backColorBrush = value;
                    RaisePropertyChanged(() => BackColorBrush);
                }
            }
        }

        private RelayCommand _shareCommand;
        [IgnoreDataMember]
        public RelayCommand ShareCommand
        {
            get
            {
                if (_shareCommand != null) return _shareCommand;
                return _shareCommand = new RelayCommand(() =>
                {
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        private RelayCommand _copyUrlCommand;
        [IgnoreDataMember]
        public RelayCommand CopyUrlCommand
        {
            get
            {
                if (_copyUrlCommand != null) return _copyUrlCommand;
                return _copyUrlCommand = new RelayCommand(() =>
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(GetSaveImageUrlFromSettings());
                    Clipboard.SetContent(dataPackage);
                });
            }
        }

        private RelayCommand _navigateHomeCommand;
        [IgnoreDataMember]
        public RelayCommand NavigateHomeCommand
        {
            get
            {
                if (_navigateHomeCommand != null) return _navigateHomeCommand;
                return _navigateHomeCommand = new RelayCommand(async () =>
                {
                    if (!string.IsNullOrEmpty(Image.Owner.Links.HomePageUrl))
                    {
                        await Launcher.LaunchUriAsync(new Uri(Image.Owner.Links.HomePageUrl));
                    }
                });
            }
        }

        private RelayCommand _downloadCommand;
        [IgnoreDataMember]
        public RelayCommand DownloadCommand
        {
            get
            {
                if (_downloadCommand != null) return _downloadCommand;
                return _downloadCommand = new RelayCommand(() =>
                {
                    var downloaditem = new DownloadItem(this);
                    var task = downloaditem.DownloadFullImageAsync(JP.Utils.Network.CTSFactory.MakeCTS());
                    var task2 = DownloadsVM.AddDownloadingImageAsync(downloaditem);
                });
            }
        }

        public Visibility LikesVisibility
        {
            get
            {
                if (Image.IsUnsplash)
                {
                    return Visibility.Visible;
                }
                else return Visibility.Collapsed;
            }
        }

        public Visibility RecommendationVisibility
        {
            get
            {
                if (Image.IsUnsplash)
                {
                    return Visibility.Collapsed;
                }
                else return Visibility.Visible;
            }
        }

        public Visibility ExifThumbVisibility
        {
            get
            {
                if (Image.IsUnsplash)
                {
                    return Visibility.Visible;
                }
                else return Visibility.Collapsed;
            }
        }

        public string PhotoByText
        {
            get
            {
                if (Image.IsUnsplash)
                {
                    return "photo by";
                }
                else return "recommended by";
            }
        }

        [IgnoreDataMember]
        public StorageFile DownloadedFile { get; set; }

        public DownloadStatus DownloadStatus { get; set; } = DownloadStatus.Pending;

        public string ShareText => $"Share {Image.Owner.Name}'s amazing photo from MyerSplash app. {Image.Urls.Full}";

        public string LikesString
        {
            get
            {
                return Image.Likes.ToString();
            }
        }

        public string LocationString
        {
            get
            {
                if (Image.Location == null || Image.Location.City == null || Image.Location.Country == null)
                {
                    return "Unknown";
                }
                return $"{Image.Location.City}, {Image.Location.Country}";
            }
        }

        public string SizeString
        {
            get
            {
                return $"{Image.Width} x {Image.Height}";
            }
        }

        private ImageService _service = new ImageService(null, new UnsplashImageFactory(false));

        public ImageItem()
        {
            ListImageBitmap = new CachedBitmapSource();
            LargeBitmap = new CachedBitmapSource();
        }

        public ImageItem(UnsplashImage image)
        {
            Image = image;
            ListImageBitmap = new CachedBitmapSource();
            LargeBitmap = new CachedBitmapSource();
        }

        public void Init()
        {
            BackColorBrush = new SolidColorBrush(Image.ColorValue.ToColor());
            MajorColor = new SolidColorBrush(Image.ColorValue.ToColor());
        }

        public string GetFileNameForDownloading()
        {
            var fileName = $"{Image.Owner.Name}  {Image.SimpleCreateTimeString}.jpg";
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                if (fileName.Contains(c))
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
            }
            return fileName;
        }

        public async Task SetDataRequestData(DataRequest request)
        {
            DataPackage requestData = request.Data;
            requestData.Properties.Title = "Share photo";
            requestData.Properties.ContentSourceWebLink = new Uri(Image.Urls.Full);
            requestData.Properties.ContentSourceApplicationLink = new Uri(Image.Urls.Full);

            requestData.SetText(ShareText);

            var file = await StorageFile.GetFileFromPathAsync(ListImageBitmap.LocalPath);
            if (file != null)
            {
                List<IStorageItem> imageItems = new List<IStorageItem>();
                imageItems.Add(file);
                requestData.SetStorageItems(imageItems);

                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(file);
                requestData.SetBitmap(imageStreamRef);
                requestData.Properties.Thumbnail = imageStreamRef;
            }
        }

        public async Task DownloadBitmapForListAsync()
        {
            var url = GetListImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return;

            var task = CheckAndGetDownloadedFileAsync();

            ListImageBitmap.ExpectedFileName = Image.ID + ".jpg";
            ListImageBitmap.RemoteUrl = url;
            await ListImageBitmap.LoadBitmapAsync();
        }

        public async Task CheckAndGetDownloadedFileAsync()
        {
            var name = GetFileNameForDownloading();
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                if (await folder.TryGetItemAsync(name) is StorageFile file)
                {
                    var pro = await file.GetBasicPropertiesAsync();
                    if (pro.Size > 10)
                    {
                        this.DownloadStatus = DownloadStatus.Ok;
                        DownloadedFile = file;
                    }
                }
            }
        }

        public string GetListImageUrlFromSettings()
        {
            var quality = App.AppSettings.LoadQuality;
            switch (quality)
            {
                case 0: return Image.Urls.Regular;
                case 1: return Image.Urls.Small;
                case 2: return Image.Urls.Thumb;
                default: return "";
            }
        }

        public string GetSaveImageUrlFromSettings()
        {
            var quality = App.AppSettings.SaveQuality;
            switch (quality)
            {
                case 0: return Image.Urls.Raw;
                case 1: return Image.Urls.Full;
                case 2: return Image.Urls.Regular;
                default: return "";
            }
        }

        public string GetDownloadLocationUrl()
        {
            return Image?.Links?.DownloadLocation;
        }

        public async Task GetExifInfoAsync()
        {
            var result = await _service.GetImageDetailAsync(Image.ID, JP.Utils.Network.CTSFactory.MakeCTS().Token);
            if (result.IsRequestSuccessful)
            {
                JsonObject.TryParse(result.JsonSrc, out JsonObject json);
                if (json != null)
                {
                    var exifObject = JsonParser.GetJsonObjFromJsonObj(json, "exif");
                    if (exifObject != null)
                    {
                        Image.Exif = JsonConvert.DeserializeObject<ImageExif>(exifObject.ToString());
                        RaisePropertyChanged(() => SizeString);
                    }

                    var locationObj = JsonParser.GetJsonObjFromJsonObj(json, "location");
                    if (locationObj != null)
                    {
                        Image.Location = JsonConvert.DeserializeObject<ImageLocation>(locationObj.ToString());
                        RaisePropertyChanged(() => LocationString);
                    }
                }
            }
        }
    }
}