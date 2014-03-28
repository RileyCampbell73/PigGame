using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PigLib
{


    [DataContract]
    public class CallBackInfo
    {
        [DataMember]
        public int DieRoll { get; private set; }
        [DataMember]
        public int TotalPoints { get; private set; }
        [DataMember]
        public int BankedPoints { get; private set; }
        [DataMember]
        public bool Ready { get; set;  }


        public CallBackInfo(int d, int t, int b)
        {
            DieRoll = d;
            TotalPoints = t;
            BankedPoints = b;
            Ready = false;
        }

    }
}
