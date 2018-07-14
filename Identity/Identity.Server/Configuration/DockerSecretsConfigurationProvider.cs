﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Identity.Server.Configuration
{
    public class DockerSecretsConfigurationProvider : ConfigurationProvider
    {
        DockerSecretsConfigurationSource Source { get; set; }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="source">The settings.</param>
        public DockerSecretsConfigurationProvider(DockerSecretsConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        private static string NormalizeKey(string key)
        {
            return key.Replace("__", ConfigurationPath.KeyDelimiter);
        }

        private static string TrimNewLine(string value)
        {
            return value.EndsWith(Environment.NewLine)
                ? value.Substring(0, value.Length - 1)
                : value;
        }

        /// <summary>
        /// Loads the docker secrets.
        /// </summary>
        public override void Load()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (Source.FileProvider == null)
            {
                if (Directory.Exists(Source.SecretsDirectory))
                {
                    Source.FileProvider = new PhysicalFileProvider(Source.SecretsDirectory);
                }
                else if (Source.Optional)
                {
                    return;
                }
                else
                {
                    throw new DirectoryNotFoundException("DockerSecrets directory doesn't exist and is not optional.");
                }
            }

            var secretsDir = Source.FileProvider.GetDirectoryContents("/");
            if (!secretsDir.Exists && !Source.Optional)
            {
                throw new DirectoryNotFoundException("DockerSecrets directory doesn't exist and is not optional.");
            }

            foreach (var file in secretsDir)
            {
                if (file.IsDirectory)
                {
                    continue;
                }

                using (var stream = file.CreateReadStream())
                using (var streamReader = new StreamReader(stream))
                {
                    if (Source.IgnoreCondition == null || !Source.IgnoreCondition(file.Name))
                    {
                        Data.Add(NormalizeKey(file.Name), TrimNewLine(streamReader.ReadToEnd()));
                    }
                }
            }
        }
    }
}