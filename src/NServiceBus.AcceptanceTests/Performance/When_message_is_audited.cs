﻿namespace NServiceBus.AcceptanceTests.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NUnit.Framework;

    public class When_message_is_audited : NServiceBusAcceptanceTest
    {

        [Test]
        public async Task Should_contain_processing_stats_headers()
        {
            var context = await Scenario.Define<Context>()
            .WithEndpoint<EndpointWithAuditOn>(b => b.Given(bus =>
            {
                bus.SendLocal(new MessageToBeAudited());
                return Task.FromResult(0);
            }))
            .WithEndpoint<EndpointThatHandlesAuditMessages>()
            .Done(c => c.IsMessageHandledByTheAuditEndpoint)
            .Run();

            Assert.IsTrue(context.Headers.ContainsKey(Headers.ProcessingStarted));
            Assert.IsTrue(context.Headers.ContainsKey(Headers.ProcessingEnded));
            Assert.IsTrue(context.IsMessageHandledByTheAuditEndpoint);
        }

        public class Context : ScenarioContext
        {
            public bool IsMessageHandledByTheAuditEndpoint { get; set; }
            public IDictionary<string, string> Headers { get; set; }
        }

        public class EndpointWithAuditOn : EndpointConfigurationBuilder
        {

            public EndpointWithAuditOn()
            {
                EndpointSetup<DefaultServer>()
                    .AuditTo<EndpointThatHandlesAuditMessages>();
            }

            class MessageToBeAuditedHandler : IHandleMessages<MessageToBeAudited>
            {
                Context context;

                public MessageToBeAuditedHandler(Context context)
                {
                    this.context = context;
                }

                public void Handle(MessageToBeAudited message)
                {
                }
            }
        }

        public class EndpointThatHandlesAuditMessages : EndpointConfigurationBuilder
        {

            public EndpointThatHandlesAuditMessages()
            {
                EndpointSetup<DefaultServer>();
            }

            class AuditMessageHandler : IHandleMessages<MessageToBeAudited>
            {
                Context context;
                IBus bus;

                public AuditMessageHandler(Context context, IBus bus)
                {
                    this.context = context;
                    this.bus = bus;
                }

                public void Handle(MessageToBeAudited message)
                {
                    context.Headers = bus.CurrentMessageContext.Headers;
                    context.IsMessageHandledByTheAuditEndpoint = true;
                }
            }
        }

        [Serializable]
        public class MessageToBeAudited : IMessage
        {
        }
    }
}
