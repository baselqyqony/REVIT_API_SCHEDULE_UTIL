using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
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

            IList<ScheduleFieldId> ordereieldsIDS = new List<ScheduleFieldId>();

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
            removeDuplicated();
            IList<ScheduleFieldId> fieldsIDS = Schedule.Definition.GetFieldOrder();


            foreach (string fieldName in FieldsNames)
            {
                foreach (ScheduleFieldId fid in fieldsIDS)
                {
                    ScheduleField schedulableField = Schedule.Definition.GetField(fid);
                    if (schedulableField.GetName().ToUpper() == fieldName.ToUpper())
                    {
                        ordereieldsIDS.Add(fid);
                        break;
                    }
                }
            }

            try
            {
                Schedule.Definition.SetFieldOrder(ordereieldsIDS);
            }
            catch (Exception ex) {  }
            
        }

        /// <summary>
        /// remove strange duplicated fields caused by revit
        /// </summary>
        private void removeDuplicated()
        {
            List<ScheduleFieldId> unneededFields = new List<ScheduleFieldId>();
            for (int i = 0; i < Schedule.Definition.GetFieldCount(); i++)
            {
                ScheduleField schedulableFieldi = Schedule.Definition.GetField(Schedule.Definition.GetFieldId(i));
                for (int j = 0; j < Schedule.Definition.GetFieldCount(); j++)
                {
                    if (i == j) break;

                    ScheduleField schedulableFieldj = Schedule.Definition.GetField(Schedule.Definition.GetFieldId(j));
                    if (schedulableFieldi.GetName().ToUpper() == schedulableFieldj.GetName().ToUpper())
                    {
                        if (!unneededFields.Contains(Schedule.Definition.GetFieldId(i)))
                            unneededFields.Add(Schedule.Definition.GetFieldId(i));
                    }

                }

            }

            foreach (ScheduleFieldId id in unneededFields)
            {
                Schedule.Definition.RemoveField(id);

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

        /// <summary>
        /// filter fields
        /// </summary>
        /// <param name="filters">applied filters array</param>
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
        /// get BuiltInCategoryFamily
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="cat">category</param>
        /// <returns></returns>
        public  Dictionary<string, List<FamilySymbol>> FindFamilyTypes(Document doc, BuiltInCategory cat)
        {
            return new FilteredElementCollector(doc)
                            .WherePasses(new ElementClassFilter(typeof(FamilySymbol)))
                            .WherePasses(new ElementCategoryFilter(cat))
                            .Cast<FamilySymbol>()
                            .GroupBy(e => e.Family.Name)
                            .ToDictionary(e => e.Key, e => e.ToList());
        }

        /// <summary>
        /// list available project parameters
        /// </summary>
        /// <param name="doc">revit document</param>
        /// <returns></returns>
        public List<string> getProjectParametersNames(Document doc)
        {
            List<string> output = new List<string>();
            BindingMap map = doc.ParameterBindings;
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                ElementBinding eleBinding = it.Current as ElementBinding;
                InstanceBinding insBinding = eleBinding as InstanceBinding;
                Definition def = it.Key;
                if (def != null)
                {
                    output.Add(def.Name);
                }
            }

            return output;
        }

        /// <summary>
        /// create project parameter
        /// source :http://spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
        /// </summary>
        /// <param name="app">revit application</param>
        /// <param name="name">revit ui application</param>
        /// <param name="type">parameter type</param>
        /// <param name="cats">category set</param>
        /// <param name="group">parameter group</param>
        /// <param name="inst">is instance variable</param>
        public  void CreateProjectParameter(Application app, string name, ParameterType type,  CategorySet cats, BuiltInParameterGroup group, bool instance)
        {
    

            string oriFile = app.SharedParametersFilename;
            string tempFile = Path.GetTempFileName() + ".txt";
            using (File.Create(tempFile)) { }
            app.SharedParametersFilename = tempFile;
    
             ExternalDefinitionCreationOptions edc=new ExternalDefinitionCreationOptions(name, type);
             ExternalDefinition def = app.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(edc) as ExternalDefinition;

            app.SharedParametersFilename = oriFile;
            File.Delete(tempFile);

            Autodesk.Revit.DB.Binding binding = app.Create.NewTypeBinding(cats);
            if (instance) binding = app.Create.NewInstanceBinding(cats);
            BindingMap map = (new UIApplication(app)).ActiveUIDocument.Document.ParameterBindings;
            map.Insert(def, binding, group);
        }

        /// <summary>
        /// create custom parameter
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="fam">family</param>
        /// <param name="parameterName">parameter Name</param>
        public Boolean addFamilyParameter( Document doc, Family fam,string parameterName)
        {
            Document familyDoc = doc.EditFamily(fam);
            Transaction t = new Transaction(familyDoc);
            Boolean result = false;
            try
            {
                
                
                t.Start("add " + parameterName + " to " + fam.Name);
                BuiltInParameterGroup addToGroup = BuiltInParameterGroup.PG_TEXT;
                ParameterType parameterType = ParameterType.Text;
                familyDoc.FamilyManager.AddParameter(parameterName, addToGroup, parameterType, true);
               // t.Commit();

             //   familyLoadOptions flo = new familyLoadOptions();
              //  familyDoc.LoadFamily(doc, flo);
               // familyDoc.Close(false);
                result = true;
               
            }
            catch {
                
             
                result = false;
             
            }
            finally
            {
                if (true == result)
                {
                    t.Commit();
                    familyLoadOptions flo = new familyLoadOptions();
                    familyDoc.LoadFamily(doc, flo);
                    familyDoc.Close(false);
                }
                else
                {
                    t.RollBack();
                    familyDoc.Close(false);
                }

            }

            return result;
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
