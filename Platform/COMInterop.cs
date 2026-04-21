// =============================================================================
// RULE ID   : cr-dotnet-0041
// RULE NAME : COM Interop Usage
// CATEGORY  : Platform
// DESCRIPTION: Application uses Component Object Model (COM) interop through
//              [ComImport], Marshal class, or COM object instantiation. COM
//              components don't exist on non-Windows cloud platforms, causing
//              complete interop failures and preventing cross-platform deployment.
// =============================================================================

using System;
using System.Runtime.InteropServices;

namespace SyntheticLegacyApp.Platform
{
    // VIOLATION cr-dotnet-0041: [ComImport] declares binding to a native COM component
    [ComImport]
    [Guid("00000000-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ILegacyComComponent
    {
        void Initialize(string config);
        string Process(string input);
        void Shutdown();
    }

    public class ComInteropWorker
    {
        // VIOLATION cr-dotnet-0041: Activating a COM object by ProgID — Windows-only
        public object CreateExcelApplication()
        {
            Type excelType = Type.GetTypeFromProgID("Excel.Application");
            return Activator.CreateInstance(excelType);
        }

        // VIOLATION cr-dotnet-0041: Marshal.ReleaseComObject is Windows COM lifecycle management
        public void GenerateExcelReport(string filePath)
        {
            dynamic excelApp = CreateExcelApplication();
            try
            {
                dynamic workbooks = excelApp.Workbooks;
                dynamic wb = workbooks.Add();
                dynamic ws = wb.Worksheets[1];
                ws.Cells[1, 1] = "Legacy Report";
                wb.SaveAs(filePath);
                wb.Close();
            }
            finally
            {
                // VIOLATION cr-dotnet-0041: ReleaseComObject is COM-specific cleanup
                Marshal.ReleaseComObject(excelApp);
            }
        }

        // VIOLATION cr-dotnet-0041: Creating COM object from registered CLSID
        public void InvokeLegacyProcessor(string input)
        {
            Type comType = Type.GetTypeFromCLSID(
                new Guid("12345678-1234-1234-1234-123456789ABC"));

            ILegacyComComponent component =
                (ILegacyComComponent)Activator.CreateInstance(comType);

            component.Initialize("default");
            string result = component.Process(input);
            Console.WriteLine($"COM result: {result}");
            component.Shutdown();

            Marshal.ReleaseComObject(component);
        }

        // VIOLATION cr-dotnet-0041: Checking COM error codes via Marshal.GetHRForException
        public void HandleComError(Exception ex)
        {
            int hr = Marshal.GetHRForException(ex);
            Console.WriteLine($"COM HRESULT: 0x{hr:X8}");
        }
    }
}
