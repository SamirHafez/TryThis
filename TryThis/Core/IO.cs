using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace TryThis.Core
{
    public class IO
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public DirectoryInfo SaveDirectory { get; private set; }
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        public IO(string directory)
        {
            SaveDirectory = new DirectoryInfo(directory);
        }

        public string Save(string code, string result)
        {
            if (!SaveDirectory.Exists)
                SaveDirectory.Create();

            var entry = new string(Enumerable.Repeat(chars, 10)
                                             .Select(c => c[Random.Next(c.Length)])
                                             .ToArray());

            var filePath = Path.Combine(SaveDirectory.FullName, string.Format("{0}.json", entry));

            var obj = new { code = code, result = result };

            var json = JsonConvert.SerializeObject(obj, Formatting.None);

            File.WriteAllText(filePath, json);

            return entry;
        }

        public bool Get(string id, out string code, out string result)
        {
            var file = SaveDirectory.GetFiles(string.Format("{0}.json", id)).FirstOrDefault();

            if (file == null)
            {
                code = result = null;
                return false;
            }

            var json = JsonConvert.DeserializeAnonymousType(File.ReadAllText(file.FullName), new { code = string.Empty, result = string.Empty });

            code = json.code;
            result = json.result;

            return true;
        }
    }
}