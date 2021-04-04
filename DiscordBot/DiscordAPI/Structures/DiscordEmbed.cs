using DiscordBot.DiscordAPI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DiscordBot.DiscordAPI.Structures
{
    /// <summary>
    /// Discord Embed Structure
    /// </summary>
    /// <remarks>
    /// All fields in an embed are nullable, and don't need to be included
    /// in the JSON object
    /// </remarks>
    public partial class Embed : DiscordStructure
    {
        public const int MAX_TITLE_LENGTH = 256;
        public const int MAX_DESCRIPTION_LENGTH = 2048;
        public const int MAX_FIELD_COUNT = 25;
        public const int MAX_FIELD_NAME_LENGTH = 256;
        public const int MAX_FIELD_VALUE_LENGTH = 1024;
        public const int MAX_FOOTER_TEXT_LENGTH = 2048;
        public const int MAX_AUTHOR_NAME_LENGTH = 256;
        public const int MAX_RICH_EMBED_CHARACTERS = 6000;

        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Embed Type
        /// </summary>
        /// <remarks>Valid types:
        /// <list type="bullet">
        /// <item>rich</item>
        /// <item>image</item>
        /// <item>video</item>
        /// <item>gifv</item>
        /// <item>article</item>
        /// <item>link</item>
        /// </list>
        /// </remarks>
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        //"YYYY-MM-DDTHH:MM:SS.MSSZ"
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("footer")]
        public EmbedFooter Footer { get; set; }
        [JsonProperty("image")]
        public EmbedImage Image { get; set; }
        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }
        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }
        [JsonProperty("provider")]
        public EmbedProvider Provider { get; set; }
        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }
        [JsonProperty("fields")]
        public List<EmbedField> Fields { get; set; } = new List<EmbedField>();

        /// <summary>
        /// Gets the total characters among sections of the embed that count
        /// towards Discord's rich embed character limit.
        /// </summary>
        [JsonIgnore]
        public int TotalRichEmbedCharacters
        {
            get
            {
                // Add up character count in relevant fields for rich embeds
                int richEmbedCharacterCount = Title?.Length ?? 0 +
                    Description?.Length ?? 0 +
                    Footer?.Text?.Length ?? 0 +
                    Author?.Name?.Length ?? 0;

                // Add up all Name and Value lengths in Fields and add them to total
                richEmbedCharacterCount += Fields.Sum(f => (f.Name?.Length ?? 0) + (f.Value?.Length ?? 0));

                return richEmbedCharacterCount;
            }
        }

        /// <summary>
        /// Compares total characters among sections of the embed
        /// against the max rich embed character limit. The Discord API
        /// will return HTTP code 400(Bad Request) if the embed exceeds this limit.
        /// </summary>
        [JsonIgnore]
        public bool IsValidRichEmbed => TotalRichEmbedCharacters <= MAX_RICH_EMBED_CHARACTERS;

        public Embed()
        {
            Type = "rich";
        }

        public Embed(string title, string type = "rich", string description = null, string url = null)
        {
            Title = title.Length <= MAX_TITLE_LENGTH ? title : title.Substring(0, MAX_TITLE_LENGTH);
            Type = type;
            Description = description?.Length <= MAX_DESCRIPTION_LENGTH ? description : description?.Substring(0, MAX_DESCRIPTION_LENGTH);
            Url = url;
        }

        #region Initialization Methods
        public Embed SetTitle(string title)
        {
            Title = title;
            return this;
        }

        public Embed SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public Embed SetColor(Color color)
        {
            Color = DiscordColor.ToInteger(color);
            return this;
        }

        public Embed SetTimestamp(DateTime time)
        {
            Timestamp = time.ToString("yyyy-MM-ddTHH:mm:ssZ");
            return this;
        }

        public Embed SetFooter(string text, string iconUrl = null, string proxyUrl = null)
        {
            Footer = new EmbedFooter
            {
                Text = text?.Length <= MAX_FOOTER_TEXT_LENGTH ? text : text.Substring(0, MAX_FOOTER_TEXT_LENGTH),
                IconUrl = iconUrl,
                ProxyIconUrl = proxyUrl
            };
            return this;
        }

        /// <summary>
        /// Adds an image to the embed
        /// </summary>
        /// <remarks>
        /// When sending an embed, proxy_url, height, and width should be left empty.
        /// Discord automatically sets these values, and they will exist on received embeds.
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="proxyUrl"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Embed SetImage(string url, string proxyUrl = null, int? height = null, int? width = null)
        {
            Image = new EmbedImage { Url = url, ProxyUrl = proxyUrl, Height = height, Width = width };
            Type = "image";
            return this;
        }

        public Embed SetThumbnail(string url, string proxyUrl = null, int? height = null, int? width = null)
        {
            Thumbnail = new EmbedThumbnail { Url = url, ProxyUrl = proxyUrl, Height = height, Width = width };
            return this;
        }

        /// <summary>
        /// This should not be set when sending an embed object
        /// </summary>
        /// <param name="url"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Embed SetVideo(string url = null, int? height = null, int? width = null)
        {
            Video = new EmbedVideo { Url = url, Height = height, Width = width };
            Type = "video";
            return this;
        }

        /// <summary>
        /// This should not be set when sending an embed object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Embed SetProvider(string name, string url = null)
        {
            Provider = new EmbedProvider { Name = name, Url = url };
            return this;
        }

        public Embed SetAuthor(string name, string url = null, string iconUrl = null, string proxyIconUrl = null)
        {
            Author = new EmbedAuthor
            {
                Name = name.Length <= MAX_AUTHOR_NAME_LENGTH ? name : name.Substring(0, MAX_AUTHOR_NAME_LENGTH),
                Url = url,
                IconUrl = iconUrl,
                ProxyIconUrl = proxyIconUrl
            };
            return this;
        }

        public Embed AddField(string name, string value, bool? inline = null)
        {
            if (Fields.Count < MAX_FIELD_COUNT)
            {
                Fields.Add(new EmbedField
                {
                    Name = name.Length <= MAX_FIELD_NAME_LENGTH ? name : name.Substring(0, MAX_FIELD_NAME_LENGTH),
                    Value = value.Length <= MAX_FIELD_VALUE_LENGTH ? value : value.Substring(0, MAX_FIELD_VALUE_LENGTH),
                    Inline = inline
                });
            }
            return this;
        }
        #endregion

        #region Nested Embed Types
        public class EmbedFooter
        {
            [JsonRequired, JsonProperty("text")]
            public string Text { get; set; }
            [JsonProperty("icon_url")]
            public string IconUrl { get; set; }
            [JsonProperty("proxy_icon_url")]
            public string ProxyIconUrl { get; set; }
        }

        public class EmbedImage
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("proxy_url")]
            public string ProxyUrl { get; set; }
            [JsonProperty("height")]
            public int? Height { get; set; }
            [JsonProperty("width")]
            public int? Width { get; set; }
        }

        public class EmbedThumbnail
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("proxy_url")]
            public string ProxyUrl { get; set; }
            [JsonProperty("height")]
            public int? Height { get; set; }
            [JsonProperty("width")]
            public int? Width { get; set; }
        }

        public class EmbedVideo
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("height")]
            public int? Height { get; set; }
            [JsonProperty("width")]
            public int? Width { get; set; }
        }

        public class EmbedProvider
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class EmbedAuthor
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("icon_url")]
            public string IconUrl { get; set; }
            [JsonProperty("proxy_icon_url")]
            public string ProxyIconUrl { get; set; }
        }

        public class EmbedField
        {
            [JsonRequired, JsonProperty("name")]
            public string Name { get; set; }
            [JsonRequired, JsonProperty("value")]
            public string Value { get; set; }
            [JsonProperty("inline")]
            public bool? Inline { get; set; }
        }
        #endregion
    }
}
