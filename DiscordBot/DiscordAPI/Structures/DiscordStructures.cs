using DiscordBot.DiscordAPI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security.Policy;
using System.Windows.Navigation;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace DiscordBot.DiscordAPI.Structures
{
    //TODO: Move all structures to their own files
    public abstract class DiscordStructure
    {
        public virtual string ToJson()
        {
            //TODO: Do any structures require different serializer setting?
            var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }

        public virtual string ToJson(JsonSerializerSettings settings, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(this, formatting, settings);
        }
    }

    /// <summary>
    /// Message structure containing the event data that is received in MESSAGE_CREATE events.
    /// Event data is usually the field "d" in the Json paylod
    /// </summary>
    public class Message : DiscordStructure
    {
        /// <summary>
        /// Message ID
        /// </summary>
        /// <remarks>
        /// Type: snowflake
        /// </remarks>
        public string ID { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelID { get; set; }

        [JsonProperty("guild_id")] //May be null
        public string? GuildID { get; set; }

        /// <summary>
        /// The author of this message.
        /// Not guaranteed to be a valid user, such as messages
        /// created through webhooks.
        /// </summary>
        [JsonProperty("author")]
        public DiscordUser User { get; set; }

        /// <summary>
        /// This field will exist in MESSAGE_CREATE and MESSAGE_UPDATE events
        /// sent from text-based guilds.
        /// </summary>
        [JsonProperty("member")]
        public DiscordGuildMember? Member { get; set; }

        /// <summary>
        /// ISO8601 timestamp of when the message was created
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// ISO8601 timestamp of when message was last edited
        /// </summary>
        [JsonProperty("edited_timestamp")]
        public string? EditedTimestamp { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        // Text-to-speech
        [JsonProperty("tts")]
        public bool Tts { get; set; }

        [JsonProperty("mention_everyone")]
        public bool MentionsEveryone { get; set; }

        [JsonProperty("mentions")]
        public List<DiscordMentionUser> Mentions { get; set; }

        [JsonProperty("mention_roles")]
        public List<string> MentionRoles { get; set; }

        [JsonProperty("mention_channels")]
        public List<DiscordMentionChannel>? MentionChannels { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; }

        //Reactions

        /// <summary>
        /// For validating when a message was sent. Can be an integer or string
        /// </summary>
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("webhook_id")]
        public string? WebhookID { get; set; }

        /// <summary>
        /// The type of message
        /// </summary>
        /// <remarks>
        /// For list of types:
        /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-types">
        /// Discord Documentation - Message Types
        /// </see>
        /// </remarks> 
        [JsonProperty("type")]
        public int Type { get; set; }

        //TODO:
        //Activity

        //Application

        //MessageReference

        //Flags

        //Stickers

        //ReferencedMessage

        /**
         *  id	snowflake	id      of the message
            channel_id	    snowflake	id of the channel the message was sent in
            guild_id?	    snowflake	id of the guild the message was sent in
            author*	        user object	the author of this message (not guaranteed to be a valid user, see below)
            member?**	    partial guild member object	    member properties for this message's author
            content	        string	    contents of the message
            timestamp	    ISO8601 timestamp	when this message was sent
            edited_timestamp	?ISO8601 timestamp	when this message was edited (or null if never)
            tts	            boolean	whether this was a TTS message
            mention_everyone	boolean	whether this message mentions everyone
            mentions***	    array of user objects, with an additional partial member field	users specifically mentioned in the message
            mention_roles	array of role object ids	roles specifically mentioned in this message
            mention_channels?****	array of channel mention objects	channels specifically mentioned in this message
            attachments	    array of attachment objects	any attached files
            embeds	        array of embed objects	any embedded content
            reactions?	    array of reaction objects	reactions to the message
            nonce?	        integer or string	used for validating a message was sent
            pinned	        boolean	whether this message is pinned
            webhook_id?	    snowflake	if the message is generated by a webhook, this is the webhook's id
            type	        integer	type of message
            activity?	    message activity object	sent with Rich Presence-related chat embeds
            application?	message application object	sent with Rich Presence-related chat embeds
            message_reference?	message reference object	reference data sent with crossposted messages
            flags?	        integer	message flags ORd together, describes extra features of the message
            stickers?	array of sticker objects	the stickers sent with the message (bots currently can only receive messages with stickers, not send)
            referenced_message?*****	?message object	the message associated with the message_reference
         */
    }

    /// <summary>
    /// Message structure containing data needed to send a message to the Discord API
    /// </summary>
    public class SendMessage : DiscordStructure
    {
        // Nonce values are random 8 digits numbers for this class
        private const int NONCE_MIN = 100000000;
        private const int NONCE_MAX = 1000000000;

        [JsonProperty("content")]
        public string Content { get; set; }
        // Nonce can accept an integer value
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        // Text-to-speech
        [JsonProperty("tts")]
        public bool Tts { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("embed")]
        public Embed Embed { get; set; }
        // Json encoded additional request fields
        [JsonProperty("payload_json")]
        public string Payload { get; set; }
        // TODO: structure as AllowedMentions class
        [JsonProperty("allowed_mentions")]
        public string AllowedMentions { get; set; }

        public SendMessage() { }
        public SendMessage(string content) { Content = content; }
        public SendMessage(Embed embed) { Embed = embed; }

        public SendMessage SetNonce()
        {
            Nonce = new Random().Next(NONCE_MIN, NONCE_MAX).ToString();
            return this;
        }
    }

    //DELETE: this has been replaced by Message
    [Obsolete("Replaced by Message structure")]
    public class ReceiveMessage : DiscordStructure
    {
        /// <summary>
        /// Message ID
        /// </summary>
        /// <remarks>
        /// Type: snowflake
        /// </remarks>
        public string ID { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelID { get; set; }
        [JsonProperty("guild_id")] //May be null
        public string GuildID { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        // Text-to-speech
        [JsonProperty("tts")]
        public bool Tts { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; }
        // Nonce can accept an integer value
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        // Json encoded additional request fields
        [JsonProperty("payload_json")]
        public string Payload { get; set; }

        /**
         *  id	snowflake	id      of the message
            channel_id	    snowflake	id of the channel the message was sent in
            guild_id?	    snowflake	id of the guild the message was sent in
            author*	        user object	the author of this message (not guaranteed to be a valid user, see below)
            member?**	    partial guild member object	    member properties for this message's author
            content	string	contents of the message
            timestamp	    ISO8601 timestamp	when this message was sent
            edited_timestamp	?ISO8601 timestamp	when this message was edited (or null if never)
            tts	            boolean	whether this was a TTS message
            mention_everyone	boolean	whether this message mentions everyone
            mentions***	    array of user objects, with an additional partial member field	users specifically mentioned in the message
            mention_roles	array of role object ids	roles specifically mentioned in this message
            mention_channels?****	array of channel mention objects	channels specifically mentioned in this message
            attachments	    array of attachment objects	any attached files
            embeds	        array of embed objects	any embedded content
            reactions?	    array of reaction objects	reactions to the message
            nonce?	        integer or string	used for validating a message was sent
            pinned	        boolean	whether this message is pinned
            webhook_id?	    snowflake	if the message is generated by a webhook, this is the webhook's id
            type	        integer	type of message
            activity?	    message activity object	sent with Rich Presence-related chat embeds
            application?	message application object	sent with Rich Presence-related chat embeds
            message_reference?	message reference object	reference data sent with crossposted messages
            flags?	        integer	message flags ORd together, describes extra features of the message
         */
    }

    public class Attachment
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        /// <summary>File size in bytes</summary>
        public int Size { get; set; }
        public string Url { get; set; }
        public string ProxyUrl { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
    }
}
