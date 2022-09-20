using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaJiCSoft.ChatOverlay
{
    // Credit to StackOverflow: https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json

    public interface IWritableOptionsInfo<T>
    {
        string Section { get; }
        string File { get; }
    }

    public class WritableOptionsInfo<T> : IWritableOptionsInfo<T>
    {
        public string Section { get; set; }
        public string File { get; set; }
    }


    public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }

    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IHostEnvironment environment;
        private readonly IOptionsMonitor<T> options;
        private readonly IWritableOptionsInfo<T> writeOptions;

        public WritableOptions(IHostEnvironment environment,
            IOptionsMonitor<T> options,
            IWritableOptionsInfo<T> writeOptions)
        {
            this.environment = environment;
            this.options = options;
            this.writeOptions = writeOptions;
        }

        public T Value => options.CurrentValue;
        public T Get(string name) => options.Get(name);

        public void Update(Action<T> applyChanges)
        {
            var fileProvider = environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(writeOptions.File);
            var physicalPath = fileInfo.PhysicalPath;

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
            var sectionObject = jObject.TryGetValue(writeOptions.Section, out JToken section) ?
                JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

            applyChanges(sectionObject);

            jObject[writeOptions.Section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }
    }
}
