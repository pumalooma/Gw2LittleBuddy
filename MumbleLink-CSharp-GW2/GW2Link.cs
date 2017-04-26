﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MumbleLink_CSharp;

namespace MumbleLink_CSharp_GW2
{
    public class GW2Link : MumbleLink
    {
        private const double MeterToInch = 39.3700787d;

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



        public Coordinates GetCoordinates()
        {
            MumbleLinkedMemory l = Read();

            /* 
             * Note that the mumble coordinates differ from the actual in-game coordinates.
             * They are in the format x,z,y and z has been negated so that underwater is negative
             * rather than positive.
             * 
             * Coordinates are based on a central point (0,0), which may be the center of the zone, 
             * where traveling west is negative, east is positive, north is positive and south is negative.
             * 
             */

            var coord = new Coordinates()
            {
                X = l.FAvatarPosition[0] * MeterToInch, //west to east
                Y = l.FAvatarPosition[2] * MeterToInch, //north to south
                Z = -l.FAvatarPosition[1] * MeterToInch, //altitude
                WorldId = BitConverter.ToInt32(l.Context, 36),
                MapId = BitConverter.ToInt32(l.Context, 28)
            };
            return coord;
        }
    }
}
