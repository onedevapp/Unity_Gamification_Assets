using UnityEngine;

namespace OneDevApp
{
    public class EncryptedPlayerPrefs : MonoInstance<EncryptedPlayerPrefs>
    {

        #region Public helper functions

        /// <summary>
        /// To Delete a key from the Player prefs
        /// </summary>
        /// <param name="key">player prefs key</param>
        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key.GetEncrypted());
        }

        /// <summary>
        /// To Delete all key from the Player prefs
        /// </summary>
        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// Is key available in the Player prefs
        /// </summary>
        /// <param name="key">player prefs key</param>
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key.GetEncrypted());
        }

        /// <summary>
        /// To Save Player prefs
        /// </summary>
        public void Save()
        {
            PlayerPrefs.Save();
        }


        #region Get Data from Player Prefs

        /// <summary>
        /// Helper function to get a value from the Player prefs 
        /// </summary>
        /// <param name="key">player prefs key</param>
        /// <param name="defaultValue">default value if player prefs dont have a key</param>
        public string GetData(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key.GetEncrypted(), defaultValue).GetDecrypted();
        }

        /// <summary>
        /// Helper function to get a int/float value from the Player prefs with default value
        /// </summary>
        /// <param name="key">player prefs key</param>
        /// <param name="defaultValue">default value if player prefs dont have a key</param>
        public ulong GetULong(string key, ulong defaultValue)
        {
            string data = GetData(key, defaultValue.ToString());
            ulong result;
            if (ulong.TryParse(data, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// To get a string value from the Player prefs
        /// </summary>
        /// <param name="key">player prefs key</param>
        public string GetString(string key)
        {
            return GetData(key, string.Empty);
        }

        /// <summary>
        /// To get a int/float value from the Player prefs with default value
        /// </summary>
        /// <param name="key">player prefs key</param>
        public ulong GetULong(string key)
        {
            return GetULong(key, 0);
        }

        /// <summary>
        /// To get a bool value from the Player prefs with default value
        /// </summary>
        /// <param name="key">player prefs key</param>
        public bool GetBool(string key)
        {
            return GetULong(key, 0) == 0;
        }

        #endregion

        #region Set Data to Player Prefs


        /// <summary>
        /// Helper function to save a value to the Player prefs 
        /// </summary>
        /// <param name="key">player prefs key</param>
        /// <param name="defaultValue">default value if player prefs dont have a key</param>
        private void SetData(string key, string value)
        {
            PlayerPrefs.SetString(key.GetEncrypted(), value.GetEncrypted());
        }


        /// <summary>
        /// To save a string value to the Player prefs
        /// </summary>
        /// <param name="key">player prefs key</param>
        /// <param name="value">value to store</param>
        public void SetString(string key, string value)
        {
            SetData(key, value);
        }

        /// <summary>
        /// To save a int/float value to the Player prefs
        /// </summary>
        /// <param name="key">player prefs key</param>
        /// <param name="value">value to store</param>
        public void SetULong(string key, ulong value)
        {
            SetData(key, value.ToString());
        }

        /// <summary>
        /// To save a int/float value to the Player prefs
        /// </summary>
        /// <param name="key">player prefs key</param>
        /// <param name="value">value to store</param>
        public void SetBool(string key, bool value)
        {
            SetData(key, value ? "1" : "0");
        }


        #endregion

        #endregion
    }
}
