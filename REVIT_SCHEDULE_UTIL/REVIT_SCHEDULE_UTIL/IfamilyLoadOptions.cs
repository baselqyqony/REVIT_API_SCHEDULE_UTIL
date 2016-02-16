using System;
namespace REVIT_SCHEDULE_UTIL
{
    interface IfamilyLoadOptions
    {
        bool OnFamilyFound(bool familyInUse, ref bool overwriteParameterValues);
        bool OnSharedFamilyFound(Autodesk.Revit.DB.Family sharedFamily, bool familyInUse, ref Autodesk.Revit.DB.FamilySource source, ref bool overwriteParameterValues);
    }
}
