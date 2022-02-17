using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace UsbModule.Win32;

/// <summary>
/// WIN32 APIs.
/// </summary>
[SupportedOSPlatform("windows")]
public static class Win32Api
{
    /// <summary>
    /// Get device registry property.
    /// </summary>
    /// <param name="hDevInfo">handle.</param>
    /// <param name="deviceInfoData">data.</param>
    /// <param name="property">property.</param>
    /// <returns>string value.</returns>
    /// <exception cref="Win32Exception">읽기 실패 시.</exception>
    public static string GetDeviceRegistryProperty(
        IntPtr hDevInfo,
        ref SetupApi.SpDevInfoData deviceInfoData,
        uint property)
    {
        SetupApi.SetupDiGetDeviceRegistryProperty(
            hDevInfo, ref deviceInfoData, property, out _, null, 0, out var size);

        if (size <= 0)
        {
            return string.Empty;
        }

        var buffer = new char[(int)size];

        var isSuccess = SetupApi.SetupDiGetDeviceRegistryProperty(
            hDevInfo, ref deviceInfoData, property, null, buffer, size, null);

        if (!isSuccess)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return new string(buffer);
    }
}
