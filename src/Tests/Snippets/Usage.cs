using MsgPack.Serialization;
using NServiceBus;
using MsgPackSerializer = NServiceBus.MsgPack.MsgPackSerializer;

class Usage
{
    Usage(EndpointConfiguration configuration)
    {
        #region MsgPackSerialization

        configuration.UseSerialization<MsgPackSerializer>();

        #endregion
    }

    void CustomSettings(EndpointConfiguration configuration)
    {
        #region MsgPackCustomSettings

        var context = new SerializationContext
        {
            DefaultDateTimeConversionMethod = DateTimeConversionMethod.UnixEpoc
        };
        var serialization = configuration.UseSerialization<MsgPackSerializer>();
        serialization.Context(context);

        #endregion
    }

    void ContentTypeKey(EndpointConfiguration configuration)
    {
        #region MsgPackContentTypeKey

        var serialization = configuration.UseSerialization<MsgPackSerializer>();
        serialization.ContentTypeKey("custom-key");

        #endregion
    }
}