﻿using MediaBrowser.Common.Extensions;
using MediaBrowser.Common.IO;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Serialization;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaBrowser.Api
{
    /// <summary>
    /// Class GetConfiguration
    /// </summary>
    [Route("/System/Configuration", "GET", Summary = "Gets application configuration")]
    public class GetConfiguration : IReturn<ServerConfiguration>
    {

    }

    /// <summary>
    /// Class UpdateConfiguration
    /// </summary>
    [Route("/System/Configuration", "POST", Summary = "Updates application configuration")]
    public class UpdateConfiguration : ServerConfiguration, IReturnVoid
    {
    }

    [Route("/System/Configuration/MetadataOptions/Default", "GET", Summary = "Gets a default MetadataOptions object")]
    public class GetDefaultMetadataOptions : IReturn<MetadataOptions>
    {

    }

    [Route("/System/Configuration/MetadataPlugins", "GET", Summary = "Gets all available metadata plugins")]
    public class GetMetadataPlugins : IReturn<List<MetadataPluginSummary>>
    {

    }

    [Route("/System/Configuration/VideoImageExtraction", "POST", Summary = "Updates image extraction for all types")]
    public class UpdateVideoImageExtraction : IReturnVoid
    {
        public bool Enabled { get; set; }
    }

    public class ConfigurationService : BaseApiService
    {
        /// <summary>
        /// The _json serializer
        /// </summary>
        private readonly IJsonSerializer _jsonSerializer;

        /// <summary>
        /// The _configuration manager
        /// </summary>
        private readonly IServerConfigurationManager _configurationManager;

        private readonly IFileSystem _fileSystem;
        private readonly IProviderManager _providerManager;

        public ConfigurationService(IJsonSerializer jsonSerializer, IServerConfigurationManager configurationManager, IFileSystem fileSystem, IProviderManager providerManager)
        {
            _jsonSerializer = jsonSerializer;
            _configurationManager = configurationManager;
            _fileSystem = fileSystem;
            _providerManager = providerManager;
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.Object.</returns>
        public object Get(GetConfiguration request)
        {
            var configPath = _configurationManager.ApplicationPaths.SystemConfigurationFilePath;

            var dateModified = _fileSystem.GetLastWriteTimeUtc(configPath);

            var cacheKey = (configPath + dateModified.Ticks).GetMD5();

            return ToOptimizedResultUsingCache(cacheKey, dateModified, null, () => _configurationManager.Configuration);
        }

        /// <summary>
        /// Posts the specified configuraiton.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Post(UpdateConfiguration request)
        {
            // Silly, but we need to serialize and deserialize or the XmlSerializer will write the xml with an element name of UpdateConfiguration

            var json = _jsonSerializer.SerializeToString(request);

            var config = _jsonSerializer.DeserializeFromString<ServerConfiguration>(json);

            _configurationManager.ReplaceConfiguration(config);
        }

        public object Get(GetDefaultMetadataOptions request)
        {
            return ToOptimizedSerializedResultUsingCache(new MetadataOptions());
        }

        public object Get(GetMetadataPlugins request)
        {
            return ToOptimizedSerializedResultUsingCache(_providerManager.GetAllMetadataPlugins().ToList());
        }

        /// <summary>
        /// This is a temporary method used until image settings get broken out.
        /// </summary>
        /// <param name="request"></param>
        public void Post(UpdateVideoImageExtraction request)
        {
            var config = _configurationManager.Configuration;

            EnableImageExtractionForType(typeof(Movie), config, request.Enabled);
            EnableImageExtractionForType(typeof(Episode), config, request.Enabled);
            EnableImageExtractionForType(typeof(AdultVideo), config, request.Enabled);
            EnableImageExtractionForType(typeof(MusicVideo), config, request.Enabled);
            EnableImageExtractionForType(typeof(Video), config, request.Enabled);
            EnableImageExtractionForType(typeof(Trailer), config, request.Enabled);

            _configurationManager.SaveConfiguration();
        }

        private void EnableImageExtractionForType(Type type, ServerConfiguration config, bool enabled)
        {
            var options = GetMetadataOptions(type, config);

            const string imageProviderName = "Screen Grabber";

            var contains = options.DisabledImageFetchers.Contains(imageProviderName, StringComparer.OrdinalIgnoreCase);

            if (!enabled && !contains)
            {
                var list = options.DisabledImageFetchers.ToList();

                list.Add(imageProviderName);

                options.DisabledImageFetchers = list.ToArray();
            }
            else if (enabled && contains)
            {
                var list = options.DisabledImageFetchers.ToList();

                list.Remove(imageProviderName);

                options.DisabledImageFetchers = list.ToArray();
            }
        }

        private MetadataOptions GetMetadataOptions(Type type, ServerConfiguration config)
        {
            var options = config.MetadataOptions
                .FirstOrDefault(i => string.Equals(i.ItemType, type.Name, StringComparison.OrdinalIgnoreCase));

            if (options == null)
            {
                var list = config.MetadataOptions.ToList();

                options = new MetadataOptions
                {
                    ItemType = type.Name
                };

                list.Add(options);

                config.MetadataOptions = list.ToArray();
            }

            return options;
        }
    }
}
