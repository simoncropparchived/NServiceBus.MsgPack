using System.Collections.Concurrent;
using System.Runtime.Serialization;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceBus.Serialization;

class MessageSerializer :
    IMessageSerializer
{
    SerializationContext context;
    ConcurrentDictionary<RuntimeTypeHandle, Func<object>> emptyTypesBag = new();

    public MessageSerializer(string? contentType, SerializationContext? context)
    {
        if (context == null)
        {
            this.context = SerializationContext.Default;
        }
        else
        {
            this.context = context;
        }
        if (contentType == null)
        {
            ContentType = "messagepack";
        }
        else
        {
            ContentType = contentType;
        }
    }

    public void Serialize(object message, Stream stream)
    {
        var messageType = message.GetType();
        if (messageType.Name.EndsWith("__impl"))
        {
            throw new("Interface based message are not supported. Create a class that implements the desired interface.");
        }

        var handle = messageType.TypeHandle;
        if (emptyTypesBag.ContainsKey(handle))
        {
            return;
        }
        MessagePackSerializer serializer;
        try
        {
            serializer = context.GetSerializer(messageType);
        }
        catch (SerializationException exception)
            when (IsEmptyTypeException(exception))
        {
            stream.WriteByte(0);
            emptyTypesBag[handle] = ConstructorDelegateBuilder.BuildConstructorFunc(messageType);
            return;
        }
        serializer.Pack(stream, message);
    }

    static bool IsEmptyTypeException(SerializationException exception)
    {
        return exception.Message.Contains("because it does not have any serializable fields nor properties.");
    }

    public object[] Deserialize(Stream stream, IList<Type> messageTypes)
    {
        return new[]
        {
            DeserializeInner(stream, messageTypes)
        };
    }

    object DeserializeInner(Stream stream, IList<Type> messageTypes)
    {
        var messageType = messageTypes.First();

        var typeHandle = messageType.TypeHandle;
        if (emptyTypesBag.TryGetValue(typeHandle, out var constructor))
        {
            return constructor();
        }
        MessagePackSerializer serializer;
        try
        {
            serializer = context.GetSerializer(messageType);
        }
        catch (SerializationException exception)
            when (IsEmptyTypeException(exception))
        {
            constructor = emptyTypesBag.GetOrAdd(
                key: typeHandle,
                valueFactory: _ => ConstructorDelegateBuilder.BuildConstructorFunc(messageType));
            return constructor();
        }

        return serializer.Unpack(stream);
    }

    public string ContentType { get; }
}