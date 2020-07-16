using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DInterestsCallings
{
    [StaticConstructorOnStartup]
    public static class Base
    {
        static Base()
        {
            InterestsSettings.WriteAll();
        }
    }
}
