using System;
using System.IO;
using log4net;
using Newtonsoft.Json.Linq;

namespace CKAN.KerbalStuff
{
    public class KSMod
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (KSMod));
        public int id; // KSID

        // These get filled in from JSON deserialisation.
        public string license;
        public string name;
        public string short_description;
        public string author;
        public KSVersion[] versions;
        public string website;

        public override string ToString()
        {
            return string.Format("{0}", name);
        }

        /// <summary>
        ///     Takes a JObject and inflates it with KS metadata.
        ///     This will not overwrite fields that already exist.
        /// </summary>
        public void InflateMetadata(JObject metadata, KSVersion version, string filename)
        {

            // Check how big our file is
            long download_size = (new FileInfo (filename)).Length;

            // Make sure resources exist.
            if (metadata["resources"] == null)
            {
                metadata["resources"] = new JObject();
            }

            if (metadata["resources"]["kerbalstuff"] == null)
            {
                metadata["resources"]["kerbalstuff"] = new JObject();
            }

            Inflate(metadata, "spec_version", "1"); // CKAN spec version
            Inflate(metadata, "name", name);
            Inflate(metadata, "license", license);
            Inflate(metadata, "abstract", short_description);
            Inflate(metadata, "author", author);
            Inflate(metadata, "version", version.friendly_version.ToString());
            Inflate(metadata, "download", Uri.EscapeUriString(version.download_path));
            Inflate(metadata, "comment", "Generated by ks2ckan");
            Inflate(metadata, "download_size", download_size);
            Inflate((JObject) metadata["resources"], "homepage", website);
            Inflate((JObject) metadata["resources"]["kerbalstuff"], "url", KSHome());
            Inflate(metadata, "ksp_version", version.KSP_version.ToString());
        }

        internal string KSHome()
        {
            Uri path = KSAPI.ExpandPath(String.Format("/mod/{0}/{1}", id, name));
            return Uri.EscapeUriString(path.ToString());
        }

        internal static void Inflate(JObject metadata, string key, string value)
        {
            if (metadata[key] == null)
            {
                log.DebugFormat("Setting {0} to {1}", key, value);
                metadata[key] = value;
            }
            else
            {
                log.DebugFormat("Leaving {0} as {1}", key, metadata[key]);
            }
        }

        internal static void Inflate(JObject metadata, string key, long value)
        {
            if (metadata[key] == null)
            {
                metadata[key] = value;
            }
        }

    }
}