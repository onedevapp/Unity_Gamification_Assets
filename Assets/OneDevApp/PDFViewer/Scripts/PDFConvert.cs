using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OneDevApp
{
    class PDFConvert
    {
        #region GhostScript Import
        /// <summary>Create a new instance of Ghostscript. This instance is passed to most other gsapi functions. The caller_handle will be provided to callback functions.
        ///  At this stage, Ghostscript supports only one instance. </summary>
        /// <param name="pinstance"></param>
        /// <param name="caller_handle"></param>
        /// <returns></returns>
        [DllImport("gsdll64", EntryPoint = "gsapi_new_instance")]
        private static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);
        /// <summary>This is the important function that will perform the conversion</summary>
        /// <param name="instance"></param>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        [DllImport("gsdll64", EntryPoint = "gsapi_init_with_args")]
        private static extern int gsapi_init_with_args(IntPtr instance, int argc, IntPtr argv);
        /// <summary>
        /// Exit the interpreter. This must be called on shutdown if gsapi_init_with_args() has been called, and just before gsapi_delete_instance().
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [DllImport("gsdll64", EntryPoint = "gsapi_exit")]
        private static extern int gsapi_exit(IntPtr instance);
        /// <summary>
        /// Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you must call gsapi_exit before gsapi_delete_instance.
        /// </summary>
        /// <param name="instance"></param>
        [DllImport("gsdll64", EntryPoint = "gsapi_delete_instance")]
        private static extern void gsapi_delete_instance(IntPtr instance);
        #endregion
        #region Variables
        private string _sDeviceFormat;
        private int _iWidth;
        private int _iHeight;
        private int _iResolutionX;
        private int _iResolutionY;
        private int _iJPEGQuality;
        private Boolean _bFitPage;
        private IntPtr _objHandle;

        /***
         * Variable for object list in Unity
         */

        #endregion
        #region Proprieties
        public string OutputFormat
        {
            get { return _sDeviceFormat; }
            set { _sDeviceFormat = value; }
        }
        public int Width
        {
            get { return _iWidth; }
            set { _iWidth = value; }
        }
        public int Height
        {
            get { return _iHeight; }
            set { _iHeight = value; }
        }
        public int ResolutionX
        {
            get { return _iResolutionX; }
            set { _iResolutionX = value; }
        }
        public int ResolutionY
        {
            get { return _iResolutionY; }
            set { _iResolutionY = value; }
        }
        public Boolean FitPage
        {
            get { return _bFitPage; }
            set { _bFitPage = value; }
        }
        /// <summary>Quality of compression of JPG</summary>
        public int JPEGQuality
        {
            get { return _iJPEGQuality; }
            set { _iJPEGQuality = value; }
        }
        #endregion
        #region Init
        public PDFConvert(IntPtr objHandle)
        {
            _objHandle = objHandle;
        }
        public PDFConvert()
        {
            _objHandle = IntPtr.Zero;
        }
        #endregion
        private byte[] StringToAnsiZ(string str)
        {
            //' Convert a Unicode string to a null terminated Ansi string for Ghostscript.
            //' The result is stored in a byte array. Later you will need to convert
            //' this byte array to a pointer with GCHandle.Alloc(XXXX, GCHandleType.Pinned)
            //' and GSHandle.AddrOfPinnedObject()
            int intElementCount;
            int intCounter;
            byte[] aAnsi;
            byte bChar;
            intElementCount = str.Length;
            aAnsi = new byte[intElementCount + 1];
            for (intCounter = 0; intCounter < intElementCount; intCounter++)
            {
                bChar = (byte)str[intCounter];
                aAnsi[intCounter] = bChar;
            }
            aAnsi[intElementCount] = 0;
            return aAnsi;
        }

        /// <summary>Convert the file!</summary>
        public bool Convert(string inputFile, string outputFile,
            int firstPage, int lastPage, string deviceFormat, int width, int height)
        {
            //Avoid to work when the file doesn't exist
            if (!System.IO.File.Exists(inputFile))
            {
                Debug.Log("File doesn't exists");
                return false;
            }
            int intReturn;
            IntPtr intGSInstanceHandle;
            object[] aAnsiArgs;
            IntPtr[] aPtrArgs;
            GCHandle[] aGCHandle;
            int intCounter;
            int intElementCount;
            IntPtr callerHandle;
            GCHandle gchandleArgs;
            IntPtr intptrArgs;
            string[] sArgs = GetGeneratedArgs(inputFile, outputFile,
                firstPage, lastPage, deviceFormat, width, height);
            // Convert the Unicode strings to null terminated ANSI byte arrays
            // then get pointers to the byte arrays.
            intElementCount = sArgs.Length;
            aAnsiArgs = new object[intElementCount];
            aPtrArgs = new IntPtr[intElementCount];
            aGCHandle = new GCHandle[intElementCount];
            // Create a handle for each of the arguments after
            // they've been converted to an ANSI null terminated
            // string. Then store the pointers for each of the handles
            for (intCounter = 0; intCounter < intElementCount; intCounter++)
            {
                aAnsiArgs[intCounter] = StringToAnsiZ(sArgs[intCounter]);
                aGCHandle[intCounter] = GCHandle.Alloc(aAnsiArgs[intCounter], GCHandleType.Pinned);
                aPtrArgs[intCounter] = aGCHandle[intCounter].AddrOfPinnedObject();
            }
            // Get a new handle for the array of argument pointers
            gchandleArgs = GCHandle.Alloc(aPtrArgs, GCHandleType.Pinned);
            intptrArgs = gchandleArgs.AddrOfPinnedObject();
            intReturn = gsapi_new_instance(out intGSInstanceHandle, _objHandle);
            callerHandle = IntPtr.Zero;
            try
            {
                intReturn = gsapi_init_with_args(intGSInstanceHandle, intElementCount, intptrArgs);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                Debug.Log(ex.ToString());

            }
            finally
            {
                for (intCounter = 0; intCounter < intReturn; intCounter++)
                {
                    aGCHandle[intCounter].Free();
                }
                gchandleArgs.Free();
                gsapi_exit(intGSInstanceHandle);
                gsapi_delete_instance(intGSInstanceHandle);
            }
            
            //Conversion was successful if return code was 0
            return (intReturn == 0);
        }
        private string[] GetGeneratedArgs(string inputFile, string outputFile,
            int firstPage, int lastPage, string deviceFormat, int width, int height)
        {
            this._sDeviceFormat = deviceFormat;
            this._iResolutionX = width;
            this._iResolutionY = height;
            //this._iJPEGQuality= quality;
            
            // Count how many extra args are need - HRangel - 11/29/2006, 3:13:43 PM
            ArrayList lstExtraArgs = new ArrayList();
            if (_sDeviceFormat == "jpg" && _iJPEGQuality > 0 && _iJPEGQuality < 101)
                lstExtraArgs.Add("-dJPEGQ=" + _iJPEGQuality);
            if (_iWidth > 0 && _iHeight > 0)
                lstExtraArgs.Add("-g" + _iWidth + "x" + _iHeight);
            if (_bFitPage)
                lstExtraArgs.Add("-dPDFFitPage");
            if (_iResolutionX > 0)
            {
                if (_iResolutionY > 0)
                    lstExtraArgs.Add("-r" + _iResolutionX + "x" + _iResolutionY);
                else
                    lstExtraArgs.Add("-r" + _iResolutionX);
            }
            // Load Fixed Args - HRangel - 11/29/2006, 3:34:02 PM
            int iFixedCount = 17;
            int iExtraArgsCount = lstExtraArgs.Count;
            string[] args = new string[iFixedCount + lstExtraArgs.Count];
            /*
            // Keep gs from writing information to standard output
        "-q",                    
        "-dQUIET",
       
        "-dPARANOIDSAFER", // Run this command in safe mode
        "-dBATCH", // Keep gs from going into interactive mode
        "-dNOPAUSE", // Do not prompt and pause for each page
        "-dNOPROMPT", // Disable prompts for user interaction          
        "-dMaxBitmap=500000000", // Set high for better performance
       
        // Set the starting and ending pages
        String.Format("-dFirstPage={0}", firstPage),
        String.Format("-dLastPage={0}", lastPage),  
       
        // Configure the output anti-aliasing, resolution, etc
        "-dAlignToPixels=0",
        "-dGridFitTT=0",
        "-sDEVICE=jpeg",
        "-dTextAlphaBits=4",
        "-dGraphicsAlphaBits=4",
            */
            args[0] = "pdf2img";//this parameter have little real use
            args[1] = "-dNOPAUSE";//I don't want interruptions
            args[2] = "-dBATCH";//stop after
            //args[3]="-dSAFER";
            args[3] = "-dPARANOIDSAFER";
            args[4] = "-sDEVICE=" + _sDeviceFormat;//what kind of export format i should provide
            args[5] = "-q";
            args[6] = "-dQUIET";
            args[7] = "-dNOPROMPT";
            args[8] = "-dMaxBitmap=500000000";
            args[9] = String.Format("-dFirstPage={0}", firstPage);
            args[10] = String.Format("-dLastPage={0}", lastPage);
            args[11] = "-dAlignToPixels=0";
            args[12] = "-dGridFitTT=0";
            args[13] = "-dTextAlphaBits=4";
            args[14] = "-dGraphicsAlphaBits=4";
            //For a complete list watch here:
            //http://pages.cs.wisc.edu/~ghost/doc/cvs/Devices.htm
            //Fill the remaining parameters
            for (int i = 0; i < iExtraArgsCount; i++)
            {
                args[15 + i] = (string)lstExtraArgs[i];
            }
            //Fill outputfile and inputfile
            args[15 + iExtraArgsCount] = string.Format("-sOutputFile={0}", outputFile);
            args[16 + iExtraArgsCount] = string.Format("{0}", inputFile);
            return args;
        }
    }
}