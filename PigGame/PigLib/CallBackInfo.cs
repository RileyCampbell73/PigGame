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
        //might need an object with ALL player data to pass to the client. Not sure how we should approach it.
        [DataMember]
        public int DieRoll { get; private set; }
        [DataMember]
        public int TotalPoints { get; private set; }
        [DataMember]
        public int BankedPoints { get; private set; }
        [DataMember]
        public bool Ready { get; set;  }


        public CallBackInfo()
        {
            DieRoll = 0;
            TotalPoints = 0;
            BankedPoints = 0;
            Ready = false;
        }

    }
}
