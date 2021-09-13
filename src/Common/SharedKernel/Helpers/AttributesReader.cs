using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WeeControl.SharedKernel.Helpers
{
    public class AttributesReader
    {
        private readonly dynamic json;

        protected AttributesReader(string fileLocation, string fileName = "attributes.json")
        {
            json = GetSerializedAsJson(fileLocation + "." + fileName, Assembly.GetExecutingAssembly());
        }

        protected void PopulateDictionary<TEnum, TValue>(ref Dictionary<TEnum, TValue> dictionary, string name)
        {
            if (dictionary != null) return;
            
            dictionary = new Dictionary<TEnum, TValue>();
            foreach (var e in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                TValue value = json[name][e.ToString()];
                dictionary.Add(e, value);
            }
        }

        private object GetSerializedAsJson(string resource, Assembly assembly)
        {
            using var stream = assembly.GetManifestResourceStream(resource);

            if (stream == null)
                return null;

            using var streamReader = new StreamReader(stream);
            var jsonStream = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject(jsonStream);
        }
    }
}