﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public abstract class ConvertingEventAggregator<TMessage, TOutMessage>: FilterChildEventAggregator<TOutMessage>, ISubscriber where TMessage: IMessage where TOutMessage: IMessage
    {
        public ConvertingEventAggregator(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override async Task HandleAsync(EventContext context, IMessage message)
        {
            if (message is TMessage)
            {
                var outMessage = this.Convert((TMessage)message);
                await this.PublishAsync<TOutMessage>(outMessage);
            }
        }

        protected abstract TOutMessage Convert(TMessage message);
    }
}
