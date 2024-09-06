using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Net.Sockets;
using UnityEngine.UIElements;

public class retro : MonoBehaviour
{
    public string game;
    public String bigEnd;
    public TextMeshProUGUI  text;
    public string systemName;

    #nullable enable

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hObject);

    [DllImport("psapi.dll", SetLastError = true)]
    static extern bool EnumProcessModules(IntPtr hProcess, IntPtr[] lphModule, uint cb, out uint lpcbNeeded);

    [DllImport("psapi.dll", CharSet = CharSet.Auto)]
    static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] char[] lpBaseName, uint nSize);

    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int PROCESS_VM_READ = 0x0010;


    public void Begin()
    {

        
        string retroArchPath = @"C:\RetroArch-Win64\retroarch.exe"; // Update this path
        string arg1 = "-L"; // First argument
        string arg2 = @"C:\RetroArch-Win64\cores\picodrive_libretro.dll"; // Second argument, update this path
        string arg3 = @GamePicker(systemName, game); // Third argument, update this path
        long memoryOffset = OffsetPicker(game); // Example offset, adjust as needed
        bool bigEndian = true;
        bool hexCounter = true;


        // Start RetroArch process
        ProcessStartInfo startInfo = new ProcessStartInfo(retroArchPath, $"{arg1} {arg2} {arg3}");
        startInfo.UseShellExecute = false; // This allows the process to run in the same security context
        Process? retroArchProcess = Process.Start(startInfo);

        if (retroArchProcess == null)
        {
            Console.WriteLine("Failed to start RetroArch process.");
            return;
        }

        Console.WriteLine($"Started RetroArch process with ID: {retroArchProcess.Id}");

        // Wait for the process to start
        retroArchProcess.WaitForInputIdle();

        // Open a handle to the RetroArch process
        IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, retroArchProcess.Id);

        if (processHandle == IntPtr.Zero)
        {
            Console.WriteLine($"Failed to open process. Error: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
            retroArchProcess.Close();
            return;
        }

        // Get the base address of picodrive_libretro.dll

        //unity stop for 3 seconds
        System.Threading.Thread.Sleep(3000);

            IntPtr moduleHandle = EnumerateModules(processHandle);

        // Calculate the final address
        long finalAddress = moduleHandle.ToInt64() + memoryOffset;

        //Print out the results
        // Read the value at the final address
        byte[] buffer = new byte[sizeof(int)];
        IntPtr bytesRead;

        bool killswitch = true;
        while(killswitch){
        bool success = ReadProcessMemory(processHandle, (IntPtr)finalAddress, buffer, buffer.Length, out bytesRead);

        if (!success)
        {
            Console.WriteLine($"Failed to read memory. Error: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
            CloseHandle(processHandle);
            retroArchProcess.Close();
            return;
        }

        int value = BitConverter.ToInt32(buffer, 0);

        if (bigEndian)
        {
            string hexValue = BitConverter.ToString(buffer).Replace("-", "");
            string bigEndianValue = lit2big(hexValue);
            if(!hexCounter){
            value = Convert.ToInt32(bigEndianValue, 16);
            }
            else
            {
                //print to debug console
                UnityEngine.Debug.Log($"Value at 0x{finalAddress:X}: 0x{bigEndianValue}");
                Console.WriteLine($"Value at 0x{finalAddress:X}: 0x{bigEndianValue}");
                
                //text.text = bigEndianValue
                text.text = bigEndianValue;
                bigEnd = bigEndianValue;
            }
        }
        if(hexCounter&&!bigEndian){
            Console.WriteLine($"Value at 0x{finalAddress:X}: 0x{value:X}");
        text.text = value.ToString("X");
        }else if(!hexCounter&&!bigEndian){
            Console.WriteLine($"Value at {finalAddress:X}: {value}");
            text.text = value.ToString();
        }

        //switch the killswitch if space is pressed
        if(Console.KeyAvailable){
            ConsoleKeyInfo key = Console.ReadKey(true);
            if(key.Key == ConsoleKey.Spacebar){
                killswitch = false;
            }
        }
        

        // Wait for 1 second
        System.Threading.Thread.Sleep(1000);


        }

        Console.WriteLine($"Base address of picodrive_libretro.dll: 0x{moduleHandle.ToInt64():X}");
        Console.WriteLine($"Memory offset: 0x{memoryOffset:X}");
        Console.WriteLine($"Final address: 0x{finalAddress:X}");

        // Clean up
        CloseHandle(processHandle);
        retroArchProcess.Close();
    }



// Enumerate all modules in the process and search for picodrive_libretro.dll
    static IntPtr EnumerateModules(IntPtr processHandle)
    {
        IntPtr[] modules = new IntPtr[1024];
    
        if (!EnumProcessModules(processHandle, modules, (uint)(modules.Length * IntPtr.Size), out uint cbNeeded))
        {
            Console.WriteLine($"Failed to enumerate modules. Error: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
            return IntPtr.Zero;
        }
    
        int moduleCount = (int)(cbNeeded / IntPtr.Size);
        for (int i = 0; i < moduleCount; i++)
        {
            char[] moduleName = new char[1024];
            //current module handle
            GetModuleFileNameEx(processHandle, modules[i], moduleName, (uint)moduleName.Length);
            string moduleNameString = new string(moduleName).TrimEnd('\0');
            if(moduleNameString.Contains("picodrive_libretro.dll")){
                Console.WriteLine("Found picodrive_libretro.dll");
                return modules[i];
            }
        }
    
        return IntPtr.Zero;
    }


    static string lit2big(string littleEndian)
    {
        char[] bigEndianChars = new char[littleEndian.Length];
        for (int i = littleEndian.Length - 2; i >= 0; i -= 2)
        {
            bigEndianChars[littleEndian.Length - 2 - i] = littleEndian[i];
            bigEndianChars[littleEndian.Length - 1 - i] = littleEndian[i + 1];
        }
    
        //Switch the order of the bytes
        char temp;
        for (int i = 0; i < 4; i++)
        {
            temp = bigEndianChars[i];
            bigEndianChars[i] = bigEndianChars[i + 4];
            bigEndianChars[i + 4] = temp;
        }
        Console.WriteLine(new string(bigEndianChars));
    
        return new string(bigEndianChars);
    }

    long OffsetPicker(string game){
    if(game == "Altered Beast"){
        return 0x1CD222;
    }
    if(game == "Castlevania Bloodlines"){
        return 0x1CB158;
    }
    if(game == "Ranger X"){
        return 0x1D0B6C;
        //hex = false
    }
    UnityEngine.Debug.Log("Game not found");
    return 0;
    
    }

    String CorePicker(string System){
        if(System == "Sega Genesis"){
    
            return "picodrive_libretro.dll";
    
        }

        return "";
    }
    String GamePicker(string System, string Game){
        if(System == "Sega Genesis"){
            if(Game == "Altered Beast"){
                return "Alt.zip";
            }
            if(Game == "Castlevania Bloodlines"){
                return "Cast.zip";
            }
            if(Game == "Ranger X"){
                return "Ranger.zip";
            }
        }
        return "";
    }
    
}