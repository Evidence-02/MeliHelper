using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class JournalSession
    {
        public List<JournalSessionMove> GetListMoves { get; set; }
        //public int GetCountMoves { get; set; }
        
        public JournalSession()
        {
            GetListMoves = new List<JournalSessionMove>();
        }

        public void AddMove(JournalMove move)
        {
            GetListMoves.Add(new JournalSessionMove() { Description = move.Description, isRegisteredInJournal = move.isRegisteredInJournal });
            //GetCountMoves = GetListMoves.Count(t => t.isRegisteredInJournal);
        }


        //public JournalMove GenerateJournalMove()
        //{
        //    return new JournalMove(null, false) {  };
        //}
    }

    public class JournalSessionMove
    {
        public string Description { get; set; }
        public bool isRegisteredInJournal { get; set; }
    }
}
