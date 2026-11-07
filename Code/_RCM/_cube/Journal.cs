using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class Journal
    {
        List<JournalMove> list_moves;
        public int GetCountMoves { get; private set; }
        
        public Journal()
        {
            list_moves = new List<JournalMove>();
        }

        public void Clear()
        {
            list_moves.Clear();
            GetCountMoves = 0;
        }

        public void AddMove(Axis axis, bool is_clockwise, bool is_short_name)
        {
            // Add new move (if needed)
            if (list_moves.Count == 0 || list_moves.Last().GetAxis != axis)  
                list_moves.Add(new JournalMove(axis, is_short_name: is_short_name));

            // Turn it
            JournalMove move = list_moves.Last();
            move.AddRotate(is_clockwise);

            // Remove finished (if needed)
            if (move.Rotates == 0)
                list_moves.Remove(move);

            // Update info
            GetCountMoves = list_moves.Count(t => t.isRegisteredInJournal);
        }

        //public void AddMove(Axis axis, int count_rotates, bool is_short_name)
        //{
        //    JournalMove move = new JournalMove(axis, is_short_name: is_short_name);
        //    move.Rotates = count_rotates;
        //    list_moves.Add(move);
        //    // Update info
        //    GetCountMoves = list_moves.Count(t => t.GetAxis.isRegisteredInJournal);
        //}

        public void AddMove(JournalSessionMove move)
        {
            list_moves.Add(new JournalMove(move));

            // Update info
            GetCountMoves = list_moves.Count(t => t.isRegisteredInJournal);
        }

        public Axis GetLastAxis()
        {
            return (list_moves.Count == 0) ? null : list_moves.Last().GetAxis;
        }

        public List<JournalMove> GetListMoves
        {
            get
            {
                return list_moves;
            }
        }
    }
}
