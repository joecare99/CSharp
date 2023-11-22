namespace WinAhnenCls.Model.GenBase
{
    public interface IGenFamily : IGenData
    {
        public interface _IIndividuals
        {
            IGenIndividual this[object Idx] { get; set; }
        }
        // Relationship
        IGenIndividual Husband { get; set; }
        IGenIndividual Wife { get; set; }
        int ChildCount { get; }
        _IIndividuals Children { get; }

        // Vital
        string MarriageDate { get; set; }
        string MarriagePlace { get; set; }
        IGenEvent Marriage { get; set; }

        // Management
        string FamilyRefID { get; set; }
        string FamilyName { get; set; }
    }
}