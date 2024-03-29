﻿using MsgPack.Serialization;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.MsgPack;
using NServiceBus.Serialization;
using NServiceBus.Settings;

namespace NServiceBus;

/// <summary>
/// Extensions for <see cref="SerializationExtensions{T}"/> to manipulate how messages are serialized.
/// </summary>
public static class MsgPackConfigurationExtensions
{
    /// <summary>
    /// Configures the <see cref="SerializationContext"/> to use.
    /// </summary>
    /// <param name="config">The <see cref="SerializationExtensions{T}"/> instance.</param>
    /// <param name="context">The <see cref="SerializationContext"/> to use.</param>
    public static void Context(this SerializationExtensions<MsgPackSerializer> config, SerializationContext context)
    {
        var settings = config.GetSettings();
        settings.Set(context);
    }

    internal static SerializationContext GetContext(this ReadOnlySettings settings) =>
        settings.GetOrDefault<SerializationContext>();

    /// <summary>
    /// Configures string to use for <see cref="Headers.ContentType"/> headers.
    /// </summary>
    /// <remarks>
    /// Defaults to "wire".
    /// </remarks>
    /// <param name="config">The <see cref="SerializationExtensions{T}"/> instance.</param>
    /// <param name="contentTypeKey">The content type key to use.</param>
    public static void ContentTypeKey(this SerializationExtensions<MsgPackSerializer> config, string contentTypeKey)
    {
        Guard.AgainstEmpty(contentTypeKey, nameof(contentTypeKey));
        var settings = config.GetSettings();
        settings.Set("NServiceBus.MessagePack.ContentTypeKey", contentTypeKey);
    }

    internal static string GetContentTypeKey(this ReadOnlySettings settings) =>
        settings.GetOrDefault<string>("NServiceBus.MessagePack.ContentTypeKey");
}