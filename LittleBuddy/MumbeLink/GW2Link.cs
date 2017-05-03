using System;
using System.Runtime.InteropServices;
using MumbleLink_CSharp;

namespace MumbleLink_CSharp_GW2
{
    public class GW2Link : MumbleLink
    {
        public struct Coordinates
        {
            public double X, Y, Z;
            public int WorldId;
            public int MapId;
        }

        public GW2Context GetContext()
        {
            var l = Read();

            int size = Marshal.SizeOf(typeof(GW2Context));

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(l.Context, 0, ptr, size);

            var result = (GW2Context)Marshal.PtrToStructure(ptr, typeof(GW2Context));

            Marshal.FreeHGlobal(ptr);

            return result;
        }
    }
}
