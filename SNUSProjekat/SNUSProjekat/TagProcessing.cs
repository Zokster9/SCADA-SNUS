using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SNUSProjekat
{
    public static class TagProcessing
    {
        private static readonly string FILE = "scadaConfig.xml";
        public static void SaveTags(Dictionary<string, Tag> tags) 
        {
            using (var writer = new StreamWriter(FILE))
            {
                var serializer = new XmlSerializer(typeof(List<Tag>));
                serializer.Serialize(writer, tags.Values.ToList());
                Console.WriteLine("TAGS SAVED");
            }
        }

        public static void LoadTags(Dictionary<string, Tag> tags)
        {
            if (!File.Exists(FILE))
            {
                Console.WriteLine("FILE DOES NOT EXIST");
            }
            else
            {
                using (var reader = new StreamReader(FILE))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Tag>));
                    var tagsList = (List<Tag>)serializer.Deserialize(reader);

                    //if ((bool)(tags?.Any()))
                    //{
                    //    foreach (Tag tag in tagsList)
                    //    {
                    //        if (tag is DigitalInput)
                    //        {
                    //            //simulationDriver = ((InTag)tag).Driver;
                    //            break;
                    //        }
                    //    }
                    //}

                    if (tagsList != null)
                    {
                        tags = tagsList.ToDictionary(tag => tag.TagName);
                    }
                }
            }
        }
    }
}