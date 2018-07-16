using System;
using Microsoft.Extensions.Configuration;

namespace Identity.Server.Configuration {
    public static class ConfigurationBuilderExtensions {
        //-----------------------------------

        /// <summary>
        ///     Adds an <see cref="IConfigurationProvider" /> that reads configuration values from docker secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
        /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder) {
            return builder.AddDockerSecrets(configureSource: null);
        }

        /// <summary>
        ///     Adds an <see cref="IConfigurationProvider" /> that reads configuration values from docker secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
        /// <param name="secretsPath">The path to the secrets directory.</param>
        /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, string secretsPath) {
            return builder.AddDockerSecrets(source => source.SecretsDirectory = secretsPath);
        }

        /// <summary>
        ///     Adds an <see cref="IConfigurationProvider" /> that reads configuration values from docker secrets.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
        /// <param name="secretsPath">The path to the secrets directory.</param>
        /// <param name="optional">Whether the directory is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, string secretsPath,
            bool optional) {
            return builder.AddDockerSecrets(source => {
                source.SecretsDirectory = secretsPath;
                source.Optional = optional;
            });
        }

        /// <summary>
        ///     Adds a docker secrets configuration source to <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder,
            Action<DockerSecretsConfigurationSource> configureSource) {
            return builder.Add(configureSource);
        }


        /// <summary>
        ///     Adds a new configuration source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
        /// <param name="configureSource">Configures the source secrets.</param>
        /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder Add<TSource>(this IConfigurationBuilder builder,
            Action<TSource> configureSource) where TSource : IConfigurationSource, new() {
            var source = new TSource();
            configureSource?.Invoke(source);
            return builder.Add(source);
        }
    }
}