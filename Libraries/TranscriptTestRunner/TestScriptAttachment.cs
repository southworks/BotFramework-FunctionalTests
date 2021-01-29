// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace TranscriptTestRunner
{
    /// <summary>
    /// <see cref="TestRunner"/> representation of an attachment.
    /// </summary>
    public class TestScriptAttachment
    {
        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>
        /// The attachment name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attachment content type.
        /// </summary>
        /// <value>
        /// The attachment content type.
        /// </value>
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the attachment content url.
        /// </summary>
        /// <value>
        /// The attachment content url.
        /// </value>
        [JsonProperty("contentUrl")]
#pragma warning disable CA1056 // Uri properties should not be strings
        public string ContentUrl { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
    }
}
