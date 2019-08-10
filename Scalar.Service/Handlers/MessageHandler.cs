﻿using Scalar.Common.NamedPipes;
using Scalar.Common.Tracing;

namespace Scalar.Service.Handlers
{
    public abstract class MessageHandler
    {
        protected void WriteToClient(NamedPipeMessages.Message message, NamedPipeServer.Connection connection, ITracer tracer)
        {
            if (!connection.TrySendResponse(message))
            {
                tracer.RelatedError("Failed to send line to client: {0}", message);
            }
        }
    }
}
