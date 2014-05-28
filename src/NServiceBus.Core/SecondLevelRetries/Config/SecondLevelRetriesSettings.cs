namespace NServiceBus.SecondLevelRetries.Config
{
    using System;

    /// <summary>
    /// Configuration settings for second level retries
    /// </summary>
    public class SecondLevelRetriesSettings
    {
        readonly Configure config;

        public SecondLevelRetriesSettings(Configure config)
        {
            this.config = config;
        }

        /// <summary>
        /// Register a custom retry policy
        /// </summary>
        /// <param name="customPolicy"></param>
        public void CustomRetryPolicy(Func<TransportMessage, TimeSpan> customPolicy)
        {
            config.Settings.Set("SecondLevelRetries.RetryPolicy", customPolicy);
        }
    }
}