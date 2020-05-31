
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OneDevApp
{
    public class SimpleCSV : MonoInstance<SimpleCSV>
    {
        private char lineSeperater = '\r'; // It defines line seperate character

        // Read data from CSV file
        public bool readData(string csvFilePath, List<Dictionary<string, string>> csvDicValues, string delimate)
        {
            // Verify required argument
            if (csvDicValues == null || string.IsNullOrEmpty(delimate))
                return false;

            csvFilePath = getFilePath(csvFilePath);

            if (!File.Exists(csvFilePath)) return false;

            try
            {
                string data = File.ReadAllText(csvFilePath, Encoding.UTF8);

                string[] records = data.Split(lineSeperater);

                if (records.Length <= 0) return false;

                string[] header = records[0].Split(delimate[0]);

                for (var i = 1; i < records.Length; i++)
                {
                    //This is to get every thing that is comma separated
                    string[] fields = records[i].Split(delimate[0]);

                    Dictionary<string, string> rowData = new Dictionary<string, string>();

                    for (var j = 0; j < header.Length; j++)
                    {
                        rowData.Add(header[j], fields[j]);
                    }

                    csvDicValues.Add(rowData);
                }

                Debug.Log("csvDicValues::" + csvDicValues.Count);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return false;
            }
        }


        // Add data to CSV file
        public bool saveData(string csvFilePath, List<Dictionary<string, string>> csvDicValues, string delimate, bool toAppend = false)
        {
            // Verify required argument
            if (csvDicValues == null || string.IsNullOrEmpty(delimate))
                return false;

            csvFilePath = getFilePath(csvFilePath);

            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                if (toAppend)
                {

                    if (!File.Exists(csvFilePath))
                    {
                        Debug.Log("File not exists to append");
                        return false;
                    }

                    string data = File.ReadAllText(csvFilePath, Encoding.UTF8);

                    string[] records = data.Split(lineSeperater);

                    if (records.Length <= 0)
                    {
                        Debug.Log("File has no data to wrtie");
                        return false;
                    }

                    string[] header = records[0].Split(delimate[0]);

                    for (var i = 0; i < csvDicValues.Count; i++)
                    {
                        stringBuilder.Append(lineSeperater);

                        for (var j = 0; j < header.Length; j++)
                        {
                            stringBuilder.Append(csvDicValues[i][header[j]]);
                            stringBuilder.Append(delimate[0]);
                        }

                        stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    }

                    // Following line adds data to CSV file
                    File.AppendAllText(csvFilePath, stringBuilder.ToString());

                    return true;
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(csvFilePath));

                    List<string> header = new List<string>(csvDicValues[0].Keys);

                    for (var i = 0; i < csvDicValues.Count; i++)
                    {
                        stringBuilder.Append(lineSeperater);

                        for (var j = 0; j < header.Count; j++)
                        {
                            stringBuilder.Append(csvDicValues[i][header[j]]);
                            stringBuilder.Append(delimate[0]);
                        }

                        stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    }

                    // Following line adds data to CSV file
                    File.WriteAllText(csvFilePath, stringBuilder.ToString());

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return false;
            }
        }

        // Get path for given CSV file
        private string getFilePath(string path)
        {
#if UNITY_EDITOR
            return Application.dataPath + "/SimpleCSV/Demo/" + path;
#elif UNITY_ANDROID
        return Application.persistentDataPath+path;
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+path;
#else
        return Application.dataPath +"/"+path;
#endif
        }
    }
}