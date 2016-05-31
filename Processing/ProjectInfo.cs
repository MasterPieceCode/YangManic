using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace Processing
{
    [Serializable]
    public class ProjectInfo : ISerializable
    {
        private static readonly BinaryFormatter _binaryformatter;
        public string FileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool KeepRatio { get; set; }
        public ObservableCollection<PaletteTile> SelectedTiles { get; set; }
        public ObservableCollection<PaletteTile> PaletteTiles { get; set; }

        static ProjectInfo()
        {
            _binaryformatter = new BinaryFormatter();
        }

        public ProjectInfo()
        {
        }

        public ProjectInfo(SerializationInfo info, StreamingContext context)
        {
            FileName = info.GetString("FileName");
            Width = info.GetInt32("Width");
            Height = info.GetInt32("Height");
            KeepRatio = info.GetBoolean("KeepRatio");
            SelectedTiles = (ObservableCollection<PaletteTile>)info.GetValue("SelectedTiles", typeof (ObservableCollection<PaletteTile>));
            PaletteTiles = (ObservableCollection<PaletteTile>)info.GetValue("PaletteTiles", typeof(ObservableCollection<PaletteTile>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FileName", FileName);
            info.AddValue("SelectedTiles", SelectedTiles);
            info.AddValue("PaletteTiles", PaletteTiles);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("KeepRatio", KeepRatio);
        }

        public static ProjectInfo Load(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    return (ProjectInfo)_binaryformatter.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    throw;
                }
            }
        }

        public void Save(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                try
                {
                    _binaryformatter.Serialize(fs, this);
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    throw;
                }
            }
        }

        private static void LogException(Exception ex)
        {
            File.AppendAllText("log.txt", string.Format("{0}: {1}\r\n{2}\r\n", DateTime.Now, ex.Message, ex.StackTrace));
        }
    }
}