namespace RecipeMaster.Util
{
    /// <summary>
    /// A class where all props can be set to null. Useful when setting props
    /// to null lets the database know to delete the record that corresponds
    /// to the nulled item.
    /// </summary>
    public abstract class Nullifyable
    {
        /// <summary>
        /// Sets all props to be null (i.e. indicates the database should
        /// delete the record corresponding to this instace).
        /// </summary>
        public abstract void Nullify();
    }
}
