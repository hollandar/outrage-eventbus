using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus.Options
{
    public class EventBusOptions
    {
        bool defaultExceptionSubscriber = false;
        bool defaultLoggingSubscriber = false;
        bool exceptionPublisher = false;
        bool loggingPublisher = false;
        bool registerDefaultRoot = false;
        bool registerDefaultClient = false;
        string defaultExceptionMessage = "Exception thrown processing event chain.";

        public bool DefaultExceptionSubscriber => defaultExceptionSubscriber;
        public bool DefaultLoggingSubscriber => defaultLoggingSubscriber;
        public bool ExceptionPublisher => exceptionPublisher;
        public bool LoggingPublisher => loggingPublisher;
        public bool RegisterDefaultRoot => registerDefaultRoot;
        public bool RegisterDefaultClient => registerDefaultClient;
        public string DefaultExceptionMessage => defaultExceptionMessage;

        public EventBusOptions UseDefaultExceptionSubscriber(bool use = true)
        {
            this.defaultExceptionSubscriber = use;
            return this;
        }

        public EventBusOptions UseDefaultLoggingSubscriber(bool use = true)
        {
            this.defaultLoggingSubscriber = use;
            return this;
        }

        public EventBusOptions AddExceptionPublisher(bool use = true)
        {
            this.exceptionPublisher = use;
            return this;
        }

        public EventBusOptions AddLoggingPublisher(bool use = true)
        {
            this.loggingPublisher = use;
            return this;
        }

        public EventBusOptions AddDefaultRootBus(bool use = true)
        {
            this.registerDefaultRoot = use;
            return this;
        }

        public EventBusOptions AddDefaultClientBus(bool use = true)
        {
            this.registerDefaultClient = use;
            return this;
        }

        public EventBusOptions SetDefaultExceptionMessage(string message)
        {
            this.defaultExceptionMessage = message;
            return this;
        }

    }
}
