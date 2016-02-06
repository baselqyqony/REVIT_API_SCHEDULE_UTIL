#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace REVIT_SCHEDULE_UTIL
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ////////// Access current selection

            ////////Selection sel = uidoc.Selection;

            ////////// Retrieve elements from database

            ////////FilteredElementCollector col
            ////////  = new FilteredElementCollector(doc)
            ////////    .WhereElementIsNotElementType()
            ////////    .OfCategory(BuiltInCategory.INVALID)
            ////////    .OfClass(typeof(Wall));

            ////////// Filtered element collector is iterable

            ////////foreach (Element e in col)
            ////////{
            ////////    Debug.Print(e.Name);
            ////////}

            ////////// Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                ViewSchedule schedule = null;
                tx.Start("Create Piles Schedule");
                List<string> fields = new List<string>();
                fields.Add("TYPE MARK");
                fields.Add("LENGTH");
                fields.Add("SOCKET LENGTH IN ROCK");
                fields.Add("WORKING COMPRESSION LOAD");
                fields.Add("REINFORCEMENT");
                fields.Add("COUNT");
            
                ScheduleUtil scUtil = new ScheduleUtil(doc, BuiltInCategory.OST_StructuralFoundation, fields);
               scUtil.generateSchedule();
               scUtil.sortBy("TYPE MARK",false);

               List<string> fieldsToBeHidden = new List<string>();
               fieldsToBeHidden.Add("TYPE MARK");

               scUtil.hideFields(fieldsToBeHidden);

                //adding filters 
                List<ScheduleFilterDataHolder> filters=new List<ScheduleFilterDataHolder>();
                ScheduleFilterDataHolder filter = new ScheduleFilterDataHolder("TYPE MARK", ScheduleFilterType.Equal, "BP-1");
                filters.Add(filter);
                scUtil.filterFields(filters);
            


               schedule = scUtil.Schedule;
                if (null != schedule)
                {
                 
                    tx.Commit();
                }
                else
                    tx.RollBack();
            }

            return Result.Succeeded;
        }
    }
}
