// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters.Json;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcJsonMvcCoreBuilderExtensions
    {
        public static IMvcCoreBuilder AddJsonFormatters(this IMvcCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            AddJsonFormatterServices(builder.Services);
            return builder;
        }

        public static IMvcCoreBuilder AddJsonFormatters(
            this IMvcCoreBuilder builder,
            Action<JsonSerializerSettings> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            AddJsonFormatterServices(builder.Services);

            builder.Services.Configure<MvcJsonOptions>((options) => setupAction(options.SerializerSettings));

            return builder;
        }

        /// <summary>
        /// Adds configuration of <see cref="MvcJsonOptions"/> for the application.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="setupAction">The <see cref="MvcJsonOptions"/> which need to be configured.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder AddJsonOptions(
           this IMvcCoreBuilder builder,
           Action<MvcJsonOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            builder.Services.Configure(setupAction);
            return builder;
        }

        // Internal for testing.
        internal static void AddJsonFormatterServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcJsonMvcOptionsSetup>());
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IPostConfigureOptions<MvcJsonOptions>, MvcJsonOptionsConfigureCompatibilityOptions>());
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApiDescriptionProvider, JsonPatchOperationsArrayProvider>());
            services.TryAddSingleton<JsonResultExecutor>();
        }

        /// <summary>
        /// Configure json responses to use camel casing. 
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="processDictionaryKeys">A flag indicating whether dictionary keys should be converted to camel casing.</param>
        /// <param name="overrideSpecifiedNames">A flag indiciating whether explicitly provided json property names, using <see cref="JsonProperty"/> should be converted to camel casing.</param>
        /// <param name="processExtensionDataNames">A flag indiciating whether json extention data properties should be converted to camel casing.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder WithJsonCamelCasing(
            this IMvcCoreBuilder builder,
            bool processDictionaryKeys = false,
            bool overrideSpecifiedNames = false,
            bool processExtensionDataNames = false)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure((MvcJsonOptions options) =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                    {
                        OverrideSpecifiedNames = overrideSpecifiedNames,
                        ProcessDictionaryKeys = processDictionaryKeys,
                        ProcessExtensionDataNames = processExtensionDataNames
                    }
                };
            });

            return builder;
        }

        /// <summary>
        /// Configure json responses to use pascal casing for all properties, including dictionary keys and properties with JsonProperty attribute
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="processDictionaryKeys">A flag indicating whether dictionary keys should be converted to pascal casing.</param>
        /// <param name="overrideSpecifiedNames">A flag indiciating whether explicitly provided json property names, using <see cref="JsonProperty"/> should be converted to pascal casing.</param>
        /// <param name="processExtensionDataNames">A flag indiciating whether json extention data properties should be converted to pascal casing.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder WithJsonPascalCasing(
            this IMvcCoreBuilder builder,
            bool processDictionaryKeys = false,
            bool overrideSpecifiedNames = false,
            bool processExtensionDataNames = false)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure((MvcJsonOptions options) =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new DefaultNamingStrategy()
                    {
                        OverrideSpecifiedNames = overrideSpecifiedNames,
                        ProcessDictionaryKeys = processDictionaryKeys,
                        ProcessExtensionDataNames = processExtensionDataNames
                    }
                };
            });

            return builder;
        }
    }
}
