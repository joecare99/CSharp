namespace WinAhnenCls.Model
{
    public enum EHejIndRedir
    {
        hIRd_Meta = -1,   // e.G. Counts, Timeline-Data
        hIRd_Ind = 0,   // Individual Data
        hIRd_Spouse = 1,  // Data of Main(Last) Spouse
        hIRd_AnySpouse = 2,  // Data of any Spouse
        hIRd_AllSpouse = 3,  // Data of all Spouses
        hIRd_Father = 4,  // Data of Father
        hIRd_Mother = 5,  // Data of Mother
        hIRd_AnyParent = 6,  // Data of any Parent
        hIRd_AllParent = 7,  // Data of all Parents
        hIRd_Child = 8,     // Data of Child
        hIRd_AnyChild = 9,  // Data of any Child
        hIRd_AllChild = 10 // Data of all Childs     
    }
}
