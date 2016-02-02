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
        /// <summary>
        /// generate schedule with current settings
        /// </summary>
        /// <returns> ViewSchedule </returns>
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
        /// <summary>
        /// generate Schedule With All Posibble Fields
        /// </summary>
        /// <returns></returns>
        public ViewSchedule generateScheduleWithAllFields()
        {

            Schedule = ViewSchedule.CreateSchedule(doc, new ElementId(scheduleCategory), ElementId.InvalidElementId);
            foreach (SchedulableField schedulableField in Schedule.Definition.GetSchedulableFields())
            {
                foreach (string fieldName in FieldsNames)
                {

                    Schedule.Definition.AddField(schedulableField);

                }
            }

            return Schedule;
        }
        /// <summary>
        /// sort schedule by selected field
        /// </summary>
        /// <param name="columnName">field name</param>
        /// <param name="isItemized">wether is itemized or not</param>
        /// <returns>viewSchedule</returns>
        public ViewSchedule sortBy(string columnName,Boolean isItemized)
        {
            if (Schedule != null)
            {
                int scheduleFieldsCount = Schedule.Definition.GetFieldCount();

               for(int i=0;i<scheduleFieldsCount;i++){
                   ScheduleField schedulableField = Schedule.Definition.GetField(i);
                    if (schedulableField.GetName().ToUpper() == columnName.ToUpper())
                    {

                        ScheduleSortGroupField sortGroupField = new ScheduleSortGroupField(schedulableField.FieldId);
                        
                        sortGroupField.ShowHeader = false;
                        
                        Schedule.Definition.AddSortGroupField(sortGroupField);
                        Schedule.Definition.IsItemized = isItemized;
                    }
                }
                return Schedule;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("please generate schedule first");
                return null;
            }



        }
    }
}
