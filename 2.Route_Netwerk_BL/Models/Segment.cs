using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Models
{
    public class Segment
    {
        public Segment()
        {
        }

        public Segment(int startPointId, int endPointId)
        {
            StartPointId = startPointId;
            EndPointId = endPointId;
        }

        public Segment(int id, int startPointId, int endPointId)
        {
            Id = id;
            StartPointId = startPointId;
            EndPointId = endPointId;
        }

        public int Id { get; set; }
        public int StartPointId { get; set; }
        public int EndPointId { get; set; }
    }
}
