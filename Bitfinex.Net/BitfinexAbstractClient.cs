﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects;

namespace Bitfinex.Net
{
    public abstract class BitfinexAbstractClient
    {
        protected string apiKey;
        protected HMACSHA512 encryptor;
        internal Log log;

        protected BitfinexAbstractClient()
        {
            log = new Log();

            if (BitfinexDefaults.LogWriter != null)
                SetLogOutput(BitfinexDefaults.LogWriter);

            if (BitfinexDefaults.LogVerbositySet)
                SetLogVerbosity(BitfinexDefaults.LogVerbosity);

            if (BitfinexDefaults.ApiKey != null && BitfinexDefaults.ApiSecret != null)
                SetApiCredentials(BitfinexDefaults.ApiKey, BitfinexDefaults.ApiSecret);
        }

        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetApiKey(apiKey);
            SetApiSecret(apiSecret);
        }

        /// <summary>
        /// Sets the API Key. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiKey">The api key</param>
        public void SetApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Api key empty");

            this.apiKey = apiKey;
        }

        /// <summary>
        /// Sets the API Secret. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiSecret(string apiSecret)
        {
            if (string.IsNullOrEmpty(apiSecret))
                throw new ArgumentException("Api secret empty");

            encryptor = new HMACSHA512(Encoding.UTF8.GetBytes(apiSecret));
        }

        /// <summary>
        /// Sets the verbosity of the log messages
        /// </summary>
        /// <param name="verbosity">Verbosity level</param>
        public void SetLogVerbosity(LogVerbosity verbosity)
        {
            log.Level = verbosity;
        }

        /// <summary>
        /// Sets the log output
        /// </summary>
        /// <param name="writer">The output writer</param>
        public void SetLogOutput(TextWriter writer)
        {
            log.TextWriter = writer;
        }

        protected BitfinexApiResult<T> ThrowErrorMessage<T>(string message)
        {
            log.Write(LogVerbosity.Warning, $"Call failed: {message}");
            var result = (BitfinexApiResult<T>)Activator.CreateInstance(typeof(BitfinexApiResult<T>));
            result.Message = message;
            return result;
        }

        protected BitfinexApiResult<T> ReturnResult<T>(T data)
        {
            var result = (BitfinexApiResult<T>)Activator.CreateInstance(typeof(BitfinexApiResult<T>));
            result.Result = data;
            result.Success = true;
            return result;
        }
    }
}