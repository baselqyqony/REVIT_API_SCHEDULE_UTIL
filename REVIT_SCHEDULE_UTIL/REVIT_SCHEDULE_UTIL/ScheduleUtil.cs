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
        public  ViewSchedule Schedule {set;get;}
        public List<string> FieldsNames { set; get; }
        public BuiltInCategory scheduleCategory { set; get; }
        public Document doc { set; get; }



        public ScheduleUtil(Document doc, BuiltInCategory scheduleCategory, List<string> FieldsNames)
        {
            this.FieldsNames = FieldsNames;
            this.scheduleCategory = scheduleCategory;
            this.doc = doc;
            Schedule = null;

        }
        /// <summary>
        /// generate schedule with current settings
        /// </summary>
        /// <returns> ViewSchedule </returns>
        public void generateSchedule()
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

            
        }
        /// <summary>
        /// generate Schedule With All Posibble Fields
        /// </summary>
        /// <returns></returns>
        public void generateScheduleWithAllFields()
        {

            Schedule = ViewSchedule.CreateSchedule(doc, new ElementId(scheduleCategory), ElementId.InvalidElementId);
            foreach (SchedulableField schedulableField in Schedule.Definition.GetSchedulableFields())
            {
                foreach (string fieldName in FieldsNames)
                {

                    Schedule.Definition.AddField(schedulableField);

                }
            }

         
        }
        /// <summary>
        /// sort schedule by selected field
        /// </summary>
        /// <param name="columnName">field name</param>
        /// <param name="isItemized">wether is itemized or not</param>
        /// <returns>viewSchedule</returns>
        public  void sortBy(string columnName,Boolean isItemized)
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
                
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("please generate schedule first");
              
            }



        }

        public void filterFields(List<ScheduleFilterDataHolder> filters)
        {

            if (Schedule != null)
            {
                int scheduleFieldsCount = Schedule.Definition.GetFieldCount();

                for (int i = 0; i < scheduleFieldsCount; i++)
                {
                    ScheduleField schedulableField = Schedule.Definition.GetField(i);
                    foreach (ScheduleFilterDataHolder filter in filters)
                    {
                        if (filter.fieldName.ToUpper() == schedulableField.GetName().ToUpper())
                        {
                            ScheduleFilter sFilter = new ScheduleFilter(schedulableField.FieldId, filter.filterType, filter.Value);
                            Schedule.Definition.AddFilter(sFilter);
                            break;
                        }
                    }
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("please generate schedule first");

            }
        }

 
        /// <summary>
        /// hide list of selected field in the schedule
        /// </summary>
        /// <param name="fieldsNames"> list of fienlds names that are going to be hidden</param>
        public void hideFields(List<string> fieldsNames)
        {
            if (Schedule != null)
            {
                foreach (string s in fieldsNames)
                {

                    for (int i = 0; i < Schedule.Definition.GetFieldCount(); i++)
                    {
                        ScheduleField SF = Schedule.Definition.GetField(i);
                        if (SF.GetName().ToUpper() == s.ToUpper())
                        {
                            SF.IsHidden = true;
                        }
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("please generate schedule first");
            
            }
            
        }    }

}
