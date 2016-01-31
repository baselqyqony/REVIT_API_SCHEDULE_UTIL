using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIT_SCHEDULE_UTIL
{
    public class ScheduleUtil
    {
        private ViewSchedule Schedule = null;
        public List<string> FieldsNames { set; get; }
        public BuiltInCategory scheduleCategory { set; get; }
        public Document doc { set; get; }

        public ScheduleUtil(Document doc, BuiltInCategory scheduleCategory, List<string> FieldsNames)
        {
            this.FieldsNames = FieldsNames;
            this.scheduleCategory = scheduleCategory;
            this.doc = doc;

        }

        public ViewSchedule generateSchedule()
        {

            Schedule = ViewSchedule.CreateSchedule(doc, new ElementId(scheduleCategory), ElementId.InvalidElementId);
            foreach (SchedulableField schedulableField in Schedule.Definition.GetSchedulableFields())
            {
                foreach (string fieldName in FieldsNames)
                {
                    if (schedulableField.GetName(doc).ToUpper() == fieldName.ToUpper())
                    {

                        Schedule.Definition.AddField(schedulableField);
                        break;
                    }
                }
            }

            return Schedule;
        }


    }
}
