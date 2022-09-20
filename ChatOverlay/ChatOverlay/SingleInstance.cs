using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaJiCSoft.ChatOverlay
{
    public interface ISingleInstance
    {
        void SendStartupArgs(string[] args);
    }


    public class SingleInstance : ISingleInstance
    {
        // TODO: Can we make this a Lifetime?
        //private bool OpenServiceHost()
        //{
        //    var NamedPipeHost = new ServiceHost(typeof(SingleInstance), new Uri("net.pipe://localhost"));
        //}
        public void SendStartupArgs(string[] args)
        {
            throw new NotImplementedException();
        }

        //public bool SingleInstance(NamedPipeManager pipeManager)
        //{
        //    Assembly assembly = Assembly.GetExecutingAssembly();
        //    bool mutexCreated;
        //    string mutexName = string.Format(CultureInfo.InvariantCulture, pipeManager.NamedPipeName, assembly.GetType().GUID, assembly.GetName().Name);

        //    mutex = new Mutex(true, mutexName, out mutexCreated);
        //    windowMessage = WinApi.User32.RegisterWindowMessage(mutexName);

        //    if (!mutexCreated)
        //    {
        //        string cmdLine = "";
        //        var args = Environment.GetCommandLineArgs();

        //        var client = pipeManager.GetClientStream(5000);
        //        client.WriteStream(Encoding.UTF8.GetBytes(args[1])); // Send first argument into the pipe

        //        mutex = null;
        //        //WinApi.User32.PostMessage((IntPtr)WinApi.User32.HWND_BROADCAST, windowMessage, IntPtr.Zero, IntPtr.Zero);

        //        Current.Shutdown();
        //        return false;
        //    }
        //    return true;
        //}
    }
}
