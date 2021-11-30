using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Interface
{
    /// <summary>
    /// Represents a record that can be stored in a database
    /// </summary>
    public interface IDatabaseRecord
    {
        /// <summary>
        /// Unique primary identifier
        /// </summary>
        int Id { get; set; }
    }
}
