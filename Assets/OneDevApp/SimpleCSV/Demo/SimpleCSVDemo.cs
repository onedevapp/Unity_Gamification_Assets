using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCSVDemo : MonoBehaviour
{
    public InputField rollNoInputField;// Reference of rollno input field
    public InputField nameInputField; // Reference of name input filed
    public Text contentArea; // Reference of contentArea where records are displayed

    string fileName = "files/SampleCSV.csv";

    // Start is called before the first frame update
    void Start()
    {
        readData();
    }

    // Read data from CSV file
    private void readData()
    {
        List<Dictionary<string, string>> csvData = new List<Dictionary<string, string>>();

        if (SimpleCSV.Instance.readData(fileName, csvData, ","))
        {
            contentArea.text ="RollNo" + "\t" + "Name"+ "\t" + "Rank";
            contentArea.text += '\n';

            foreach (Dictionary<string, string> rows in csvData)
            {               
                contentArea.text += rows["RollNo"] + "\t" + rows["Name"]+ "\t" + rows["Rank"];
                contentArea.text += '\n';
            }
        }
    }

    public void addData()
    {
        List<Dictionary<string, string>> csvData = new List<Dictionary<string, string>>();

        Dictionary<string, string> rows = new Dictionary<string, string>();
        rows.Add("RollNo", rollNoInputField.text);
        rows.Add("Name", nameInputField.text);

        csvData.Add(rows);

        SimpleCSV.Instance.saveData(fileName, csvData, ",", true);

        rollNoInputField.text = "";
        nameInputField.text = "";
        contentArea.text = "";
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        readData();
    }
}
