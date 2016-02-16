#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
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
           
           
            ////Dictionary<string, List<FamilySymbol>> categoryFamilies = scUtil.FindFamilyTypes(doc, BuiltInCategory.OST_StructuralFoundation);
            ////List<List<FamilySymbol>> familySymbolsLists = categoryFamilies.Values.ToList();
            ////foreach (List<FamilySymbol> familySymbolList in familySymbolsLists)
            ////{
            ////    foreach (FamilySymbol fs in familySymbolList)
            ////    {
            ////        try
            ////        {
            ////            Parameter p = fs.GetParameters("BK").First();
            ////        }
            ////        catch
            ////        {
            ////            scUtil.addParameter(doc, fs.Family, "BK");
            ////        }

            ////    }
            ////}
         

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("making schedule");
                List<string> fields = new List<string>();
                fields.Add("TYPE MARK");
                fields.Add("LENGTH");
                fields.Add("SOCKET LENGTH IN ROCK");
                fields.Add("WORKING COMPRESSION LOAD");
                fields.Add("REINFORCEMENT");
                fields.Add("COUNT");
                fields.Add("BK");

                ScheduleUtil scUtil = new ScheduleUtil(doc, BuiltInCategory.OST_StructuralFoundation, fields);
                // scUtil.CreateProjectParameter(app, "BK",ParameterType.Text, BuiltInCategory.OST_StructuralFoundation, BuiltInParameterGroup.PG_TEXT, true);
                Category foundationCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation);
                CategorySet cats1 = app.Create.NewCategorySet();

                cats1.Insert(foundationCategory);

                if (!scUtil.getProjectParametersNames(doc).Contains("BK"))
                scUtil.CreateProjectParameter(app, "BK", ParameterType.Text, cats1, BuiltInParameterGroup.PG_TEXT, true);
           

            
                ViewSchedule schedule = null;
         
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
