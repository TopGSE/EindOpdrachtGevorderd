using _1.Route_Netwerk_WPF.Models;
using _2.Route_Netwerk_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.Route_Netwerk_WPF.Dto
{
    public class SegmentMapper
    {
        public static Segment MapToDomain(SegmentUI segmentui)
        {
            return new Segment(segmentui.Id, segmentui.StartPointId, segmentui.EndPointId);
        }

        public static SegmentUI MapToUI(Segment segment)
        {
            return new SegmentUI(segment.Id, segment.StartPointId, segment.EndPointId);
        }
    }
}
