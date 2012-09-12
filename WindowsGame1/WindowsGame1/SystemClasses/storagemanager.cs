/* COPYRIGHTS 
 * 
 * EngineTemplate Copyrights © Marc 'XMR1' Stanglmayr 2011 
 * Contact @ http://www.xmr1.net/ or support@xmr1.net 
 * 
 * Licence: GNU/GPL3 
 */


using System;
using System.IO;
using System.Xml.Serialization;
//using EngineTemplate.Debug;

namespace WindowsGame1
{
    public abstract class storagemanager
    {
        /// <summary> 
        /// Saves a given object of a given type under a specified FileName. 
        /// </summary> 
        /// <param name="location">The path where the file should be saved (including file name!).</param> 
        /// <param name="type">The type of the object.</param> 
        /// <param name="obj">The object itself.</param> 
        internal static bool Save(String location, System.Type type, object obj)
        {
            Boolean everythingok = true;
            
            try
            {
                if (String.IsNullOrEmpty(location))
                {
                    throw new ArgumentNullException("location", "Location cannot be null or empty. Must be a path to a specific file.");
                }
                if (!location.Contains("\\"))
                {
                    throw new InvalidDataException("Location must be a path, but it isn't!");
                }
                if (obj == null)
                {
                    throw new NullReferenceException("File to be saved is null. Saving not useful.");
                }
                if (type == null)
                {
                    throw new NullReferenceException("Type of file to be saved is null. Saving not useful.");
                }
                String path = location.Substring(0, location.LastIndexOf('\\'));
                CreateDirectoriesAsRequired(path);
                using (StreamWriter sw = new StreamWriter(location))
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    serializer.Serialize(sw.BaseStream, obj);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Unable to save object of type '{0}' to location '{1}'. Reason:" + Environment.NewLine + ex.ToString(), type.ToString(), location));
                everythingok = false;
            }

            return everythingok;
        }

        /// <summary> 
        /// Saves the current object at the given location. 
        /// </summary> 
        /// <param name="location">The path where the file should be saved (including file name!).</param> 
        internal bool Save(String location)
        {
            return Save(location, this.GetType(), this);
        }

        /// <summary> 
        /// Creates directories if they don't exist yet based on the given path. 
        /// </summary> 
        /// <param name="path">A valid folder path.</param> 
        private static void CreateDirectoriesAsRequired(String path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        /// <summary> 
        /// Loads the current object from the given location. 
        /// </summary> 
        /// <param name="location">The path where the file is stored (including file name!).</param> 
        /// <returns></returns> 
        internal object Load(String location)
        {
            return Load(location, this.GetType());
        }

        /// <summary> 
        /// Loads a given object based on the given location. 
        /// </summary> 
        /// <param name="location">The path where the file is stored (including file name!).</param> 
        /// <param name="type">The type of the object to be loaded.</param> 
        /// <returns></returns> 
        internal static object Load(String location, System.Type type)
        {
            try
            {
                if (String.IsNullOrEmpty(location))
                {
                    throw new ArgumentNullException("location", "Location cannot be null or empty. Must be a path to a specific file.");
                }
                if (!location.Contains("\\"))
                {
                    throw new InvalidDataException("Location must be a path, but it isn't!");
                }
                if (type == null)
                {
                    throw new NullReferenceException("Type of file to be saved is null. Saving not useful.");
                }
                if (!File.Exists(location))
                {
                    Console.WriteLine("Failed loading file '" + location + "'. The file does not exist!");
                    return null;
                }
                using (StreamReader sr = new StreamReader(location))
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    return serializer.Deserialize(sr.BaseStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Unable to load object of type '{0}' from location '{1}'. Reason:" + Environment.NewLine + ex.ToString(), type.ToString(), location));
            }
            return null;
        }
    }
}