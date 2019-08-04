﻿using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Zengo.Core;

namespace Zengo.Json
{
    public class JsonConfig : Config
    {
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
            = new JsonSerializerSettings
            {
                //CheckAdditionalContent = true,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                FloatFormatHandling = FloatFormatHandling.String,
                Formatting = Formatting.Indented,
                MissingMemberHandling = MissingMemberHandling.Error,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                StringEscapeHandling = StringEscapeHandling.Default,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
            };
    }
}