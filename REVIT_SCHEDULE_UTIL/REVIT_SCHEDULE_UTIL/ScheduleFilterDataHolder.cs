using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REVIT_SCHEDULE_UTIL
{

    public class ScheduleFilterDataHolder
    {
        public String fieldName { set; get;}
        public ScheduleFilterType filterType { set; get; }
        public string Value { set; get; }

        public ScheduleFilterDataHolder(String fieldName, ScheduleFilterType filterType, string Value)
        {
            this.fieldName = fieldName;
            this.filterType = filterType;
            this.Value = Value;
        }
    }
}
