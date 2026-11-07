using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class JournalMove
    {
        public Axis GetAxis { get; set; }
        public int Rotates { get; set; }
        public int RotatesMax { get; set; }
        public bool isShortName { get; set; }
        public bool isRegisteredInJournal { get; set; }
        public string Description { get; set; }

        public JournalMove(Axis axis, bool is_short_name = false)
        {
            this.GetAxis = axis;
            this.Rotates = 0;
            this.RotatesMax = axis.GetCountRotates;
            this.isShortName = is_short_name;
            this.isRegisteredInJournal = axis.isRegisteredInJournal;
            this.Description = GenerateMoveDescriptionFromAxis();
        }

        public JournalMove(JournalSessionMove move)
        {
            this.Description = move.Description;
            this.isRegisteredInJournal = move.isRegisteredInJournal;
        }

        public void AddRotate(bool is_clockwise)
        {
            Rotates += (is_clockwise) ? 1 : -1;
            if (Rotates <  0         ) Rotates += RotatesMax;
            if (Rotates >= RotatesMax) Rotates -= RotatesMax;
            this.Description = GenerateMoveDescriptionFromAxis();
        }

        string GenerateMoveDescriptionFromAxis()
        {
            if (GetAxis == null)
                return "?";

            string label = GetAxis.GetDescription;
            switch (RotatesMax)
            {
                case 4:
                    switch (Rotates)
                    {
                        case 1: return label;
                        case 2: return label + "2";
                        case 3: return label + "'";
                    }
                    break;
                    
                case 5:
                    switch (Rotates)
                    {
                        case 1: return label;
                        case 2: return label + "2";
                        case 3: return label + "2'";
                        case 4: return label + "'";
                    }
                    break;
                    
                case 2:
                    switch (Rotates)
                    {
                        case 1: return label + (isShortName ? "" : "2");
                    }
                    break;

                case 3:
                    switch (Rotates)
                    {
                        case 1: return label;
                        case 2: return label + "'";
                    }
                    break;
            }
            return label + "(?)";
        }






    }
}
